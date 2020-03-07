using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.FragmentExtensions;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.References;

namespace Database.Core.Validation.Rules
{
    public class ImplicitConversionInInsertStatement : ImplicitConversionRuleBase<InsertStatement>
    {
        public ImplicitConversionInInsertStatement(ILogger logger) : base(logger)
        {
        }
        
        protected override IEnumerable<FieldPairReference> GetFieldPairReferences(SchemaFile file, InsertStatement insertStatement)
        {
            var references = insertStatement
                .GetFieldPairs(Logger, file)
                .ToList();

            return references;
        }

        protected override string GetValidationMessage(FieldPairReference pair)
        {
            return $"{typeof(InsertSpecification).Name}: Column \"{pair.Left.Name}\" is of \"{pair.Left.Type}\" type and value is of \"{pair.Right.Type}\" type.";
        }
    }
}
