using Database.Core.Schema;

namespace Database.Core.Generator
{
    public interface IDatabaseSchemaGenerator
    {
        SchemaDefinition GenerateSchema();
    }
}
