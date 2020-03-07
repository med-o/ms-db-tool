using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.References;

namespace Database.Core.FragmentExtensions
{
    public static class MergeActionExtensions
    {
        public static IList<FieldPairReference> GetFieldPairReferences(
            this MergeAction mergeAction,
            ILogger logger,
            SchemaFile file
        )
        {
            switch (mergeAction)
            {
                case InsertMergeAction insertMergeAction:
                    return insertMergeAction
                        .Source
                        .GetFieldPairReferences(logger, file);
                case UpdateMergeAction updateMergeAction:
                    return updateMergeAction
                        .SetClauses
                        .GetFieldPairs(logger, file);
                default:
                    return new List<FieldPairReference>();
            }
        }
    }
}
