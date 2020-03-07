using System.Collections.Generic;
using System.Linq;
using Database.Core.Validation.Settings;

namespace Database.Core.Validation
{
    public class ValidationRulesFactory : IValidationRulesFactory
    {
        private readonly IEnumerable<IValidationRule> _validationRules;
        private readonly IValidationRuleSettingsRepository _ruleSettingsRepository;

        public ValidationRulesFactory(
            IEnumerable<IValidationRule> validationRules,
            IValidationRuleSettingsRepository ruleSettingsRepository
        ) 
        {
            _validationRules = validationRules;
            _ruleSettingsRepository = ruleSettingsRepository;
        }

        public IEnumerable<IValidationRule> Get()
        {
            var settings = _ruleSettingsRepository.GetSettings();
            var configuredRules = _validationRules
                .Join(settings, r => r.Type, s => s.Type, (r, s) => r.Configure(s))
                .ToList();

            return configuredRules;
        }
    }
}
