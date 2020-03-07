namespace Database.Core.Schema.References
{
    public class SchemaObjectReference
    {
        public string Alias { get; set; }

        public string Identifier { get; set; }

        public SchemaObject Value { get; set; }
    }

    public static class SchemaObjectReferenceExtensions
    {
        public static bool HasAlias(this SchemaObjectReference objectReference)
        {
            return !string.IsNullOrWhiteSpace(objectReference.Alias);
        }
    }
}
