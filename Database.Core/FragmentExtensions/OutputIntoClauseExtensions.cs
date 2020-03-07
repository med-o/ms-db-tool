using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.References;

namespace Database.Core.FragmentExtensions
{
    public static class OutputIntoClauseExtensions
    {
        public static IList<FieldPairReference> GetFieldPairs(
            this OutputIntoClause outputIntoClause,
            ILogger logger,
            SchemaFile file
        )
        {
            var intoFields = outputIntoClause
                .IntoTable
                .GetSchemaObjectReferences(logger, file)
                .First()
                .Value
                .Columns;

            var sourceFields = outputIntoClause
                .SelectColumns
                .GetFields(logger, file)
                .ToList();

            return intoFields
                .Zip(sourceFields, (target, source) => new FieldPairReference()
                {
                    Left = target,
                    Right = source,
                    Fragment = outputIntoClause
                })
                .ToList();
        }
    }
}
