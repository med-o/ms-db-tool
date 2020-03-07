using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.FragmentExtensions;
using Database.Core.Logging;
using Database.Core.Schema;

namespace Database.Core.Validation.Rules
{
    public class NoStringColumnWithDefaultLength : ValidationRule<SqlDataTypeReference>
    {
        public NoStringColumnWithDefaultLength(ILogger logger) : base(logger)
        {
        }

        public override ValidationRuleType Type => ValidationRuleType.NoStringColumnWithDefaultLength;

        public override IList<ValidationResult> Execute(SchemaFile file)
        {
            // TODO : when it's variable declaration it will trim it at 1 otherwise at 30 characters
            return Fragments
                .Where(x =>
                    (x.SqlDataTypeOption == SqlDataTypeOption.Char
                    || x.SqlDataTypeOption == SqlDataTypeOption.NChar
                    || x.SqlDataTypeOption == SqlDataTypeOption.VarChar
                    || x.SqlDataTypeOption == SqlDataTypeOption.NVarChar)
                    && !x.Parameters.Any())
                .ToValidationResults();
        }
    }
}
