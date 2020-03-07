namespace Database.Core.Settings
{
    public class DatabaseSchemaFileLocations
    {
        public string DatabaseFilesLocation { get; set; }
        public string UserDefinedTypesLocation { get; set; }
        public string UserDefinedTableTypesLocation { get; set; }
        public string TablesLocation { get; set; }
        public string ViewsLocation { get; set; }
        public string StoredProceduresLocation { get; set; }
        public string FunctionsLocation { get; set; }

        public bool GenerateSchemaDefinitionFiles { get; set; }
        public string SchemaDefinitionFilesLocation { get; set; }
    }
}
