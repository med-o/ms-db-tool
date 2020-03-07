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
    public static class SelectStatementExtensions
    {
        public static IList<Field> GetFields(
            this SelectStatement selectStatement,
            ILogger logger, 
            SchemaFile file
        )
        {
            var cteReferences = selectStatement
                .WithCtesAndXmlNamespaces
                ?.CommonTableExpressions
                .GetSchemaObjectReferences(logger, file)
                .ToList()
                ?? new List<SchemaObjectReference>();

            using (new StatementContext(file.FileContext, cteReferences))
            {
                var columns = selectStatement
                    .QueryExpression
                    .GetFields(logger, file)
                    .ToList();

                return columns;
            }
        }

        public static IList<FieldPairReference> GetFieldPairs(
            this SelectStatement selectStatement,
            ILogger logger,
            SchemaFile file
        )
        {
            // TODO : make use of cteReferences below? or would that mean the scope can get polluted?
            var ctePairs = selectStatement
                .WithCtesAndXmlNamespaces
                ?.CommonTableExpressions
                .GetFieldPairReferences(logger, file)
                .ToList()
                ?? new List<FieldPairReference>();

            var cteReferences = selectStatement
                .WithCtesAndXmlNamespaces
                ?.CommonTableExpressions
                .GetSchemaObjectReferences(logger, file)
                .ToList()
                ?? new List<SchemaObjectReference>();

            using (new StatementContext(file.FileContext, cteReferences)) 
            {
                var queryExpressionPairs = selectStatement
                    .QueryExpression
                    .GetFieldPairReferences(logger, file)
                    .ToList();

                return ctePairs
                    .Concat(queryExpressionPairs)
                    .ToList();
            }
        }
    }
}
