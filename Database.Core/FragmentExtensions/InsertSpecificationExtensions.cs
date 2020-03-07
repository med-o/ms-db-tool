using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Contextes;
using Database.Core.Schema.References;

namespace Database.Core.FragmentExtensions
{
    public static class InsertSpecificationExtensions
    {
        public static IEnumerable<SchemaObjectReference> GetSchemaObjectReferences(
            this InsertSpecification insertSpecification,
            ILogger logger,
            SchemaFile file
        )
        {
            var targetReferences = insertSpecification
                .Target
                .GetSchemaObjectReferences(logger, file)
                .ToList();

            var insertSourceReferences = insertSpecification
                .InsertSource
                .GetSchemaObjectReferences(logger, file)
                .ToList();

            var targetReference = targetReferences.First();
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

            return targetReferences
                .Concat(insertSourceReferences)
                .Concat(outputIntoReferences)
                .ToList();
        }

        public static IList<FieldPairReference> GetFieldPairReferences(
            this InsertSpecification insertSpecification,
            ILogger logger,
            SchemaFile file
        )
        {
            var newReferences = insertSpecification
                .GetSchemaObjectReferences(logger, file)
                .ToList();

            using (new StatementContext(file.FileContext, newReferences))
            {
                var sourceColumns = insertSpecification
                    .InsertSource
                    .GetFields(logger, file)
                    .ToList();

                var target = insertSpecification
                    .Target
                    .GetSchemaObjectReferences(logger, file)
                    .First();

                var targetColumns = target
                    .Value
                    .Columns;

                var targetColumnsWithoutIdentity = targetColumns
                    .Where(x => !x.HasIdentity)
                    .ToList();

                var targetPairs = new List<FieldPairReference>();

                // it there are no source/target columns means we don't actually know schema definition for it.. so skip
                if (target.Value.Type != SchemaObjectType.NotSpecified && targetColumns.Any() && sourceColumns.Any())
                {
                    if (insertSpecification.Columns.Any())
                    {
                        var selectedColumns = insertSpecification.Columns
                            .Join(targetColumns, 
                                x => x.MultiPartIdentifier.Identifiers.Last().Value,
                                y => y.Name,
                                (x, y) => new { Fragment = x, Known = y },
                                StringComparer.InvariantCultureIgnoreCase)
                            .ToList();
                        
                        if (sourceColumns.Count == selectedColumns.Count)
                        {
                            targetPairs = selectedColumns
                                .Zip(sourceColumns, (t, s) => new FieldPairReference()
                                {
                                    Left = t.Known,
                                    Right = s,
                                    Fragment = t.Fragment,
                                })
                                .ToList();
                        }
                        else
                        {
                            logger.Log(LogLevel.Error, 
                                $"Can't match up columns in insert statement. " +
                                $"Count of columns selected from traget ({selectedColumns.Count}) " +
                                $"doesn't match with count of source columns ({sourceColumns.Count})");
                        }
                    }
                    else if (sourceColumns.Count == targetColumns.Count)
                    {
                        targetPairs = targetColumns
                            .Zip(sourceColumns, (t, s) => new FieldPairReference()
                            {
                                Left = t,
                                Right = s,
                                Fragment = insertSpecification.Target
                            })
                            .ToList();
                    }
                    else if (sourceColumns.Count == targetColumnsWithoutIdentity.Count)
                    {
                        targetPairs = targetColumnsWithoutIdentity
                            .Zip(sourceColumns, (t, s) => new FieldPairReference()
                            {
                                Left = t,
                                Right = s,
                                Fragment = insertSpecification.Target
                            })
                            .ToList();
                    }
                    else
                    {
                        logger.Log(LogLevel.Error, $"Can't match up columns in insert statement. Target: {targetColumns.Count} vs Source : {sourceColumns.Count}");
                    }
                }

                var insertSourcePairs = insertSpecification
                    .InsertSource
                    .GetFieldPairReferences(logger, file)
                    .ToList();

                var outputIntoPairs = insertSpecification
                    .OutputIntoClause
                    ?.GetFieldPairs(logger, file)
                    ?? new List<FieldPairReference>();

                return targetPairs
                    .Concat(insertSourcePairs)
                    .Concat(outputIntoPairs)
                    .ToList();
            }
        }
    }
}
