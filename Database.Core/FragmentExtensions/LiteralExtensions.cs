using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.FragmentExtensions
{
    public static class LiteralExtensions
    {
        public static Field GetField(this Literal literal, string columnName, ILogger logger, SchemaFile file)
        {
            switch (literal)
            {
                case StringLiteral stringLiteral:
                    return new StringField()
                    {
                        Type = FieldType.String,
                        Origin = OriginType.Literal,
                        IsNullable = false,
                        Name = columnName ?? $"StringLiteral: \"{stringLiteral.Value}\"",
                        Length = stringLiteral.Value.Length,
                    };

                case IntegerLiteral integerLiteral:
                    return new DefaultField()
                    {
                        Type = FieldType.Int,
                        Origin = OriginType.Literal,
                        IsNullable = false,
                        Name = columnName ?? $"IntegerLiteral: \"{integerLiteral.Value}\"",
                    };

                case NullLiteral nullLiteral:
                    return new DefaultField()
                    {
                        Type = FieldType.Null,
                        Origin = OriginType.Literal,
                        IsNullable = true,
                        Name = columnName ?? nullLiteral.Value,
                    };

                case NumericLiteral numericLiteral:                    
                    switch (numericLiteral.LiteralType)
                    {
                        case LiteralType.Numeric when decimal.TryParse(numericLiteral.Value, out var n):
                            // TODO : parse the value or use the decimal
                            //var f = Math.Floor(n);
                            //var c = Math.Ceiling(n);
                            return new DecimalField()
                            {
                                Type = FieldType.Decimal,
                                Origin = OriginType.Literal,
                                IsNullable = false,
                                Name = columnName ?? $"NumericLiteral: \"{numericLiteral.Value}\"",
                                Precision = numericLiteral.Value.Length - 1,
                                Scale = numericLiteral.Value.Split('.')[1].Length,
                            };
                        case LiteralType.Real when double.TryParse(numericLiteral.Value, out var d):
                            return new DefaultField()
                            {
                                Type = FieldType.Float,
                                Origin = OriginType.Literal,
                                IsNullable = false,
                                Name = columnName ?? $"NumericLiteral: \"{numericLiteral.Value}\"",
                            };
                    }
                    break;

                case BinaryLiteral binaryLiteral:
                    return new DefaultField()
                    {
                        Type = FieldType.Binary,
                        Origin = OriginType.Literal,
                        Name = columnName ?? $"BinaryLiteral: \"{binaryLiteral.Value}\"",
                    };

                case DefaultLiteral defaultLiteral:
                    return new DefaultField()
                    {
                        Type = FieldType.WildCard,
                        Origin = OriginType.Literal,
                        Name = columnName ?? "DEFAULT",
                    };

                default:
                    // TODO : other literals..
                    //Real = 1,
                    //Money = 2,
                    //Max = 7,
                    //Odbc = 8,
                    //Identifier = 9,

                    logger.Log(LogLevel.Warning, 
                        LogType.NotSupportedYet,
                        file.Path, 
                        $"Unable to determine column type from \"{literal.GetType()}\" literal. Fragment: \"{literal.GetTokenText()}\"");
                    break;
            }
            
            return new UnknownField()
            {
                Name = "UnknownLiteral",
                Origin = OriginType.Literal,
            };
        }
    }
}
