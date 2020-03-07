using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Database.Core.Visitors
{
    public abstract class TSqlFragmentVisitor<TFragment> : TSqlFragmentVisitor 
        where TFragment : TSqlFragment
    {
        public IList<TFragment> Fragments { get; protected set; }

        public TSqlFragmentVisitor()
        {
            Fragments = new List<TFragment>();
        }

        // TODO : revisit this design
        public override void Visit(TSqlFragment node)
        {
            base.Visit(node);

            if (node is TFragment)
                Fragments.Add(node as TFragment);
        }
    }
}
