namespace Database.Core.Schema.Types
{
    public class TemporaryTable : SchemaObject
    {
        public TemporaryTable()
        {
            Database = TempDb;
            Schema = DefaultSchema;
        }

        public override SchemaObjectType Type => SchemaObjectType.TemporaryTable;
    }
}
