using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Database.Core.Context
{
    public interface IDatabaseContextProvider
    {
        IList<DatabaseContext> Get(string fileContent, TSqlScript script);
    }
}
