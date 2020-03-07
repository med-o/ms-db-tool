using System.Collections.Generic;
using Database.Core.Schema;
using Database.Core.Validation.Settings;

namespace Database.Core.Validation
{
    public interface IValidationRule
    {
        ValidationRuleType Type { get; }

        ValidationRuleSettings Settings { get; }

        IValidationRule Configure(ValidationRuleSettings settings);

        IList<ValidationResult> Validate(SchemaFile file);
    }
}
