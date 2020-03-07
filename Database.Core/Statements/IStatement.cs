using System.Collections.Generic;
using Database.Core.Schema;

namespace Database.Core.Statements
{
    public interface IStatement
    {
        SchemaObjectType Type { get; }

        void Init();

        IStatement Collect(SchemaFile file);

        IList<SchemaObject> Create(SchemaFile file);
    }
}
