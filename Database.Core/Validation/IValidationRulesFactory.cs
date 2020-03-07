using System.Collections.Generic;

namespace Database.Core.Validation
{
    public interface IValidationRulesFactory
    {
        IEnumerable<IValidationRule> Get();
    }
}
