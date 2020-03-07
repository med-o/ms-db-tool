using System.Collections.Generic;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.Schema.Types
{
    public class StoredProcedure : SchemaObject
    {
        public override SchemaObjectType Type => SchemaObjectType.StoredProcedure;

        public IList<Parameter> Parameters { get; set; }

        // TODO : columns vs datasets
    }
}
