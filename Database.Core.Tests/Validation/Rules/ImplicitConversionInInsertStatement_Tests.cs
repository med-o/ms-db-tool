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
    public class ImplicitConversionInInsertStatement_Tests : ValidationRuleTestBase<ImplicitConversionInInsertStatement>
    {
        protected override ImplicitConversionInInsertStatement GetObjectUnderTest()
        {
            return new ImplicitConversionInInsertStatement(Logger);
        }

        protected override string GetTestFileName()
        {
            return @"Validation\Rules\ImplicitConversionInInsertStatement_Tests.sql";
        }

        protected override void AssertResults(ImplicitConversionInInsertStatement rule, SchemaFile file, IList<ValidationResult> results)
        {
            Assert.IsTrue(results.Count == 1);
            Assert.IsTrue(results.First().Fragment.GetTokenText().StartsWith("photo_id"));
            Assert.AreEqual($"InsertSpecification: Column \"photo_id\" is of \"{FieldType.Int}\" type and value is of \"{FieldType.BigInt}\" type.", results.First().Message);
        }
    }
}
