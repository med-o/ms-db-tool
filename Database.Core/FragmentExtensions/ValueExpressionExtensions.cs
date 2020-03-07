using System;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.FragmentExtensions
{
    public static class ValueExpressionExtensions
    {
        public static Field GetField(this ValueExpression valueExpression, string columnName, ILogger logger, SchemaFile file)
        {
            if (valueExpression is Literal literal)
            {
                return literal.GetField(columnName, logger, file);
            }

            if (valueExpression is VariableReference variableReference)
            {
                var variable = file
                    .FileContext
                    .Variables
                    .SelectMany(x => x)
                    .Distinct(new KeyEqualityComparer<Field, string>(x => x.Name))
                    .FirstOrDefault(x => x.Name.Equals(variableReference.Name, StringComparison.InvariantCultureIgnoreCase));

                if (variable == null)
                {
                    logger.Log(LogLevel.Error, $"Variable is not defined. Fragment: {variableReference.GetTokenText()}");
                }

                return variable?.Copy(columnName) ?? new UnknownField()
                {
                    Name = columnName
                };
            }

            if (valueExpression is GlobalVariableExpression globalVariableExpression)
            {
                switch (globalVariableExpression.Name.ToUpper())
                {
                    case "@@ROWCOUNT":
                    case "@@CURSOR_ROWS":
                    case "@@ERROR": // TODO : verify this is the case
                    case "@@PROCID":
                    case "@@TIMETICKS":
                    case "@@CPU_BUSY":
                    case "@@IO_BUSY":
                    case "@@IDLE":
                    case "@@PACK_RECEIVED":
                    case "@@PACK_SENT":
                    case "@@CONNECTIONS":
                    case "@@PACKET_ERRORS":
                    case "@@TOTAL_READ":
                    case "@@TOTAL_WRITE":
                    case "@@TOTAL_ERRORS":
                        return new DefaultField()
                        {
                            Name = columnName,
                            Type = FieldType.Int,
                            Origin = OriginType.GlobalVariable,
                            IsNullable = false,
                        };
                    case "SCOPE_IDENTITY":
                    case "@@IDENTITY":
                        {
                            // https://docs.microsoft.com/en-us/sql/t-sql/functions/scope-identity-transact-sql
                            return new DecimalField()
                            {
                                Name = columnName,
                                Type = FieldType.Decimal,
                                Origin = OriginType.GlobalVariable,
                                IsNullable = false,
                                Precision = 38,
                                Scale = 0,
                            };
                        }
                    case "@@SERVERNAME":
                        return new StringField()
                        {
                            Name = columnName,
                            Type = FieldType.String, // nvarchar
                            Origin = OriginType.GlobalVariable,
                            IsNullable = false,
                            Length = 15,
                        };
                    case "@@SPID":
                        return new DefaultField()
                        {
                            Name = columnName,
                            Type = FieldType.SmallInt,
                            Origin = OriginType.GlobalVariable,
                            IsNullable = false,
                        };
                    case "@@DATEFIRST":
                        return new DefaultField()
                        {
                            Name = columnName,
                            Type = FieldType.TinyInt,
                            Origin = OriginType.GlobalVariable,
                            IsNullable = false,
                        };
                    default:
                        break;
                }
            }

            logger.Log(LogLevel.Warning, 
                LogType.NotSupportedYet,
                file.Path, 
                $"\"{valueExpression.GetType()}\" value expression is not supported yet. " +
                $"Fragment: \"{valueExpression.GetTokenText()}\"");

            return new UnknownField()
            {
                Name = columnName
            };
        }
    }
}
