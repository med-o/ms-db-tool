using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.References;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.FragmentExtensions
{
    public static class ColumnReferenceExpressionExtensions
    {
        public static string GetQualifiedIdentfier(this ColumnReferenceExpression columnReferenceExpression)
        {
            return columnReferenceExpression
                .MultiPartIdentifier
                .Identifiers
                .Select(x => x.Value)
                .GetQualifiedIdentfier();
        }

        // TODO : consolidate with below extension?
        public static Field GetField(
            this ColumnReferenceExpression columnReferenceExpression, 
            ILogger logger, 
            SchemaFile file,
            IEnumerable<ColumnDefinition> columns
        )
        {
            var columnReferenceName = columnReferenceExpression.MultiPartIdentifier.Identifiers.First().Value;
            var column = columns.FirstOrDefault(x => x.ColumnIdentifier.Value.Equals(columnReferenceName));

            return column.DataType.GetField(column.ColumnIdentifier.Value, column.IsNullable(), logger, file);
        }

        public static Field GetField(
            this ColumnReferenceExpression columnReferenceExpression,
            string columnName,
            ILogger logger,
            SchemaFile file
        )
        {
            // TODO : there are other pseudo columns, find out what they're for
            if (columnReferenceExpression.ColumnType == ColumnType.PseudoColumnAction)
            {
                return new StringField()
                {
                    Name = columnName ?? "$action",
                    Type = FieldType.String, // nvarchar(10)
                    Origin = OriginType.SystemType,
                    Length = 10,
                };
            }

            var identifiers = columnReferenceExpression
                .MultiPartIdentifier
                .Identifiers
                .Select(x => x.Value)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            var columnReferenceName = identifiers.Last();
            if (string.IsNullOrWhiteSpace(columnName))
            {
                columnName = columnReferenceName;
            }

            if (identifiers.Count > 1)
            {
                var schemaObjectReferenceName = identifiers[identifiers.Count - 2];
                var databaseObject = file.FileContext.GetSchema(schemaObjectReferenceName);
                
                if (databaseObject != null)
                {
                    var column = databaseObject
                        .Columns
                        .FirstOrDefault(c => c.Name.Equals(columnReferenceName, StringComparison.InvariantCultureIgnoreCase))
                        ?? databaseObject
                        .Columns
                        .FirstOrDefault(c => c.Type.Equals(FieldType.WildCard));

                    if (column != null)
                    {
                        return column.Copy(columnName);
                    }

                    logger.Log(LogLevel.Error, 
                        LogType.MissingColumnDefinition,
                        file.Path,
                        $"Column \"{columnReferenceName}\" does not exists in " +
                        $"\"{databaseObject.GetQualifiedIdentfier()}\" {databaseObject.Type.ToString().ToLower()}. " +
                        $"This most likely means that underlying table definition is outdated. " +
                        $"If it is a system object add it to the list of wildcard tables.");
                }
                else
                {
                    logger.Log(LogLevel.Error, 
                        LogType.MissingSchemaObject,
                        file.Path,
                        $"Column prefix \"{schemaObjectReferenceName}\" doesn't match any schema object reference");
                }
            }
            else
            {
                // guess and take first column with the same name
                // this should be safe because you can't select a column that is present in more than one table reference without a prefix
                // search the stack from the top and take first value that has the same name
                var column = file
                    .FileContext
                    .StatementReferences
                    .SelectMany(x => x)
                    .SelectMany(x => x.Value.Columns)
                    .FirstOrDefault(x => x.Name != null && x.Name.Equals(columnReferenceName, StringComparison.InvariantCultureIgnoreCase));

                if (column != null)
                {
                    return column.Copy(columnName);
                }
            }

            logger.Log(LogLevel.Warning, 
                LogType.AddingUnknownColumn,
                file.Path, 
                $"Column \"{columnReferenceName}\" doesn't exist in any referenced table. " +
                $"Adding column with undefined type.");

            return new UnknownField()
            {
                Name = columnName,
            };
        }
    }
}
