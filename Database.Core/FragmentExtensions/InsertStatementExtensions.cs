using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Contextes;
using Database.Core.Schema.References;

namespace Database.Core.FragmentExtensions
{
    public static class InsertStatementExtensions
    {
        public static IList<FieldPairReference> GetFieldPairs(
            this InsertStatement insertStatement,
            ILogger logger,
            SchemaFile file
        )
        {
            // TODO : make use of cteReferences below? or would that mean the scope can get polluted?
            var ctePairs = insertStatement
                .WithCtesAndXmlNamespaces
                ?.CommonTableExpressions
                .GetFieldPairReferences(logger, file)
                .ToList()
                ?? new List<FieldPairReference>();

            var cteReferences = insertStatement
                .WithCtesAndXmlNamespaces
                ?.CommonTableExpressions
                .GetSchemaObjectReferences(logger, file)
                .ToList()
                ?? new List<SchemaObjectReference>();

            using (new StatementContext(file.FileContext, cteReferences))
            {
                var insertSpecificationPairs = insertStatement
                    .InsertSpecification
                    .GetFieldPairReferences(logger, file)
                    .ToList();

                return ctePairs
                    .Concat(insertSpecificationPairs)
                    .ToList();
            }
        }
    }
}
