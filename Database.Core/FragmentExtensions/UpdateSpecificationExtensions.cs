using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Contextes;
using Database.Core.Schema.References;

namespace Database.Core.FragmentExtensions
{
    public static class UpdateSpecificationExtensions
    {
        public static IEnumerable<SchemaObjectReference> GetSchemaObjectReferences(
            this UpdateSpecification updateSpecification,
            ILogger logger,
            SchemaFile file
        )
        {
            var databaseObjectReferences = updateSpecification
                .FromClause
                ?.TableReferences
                .GetSchemaObjectReferences(logger, file)
                .ToList()
                ?? new List<SchemaObjectReference>();

            SchemaObjectReference targetReference;
            using (new StatementContext(file.FileContext, databaseObjectReferences))
            {
                targetReference = updateSpecification
                        .Target
                        .GetSchemaObjectReferences(logger, file)
                        .First();
            }

            // TODO : would it be a big deal if I add it twice? it would simplify the logic here
            if (!databaseObjectReferences
                    .Any(x => string.Equals(x.Alias, targetReference.Alias)
                        && string.Equals(x.Identifier, targetReference.Identifier)))
            {
                databaseObjectReferences.Add(targetReference);
            }

            var outputIntoReferences = new List<SchemaObjectReference>() {
                new SchemaObjectReference()
                {
                    Alias = "inserted",
                    Identifier = targetReference.Identifier,
                    Value = targetReference.Value
                },
                new SchemaObjectReference()
                {
                    Alias = "deleted",
                    Identifier = targetReference.Identifier,
                    Value = targetReference.Value
                }
            };

            return databaseObjectReferences
                .Concat(outputIntoReferences)
                .ToList();
        }

        public static IList<FieldPairReference> GetFieldPairReferences(
            this UpdateSpecification updateSpecification,
            ILogger logger,
            SchemaFile file
        )
        {
            var newReferences = updateSpecification
                .GetSchemaObjectReferences(logger, file)
                .ToList();

            using (new StatementContext(file.FileContext, newReferences))
            {
                var setClausePairs = updateSpecification
                    .SetClauses
                    .GetFieldPairs(logger, file)
                    .ToList();

                var fromCalusePairs = updateSpecification
                    .FromClause
                    ?.TableReferences
                    .GetFieldPairs(logger, file)
                    ?? new List<FieldPairReference>();

                var whereClauePairs = updateSpecification
                    .WhereClause
                    ?.SearchCondition
                    .GetFieldPairs(logger, file)
                    ?? new List<FieldPairReference>();

                var outputIntoPairs = updateSpecification
                    .OutputIntoClause
                    ?.GetFieldPairs(logger, file)
                    ?? new List<FieldPairReference>();

                return setClausePairs
                    .Concat(fromCalusePairs)
                    .Concat(whereClauePairs)
                    .Concat(outputIntoPairs)
                    .ToList();
            }
        }
    }
}
