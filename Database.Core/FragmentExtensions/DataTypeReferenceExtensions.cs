using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.FragmentExtensions
{
    public static class DataTypeReferenceExtensions
    {
        public static FieldType GetFieldType(this DataTypeReference dataTypeReference, ILogger logger, SchemaFile file)
        {
            if (dataTypeReference is SqlDataTypeReference sqlDataTypeReference)
            {
                return sqlDataTypeReference.GetFieldType();
            }

            if (dataTypeReference is UserDataTypeReference)
            {
                return FieldType.UserDataType;
            }

            if (dataTypeReference is XmlDataTypeReference)
            {
                return FieldType.Xml;
            }

            logger.Log(LogLevel.Warning, 
                LogType.NotSupportedYet, 
                file.Path,
                $"{dataTypeReference.GetType()} column type is not supported yet. " +
                $"Fragment: \"{dataTypeReference.GetTokenText()}\"");

            return FieldType.NotSpecified;
        }

        public static Field GetField(this DataTypeReference dataTypeReference, string name, bool isNullable, ILogger logger, SchemaFile file)
        {
            if (dataTypeReference is SqlDataTypeReference sqlDataTypeReference)
            {
                return sqlDataTypeReference.GetField(name, isNullable, logger, file);
            }
            if (dataTypeReference is UserDataTypeReference userDataTypeReference)
            {
                return userDataTypeReference.GetField(name, isNullable, logger, file);
            }
            if (dataTypeReference is XmlDataTypeReference xmlDataTypeReference)
            {
                return new DefaultField()
                {
                    Name = name,
                    Type = FieldType.Xml,
                    IsNullable = isNullable,
                };
            }

            logger.Log(LogLevel.Warning, 
                LogType.NotSupportedYet,
                file.Path,
                $"{dataTypeReference.GetType()} data type reference is not supported yet. " +
                $"Fragment: \"{dataTypeReference.GetTokenText()}\"");

            return new UnknownField()
            {
                Name = name,
            };
        }
    }
}
