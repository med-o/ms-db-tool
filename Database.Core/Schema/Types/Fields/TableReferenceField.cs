namespace Database.Core.Schema.Types.Fields
{
    public class TableReferenceField : Field
    {
        public SchemaObject Reference { get; set; }
    }
}
