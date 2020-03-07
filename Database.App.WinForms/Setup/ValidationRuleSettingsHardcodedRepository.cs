using System.Collections.Generic;
using Database.Core.Logging;
using Database.Core.Validation;
using Database.Core.Validation.Settings;

namespace Database.App.WinForms.Setup
{
    public class ValidationRuleSettingsHardcodedRepository : IValidationRuleSettingsRepository
    {
        public IEnumerable<ValidationRuleSettings> GetSettings()
        {
            return new List<ValidationRuleSettings>()
            {
                new ValidationRuleSettings()
                {
                    Type = ValidationRuleType.ImplicitConversion,
                    Level = LogLevel.Error,
                    Enabled = true,
                    Name = "no_implicit_conversion",
                    Label = "No implicit conversion",
                    Description = "Implicit conversion adds unnecessary overhead and impacts performance.",
                },
                new ValidationRuleSettings()
                {
                    Type = ValidationRuleType.NoLeadingWildcard,
                    Level = LogLevel.Error,
                    Enabled = true,
                    Name = "no_leading_wildcard",
                    Label = "No \"LIKE '%wildcard'\"",
                    Description = "Avoid like expression with leading wildcard because it may result in full table scan.",
                },
                new ValidationRuleSettings()
                {
                    Type = ValidationRuleType.NoSelectStart,
                    Level = LogLevel.Warning,
                    Enabled = true,
                    Name = "no_select_star",
                    Label = "No \"SELECT *\"",
                    Description = "You should not use \"SELECT *\" expression because it will most likely result in key look-ups",
                },
                new ValidationRuleSettings()
                {
                    Type = ValidationRuleType.MissingColumnPrefix,
                    Level = LogLevel.Warning,
                    Enabled = true,
                    Name = "no_column_prefix",
                    Label = "Missing column prefix",
                    Description = "All columns should be prefixed, this is mostly for readability.",
                },
                //new ValidationRuleSettings()
                //{
                //    Type = ValidationRuleType.RoundingFound,
                //    Level = LogLevel.Warning,
                //    Enabled = false,
                //    Name = "rounding",
                //    Label = "Rounding",
                //    Description = "Round numbers in code, SQL server doesn't have build in functionality to do bankers rounding that we usually use.",
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
                    Label = "String with default length",
                    Description = "Default column lenght is used, column content can be trimmed at 30 or 1 character.",
                }
            };
        }
    }
}
