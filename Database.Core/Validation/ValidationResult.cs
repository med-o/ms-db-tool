using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Database.Core.Validation
{
    public class ValidationResult
    {
        public string Message { get; set; }

        public TSqlFragment Fragment { get; set; }
    }
}
