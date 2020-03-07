using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Schema;

namespace Database.Core.FragmentExtensions
{
    public static class SchemaObjectNameExtensions
    {
        public static string GetQualifiedIdentfier(this SchemaObjectName schemaObjectName, SchemaFile file)
        {
            var qualifiers = new List<string>() {
                schemaObjectName.DatabaseIdentifier?.Value ?? file.Context.Name,
                string.IsNullOrEmpty(schemaObjectName.SchemaIdentifier?.Value)
                    ? SchemaObject.DefaultSchema
                    : schemaObjectName.SchemaIdentifier.Value,
                schemaObjectName.BaseIdentifier?.Value,
            };

            return qualifiers.GetQualifiedIdentfier();
        }

        public static string GetTemporaryQualifiedIdentfier(this SchemaObjectName schemaObjectName)
        {
            var qualifiers = new List<string>() {
                SchemaObject.TempDb,
                string.IsNullOrEmpty(schemaObjectName.SchemaIdentifier?.Value)
                    ? SchemaObject.DefaultSchema
                    : schemaObjectName.SchemaIdentifier.Value,
                schemaObjectName.BaseIdentifier?.Value,
            };

            return qualifiers.GetQualifiedIdentfier();
        }
    }
}
