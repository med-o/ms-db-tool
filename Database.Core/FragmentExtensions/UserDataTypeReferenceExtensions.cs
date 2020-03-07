using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Types;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.FragmentExtensions
{
    public static class UserDataTypeReferenceExtensions
    {
        public static Field GetField(
            this UserDataTypeReference userDataTypeReference, 
            string name, 
            bool isNullable, 
            ILogger logger, 
            SchemaFile file
        )
        {
            var identifier = userDataTypeReference.Name.GetQualifiedIdentfier(file);
            var identifiers = identifier.Split('.');

            if (userDataTypeReference.Name.Identifiers.Count.Equals(1))
            {
                // TODO : trying if it is a system data type.. I basically have 2 options
                // 1) hardcode what I'm interested in here
                // 2) create SQL files and process those first
                // we'll see how big this gets..
                var systemTypeName = userDataTypeReference.Name.Identifiers.First().Value;
                switch (systemTypeName.ToUpper())
                {
                    case "SYSNAME":
                        return new StringField()
                        {
                            Name = name,
                            Type = FieldType.String, // nvarchar
                            Origin = OriginType.SystemType,
                            Length = 128,
                            IsNullable = false,
                        };
                    case "GEOGRAPHY":
                        return new TableReferenceField()
                        {
                            Name = name,
                            Type = FieldType.Table,
                            Origin = OriginType.SystemType,
                            IsNullable = false,
                            Reference = new Table()
                            {
                                File = file,
                                Database = identifiers[0],
                                Schema = identifiers[1],
                                Identifier = identifiers[2],
                                Columns = new List<Field>()
                                    {
                                        new DefaultField()
                                        {
                                            Name = "Lat",
                                            Type = FieldType.Float,
                                            Origin = OriginType.Table,
                                            IsNullable = false,
                                        },
                                        new DefaultField()
                                        {
                                            Name = "Long",
                                            Type = FieldType.Float,
                                            Origin = OriginType.Table,
                                            IsNullable = false,
                                        },
                                    }
                            }
                        };
                }
            }

            var reference = file.Schema.ContainsKey(identifier)
                ? file.Schema[identifier]
                : new Unknown()
                {
                    File = file,
                    Database = identifiers[0],
                    Schema = identifiers[1],
                    Identifier = identifiers[2],
                };

            if (reference.Type == SchemaObjectType.NotSpecified)
            {
                logger.Log(LogLevel.Warning, 
                    LogType.MissingSchemaObject,
                    file.Path, 
                    $"\"{identifier}\" user data type reference is missing in the schema.");
            }

            return new TableReferenceField()
            {
                Name = name,
                Type = FieldType.Table,
                Origin = OriginType.Reference,
                IsNullable = isNullable,
                Reference = reference,
            };
        }
    }
}
