using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.References;

namespace Database.Core.FragmentExtensions
{
    public static class QuerySpecificationExtensions
    {
        public static IList<FieldPairReference> GetFieldPairReferences(
            this QuerySpecification querySpecification, 
            ILogger logger,
            SchemaFile file
        )
        {
            var selectVariablePairs = querySpecification
                .SelectElements
                .GetFieldPairs(logger, file);

            var fromCalusePairs = querySpecification
                .FromClause
                ?.TableReferences
                .GetFieldPairs(logger, file)
                ?? new List<FieldPairReference>();

            var whereClausePairs = querySpecification
                .WhereClause
                ?.SearchCondition
                .GetFieldPairs(logger, file)
                ?? new List<FieldPairReference>();
            
            return selectVariablePairs
                .Concat(fromCalusePairs)
                .Concat(whereClausePairs)
                .ToList();
        }
    }
}
