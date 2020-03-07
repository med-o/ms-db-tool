using Database.Core.Logging;

namespace Database.Core.Validation.Settings
{
    public class ValidationRuleSettings
    {
        public ValidationRuleType Type { get; set; }
        public LogLevel Level { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
    }
}
