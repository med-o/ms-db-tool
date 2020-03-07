using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Database.Core.IO
{
    public interface IFormatter
    {
        string FormatSql(TSqlScript tsqlScript, SqlVersion version);
    }
}
