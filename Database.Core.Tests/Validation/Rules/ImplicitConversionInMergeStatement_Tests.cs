using System.Collections.Generic;
using NUnit.Framework;
using Database.Core.FragmentExtensions;
using Database.Core.Schema;
using Database.Core.Validation;
using Database.Core.Validation.Rules;

namespace Database.Core.Tests.Validation.Rules
{
    public class ImplicitConversionInMergeStatement_Tests : ValidationRuleTestBase<ImplicitConversionInMergeStatement>
    {
        protected override ImplicitConversionInMergeStatement GetObjectUnderTest()
        {
            return new ImplicitConversionInMergeStatement(Logger);
        }

        protected override string GetTestFileName()
        {
            return @"Validation\Rules\ImplicitConversionInMergeStatement_Tests.sql";
        }

        protected override void AssertResults(ImplicitConversionInMergeStatement rule, SchemaFile file, IList<ValidationResult> results)
        {
            Assert.IsTrue(results.Count == 2);
            Assert.AreEqual("target.photo_id = source.photo_id", results[0].Fragment.GetTokenText()); // CTE
            Assert.AreEqual("target.photo_id = @photo_id", results[1].Fragment.GetTokenText()); // set clause
        }
    }
}
