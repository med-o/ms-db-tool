using System;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.FragmentExtensions
{
    public static class SqlDataTypeReferenceExtensions
    {
        public static Field GetField(this SqlDataTypeReference sqlDataTypeReference, string name, bool isNullable, ILogger logger, SchemaFile file)
        {
            var type = sqlDataTypeReference.GetFieldType(logger, file);
            switch (type)
            {
                case FieldType.String:
                    return new StringField()
                    {
                        Name = name,
                        Type = type,
                        IsNullable = isNullable,
                        Length = sqlDataTypeReference.GetStringLength(logger),
                    };
                case FieldType.Decimal:
                case FieldType.Numeric:
                    return new DecimalField()
                    {
                        Name = name,
                        Type = type,
                        IsNullable = isNullable,
                        Precision = sqlDataTypeReference.GetPrecision(logger),
                        Scale = sqlDataTypeReference.GetScale(logger),
                    };
                default:
                    return new DefaultField()
                    {
                        Name = name,
                        Type = type,
                        IsNullable = isNullable,
                    };
            }
        }

        public static int GetStringLength(this SqlDataTypeReference sqlDataTypeReference, ILogger logger)
        {
            if (sqlDataTypeReference.Parameters.Any())
            {
                var parameter = sqlDataTypeReference.Parameters.First();
                switch (parameter.LiteralType)
                {
                    case LiteralType.Integer:
                        return int.Parse(parameter.Value);
                    case LiteralType.Max:
                        return 8000;
                }
            }

            // TODO : when it's variable declaration it will trim it at 1 otherwise at 30 characters

            // https://docs.microsoft.com/en-us/sql/t-sql/functions/cast-and-convert-transact-sql?view=sql-server-2017#arguments
            // length - An optional integer that specifies the length of the target data type. The default value is 30.
            return 30;
        }

        public static int GetPrecision(this SqlDataTypeReference sqlDataTypeReference, ILogger logger)
        {
            if (sqlDataTypeReference.Parameters.Any())
            {
                var parameter = sqlDataTypeReference.Parameters.First();
                switch (parameter.LiteralType)
                {
                    case LiteralType.Integer:
                        return int.Parse(parameter.Value);
                }
            }

            // The default precision is 18
            return 18;
        }

        public static int GetScale(this SqlDataTypeReference sqlDataTypeReference, ILogger logger)
        {
            if (sqlDataTypeReference.Parameters.Any())
            {
                var parameter = sqlDataTypeReference.Parameters.Last();
                switch (parameter.LiteralType)
                {
                    case LiteralType.Integer:
                        return int.Parse(parameter.Value);
                }
            }

            // The default scale is 0
            return 0;
        }

        public static FieldType GetFieldType(this SqlDataTypeReference sqlDataTypeReference)
        {
            switch (sqlDataTypeReference.SqlDataTypeOption)
            {
                case SqlDataTypeOption.BigInt:
                    return FieldType.BigInt;
                case SqlDataTypeOption.Int:
                    return FieldType.Int;
                case SqlDataTypeOption.SmallInt:
                    return FieldType.SmallInt;
                case SqlDataTypeOption.TinyInt:
                    return FieldType.TinyInt;
                case SqlDataTypeOption.Bit:
                    return FieldType.Bit;
                case SqlDataTypeOption.Numeric:
                case SqlDataTypeOption.Decimal:
                    return FieldType.Decimal;
                case SqlDataTypeOption.Money:
                    return FieldType.Money;
                case SqlDataTypeOption.SmallMoney:
                    return FieldType.SmallMoney;
                case SqlDataTypeOption.Float:
                    return FieldType.Float;
                case SqlDataTypeOption.Real:
                    return FieldType.Real;
                case SqlDataTypeOption.DateTime:
                    return FieldType.DateTime;
                case SqlDataTypeOption.SmallDateTime:
                    return FieldType.SmallDateTime;
                case SqlDataTypeOption.Char:
                case SqlDataTypeOption.VarChar:
                case SqlDataTypeOption.NChar:
                case SqlDataTypeOption.NVarChar:
                    return FieldType.String;
                case SqlDataTypeOption.Text:
                case SqlDataTypeOption.NText:
                    return FieldType.Text;
                case SqlDataTypeOption.Binary:
                    return FieldType.Binary;
                case SqlDataTypeOption.VarBinary:
                    return FieldType.VarBinary;
                case SqlDataTypeOption.Image:
                    return FieldType.Image;
                case SqlDataTypeOption.Table:
                    return FieldType.Table;
                case SqlDataTypeOption.Timestamp:
                    return FieldType.Timestamp;
                case SqlDataTypeOption.UniqueIdentifier:
                    return FieldType.UniqueIdentifier;
                case SqlDataTypeOption.Date:
                    return FieldType.Date;
                case SqlDataTypeOption.Time:
                    return FieldType.Time;
                case SqlDataTypeOption.DateTime2:
                    return FieldType.DateTime2;
                case SqlDataTypeOption.DateTimeOffset:
                    return FieldType.DateTimeOffset;
                case SqlDataTypeOption.Sql_Variant:
                    return FieldType.SqlVariant;
                default:
                    throw new ArgumentException($"{sqlDataTypeReference.SqlDataTypeOption} data type option is not supported.");
            }
        }
    }
}
