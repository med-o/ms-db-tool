using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.References;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.FragmentExtensions
{
    public static class SelectElementExtensions
    {
        public static IList<Field> GetFields(this IEnumerable<SelectElement> selectElements, 
            ILogger logger,
            SchemaFile file
        )
        {
            var columns = new List<Field>();

            foreach (var selectElement in selectElements)
            {
                switch (selectElement)
                {
                    case SelectStarExpression selectStarExpression:
                        {
                            if (selectStarExpression.Qualifier?.Count > 0)
                            {
                                var schemaObjectReferenceName = selectStarExpression.Qualifier.Identifiers.Last().Value;
                                var databaseObject = file.FileContext.GetSchema(schemaObjectReferenceName);
                                columns.AddRange(databaseObject.Columns);
                            }
                            else
                            {
                                // TODO : is it ok if I just take references from the top of the stack?
                                var allColumns = file
                                    .FileContext
                                    .StatementReferences
                                    .Peek()
                                    .SelectMany(x => x.Value.Columns)
                                    .ToList();

                                columns.AddRange(allColumns);
                            }
                            break;
                        }

                    case SelectScalarExpression selectScalarExpression:
                        {
                            // NOTE : column name can be null, that just means it's a ColumnReference statement and we will get the value later
                            var columnName = selectScalarExpression.ColumnName?.Value;
                            var column = selectScalarExpression
                                .Expression
                                .GetField(columnName, logger, file);

                            columns.Add(column);
                            break;
                        }

                    case SelectSetVariable selectSetVariable:
                        {
                            // A SELECT statement that assigns a value to a variable must not be combined with data-retrieval operations 
                            return new List<Field>();
                        }

                    default:
                        {
                            logger.Log(LogLevel.Warning, 
                                LogType.NotSupportedYet,
                                file.Path, 
                                $"\"{selectElement.GetType()}\" select element is not supported yet." +
                                $" Fragment: \"{selectElement.GetTokenText()}\"");
                            break;
                        }
                }
            }

            return columns;

            // TODO : using distinct here removes things like selecting NULL twice.. do I need to do any filtering?
            //return columns
            //    .Distinct(new KeyEqualityComparer<Field, string>(x => x.Name ?? string.Empty))
            //    .ToList();
        }

        public static IList<FieldPairReference> GetFieldPairs(this IEnumerable<SelectElement> selectElements,
            ILogger logger,
            SchemaFile file
        )
        {
            var pairs = new List<FieldPairReference>();

            foreach (var selectElement in selectElements)
            {
                switch (selectElement)
                {
                    case SelectSetVariable selectSetVariable:
                        {
                            // TODO : do we need to check selectSetVariable.AssignmentKind ?
                            var newValue = selectSetVariable.Expression.GetField(null, logger, file);
                            var variableToAssign = selectSetVariable.Variable.GetField(null, logger, file);

                            pairs.Add(new FieldPairReference()
                            {
                                Left = variableToAssign,
                                Right = newValue,
                                Fragment = selectSetVariable
                            });

                            break;
                        }

                    case SelectScalarExpression selectScalarExpression:

                        switch (selectScalarExpression.Expression)
                        {
                            case ScalarSubquery scalarSubquery:
                                return scalarSubquery
                                    .QueryExpression
                                    .GetFieldPairReferences(logger, file);
                        }

                        break;

                    default:
                        // A SELECT statement that does not assign a value to a variable can be ignored, nothing to compare
                        break;
                }
            }

            return pairs;
        }
    }
}