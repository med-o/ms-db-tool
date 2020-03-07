//using Microsoft.SqlServer.TransactSql.ScriptDom;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Gatekeeper.ValidationRules
//{
//    // TODO : find correct type
//    public class DontDoMath : ValidationRule<SelectScalarExpression>
//    {
//        public DontDoMath()
//            : base(new ValidationRuleSettings()
//            {
//                Enabled = true,
//                Name = "no_complex_math",
//                Label = "Don't do math",
//                Description = "Complex calculations should be done in code. If you can't avoid it take into account precision and scale limitations.",
//                Style = new Info() // TODO : create default style
//            })
//        {
//        }

//        public override IList<SelectScalarExpression> Execute()
//        {
//            // TODO ..
//            return new List<SelectScalarExpression>();
//        }
//    }
//}
