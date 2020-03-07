using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Database.Core.IO
{
    public class ParserOutput
    {
        public TSqlScript TsqlScript { get; set; }
        public ICollection<ParseError> ParsingErrors { get; set; }
    }
}
