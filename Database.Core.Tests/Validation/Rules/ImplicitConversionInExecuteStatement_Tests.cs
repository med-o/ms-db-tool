using System;
using System.Collections.Generic;
using NUnit.Framework;
using Database.Core.FragmentExtensions;
using Database.Core.Schema;
using Database.Core.Schema.Types.Fields;
using Database.Core.Validation;
using Database.Core.Validation.Rules;

namespace Database.Core.Tests.Validation.Rules
{
    public class ImplicitConversionInExecuteSpecification_Tests : ValidationRuleTestBase<ImplicitConversionInExecuteStatement>
    {
        protected override ImplicitConversionInExecuteStatement GetObjectUnderTest()
        {
            return new ImplicitConversionInExecuteStatement(Logger);
        }

        protected override string GetTestFileName()
        {
            return @"Validation\Rules\ImplicitConversionInExecuteStatement_Tests.sql";
        }

        protected override void AssertResults(ImplicitConversionInExecuteStatement rule, SchemaFile file, IList<ValidationResult> results)
        {
            Assert.IsTrue(results.Count == 1);
            Assert.IsTrue(results[0].Fragment.GetTokenText().StartsWith("@the_other_id_filed", StringComparison.InvariantCultureIgnoreCase));
            Assert.AreEqual($"ExecuteSpecification: Parameter \"@some_id\" is of \"{FieldType.BigInt}\" type and value is of \"{FieldType.Int}\" type.", results[0].Message);
        }
    }
}
