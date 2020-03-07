using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.FragmentExtensions;
using Database.Core.Logging;
using Database.Core.Schema;

namespace Database.Core.Validation.Rules
{
    public class SchemaNotDefined : ValidationRule<SchemaObjectName>
    {
        public SchemaNotDefined(ILogger logger) : base(logger)
        {
        }

        public override ValidationRuleType Type => ValidationRuleType.SchemaNotDefined;

        public override IList<ValidationResult> Execute(SchemaFile file)
        {
            return Fragments
                .Where(SchemaIdentifierIsEmpty)
                .ToValidationResults();
        }

        private static bool SchemaIdentifierIsEmpty(SchemaObjectName fragment)
        {
            // TODO : this incorrectly flags SP params and CTEs, I need access to parent fragment.. 
            // use DatabaseFile to access it? or can I somehow navigate up one level?
            return fragment.SchemaIdentifier == null || fragment.SchemaIdentifier.Value == string.Empty;
        }
    }
}
