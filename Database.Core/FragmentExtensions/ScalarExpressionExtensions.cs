using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.FragmentExtensions
{
    // TODO : consolidate these 2 extensions 
    public static class ScalarExpressionExtensions
    {
        public static Field GetField(
            this ScalarExpression scalarExpression, 
            ColumnDefinition column, 
            IEnumerable<ColumnDefinition> columns, 
            ILogger logger, 
            SchemaFile file
        )
        {
            switch (scalarExpression)
            {
                case ValueExpression valueExpression:
                    return valueExpression.GetField(column.ColumnIdentifier.Value, logger, file);

                case ParenthesisExpression parenthesisExpression:
                    return parenthesisExpression.Expression.GetField(column, columns, logger, file);

                case ColumnReferenceExpression columnReferenceExpression:
                    return columnReferenceExpression.GetField(logger, file, columns);

                case BinaryExpression binaryExpression:
                    {
                        // TODO : what if you're working with different types, which one should be reported? (..compute the value)
                        var first = binaryExpression.FirstExpression.GetField(column, columns, logger, file);
                        var second = binaryExpression.SecondExpression.GetField(column, columns, logger, file);
                        var result = first.Type != FieldType.NotSpecified ? first : second;
                        return result.Copy(column.ColumnIdentifier.Value);
                    }

                case ConvertCall convertCall:
                    {
                        var name = column.ColumnIdentifier.Value;
                        var isNullable = column.IsNullable();
                        return convertCall.DataType.GetField(name, isNullable, logger, file);
                    }

                case FunctionCall functionCall:
                    {
                        // TODO : do not return first.. compute the value
                        var first = functionCall
                            .Parameters
                            .Select(x => x.GetField(column, columns, logger, file))
                            .FirstOrDefault(x => x.Type != FieldType.NotSpecified);

                        if (first != null)
                        {
                            return first;
                        }

                        break;
                    }
            }

            logger.Log(LogLevel.Error, $"Unable to determine column type from scalar expression. Fragment: \"{scalarExpression.GetTokenText()}\"");

            return new UnknownField()
            {
                Name = column.ColumnIdentifier.Value,
            };
        }

        public static Field GetField(
            this ScalarExpression scalarExpression,
            string columnName,
            ILogger logger,
            SchemaFile file
        )
        {
            switch (scalarExpression)
            {
                case ValueExpression valueExpression:
                    return valueExpression.GetField(columnName, logger, file);

                case UnaryExpression unaryExpression:
                    return unaryExpression.Expression.GetField(columnName, logger, file);

                case ParenthesisExpression parenthesisExpression:
                    return parenthesisExpression.Expression.GetField(columnName, logger, file);

                case ColumnReferenceExpression columnReferenceExpression:
                    return columnReferenceExpression.GetField(columnName, logger, file);

                case BinaryExpression binaryExpression:
                    {
                        // TODO : what if you're working with different types, which one should be reported? (..compute the value)
                        var first = binaryExpression.FirstExpression.GetField(columnName, logger, file);
                        var second = binaryExpression.SecondExpression.GetField(columnName, logger, file);
                        var result = first.Type != FieldType.NotSpecified ? first : second;
                        return result.Copy(columnName ?? binaryExpression.GetTokenText());
                    }

                case ConvertCall convertCall:
                    {
                        // TODO : is the result nullable if the column we're converting is nullable?
                        var isNullable = convertCall.Parameter.GetField(columnName, logger, file).IsNullable;
                        return convertCall.DataType.GetField(columnName, isNullable, logger, file);
                    }

                case TryConvertCall tryConvertCall:
                    {
                        // TODO : is the result nullable if the column we're converting is nullable?
                        var isNullable = tryConvertCall.Parameter.GetField(columnName, logger, file).IsNullable;
                        return tryConvertCall.DataType.GetField(columnName, isNullable, logger, file);
                    }

                case CastCall castCall:
                    {
                        // TODO : is the result nullable if the column we're casting is nullable?
                        var isNullable = castCall.Parameter.GetField(columnName, logger, file).IsNullable;
                        return castCall.DataType.GetField(columnName, isNullable, logger, file);
                    }

                case TryCastCall tryCastCall:
                    {
                        // TODO : is the result nullable if the column we're casting is nullable?
                        var isNullable = tryCastCall.Parameter.GetField(columnName, logger, file).IsNullable;
                        return tryCastCall.DataType.GetField(columnName, isNullable, logger, file);
                    }

                case FunctionCall functionCall:
                    return functionCall.GetField(columnName, logger, file);

                // TODO : consolidate left and right
                case LeftFunctionCall leftFunctionCall:
                    var leftField = leftFunctionCall.Parameters.Last().GetField(columnName, logger, file);
                    return new StringField()
                    {
                        Name = columnName,
                        IsNullable = false,
                        Length = (leftField as StringField)?.Length ?? 0,
                        Type = FieldType.String,
                    };

                case RightFunctionCall rightFunctionCall:
                    var rightField = rightFunctionCall.Parameters.Last().GetField(columnName, logger, file);
                    return new StringField()
                    {
                        Name = columnName,
                        IsNullable = false,
                        Length = (rightField as StringField)?.Length ?? 0,
                        Type = FieldType.String,
                    };

                case SearchedCaseExpression searchedCaseExpression:
                    {
                        var columns = searchedCaseExpression
                            .WhenClauses // TODO : do we care about these boolean expressions?
                            .Select(x => x.ThenExpression.GetField(columnName, logger, file))
                            .ToList();

                        var elseColumn = searchedCaseExpression
                            .ElseExpression
                            ?.GetField(columnName, logger, file);

                        if (elseColumn != null)
                        {
                            columns.Add(elseColumn);
                        }

                        // TODO : compute column type, do not return first..
                        // https://docs.microsoft.com/en-us/sql/t-sql/functions/cast-and-convert-transact-sql?view=sql-server-2017#implicit-conversions

                        return columns.First();
                    }

                case SimpleCaseExpression simpleCaseExpression:
                    {
                        var columns = simpleCaseExpression
                            .WhenClauses
                            .Select(x => x.ThenExpression.GetField(columnName, logger, file))
                            .ToList();

                        var elseColumn = simpleCaseExpression
                            .ElseExpression
	                        ?.GetField(columnName, logger, file);

                        if (elseColumn != null)
                        {
                            columns.Add(elseColumn);
                        }

                        // TODO : compute column type, do not return first..
                        return columns.First();
                    }

                case ScalarSubquery scalarSubquery:
                    {
                        var column = scalarSubquery
                            .QueryExpression
                            .GetFields(logger, file)
                            .First()
                            .Copy(columnName);

                        return column;
                    }

                case CoalesceExpression coalesceExpression:
                    {
                        var columns = coalesceExpression
                            .Expressions
                            .Select(x => x.GetField(columnName, logger, file))
                            .Where(x => x.Type != FieldType.NotSpecified)
                            .ToList();

                        // TODO : compute column type, do not return first..
                        // https://docs.microsoft.com/en-us/sql/t-sql/language-elements/coalesce-transact-sql?view=sql-server-2017
                        // Returns the data type of expression with the highest data type precedence. If all expressions are nonnullable, the result is typed as nonnullable.
                        return columns
                            .FirstOrDefault()
                            ?.Copy(columnName) ?? new UnknownField()
                            {
                                Name = columnName,
                            }; 
                    }

                case NullIfExpression nullIfExpression:
                    {
                        // Returns the same type as the first expression
                        return nullIfExpression
                            .FirstExpression
                            .GetField(columnName, logger, file)
                            .Copy(columnName);
                    }

                case AtTimeZoneCall atTimeZoneCall:
                    {
                        return atTimeZoneCall
                            .DateValue
                            .GetField(columnName, logger, file);
                    }

                case IIfCall iifCall:
                    {
                        var firstField = iifCall.ThenExpression.GetField(columnName, logger, file);
                        var secondField = iifCall.ElseExpression.GetField(columnName, logger, file);
                        // TODO : Returns the data type with the highest precedence from the types in true_value and false_value. For more information
                        return firstField;
                    }

                case ParameterlessCall parameterlessCall:
                    return parameterlessCall.GetField(columnName, logger, file);

                default:
                    {
                        logger.Log(LogLevel.Warning, 
                            LogType.NotSupportedYet,
                            file.Path, 
                            $"Unable to determine column type from scalar expression. Fragment: \"{scalarExpression.GetTokenText()}\"");

                        return new UnknownField()
                        {
                            Name = columnName,
                        };
                    }
            }
        }
    }
}
