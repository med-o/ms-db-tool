using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Database.Core.Logging
{
    public class SchemaFileLogEntry
    {
        public string Message { get; set; }
        public LogLevel Level { get; set; }
        public LogType Type { get; set; }
        public string File { get; set; }
    }
}
