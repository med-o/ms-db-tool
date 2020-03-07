using System;
using System.Collections.Generic;
using System.Linq;
using Database.Core.Schema.References;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.Schema
{
    public class SchemaFileContext
    {
        public SchemaFileContext()
        {
            Variables = new Stack<List<Field>>();
            StatementReferences = new Stack<List<SchemaObjectReference>>();

            // Adds initial, empty context
            Variables.Push(new List<Field>());
            StatementReferences.Push(new List<SchemaObjectReference>());
        }

        public Stack<List<Field>> Variables { get; set; }

        public Stack<List<SchemaObjectReference>> StatementReferences { get; set; }

        public SchemaObject GetSchema(string schemaObjectReferenceName)
        {
            // search the stack from the top and take first value that satisfies unique key
            var aliasedDatabaseObjectReferences = StatementReferences
                .SelectMany(x => x)
                .Where(x => x.HasAlias())
                .Distinct(new KeyEqualityComparer<SchemaObjectReference, string>(x => x.Alias))
                .ToDictionary(k => k.Alias, v => v.Value, StringComparer.InvariantCultureIgnoreCase);

            var directDatabaseObjectReferences = StatementReferences
                .SelectMany(x => x)
                .Where(x => !x.HasAlias())
                .Distinct(new KeyEqualityComparer<SchemaObjectReference, string>(x => $"{x.Alias}.{x.Identifier}"))
                .ToDictionary(k => k.Identifier.Split('.').Last(), v => v.Value, StringComparer.InvariantCultureIgnoreCase);

            SchemaObject databaseObject = null;
            if (aliasedDatabaseObjectReferences.ContainsKey(schemaObjectReferenceName))
            {
                databaseObject = aliasedDatabaseObjectReferences[schemaObjectReferenceName];
            }
            else if (directDatabaseObjectReferences.ContainsKey(schemaObjectReferenceName))
            {
                databaseObject = directDatabaseObjectReferences[schemaObjectReferenceName];
            }

            return databaseObject;
        }
    }
}
