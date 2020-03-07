using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Contextes;
using Database.Core.Schema.References;

namespace Database.Core.FragmentExtensions
{
    public static class MergeSpecificationExtensions
    {
        public static IEnumerable<SchemaObjectReference> GetSchemaObjectReferences(
            this MergeSpecification mergeSpecification,
            ILogger logger,
            SchemaFile file
        )
        {
            var targetReference = mergeSpecification
                .Target
                .GetSchemaObjectReferences(logger, file)
                .First();

            targetReference.Alias = targetReference.Alias ?? mergeSpecification.TableAlias?.Value;

            var tableReferenceReferences = mergeSpecification
                .TableReference
                .GetSchemaObjectReferences(logger, file)
                .ToList();

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

            return new List<SchemaObjectReference>() { targetReference }
                .Concat(tableReferenceReferences)
                .Concat(outputIntoReferences)
                .ToList();
        }

        public static IList<FieldPairReference> GetFieldPairReferences(
            this MergeSpecification mergeSpecification,
            ILogger logger,
            SchemaFile file
        )
        {
            // TODO : find out why I have missing references.. 

            var newReferences = mergeSpecification
                .GetSchemaObjectReferences(logger, file)
                .ToList();

            using (new StatementContext(file.FileContext, newReferences))
            {
                var searchConditionPairs = mergeSpecification
                    .SearchCondition
                    .GetFieldPairs(logger, file);

                var actionClausesReferences = mergeSpecification
                    .ActionClauses
                    .SelectMany(actionClause =>
                    {
                        var innerSearchConditionPairs = actionClause
                            .SearchCondition
                            ?.GetFieldPairs(logger, file)
                            ?? new List<FieldPairReference>();

                        var actionPairs = actionClause
                            .Action
                            .GetFieldPairReferences(logger, file);

                        return innerSearchConditionPairs
                            .Concat(actionPairs)
                            .ToList();
                    })
                    .ToList();
                
                var outputIntoPairs = mergeSpecification
                    .OutputIntoClause
                    ?.GetFieldPairs(logger, file)
                    ?? new List<FieldPairReference>();

                return searchConditionPairs
                    .Concat(actionClausesReferences)
                    .Concat(outputIntoPairs)
                    .ToList();
            }
        }
    }
}
