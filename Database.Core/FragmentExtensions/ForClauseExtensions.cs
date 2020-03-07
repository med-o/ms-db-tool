using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.FragmentExtensions
{
    public static class ForClauseExtensions
    {
        public static IList<Field> GetFields(
            this ForClause forClause,
            ILogger logger,
            SchemaFile file
        )
        {
            switch (forClause)
            {
                case XmlForClause xmlForClause:
                    return new List<Field>()
                    {
                        new DefaultField()
                        {
                            Name = null,
                            Type = FieldType.Xml,
                            Origin = OriginType.ForClause,
                            IsNullable = false,
                        }
                    };

                case JsonForClause jsonForClause:
                    return new List<Field>()
                    {
                        new DefaultField()
                        {
                            Name = null,
                            Type = FieldType.Json,
                            Origin = OriginType.ForClause,
                            IsNullable = false,
                        }
                    };

                default:
                    logger.Log(LogLevel.Warning, 
                        LogType.NotSupportedYet,
                        file.Path, 
                        $"\"{forClause.GetType()}\" for clause is not supported yet. " +
                        $"Fragment: {forClause.GetTokenText()}");

                    return new List<Field>()
                    {
                        new UnknownField()
                        {
                            Name = null,
                            Origin = OriginType.ForClause,
                        }
                    };
            }
        }
    }
}
