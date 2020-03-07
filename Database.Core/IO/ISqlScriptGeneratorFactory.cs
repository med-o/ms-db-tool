using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Database.Core.IO
{
    public interface ISqlScriptGeneratorFactory
    {
        SqlScriptGenerator Get(SqlVersion version);
    }
}
