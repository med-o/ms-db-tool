using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.Schema.References
{
    public class FieldPairReference
    {
        public Field Left { get; set; }

        public Field Right { get; set; }

        public TSqlFragment Fragment { get; set; }
    }
}
