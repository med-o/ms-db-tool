using System.Collections.Generic;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.Schema.Types
{
    public class Function : SchemaObject
    {
        public override SchemaObjectType Type => SchemaObjectType.Function;

        public IList<Parameter> Parameters { get; set; }
    }
}
