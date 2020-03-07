using Database.Core.Schema.Types.Fields;

namespace Database.Core.Schema.References
{
    public class FieldReference
    {
        public string Alias { get; set; }

        public string Identifier { get; set; }

        public Field Value { get; set; }
    }

    public static class FieldReferenceExtensions
    {
        public static bool HasAlias(this FieldReference objectReference)
        {
            return !string.IsNullOrWhiteSpace(objectReference.Alias);
        }
    }
}
