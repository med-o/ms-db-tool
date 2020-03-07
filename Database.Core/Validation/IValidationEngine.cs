using System.Collections.Generic;
using Database.Core.Schema;

namespace Database.Core.Validation
{
    public interface IValidationEngine
    {
        IEnumerable<IValidationRule> GetValidationRules();

        IDictionary<IValidationRule, IList<ValidationResult>> ValidateFile(SchemaFile file);

        void ValidateDefinition(SchemaDefinition schema);
    }
}
