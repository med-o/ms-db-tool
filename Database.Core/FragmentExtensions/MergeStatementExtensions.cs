using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Contextes;
using Database.Core.Schema.References;

namespace Database.Core.FragmentExtensions
{
    public static class MergeStatementExtensions
    {
        public static IList<FieldPairReference> GetFieldPairs(
            this MergeStatement mergeStatement,
            ILogger logger,
            SchemaFile file
        )
        {
            var ctePairs = mergeStatement
                .WithCtesAndXmlNamespaces
                ?.CommonTableExpressions
                .GetFieldPairReferences(logger, file)
                .ToList()
                ?? new List<FieldPairReference>();

            var cteReferences = mergeStatement
                .WithCtesAndXmlNamespaces
                ?.CommonTableExpressions
                .GetSchemaObjectReferences(logger, file)
                .ToList()
                ?? new List<SchemaObjectReference>();

            using (new StatementContext(file.FileContext, cteReferences))
            {
                var mergeSpecificationPairs = mergeStatement
                    .MergeSpecification
                    .GetFieldPairReferences(logger, file)
                    .ToList();

                return ctePairs
                    .Concat(mergeSpecificationPairs)
                    .ToList();
            }
        }
    }
}
