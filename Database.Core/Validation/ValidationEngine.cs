using System;
using System.Collections.Generic;
using System.Linq;
using Database.Core.Logging;
using Database.Core.Schema;

namespace Database.Core.Validation
{
    public class ValidationEngine : IValidationEngine
    {
        private readonly IEnumerable<IValidationRule> _validationRules;
        private readonly ILogger _logger;

        public ValidationEngine(ILogger logger, IValidationRulesFactory factory) 
        {
            _logger = logger;
            _validationRules = factory.Get();
        }

        public IEnumerable<IValidationRule> GetValidationRules()
        {
            return _validationRules;
        }

        public IDictionary<IValidationRule, IList<ValidationResult>> ValidateFile(SchemaFile file)
        {
            _logger.Log(LogLevel.Information, $"Validating {file.Path}");

            var validationResults = new Dictionary<IValidationRule, IList<ValidationResult>>();

            foreach (var validationRule in _validationRules)
            {
                try
                {
                    var validationResult = validationRule.Validate(file);
                    validationResults.Add(validationRule, validationResult);
                }
                catch (Exception e) // TODO ... handle only some exceptions?
                {
                    _logger.Log(LogLevel.Error, e.Message);
                }
            }

            return validationResults;
        }

        public void ValidateDefinition(SchemaDefinition schema)
        {
            _logger.Log(LogLevel.Information, "Validate schema ... start");
            _logger.Log(string.Empty);

            var validationResults = schema
                .Values
                .SelectMany(schemaObject =>
                {
                    var validationResult = ValidateFile(schemaObject.File);
                    _logger.LogValidationErrors(validationResult, schemaObject.File);
                    return validationResult;
                })
                .ToList();

            _logger.Log(LogLevel.Information, "Validate schema ... end");
            _logger.Log(string.Empty);
        }
    }
}
