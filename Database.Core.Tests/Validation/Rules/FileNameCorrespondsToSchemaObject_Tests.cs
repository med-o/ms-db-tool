using System;
using System.Collections.Generic;
using NUnit.Framework;
using Database.Core.Schema;
using Database.Core.Validation;
using Database.Core.Validation.Rules;

namespace Database.Core.Tests.Validation.Rules
{
    public class FileNameCorrespondsToSchemaObject_Tests : ValidationRuleTestBase<FileNameCorrespondsToSchemaObject>
    {
        protected override FileNameCorrespondsToSchemaObject GetObjectUnderTest()
        {
            return new FileNameCorrespondsToSchemaObject(Logger);
        }

        protected override string GetTestFileName()
        {
            return "Validation\\Rules\\dbo.FileNameCorrespondsToSchemaObject_Tests.sql";
        }

        protected override void AssertResults(FileNameCorrespondsToSchemaObject rule, SchemaFile file, IList<ValidationResult> results)
        {
            // TODO : triggers

            Assert.AreEqual(4, results.Count);

            var tablesMessage = $"{GetTestFileName()}\" defines \"dbo.photo.{file.Settings.FileConvetions.TablesFileExtension}\" schema object.";
            var viewsMessage = $"{GetTestFileName()}\" defines \"dbo.vw_photo.{file.Settings.FileConvetions.ViewsFileExtension}\" schema object.";
            var proceduresMessage = $"{GetTestFileName()}\" defines \"dbo.sp_get_photo.{file.Settings.FileConvetions.ProceduresFileExtension}\" schema object.";
            var functionsMessage = $"{GetTestFileName()}\" defines \"dbo.fn_photo_sold.{file.Settings.FileConvetions.FunctionsFileExtension}\" schema object.";

            Assert.IsTrue(results[0].Message.EndsWith(tablesMessage, StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(results[1].Message.EndsWith(viewsMessage, StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(results[2].Message.EndsWith(proceduresMessage, StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(results[3].Message.EndsWith(functionsMessage, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
