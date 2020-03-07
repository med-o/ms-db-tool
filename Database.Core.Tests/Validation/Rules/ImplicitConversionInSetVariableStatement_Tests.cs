using System.Collections.Generic;
using NUnit.Framework;
using Database.Core.FragmentExtensions;
using Database.Core.Schema;
using Database.Core.Schema.Types.Fields;
using Database.Core.Validation;
using Database.Core.Validation.Rules;

namespace Database.Core.Tests.Validation.Rules
{
    public class ImplicitConversionInSetVariableStatement_Tests : ValidationRuleTestBase<ImplicitConversionInSetVariableStatement>
    {
        protected override ImplicitConversionInSetVariableStatement GetObjectUnderTest()
        {
            return new ImplicitConversionInSetVariableStatement(Logger);
        }

        protected override string GetTestFileName()
        {
            return @"Validation\Rules\ImplicitConversionInSetVariableStatement_Tests.sql";
        }

        protected override void AssertResults(ImplicitConversionInSetVariableStatement rule, SchemaFile file, IList<ValidationResult> results)
        {
            Assert.IsTrue(results.Count == 2);
            Assert.AreEqual("SET @photoId = (SELECT TOP 1 A.photoId FROM dbo.photo A ORDER BY A.photoId DESC)", results[0].Fragment.GetTokenText());
            Assert.AreEqual($"SetVariableStatement: Variable \"@photoId\" is of \"{FieldType.BigInt}\" type and expression is of \"{FieldType.Int}\" type.", results[0].Message);
            Assert.AreEqual("SET @anotherPhotoId = @photoId", results[1].Fragment.GetTokenText());
            Assert.AreEqual($"SetVariableStatement: Variable \"@anotherPhotoId\" is of \"{FieldType.Int}\" type and expression is of \"{FieldType.BigInt}\" type.", results[1].Message);
        }
    }
}
