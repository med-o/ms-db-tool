using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.FragmentExtensions
{
    public static class FunctionCallExtensions
    {
        public static Field GetField(this FunctionCall functionCall,
            string columnName,
            ILogger logger, 
            SchemaFile file
        )
        {
            var functionName = functionCall.FunctionName.Value;

            columnName = columnName ?? functionCall.GetTokenText();

            // scalar valued system functions
            switch (functionName.ToUpper())
            {
                case "SCOPE_IDENTITY":
                case "@@IDENTITY": // TODO : this is global variable and not a function
                    {
                        // https://docs.microsoft.com/en-us/sql/t-sql/functions/scope-identity-transact-sql
                        return new DecimalField()
                        {
                            Name = columnName,
                            IsNullable = false,
                            Precision = 38,
                            Scale = 0,
                            Origin = OriginType.FunctionReturn,
                        };
                    }
                case "NEWID":
                    return new DefaultField()
                    {
                        Name = columnName,
                        IsNullable = false,
                        Type = FieldType.UniqueIdentifier,
                        Origin = OriginType.FunctionReturn,
                    };
                case "COUNT":
                case "BINARY_CHECKSUM":
                case "DATEDIFF":
                case "OBJECTPROPERTY":
                case "ERROR_NUMBER":
                case "ERROR_SEVERITY":
                case "ERROR_STATE":
                case "ERROR_LINE":
                case "DB_ID":
                    return new DefaultField()
                    {
                        Name = columnName,
                        IsNullable = false,
                        Type = FieldType.Int,
                        Origin = OriginType.FunctionReturn,
                    };
                case "ERROR_MESSAGE":
                    return new StringField()
                    {
                        Name = columnName,
                        IsNullable = false,
                        Type = FieldType.String, // NVARCHAR
                        Origin = OriginType.FunctionReturn,
                        Length = 4000,
                    };
                case "ERROR_PROCEDURE":
                case "HOST_NAME":
                case "SUSER_SNAME":
                case "DATEPART":
                    return new StringField()
                    {
                        Name = columnName,
                        IsNullable = false,
                        Type = FieldType.String, // NVARCHAR
                        Origin = OriginType.FunctionReturn,
                        Length = 128,
                    };
                case "COUNT_BIG":
                case "ROW_NUMBER":
                case "RANK":
                case "DENSE_RANK":
                case "NTILE":
                    return new DefaultField()
                    {
                        Name = columnName,
                        IsNullable = false,
                        Type = FieldType.BigInt,
                        Origin = OriginType.FunctionReturn,
                    };
                case "LOG":
                case "PERCENTILE_CONT":
                case "PERCENTILE_DISC":
                case "PERCENT_RANK":
                case "STDEV":
                case "STDEVP":
                case "RAND":
                    return new DefaultField()
                    {
                        Name = columnName,
                        IsNullable = false,
                        Type = FieldType.Float, // TODO : some are FLOAT and some FLOAT(53), does that matter?
                        Origin = OriginType.FunctionReturn,
                    };
                case "GETDATE":
                case "DATEADD":
                case "GETUTCDATE":
                    return new DefaultField()
                    {
                        Name = columnName,
                        IsNullable = false,
                        Type = FieldType.DateTime,
                        Origin = OriginType.FunctionReturn,
                    };
                case "DATENAME":
                    return new StringField()
                    {
                        Name = columnName,
                        IsNullable = false,
                        Type = FieldType.String,
                        Origin = OriginType.FunctionReturn,
                        Length = 0, // TODO..
                    };
                case "SYSDATETIME":
                    return new DefaultField()
                    {
                        Name = columnName,
                        IsNullable = false,
                        Type = FieldType.DateTime2, // TODO : datetime2(7) .. does that matter?
                        Origin = OriginType.FunctionReturn,
                    };
                case "TRIM":
                case "LTRIM":
                case "RTRIM":
                case "UPPER":
                case "LOWER":
                case "SUM": // TODO : Returns the summation of all expression values in the most precise expression data type.
                case "MIN":
                case "MAX":
                    {
                        return functionCall
                            .Parameters
                            .First()
                            .GetField(columnName, logger, file);
                    }
                case "AVG": // The evaluated result of expression determines the return type
                    {
                        var column = functionCall
                            .Parameters
                            .First()
                            .GetField(columnName, logger, file);

                        // https://docs.microsoft.com/en-us/sql/t-sql/functions/avg-transact-sql
                        switch (column.Type)
                        {
                            case FieldType.SmallInt:
                            case FieldType.TinyInt:
                                column.Type = FieldType.Int;
                                break;
                            case FieldType.Decimal:
                            case FieldType.Numeric:
                                // TODO : compute the value, this will return decimal(p, s) for now
                                // decimal category (p, s) returns decimal(38, s) divided by decimal(10, 0)
                                break;
                            case FieldType.SmallMoney:
                                column.Type = FieldType.Money;
                                break;
                            case FieldType.Real:
                                column.Type = FieldType.Float;
                                break;
                        }

                        return column;
                    }
                case "STUFF":
                {
                    // TODO : this can return binary data as well
                    // TODO : can I determine length from character_expression and replaceWith_expression?
                    return new StringField()
                    {
                        Name = columnName,
                        IsNullable = false,
                        Type = FieldType.String,
                        Origin = OriginType.FunctionReturn,
                        Length = 0,
                    };
                }
                case "LEN":
                case "CHARINDEX":
                    // TODO : bigint if expression is nvarchar(max), varbinary(max), or varchar(max) data type; int otherwise
                    return new DefaultField()
                    {
                        Name = columnName,
                        IsNullable = false,
                        Type = FieldType.Int,
                        Origin = OriginType.FunctionReturn,
                    };
                case "OBJECT_NAME":
                    // this is the SYSNAME type
                    return new StringField()
                    {
                        Name = columnName,
                        Type = FieldType.String, // nvarchar
                        Origin = OriginType.SystemType,
                        Length = 128,
                        IsNullable = false,
                    };

                default:
                    break;
            }

            // scalar valued user functions
            //Microsoft.SqlServer.TransactSql.ScriptDom.ExpressionCallTarget
            //Microsoft.SqlServer.TransactSql.ScriptDom.MultiPartIdentifierCallTarget
            //Microsoft.SqlServer.TransactSql.ScriptDom.UserDefinedTypeCallTarget
            var functionKey = string.Empty;

            if (functionCall.CallTarget is MultiPartIdentifierCallTarget multiPartIdentifierCallTarget)
            {
                // TODO : make this pretty somehow
                var identifiersCount = multiPartIdentifierCallTarget.MultiPartIdentifier.Identifiers.Count();
                var qualifiedIdentifiers = new List<string>();

                if (identifiersCount.Equals(1))
                {
                    qualifiedIdentifiers.Add(file.Context.Name);
                    qualifiedIdentifiers.Add(multiPartIdentifierCallTarget.MultiPartIdentifier.Identifiers.First().Value);
                }
                else if (identifiersCount.Equals(2))
                {
                    qualifiedIdentifiers.AddRange(multiPartIdentifierCallTarget.MultiPartIdentifier.Identifiers.Select(x => x.Value));
                }
                else
                {
                    logger.Log(LogLevel.Error, $"Unable to determine qualified identifier. Fragment: \"{functionCall.GetTokenText()}\"");
                }
                qualifiedIdentifiers.Add(functionName);

                functionKey = qualifiedIdentifiers.GetQualifiedIdentfier();
            }

            if (file.Schema.ContainsKey(functionKey))
            {
                var function = file.Schema[functionKey];
                var firstColumn = function.Columns.FirstOrDefault();
                if (firstColumn != null)
                {
                    return firstColumn.Copy(columnName);
                }
            }

            // TODO : table valued user functions

            // TODO : failsafe but do not return first.. compute the value
            var first = functionCall
                .Parameters
                .Select(x => x.GetField(columnName, logger, file))
                .FirstOrDefault(x => x.Type != FieldType.NotSpecified);

            if (first != null)
            {
                return first;
            }

            logger.Log(LogLevel.Error, $"Unable to determine column type from function call. Fragment: \"{functionCall.GetTokenText()}\"");

            return new UnknownField()
            {
                Name = columnName,
            };
        }
    }
}
