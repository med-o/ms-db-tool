using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Types;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.FragmentExtensions
{
    public static class FunctionReturnTypeExtensions
    {
        public static List<Field> GetFields(this FunctionReturnType returnType, ILogger logger, SchemaFile file)
        {
            var columns = new List<Field>();

            if (returnType is ScalarFunctionReturnType scalarFunctionReturnType)
            {
                // TODO : name?
                var column = scalarFunctionReturnType.DataType.GetField(null, false, logger, file);
                columns.Add(column);
            }

            if (returnType is TableValuedFunctionReturnType tableValuedFunctionReturnType)
            {
                var tableValuedFunctionColumns = tableValuedFunctionReturnType
                    .DeclareTableVariableBody
                    .Definition
                    .ColumnDefinitions
                    .GetFields(logger, file)
                    .ToList();

                columns.AddRange(tableValuedFunctionColumns);

                var tableReference = new Table()
                {
                    Columns = tableValuedFunctionColumns,
                    File = file,
                    Database = SchemaObject.TempDb,
                    Schema = SchemaObject.DefaultSchema,
                    Identifier = tableValuedFunctionReturnType.DeclareTableVariableBody.VariableName.Value,
                };

                var field = new TableReferenceField()
                {
                    Name = tableValuedFunctionReturnType.DeclareTableVariableBody.VariableName.Value,
                    Type = FieldType.Table,
                    IsNullable = false,
                    Reference = tableReference,
                };

                file.FileContext.Variables.Peek().Add(field);
            }

            if (returnType is SelectFunctionReturnType selectFunctionReturnType)
            {
                var tableValuedFunctionColumns = selectFunctionReturnType
                    .SelectStatement
                    .GetFields(logger, file);

                columns.AddRange(tableValuedFunctionColumns);
            }

            if (!columns.Any())
            {
                logger.Log(LogLevel.Warning, 
                    LogType.NotSupportedYet,
                    file.Path, 
                    $"Unable to determine return values for a function. \"{returnType.GetType()}\" is not supported yet.");
            }

            return columns;
        }
    }
}
