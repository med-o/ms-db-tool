using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.FragmentExtensions;
using Database.Core.Logging;
using Database.Core.Schema;

namespace Database.Core.Validation.Rules
{
    public class NoSelectStart : ValidationRule<SelectStarExpression>
    {
        public NoSelectStart(ILogger logger) : base(logger)
        {
        }

        public override ValidationRuleType Type => ValidationRuleType.NoSelectStart;

        public override IList<ValidationResult> Execute(SchemaFile file)
        {
            return Fragments.ToValidationResults();
        }
    }
}
