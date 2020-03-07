using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.References;

namespace Database.Core.FragmentExtensions
{
    public static class ExecuteStatementExtensions
    {
        public static IList<FieldPairReference> GetFieldPairs(
            this ExecuteStatement executeStatement,
            ILogger logger,
            SchemaFile file
        )
        {
            var executeSpecificationPairs = executeStatement
                .ExecuteSpecification
                .GetFieldPairReferences(logger, file)
                .ToList();

            return executeSpecificationPairs;
        }
    }
}
