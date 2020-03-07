namespace Database.Core.Schema.Types
{
    public class DerivedTable : SchemaObject
    {
        public DerivedTable()
        {
            Database = TempDb;
            Schema = DefaultSchema;
        }

        public override SchemaObjectType Type => SchemaObjectType.DerivedTable;
    }
}
