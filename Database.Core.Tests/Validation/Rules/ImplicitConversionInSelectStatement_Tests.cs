using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Database.Core.FragmentExtensions;
using Database.Core.Schema;
using Database.Core.Schema.Types.Fields;
using Database.Core.Validation;
using Database.Core.Validation.Rules;

namespace Database.Core.Tests.Validation.Rules
{
    public class ImplicitConversionInSelectStatement_Tests : ValidationRuleTestBase<ImplicitConversionInSelectStatement>
    {
        protected override ImplicitConversionInSelectStatement GetObjectUnderTest()
        {
            return new ImplicitConversionInSelectStatement(Logger);
        }

        protected override string GetTestFileName()
        {
            return @"Validation\Rules\ImplicitConversionInSelectStatement_Tests.sql";
        }

        protected override void AssertResults(ImplicitConversionInSelectStatement rule, SchemaFile file, IList<ValidationResult> results)
        {
            Assert.IsTrue(results.Count == 3);
            Assert.AreEqual("ld.photo_id = p.photoId", results[0].Fragment.GetTokenText());
            Assert.AreEqual($"SelectStatement: First column is of \"{FieldType.BigInt}\" type and second column is of \"{FieldType.Int}\" type.", results[0].Message);

            Assert.AreEqual("p.photoId = @photo_id", results[1].Fragment.GetTokenText());
            Assert.AreEqual($"SelectStatement: First column is of \"{FieldType.Int}\" type and second column is of \"{FieldType.BigInt}\" type.", results[1].Message);

            Assert.AreEqual("p.photoId = @photo_id", results[2].Fragment.GetTokenText());
            Assert.AreEqual($"SelectStatement: First column is of \"{FieldType.Int}\" type and second column is of \"{FieldType.BigInt}\" type.", results[2].Message);
        }
    }
}
