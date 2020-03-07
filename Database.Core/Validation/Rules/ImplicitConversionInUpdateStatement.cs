using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.FragmentExtensions;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.References;

namespace Database.Core.Validation.Rules
{
    public class ImplicitConversionInUpdateStatement : ImplicitConversionRuleBase<UpdateStatement>
    {
        public ImplicitConversionInUpdateStatement(ILogger logger) : base(logger)
        {
        }
        
        protected override IEnumerable<FieldPairReference> GetFieldPairReferences(SchemaFile file, UpdateStatement updateStatement)
        {
            var references = updateStatement
                .GetFieldPairs(Logger, file)
                .ToList();

            return references;
        }
    }
}
