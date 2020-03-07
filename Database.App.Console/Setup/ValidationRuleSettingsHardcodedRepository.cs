using System.Collections.Generic;
using Database.Core.Logging;
using Database.Core.Validation;
using Database.Core.Validation.Settings;

namespace Database.App.Console.Setup
{
    public class ValidationRuleSettingsHardcodedRepository : IValidationRuleSettingsRepository
    {
        public IEnumerable<ValidationRuleSettings> GetSettings()
        {
            return new List<ValidationRuleSettings>()
            {
                // NOTE : focusing on implicit conversion for now, other rules would create too much noise

                new ValidationRuleSettings()
                {
                    Type = ValidationRuleType.ImplicitConversion,
                    Level = LogLevel.Error,
                    Enabled = true,
                    Name = "implicit_conversion",
                    Label = "Implicit conversion",
                    Description = "Implicit conversion adds unnecessary overhead and can impact performance.",
                },

                //new ValidationRuleSettings()
                //{
                //    Type = ValidationRuleType.FileNameCorrespondsToSchemaObject,
                //    Level = LogLevel.Error,
                //    Enabled = true,
                //    Name = "FileNameCorrespondsToSchemaObject",
                //    Label = "File name differs from schema object it creates",
                //    Description = "Does not follow naming convetion.",
                //},

                new ValidationRuleSettings()
                {
                    Type = ValidationRuleType.DecimalFieldWithDefaultValues,
                    Level = LogLevel.Warning,
                    Enabled = true,
                    Name = "DecimalFieldWithDefaultValues",
                    Label = "Decimal field with default values",
                    Description = "Default is DECIMAL(18, 0) which means decimal part is truncated.",
                },

                new ValidationRuleSettings()
                {
                    Type = ValidationRuleType.NoStringColumnWithDefaultLength,
                    Level = LogLevel.Warning,
                    Enabled = true,
                    Name = "NoStringColumnWithDefaultLength",
                    Label = "No string column with default length",
                    Description = "Default length is VARCHAR(30) or VARCHAR(1) for local variables which means string can be truncated.",
                },
            };
        }
    }
}
