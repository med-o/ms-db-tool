using System;
using System.Collections.Generic;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.Schema.Contextes
{
    public class LocalVariableContext : IDisposable
    {
        private readonly SchemaFileContext _schemaFileContext;

        public LocalVariableContext(SchemaFileContext schemaFileContext, List<Field> variables)
        {
            _schemaFileContext = schemaFileContext;
            _schemaFileContext.Variables.Push(variables);
        }

        public void Dispose()
        {
            _schemaFileContext.Variables.Pop();
        }
    }
}
