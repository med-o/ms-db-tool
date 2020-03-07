using Database.Core.Schema;

namespace Database.Core.Generator
{
    public interface ILocalFileSchemaGenerator
    {
        void AddLocalSchema(SchemaFile file);
    }
}
