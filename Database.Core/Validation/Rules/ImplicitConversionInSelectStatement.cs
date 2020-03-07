using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.FragmentExtensions;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.References;

namespace Database.Core.Validation.Rules
{
    public class ImplicitConversionInSelectStatement : ImplicitConversionRuleBase<SelectStatement>
    {
        public ImplicitConversionInSelectStatement(ILogger logger) : base(logger)
        {
        }

        protected override IEnumerable<FieldPairReference> GetFieldPairReferences(SchemaFile file, SelectStatement selectStatement)
        {
            var references = selectStatement
                .GetFieldPairs(Logger, file)
                .ToList();
            
            return references;
        }
    }
}
