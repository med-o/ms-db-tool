using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.References;

namespace Database.Core.FragmentExtensions
{
    public static class BooleanExpressionExtensions
    {
        public static IList<FieldPairReference> GetFieldPairs(this BooleanExpression booleanExpression,
            ILogger logger,
            SchemaFile file
        )
        {
            switch (booleanExpression)
            {
                case BooleanComparisonExpression booleanComparisonExpression:
                    {
                        var column1 = booleanComparisonExpression.FirstExpression.GetField(string.Empty, logger, file);
                        var column2 = booleanComparisonExpression.SecondExpression.GetField(string.Empty, logger, file);

                        return new List<FieldPairReference>()
                        {
                            new FieldPairReference() {
                                Left = column1,
                                Right = column2,
                                Fragment = booleanComparisonExpression
                            }
                        };
                    }

                case BooleanBinaryExpression booleanBinaryExpression:
                    {
                        // TODO : and / or .. check the type? does it matter here?
                        var firstExpressionColumns = booleanBinaryExpression.FirstExpression.GetFieldPairs(logger, file);
                        var secondExpressionColumns = booleanBinaryExpression.SecondExpression.GetFieldPairs(logger, file);

                        return firstExpressionColumns.Concat(secondExpressionColumns).ToList();
                    }

                case BooleanTernaryExpression booleanTernaryExpression:
                    {
                        var x = booleanTernaryExpression.FirstExpression.GetField(null, logger, file);
                        var y = booleanTernaryExpression.SecondExpression.GetField(null, logger, file);
                        var z = booleanTernaryExpression.ThirdExpression.GetField(null, logger, file);

                        return new List<FieldPairReference>()
                        {
                            new FieldPairReference() { Left = x, Right = y, Fragment = booleanTernaryExpression },
                            new FieldPairReference() { Left = x, Right = z, Fragment = booleanTernaryExpression },
                            new FieldPairReference() { Left = y, Right = z, Fragment = booleanTernaryExpression },
                        };
                    }

                case InPredicate inPredicate:
                    {
                        var column = inPredicate.Expression.GetField(null, logger, file);
                        var fieldPairs = inPredicate
                            .Values
                            .Select(x =>
                            {
                                var field = x.GetField(null, logger, file);
                                return new FieldPairReference() { Left = column, Right = field, Fragment = inPredicate };
                            })
                            .ToList();

                        return fieldPairs;
                    }

                case BooleanParenthesisExpression booleanParenthesisExpression:
                    return booleanParenthesisExpression
                        .Expression
                        .GetFieldPairs(logger, file);

                case LikePredicate likePredicate:
                    {
                        var x = likePredicate.FirstExpression.GetField(null, logger, file);
                        var y = likePredicate.SecondExpression.GetField(null, logger, file);
                        return new List<FieldPairReference>()
                        {
                            new FieldPairReference() { Left = x, Right = y, Fragment = likePredicate },
                        };
                    }

                // we don't care about these for now
                case BooleanIsNullExpression b: break;
                case BooleanNotExpression c: break;
                case ExistsPredicate f: break;
                case FullTextPredicate g: break;

                case BooleanExpressionSnippet a:
                case EventDeclarationCompareFunctionParameter e:
                case SubqueryComparisonPredicate j:
                case TSEqualCall k:
                case UpdateCall l:
                default:
                {
                    logger.Log(LogLevel.Warning, 
                        LogType.NotSupportedYet,
                        file.Path, 
                        $"\"{booleanExpression.GetType()}\" boolean expression is not supported yet.");
                    break;
                }
            }

            return new List<FieldPairReference>();
        }
    }
}