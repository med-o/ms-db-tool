using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.FragmentExtensions;
using Database.Core.Logging;
using Database.Core.Schema;

namespace Database.Core.Validation.Rules
{
    public class DecimalFieldWithDefaultValues : ValidationRule<SqlDataTypeReference>
    {
        public DecimalFieldWithDefaultValues(ILogger logger) : base(logger)
        {
        }

        public override ValidationRuleType Type => ValidationRuleType.DecimalFieldWithDefaultValues;

        public override IList<ValidationResult> Execute(SchemaFile file)
        {
            // When there are no parameters for these types default will be used, default scale is 0 so flag it
            return Fragments
                .Where(x => (x.SqlDataTypeOption == SqlDataTypeOption.Decimal || x.SqlDataTypeOption == SqlDataTypeOption.Numeric) && !x.Parameters.Any())
                .ToValidationResults();
        }
    }
}
