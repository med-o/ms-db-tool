using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Database.Core.Visitors
{
    public class UseStatementVisitor : TSqlFragmentVisitor<UseStatement>
    {
    }
}
