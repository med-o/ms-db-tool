using System.Configuration;
using System.IO;

namespace Database.Core.Settings
{
    public class DatabaseSchemaSettingsAppConfigRepository : IDatabaseSchemaSettingRepository
    {
        private DatabaseSchemaSettings _settings;

        public virtual DatabaseSchemaSettings Get()
        {
            _settings = _settings ?? new DatabaseSchemaSettings()
            {
                FileLocations = new DatabaseSchemaFileLocations()
                {
                    DatabaseFilesLocation = ConfigurationManager.AppSettings.Get("SchemaDefinitionInputFilesLocation"),

                    UserDefinedTypesLocation = GetSchemaLocation(ConfigurationManager.AppSettings.Get("SchemaLocationUserDefinedTypes")),
                    UserDefinedTableTypesLocation = GetSchemaLocation(ConfigurationManager.AppSettings.Get("SchemaLocationUserDefinedTableTypes")),
                    TablesLocation = GetSchemaLocation(ConfigurationManager.AppSettings.Get("SchemaLocationTables")),
                    ViewsLocation = GetSchemaLocation(ConfigurationManager.AppSettings.Get("SchemaLocationViews")),
                    StoredProceduresLocation = GetSchemaLocation(ConfigurationManager.AppSettings.Get("SchemaLocationStoredProcedures")),
                    FunctionsLocation = GetSchemaLocation(ConfigurationManager.AppSettings.Get("SchemaLocationFunctions")),

                    GenerateSchemaDefinitionFiles = ConfigurationManager.AppSettings.Get("GenerateSchemaDefinitionFiles")?.Equals("true") ?? false,
                    SchemaDefinitionFilesLocation = ConfigurationManager.AppSettings.Get("SchemaDefinitionOutputFilesLocation"),
                },

                // TODO : either add to config file or split up the repo
                FileConvetions = new DatabaseSchemaFileConvetions()
                {
                    UserDefinedTypesFileExtension = "SCHEMA",
                    TablesFileExtension = "SCHEMA",
                    ViewsFileExtension = "VIW",
                    FunctionsFileExtension = "UDF",
                    ProceduresFileExtension = "PRC",
                    TriggersFileExtension = "SQL", // TODO
                }
            };

            return _settings;
        }

        private static string GetSchemaLocation(string schemaObjectPath)
        {
            return Path.Combine(ConfigurationManager.AppSettings.Get("SchemaDefinitionInputFilesLocation"), schemaObjectPath);
        }
    }
}
