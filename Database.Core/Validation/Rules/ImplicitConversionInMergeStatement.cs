using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.FragmentExtensions;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.References;

namespace Database.Core.Validation.Rules
{
    public class ImplicitConversionInMergeStatement : ImplicitConversionRuleBase<MergeStatement>
    {
        public ImplicitConversionInMergeStatement(ILogger logger) : base(logger)
        {
        }
        
        protected override IEnumerable<FieldPairReference> GetFieldPairReferences(SchemaFile file, MergeStatement mergeStatement)
        {
            var references = mergeStatement
                .GetFieldPairs(Logger, file)
                .ToList();

            return references;
        }
    }
}
