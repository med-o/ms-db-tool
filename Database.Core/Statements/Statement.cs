using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Visitors;

namespace Database.Core.Statements
{
    public abstract class Statement<TStatement> : TSqlFragmentVisitor<TStatement>, IStatement
        where TStatement : TSqlStatement
    {
        public ILogger Logger { get; private set; }

        public Statement(ILogger logger)
        {
            Logger = logger;
        }

        public abstract SchemaObjectType Type { get; }

        public void Init()
        {
            Fragments = new List<TStatement>();
        }

        public IStatement Collect(SchemaFile file)
        {
            Init();
            file.TsqlScript.Accept(this);
            return this;
        }

        public abstract IList<SchemaObject> Create(SchemaFile file);
    }
}
