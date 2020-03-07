using System.Collections.Generic;

namespace Database.Core.Validation.Settings
{
    public interface IValidationRuleSettingsRepository
    {
        IEnumerable<ValidationRuleSettings> GetSettings();
    }
}
