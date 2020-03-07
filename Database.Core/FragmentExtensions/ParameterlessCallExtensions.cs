using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.FragmentExtensions
{
    public static class ParameterlessCallExtensions
    {
        public static Field GetField(
            this ParameterlessCall parameterlessCall,
            string columnName,
            ILogger logger,
            SchemaFile file
        )
        {
            switch (parameterlessCall.ParameterlessCallType)
            {
                case ParameterlessCallType.User:
                    return new StringField()
                    {
                        Name = columnName ?? "USER",
                        Type = FieldType.String, // CHAR
                        Origin = OriginType.SystemType,
                        Length = 0, // TODO
                        IsNullable = false,
                    };
                case ParameterlessCallType.CurrentUser:
                    // this is the SYSNAME type
                    return new StringField()
                    {
                        Name = columnName,
                        Type = FieldType.String, // NVARCHAR
                        Origin = OriginType.SystemType,
                        Length = 128,
                        IsNullable = false,
                    };
                case ParameterlessCallType.SessionUser:
                    return new StringField()
                    {
                        Name = columnName ?? "SESSION_USER",
                        Type = FieldType.String, // NVARCHAR
                        Origin = OriginType.SystemType,
                        Length = 128,
                        IsNullable = false,
                    };
                case ParameterlessCallType.SystemUser:
                    return new StringField()
                    {
                        Name = columnName ?? "SYSTEM_USER",
                        Type = FieldType.String, // NCHAR
                        Origin = OriginType.SystemType,
                        Length = 0, // TODO
                        IsNullable = false,
                    };
                case ParameterlessCallType.CurrentTimestamp:
                    return new DefaultField()
                    {
                        Name = columnName ?? "CURRENT_TIMESTAMP",
                        Type = FieldType.DateTime,
                        Origin = OriginType.SystemType,
                        IsNullable = false,
                    };
                default:
                    logger.Log(LogLevel.Warning, 
                        LogType.NotSupportedYet,
                        file.Path, 
                        $"Unable to determine column type from parameterless call. Fragment: \"{parameterlessCall.GetTokenText()}\"");
                    return new UnknownField() { Name = columnName };
            }
        }
    }
}
