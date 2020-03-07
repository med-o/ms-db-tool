using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Database.Core.Schema.Types;

namespace Database.Core.Schema
{
    // TODO : make use of concurrent dictionary and run things in parallel

    [KnownType(typeof(UserDefinedType))]
    [KnownType(typeof(UserDefinedTableType))]
    [KnownType(typeof(Table))]
    [KnownType(typeof(View))]
    [KnownType(typeof(StoredProcedure))]
    [KnownType(typeof(Function))]
    [KnownType(typeof(Unknown))]
    [CollectionDataContract(Name = "Schema", ItemName = "SchemaObject", KeyName = "QualifiedName", ValueName = "Value")]
    public class SchemaDefinition : ConcurrentDictionary<string, SchemaObject>
    {
        public SchemaDefinition() : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        internal void Add()
        {
            throw new NotImplementedException();
        }
    }

    public static class SchemaDefinitionExtensions
    {
        public static void Add(this SchemaDefinition source, KeyValuePair<string, SchemaObject> item)
        {
            source.AddOrUpdate(item.Key, item.Value, (key, databaseObject) => databaseObject);
        }

        public static void AddRange(this SchemaDefinition source, Dictionary<string, SchemaObject> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("Collection is null");
            }

            foreach (var item in collection)
            {
                source.Add(item);
            }
        }
    }
}
