using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Context;
using Database.Core.Settings;

namespace Database.Core.Schema
{
    public class SchemaFile
    {
        public SchemaFile()
        {
            Schema = new SchemaDefinition();
            LocalSchema = new SchemaDefinition();
            FileContext = new SchemaFileContext();
        }

        public string Path { get; set; }
        public SchemaDefinition Schema { get; set; }
        public SchemaDefinition LocalSchema { get; set; }
        public SchemaFileContext FileContext { get; set; }
        public DatabaseContext Context { get; set; }
        public DatabaseSchemaSettings Settings { get; set; }
        public TSqlScript TsqlScript { get; set; }
    }
}
