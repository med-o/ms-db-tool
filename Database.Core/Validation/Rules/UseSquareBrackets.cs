//using Gatekeeper.ValidationEngine;
//using Microsoft.SqlServer.TransactSql.ScriptDom;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Gatekeeper.ValidationRules
//{
//    // TODO : find correct type
//    public class UseSquareBrackets : ValidationRule<SelectScalarExpression>
//    {
//        public UseSquareBrackets() : base (new ValidationRuleSettings()
//            {
//                Enabled = true,
//                Name = "no_complex_math",
//                Label = "Use \"[]\"",
//                Description = "Wrap columns which are named like tsql keyword in [] for better readability.",
//                Style = new Info() // TODO : create default style
//            })
//        {
//        }

//        public override IList<SelectScalarExpression> Execute()
//        {
//            // TODO
//            return new List<SelectScalarExpression>();
//        }
//    }
//}
