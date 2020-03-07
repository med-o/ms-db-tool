using System.Collections.Generic;
using System.Runtime.Serialization;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.Schema
{
    public abstract class SchemaObject
    {
        public const string MasterDb = "master";
        public const string TempDb = "TempDb";

        public const string SystemSchema = "sys";
        public const string DefaultSchema = "dbo";

        protected SchemaObject()
        {
            Columns = new List<Field>();
        }

        public string Database { get; set; }
        public string Schema { get; set; }
        public string Identifier { get; set; }

        public abstract SchemaObjectType Type { get; }

        [IgnoreDataMember] // NOTE : do not serialize, causes cyclic dependency
        public SchemaFile File { get; set; }

        // TODO : replace with datasets.. because of SPs..? Or just move it from this base class..
        public IList<Field> Columns { get; set; }
    }
}
