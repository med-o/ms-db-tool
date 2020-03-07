using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.FragmentExtensions;
using Database.Core.Logging;
using Database.Core.Schema;

namespace Database.Core.Validation.Rules
{
    public class FileNameCorrespondsToSchemaObject : ValidationRule<TSqlStatement>
    {
        public FileNameCorrespondsToSchemaObject(ILogger logger) : base(logger)
        {
        }

        public override ValidationRuleType Type => ValidationRuleType.FileNameCorrespondsToSchemaObject;

        public override IList<ValidationResult> Execute(SchemaFile file)
        {
            return Fragments
                .Select(statement => new
                {
                    Name = GetName(statement, file),
                    Fragment = statement,
                })
                .Where(x => !file.Path.EndsWith(x.Name, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Fragment.ToValidationResult($"File \"{file.Path}\" defines \"{x.Name}\" schema object."))
                .ToList();
        }

        private static string GetName(TSqlStatement statement, SchemaFile file)
        {
            switch (statement)
            {
                case CreateTypeStatement createTypeStatement: // handles CreateTypeTableStatement as well
                    return new List<string>()
                    {
                        createTypeStatement.Name.SchemaIdentifier?.Value ?? SchemaObject.DefaultSchema,
                        createTypeStatement.Name.BaseIdentifier.Value,
                        file.Settings.FileConvetions.UserDefinedTypesFileExtension,
                    }
                    .GetQualifiedIdentfier();

                case CreateTableStatement createTableStatement:
                    return createTableStatement.SchemaObjectName.BaseIdentifier.Value.StartsWith("#") // skip temp tables
                        ? string.Empty
                        : new List<string>()
                        {
                            createTableStatement.SchemaObjectName.SchemaIdentifier?.Value ?? SchemaObject.DefaultSchema,
                            createTableStatement.SchemaObjectName.BaseIdentifier.Value,
                            file.Settings.FileConvetions.TablesFileExtension,
                        }
                        .GetQualifiedIdentfier();

                case ViewStatementBody viewStatementBody:
                    return new List<string>()
                    {
                        viewStatementBody.SchemaObjectName.SchemaIdentifier?.Value ?? SchemaObject.DefaultSchema,
                        viewStatementBody.SchemaObjectName.BaseIdentifier.Value,
                        file.Settings.FileConvetions.ViewsFileExtension,
                    }
                    .GetQualifiedIdentfier();

                case FunctionStatementBody functionStatementBody:
                    return new List<string>()
                    {
                        functionStatementBody.Name.SchemaIdentifier?.Value ?? SchemaObject.DefaultSchema,
                        functionStatementBody.Name.BaseIdentifier.Value,
                        file.Settings.FileConvetions.FunctionsFileExtension,
                    }
                    .GetQualifiedIdentfier();

                case ProcedureStatementBody procedureStatementBody:
                    return new List<string>()
                    {
                        procedureStatementBody.ProcedureReference.Name.SchemaIdentifier?.Value ?? SchemaObject.DefaultSchema,
                        procedureStatementBody.ProcedureReference.Name.BaseIdentifier.Value,
                        file.Settings.FileConvetions.ProceduresFileExtension,
                    }
                    .GetQualifiedIdentfier();

                // TODO : triggers?

                default:
                    return string.Empty;
            }
        }
    }
}
