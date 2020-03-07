using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Contextes;
using Database.Core.Schema.References;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.FragmentExtensions
{
    public static class QueryExpressionExtensions
    {
        public static IList<Field> GetFields(
            this QueryExpression queryExpression, 
            ILogger logger, 
            SchemaFile file
        )
        {
            switch (queryExpression)
            {
                case QuerySpecification querySpecification:

                    var newReferences = queryExpression
                        .GetSchemaObjectReferences(logger, file)
                        .ToList();

                    using (new StatementContext(file.FileContext, newReferences))
                    {
                        return querySpecification.ForClause != null
                            ? querySpecification
                                .ForClause
                                .GetFields(logger, file)
                            : querySpecification
                                .SelectElements
                                .GetFields(logger, file);
                    }

                case BinaryQueryExpression binaryQueryExpression:
                    // TODO : When data types differ, the resulting data type is determined based on the rules for data type precedence
                    // TODO : do I need different behaviour for various types? Union, Except, Intersect
                    return binaryQueryExpression
                        .FirstQueryExpression
                        .GetFields(logger, file);

                case QueryParenthesisExpression queryParenthesisExpression:
                    return queryParenthesisExpression
                        .QueryExpression
                        .GetFields(logger, file);

                default:
                    logger.Log(LogLevel.Warning, 
                        LogType.NotSupportedYet,
                        file.Path, 
                        $"\"{queryExpression.GetType()}\" query expression is not supported yet.");
                    return new List<Field>();
            }
        }

        public static IList<SchemaObjectReference> GetSchemaObjectReferences(
            this QueryExpression queryExpression,
            ILogger logger,
            SchemaFile file
        )
        {
            switch (queryExpression)
            {
                case QuerySpecification querySpecification:
                    return querySpecification
                        .FromClause
                        ?.TableReferences
                        .GetSchemaObjectReferences(logger, file)
                        .ToList()
                        ?? new List<SchemaObjectReference>();

                case BinaryQueryExpression binaryQueryExpression:
                    // TODO : When data types differ, the resulting data type is determined based on the rules for data type precedence
                    // TODO : do I need different behaviour for various types? Union, Except, Intersect
                    return binaryQueryExpression
                        .FirstQueryExpression
                        .GetSchemaObjectReferences(logger, file);

                case QueryParenthesisExpression queryParenthesisExpression:
                    return queryParenthesisExpression
                        .QueryExpression
                        .GetSchemaObjectReferences(logger, file);

                default:
                    logger.Log(LogLevel.Warning, 
                        LogType.NotSupportedYet,
                        file.Path, 
                        $"\"{queryExpression.GetType()}\" query expression is not supported yet.");
                    return new List<SchemaObjectReference>();
            }
        }

        public static IList<FieldPairReference> GetFieldPairReferences(
            this QueryExpression queryExpression,
            ILogger logger,
            SchemaFile file
        )
        {
            switch (queryExpression)
            {
                case QuerySpecification querySpecification:
                    var newReferences = queryExpression
                        .GetSchemaObjectReferences(logger, file)
                        .ToList();

                    using (new StatementContext(file.FileContext, newReferences))
                    {
                        return querySpecification.GetFieldPairReferences(logger, file);
                    }

                case BinaryQueryExpression binaryQueryExpression:

                    var firstQueryExpressionPairs = binaryQueryExpression
                        .FirstQueryExpression
                        .GetFieldPairReferences(logger, file);
                    var secondQueryExpressionPairs = binaryQueryExpression
                        .SecondQueryExpression
                        .GetFieldPairReferences(logger, file);

                    return firstQueryExpressionPairs
                        .Concat(secondQueryExpressionPairs)
                        .ToList();

                case QueryParenthesisExpression queryParenthesisExpression:
                    return queryParenthesisExpression
                        .QueryExpression
                        .GetFieldPairReferences(logger, file);

                default:
                    logger.Log(LogLevel.Warning, 
                        LogType.NotSupportedYet,
                        file.Path, 
                        $"\"{queryExpression.GetType()}\" query expression is not supported yet.");
                    return new List<FieldPairReference>();
            }
        }
    }
}
