using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Database.Core.Context;
using Database.Core.IO;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Settings;
using Database.Core.Statements;

namespace Database.Core.Generator
{
    public class DatabaseSchemaGenerator : IDatabaseSchemaGenerator
    {
        private readonly IParser _parser;
        private readonly List<IStatement> _statements;
        private readonly IDatabaseSchemaSettingRepository _settingsRepository;
        private readonly IDatabaseContextProvider _contextProvider;
        private readonly ILogger _logger;

        private SchemaDefinition _databaseSchema;

        public DatabaseSchemaGenerator(
            IEnumerable<IStatement> statements, 
            IParser parser,
            IDatabaseSchemaSettingRepository settingsRepository,
            IDatabaseContextProvider contextProvider,
            ILogger logger
        )
        {
            _statements = statements.ToList();
            _parser = parser;
            _settingsRepository = settingsRepository;
            _contextProvider = contextProvider;
            _logger = logger;
        }

        public SchemaDefinition GenerateSchema()
        {
            _logger.Log(LogLevel.Information, "Generate schema ... start");
            _logger.Log(string.Empty);

            _databaseSchema = new SchemaDefinition();

            var settings = _settingsRepository.Get();

            Create(SchemaObjectType.UserDefinedType, settings);
            Create(SchemaObjectType.UserDefinedTableType, settings);
            Create(SchemaObjectType.Table, settings);
            Create(SchemaObjectType.View, settings);
            Create(SchemaObjectType.Function, settings);
            Create(SchemaObjectType.StoredProcedure, settings);

            _logger.Log(LogLevel.Information, "Generate schema ... end");
            _logger.Log(string.Empty);

            return _databaseSchema;
        }

        private void Create(SchemaObjectType type, DatabaseSchemaSettings settings)
        {
            _logger.Log(LogLevel.Information, $"Generate schema ... creating {type.ToString().ToLower()}s");
            _logger.Log(string.Empty);

            var fileNames = GetFileNames(type, settings);

            // TODO : as parallel (use concurrent dictionary for schema?)
            // same as below, what about missing references?

            foreach (var fileName in fileNames)
            {
                _logger.Log(LogLevel.Information, $"Creating schema from {fileName}");
                using (var reader = new StreamReader(fileName))
                {
                    var fileContent = reader.ReadToEnd();
                    var parserOutput = _parser.ParseString(fileContent);

                    if (parserOutput.ParsingErrors != null && parserOutput.ParsingErrors.Count > 0)
                    {
                        _logger.LogParsingErrors(parserOutput, fileName);
                        continue; // just move on
                    }

                    // TODO : How to order views and SPs to avoid missing schema references?
                    // how many times would we need to rerun the process?
                    // identify dependencies?

                    var newDatabaseObjects = _statements
                        .Where(statement => statement.Type.Equals(type))
                        .SelectMany(statement => CreateFromStatement(statement, settings, fileName, fileContent, parserOutput))
                        .ToList(); // eval
                }
            }

            if (fileNames.Any())
            {
                _logger.Log(string.Empty);
            }
        }

        private IDictionary<string, SchemaObject> CreateFromStatement(
            IStatement statement,
            DatabaseSchemaSettings settings, 
            string fileName, 
            string fileContent, 
            ParserOutput parserOutput
        )
        {
            var files = CreateFiles(fileName, fileContent, parserOutput, settings);

            // TODO : consolidate Collect() and Create()?
            var newSchema = files
                .SelectMany(file => statement
                    .Collect(file)
                    .Create(file)
                )
                .ToDictionary(k => k.GetQualifiedIdentfier());

            _databaseSchema.AddRange(newSchema);

            return newSchema;
        }

        private IList<SchemaFile> CreateFiles(string fileName, string fileContent, ParserOutput parserOutput, DatabaseSchemaSettings settings)
        {
            var contexts = _contextProvider.Get(fileContent, parserOutput.TsqlScript);
            var files = contexts
                .Select(context => new SchemaFile()
                {
                    Context = context,
                    Schema = _databaseSchema,
                    Settings = settings,
                    Path = fileName,
                    TsqlScript = parserOutput.TsqlScript,
                })
                .ToList();

            return files;
        }

        private static IList<string> GetFileNames(SchemaObjectType type, DatabaseSchemaSettings settings)
        {
            var location = string.Empty;
            switch (type)
            {
                case SchemaObjectType.UserDefinedType:
                    location = settings.FileLocations.UserDefinedTypesLocation;
                    break;
                case SchemaObjectType.UserDefinedTableType:
                    location = settings.FileLocations.UserDefinedTableTypesLocation;
                    break;
                case SchemaObjectType.Table:
                    location = settings.FileLocations.TablesLocation;
                    break;
                case SchemaObjectType.View:
                    location = settings.FileLocations.ViewsLocation;
                    break;
                case SchemaObjectType.StoredProcedure:
                    location = settings.FileLocations.StoredProceduresLocation;
                    break;
                case SchemaObjectType.Function:
                    location = settings.FileLocations.FunctionsLocation;
                    break;
                default:
                    throw new ArgumentException($"{type} SchemaType is not supported.");
            }
            return GetFileNames(location);
        }

        private static IList<string> GetFileNames(string location)
        {
            if (!Directory.Exists(location)) return new List<string>();

            var fileNames = Directory.GetFiles(location).ToList();

            foreach (var directoryLocation in Directory.GetDirectories(location))
            {
                var nestedFileNames = GetFileNames(directoryLocation);
                fileNames.AddRange(nestedFileNames);
            }

            return fileNames;
        }
    }
}
