using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Types;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.FragmentExtensions
{
    public static class ExecutableEntityExtensions
    {
        public static SchemaObject GetSchema(
            this ExecutableEntity executableEntity,
            ILogger logger,
            SchemaFile file
        )
        {
            switch (executableEntity)
            {
                case ExecutableProcedureReference executableProcedureReference:
                    {
                        // TODO : requires more work, there is also a Variable property, who knows what's that for
                        var identifier = executableProcedureReference.ProcedureReference.ProcedureReference.Name.GetQualifiedIdentfier(file);
                        if (file.Schema.ContainsKey(identifier))
                        {
                            return file.Schema[identifier];
                        }

                        var parameters = new List<Parameter>();
                        var schema = SchemaObject.SystemSchema;
                        var baseIdentifier = executableProcedureReference.ProcedureReference.ProcedureReference.Name.BaseIdentifier.Value;
                        var columns = new List<Field>();

                        switch (baseIdentifier.ToUpper())
                        {
                            case "SP_EXECUTESQL":
                                parameters.Add(new Parameter(new StringField()
                                {
                                    Name = "@stmt",
                                    Type = FieldType.String, // nvarchar(max)
                                    Origin = OriginType.SystemType,
                                    IsNullable = false,
                                    Length = 0, // TODO
                                })
                                {
                                    HasDefaultValue = false,
                                    IsOutput = false,
                                    IsReadOnly = false,
                                });
                                break;

                            case "SP_ADDEXTENDEDPROPERTY":
                                parameters = new List<Parameter>()
                                {
                                    new Parameter(new StringField()
                                    {
                                        Name = "@name",
                                        Type = FieldType.String, // sysname
                                        Origin = OriginType.SystemType,
                                        Length = 128,
                                    })
                                    {
                                        HasDefaultValue = false,
                                        IsOutput = false,
                                        IsReadOnly = false,
                                    },
                                    new Parameter(new DefaultField()
                                    {
                                        Name = "@value",
                                        Type = FieldType.SqlVariant,
                                        Origin = OriginType.SystemType,
                                    })
                                    {
                                        HasDefaultValue = true,
                                        IsOutput = false,
                                        IsReadOnly = false,
                                    },
                                    new Parameter(new StringField()
                                    {
                                        Name = "@level0type",
                                        Type = FieldType.String, // varchar(128)
                                        Origin = OriginType.SystemType,
                                    })
                                    {
                                        HasDefaultValue = true,
                                        IsOutput = false,
                                        IsReadOnly = false,
                                    },
                                    new Parameter(new StringField()
                                    {
                                        Name = "@level0name",
                                        Type = FieldType.String, // sysname
                                        Origin = OriginType.SystemType,
                                        Length = 128,
                                    })
                                    {
                                        HasDefaultValue = true,
                                        IsOutput = false,
                                        IsReadOnly = false,
                                    },
                                    new Parameter(new StringField()
                                    {
                                        Name = "@level1type",
                                        Type = FieldType.String, // varchar(128)
                                        Origin = OriginType.SystemType,
                                    })
                                    {
                                        HasDefaultValue = true,
                                        IsOutput = false,
                                        IsReadOnly = false,
                                    },
                                    new Parameter(new StringField()
                                    {
                                        Name = "@level1name",
                                        Type = FieldType.String, // sysname
                                        Origin = OriginType.SystemType,
                                        Length = 128,
                                    })
                                    {
                                        HasDefaultValue = true,
                                        IsOutput = false,
                                        IsReadOnly = false,
                                    },
                                    new Parameter(new StringField()
                                    {
                                        Name = "@level2type",
                                        Type = FieldType.String, // varchar(128)
                                        Origin = OriginType.SystemType,
                                    })
                                    {
                                        HasDefaultValue = true,
                                        IsOutput = false,
                                        IsReadOnly = false,
                                    },
                                    new Parameter(new StringField()
                                    {
                                        Name = "@level2name",
                                        Type = FieldType.String, // sysname
                                        Origin = OriginType.SystemType,
                                        Length = 128,
                                    })
                                    {
                                        HasDefaultValue = true,
                                        IsOutput = false,
                                        IsReadOnly = false,
                                    },
                                };
                                break;

                            case "XP_CMDSHELL":
                                parameters.Add(new Parameter(new StringField()
                                {
                                    Name = "@command_string",
                                    Type = FieldType.String, // varchar(8000) / nvarchar(4000)
                                    Origin = OriginType.SystemType,
                                    IsNullable = false,
                                    Length = 8000, // TODO
                                })
                                {
                                    HasDefaultValue = false,
                                    IsOutput = false,
                                    IsReadOnly = false,
                                });
                                columns.Add(new StringField()
                                {
                                    Name = null,
                                    Type = FieldType.String, // nvarchar(255)
                                    Origin = OriginType.SystemType,
                                    IsNullable = false,
                                    Length = 255,
                                });
                                break;

                            default:
                                logger.Log(LogLevel.Error, 
                                    LogType.MissingSchemaObject,
                                    file.Path,
                                    $"\"{identifier}\" executable entity is missing in the schema.");

                                schema = executableProcedureReference
                                    .ProcedureReference
                                    .ProcedureReference
                                    .Name
                                    .SchemaIdentifier
                                    ?.Value 
                                    ?? SchemaObject.DefaultSchema;

                                break;
                        }

                        var storedProc = new StoredProcedure()
                        {
                            File = file,
                            Database = executableProcedureReference
                                .ProcedureReference
                                .ProcedureReference
                                .Name
                                .DatabaseIdentifier
                                ?.Value 
                                ?? file.Context.Name,
                            Schema = schema,
                            Identifier = baseIdentifier,
                            Parameters = parameters,
                            Columns = columns,
                        };

                        return storedProc;
                    }

                // TODO : ugh, what can I do with this?
                case ExecutableStringList executableStringList:
                default:
                    //logger.Log(LogLevel.NotSupportedYet, $"\"{executableEntity.GetType()}\" executable entity is not supported yet.");
                    return new Unknown()
                    {
                        File = file,
                        Database = file.Context.Name,
                        Schema = SchemaObject.DefaultSchema,
                        Identifier = "Unknown",
                    };
            }
        }

        public static IList<Field> GetFields(
            this ExecutableEntity executableEntity,
            ILogger logger,
            SchemaFile file
        )
        {
            return executableEntity.GetSchema(logger, file).Columns.ToList();
        }

        public static IList<Parameter> GetParameters(
            this ExecutableEntity executableEntity,
            ILogger logger,
            SchemaFile file
        )
        {
            var executable = executableEntity.GetSchema(logger, file);

            switch (executable)
            {
                case StoredProcedure storedProcedure:
                    return storedProcedure.Parameters;
                case Function function:
                    return function.Parameters;
                default:
                    return new List<Parameter>();
            }
        }
    }
}
