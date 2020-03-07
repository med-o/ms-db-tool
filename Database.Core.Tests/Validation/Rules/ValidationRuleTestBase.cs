using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NUnit.Framework;
using Database.Core.Context;
using Database.Core.IO;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Settings;
using Database.Core.Statements;
using Database.Core.Validation;
using Database.Core.Validation.Settings;
using Database.Core.Generator;

namespace Database.Core.Tests.Validation.Rules
{
    [TestFixture]
    public abstract class ValidationRuleTestBase<TValidationRule>
        where TValidationRule : IValidationRule
    {
        public ValidationRuleTestBase()
        {
            Logger = new TraceLogger(new PlainTextLogEntryFormatter());
        }

        protected ILogger Logger { get; set; }

        [Test]
        public void ValidateRule()
        {
            // arrange
            var underTest = GetObjectUnderTest();
            var settings = GetSettings(underTest);
            underTest.Configure(settings);
            var file = GetFile(underTest);
            Configure(underTest, file);

            // act
            var results = underTest.Validate(file);

            // assert
            AssertResults(underTest, file, results);
        }

        // override if needed
        protected virtual void Configure(TValidationRule rule, SchemaFile file)
        {
        }

        protected abstract TValidationRule GetObjectUnderTest();

        protected abstract string GetTestFileName();

        protected abstract void AssertResults(TValidationRule rule, SchemaFile file, IList<ValidationResult> results);

        private SchemaFile GetFile(TValidationRule rule)
        {
            var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), GetTestFileName());
            var fileContent = GetFileContent(fileName);
            var script = GetTSqlScript(fileName, fileContent);
            var databaseContext = GetDatabaseContext(fileContent, script);
            var settings = GetSettings(rule);

            // TODO : how to plug in existing schema?
            // I reckon that we can just define all dependencies in SQL file and generate it from there?
            // or should this be a dependency hard coded in the derived class?

            var file = new SchemaFile()
            {
                Context = databaseContext,
                Path = fileName,
                Settings = new DatabaseSchemaSettings()
                {
                    FileConvetions = new DatabaseSchemaFileConvetions()
                    {
                        UserDefinedTypesFileExtension = "SQL",
                        TablesFileExtension = "SQL",
                        ViewsFileExtension = "SQL",
                        FunctionsFileExtension = "SQL",
                        ProceduresFileExtension = "SQL",
                        TriggersFileExtension = "SQL",
                    },
                },
                TsqlScript = script,
            };

            var localFileSchemaGenerator = new LocalFileSchemaGenerator(Logger);
            localFileSchemaGenerator.AddLocalSchema(file);

            return file;
        }

        private string GetFileContent(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                return reader.ReadToEnd();
            }
        }

        private TSqlScript GetTSqlScript(string fileName, string fileContent)
        {
            var parserOutput = new Parser().ParseString(fileContent);

            if (parserOutput.ParsingErrors != null && parserOutput.ParsingErrors.Any())
            {
                Logger.LogParsingErrors(parserOutput, string.Empty);
            }

            return parserOutput.TsqlScript;
        }

        private DatabaseContext GetDatabaseContext(string fileContent, TSqlScript script)
        {
            return new DatabaseContextProvider(Logger)
                .Get(fileContent, script)
                .First();
        }

        private ValidationRuleSettings GetSettings(TValidationRule rule)
        {
            return new ValidationRuleSettings() {
                Enabled = true,
                Level = LogLevel.Information,
                Name = "Test",
                Description = "Test",
                Label = "Test",
                Type = rule.Type,
            };
        }
    }
}
