using System.Collections.Generic;
using System.Linq;

namespace Database.Core.Schema
{
    public static class SchemaObjectExtensions
    {
        public static string GetQualifiedIdentfier(this SchemaObject databaseObject)
        {
            var qualifiers = new List<string>() {
                databaseObject.Database,
                databaseObject.Schema,
                databaseObject.Identifier
            };

            return GetQualifiedIdentfier(qualifiers);
        }
        
        public static string GetQualifiedIdentfier(this IEnumerable<string> qualifiers)
        {
            return string.Join(".", qualifiers.Where(x => !string.IsNullOrWhiteSpace(x)));
        }
    }
}
