using System.Collections.Generic;
using NUnit.Framework;
using Database.Core.FragmentExtensions;
using Database.Core.Schema;
using Database.Core.Validation;
using Database.Core.Validation.Rules;

namespace Database.Core.Tests.Validation.Rules
{
    public class ImplicitConversionInUpdateStatement_Tests : ValidationRuleTestBase<ImplicitConversionInUpdateStatement>
    {
        protected override ImplicitConversionInUpdateStatement GetObjectUnderTest()
        {
            return new ImplicitConversionInUpdateStatement(Logger);
        }

        protected override string GetTestFileName()
        {
            return @"Validation\Rules\ImplicitConversionInUpdateStatement_Tests.sql";
        }

        protected override void AssertResults(ImplicitConversionInUpdateStatement rule, SchemaFile file, IList<ValidationResult> results)
        {
            Assert.IsTrue(results.Count == 5);
            Assert.AreEqual("au.photoId = @photo_id", results[0].Fragment.GetTokenText()); // CTE
            Assert.AreEqual("sold_by = @sold_by", results[1].Fragment.GetTokenText()); // set clause
            Assert.AreEqual("cte.photoId = wa.photo_id", results[2].Fragment.GetTokenText()); // from clause
            Assert.AreEqual("photo_id = @photo_id", results[3].Fragment.GetTokenText()); // where clause
            Assert.AreEqual("OUTPUT inserted.photo_id, cte.UserId INTO @table_var", results[4].Fragment.GetTokenText()); // output into clause
        }
    }
}
