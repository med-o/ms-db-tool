namespace Database.Core.Schema.Types
{
    public class Cte : SchemaObject
    {
        public Cte()
        {
            Database = TempDb;
            Schema = DefaultSchema;
        }

        public override SchemaObjectType Type => SchemaObjectType.Cte;
    }
}
