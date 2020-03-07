using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.FragmentExtensions
{
    public static class ColumnDefinitionExtensions
    {
        public static IEnumerable<Field> GetFields(this IEnumerable<ColumnDefinition> columns, ILogger logger, SchemaFile file)
        {
            return columns.Select(column => column.GetField(columns, logger, file));
        }

        private static Field GetField(this ColumnDefinition column, IEnumerable<ColumnDefinition> columns, ILogger logger, SchemaFile file)
        {
            if (column.DataType != null)
            {
                var name = column.ColumnIdentifier.Value;
                var field = column.DataType.GetField(name, false, logger, file);
                field.HasIdentity = column.HasIdentity();
                field.IsNullable = column.IsNullable();
                return field;
            }

            if (column.ComputedColumnExpression != null && column.ComputedColumnExpression is ScalarExpression scalarExpression)
            {
                return scalarExpression.GetField(column, columns, logger, file);
            }

            logger.Log(LogLevel.Warning, 
                LogType.NotSupportedYet,
                file.Path, 
                $"Column type is not supported yet. Fragment: \"{column.GetTokenText()}\"");

            return new DefaultField();
        }

        public static bool HasIdentity(this ColumnDefinition column)
        {
            return column.IdentityOptions != null;
        }

        public static bool IsNullable(this ColumnDefinition column)
        {
            if (column.Constraints != null && column.Constraints.Any())
            {
                var uniqueConstraintDefinition = (UniqueConstraintDefinition)column
                    .Constraints
                    .FirstOrDefault(x => x is UniqueConstraintDefinition);

                if (uniqueConstraintDefinition != null && uniqueConstraintDefinition.IsPrimaryKey)
                {
                    return false;
                }

                var nullableConstraintDefinition = (NullableConstraintDefinition)column
                    .Constraints
                    .FirstOrDefault(x => x is NullableConstraintDefinition);

                // TODO : determine if it's nullable 
                return nullableConstraintDefinition?.Nullable ?? false;
            }

            // TODO : add logging

            return false;
        }
    }
}