using System.Collections.Generic;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.Schema.Types
{
    public class Unknown : SchemaObject
    {
        public override SchemaObjectType Type => SchemaObjectType.NotSpecified;

        public Unknown()
        {
            Columns = new List<Field>()
            {
                new WildCardField()
            };
        }
    }
}
