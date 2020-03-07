using System.Collections.Generic;

namespace Database.Core.Validation.Settings
{
    class ValidationRuleSettingsDatabaseRepository : IValidationRuleSettingsRepository
    {
        public IEnumerable<ValidationRuleSettings> GetSettings()
        {
            // TODO : hook up the DB, using hardcoded overrides for now
            throw new System.NotImplementedException();
        }
    }
}
