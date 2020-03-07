using Database.Core.Schema;

namespace Database.Core.IO
{
    public interface ISchemaWriter
    {
        void Write(SchemaDefinition schema);
    }
}
