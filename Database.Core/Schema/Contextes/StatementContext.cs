using System;
using System.Collections.Generic;
using Database.Core.Schema.References;

namespace Database.Core.Schema.Contextes
{
    public class StatementContext : IDisposable
    {
        private readonly SchemaFileContext _schemaFileContext;

        public StatementContext(SchemaFileContext schemaFileContext, List<SchemaObjectReference> statementReferences)
        {
            _schemaFileContext = schemaFileContext;
            _schemaFileContext.StatementReferences.Push(statementReferences);
        }

        public void Dispose()
        {
            _schemaFileContext.StatementReferences.Pop();
        }
    }
}
