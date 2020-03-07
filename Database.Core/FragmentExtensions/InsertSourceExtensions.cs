using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.References;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.FragmentExtensions
{
    public static class InsertSourceExtensions
    {
        public static IEnumerable<SchemaObjectReference> GetSchemaObjectReferences(
            this InsertSource insertSource,
            ILogger logger,
            SchemaFile file
        )
        {
            switch (insertSource)
            {
                case SelectInsertSource selectInsertSource:
                    return selectInsertSource
                        .Select
                        .GetSchemaObjectReferences(logger, file)
                        .ToList();

                default:
                    return new List<SchemaObjectReference>();
            }
        }

        public static IList<Field> GetFields(
            this InsertSource insertSource,
            ILogger logger,
            SchemaFile file
        )
        {
            switch (insertSource)
            {
                // TODO : when we have more than one VALUES() source they should all have same datatypes, is grabbing the first enough?
                case ValuesInsertSource valuesInsertSource:
                    if (valuesInsertSource.IsDefaultValues)
                    {
                        return file
                            .FileContext
                            .StatementReferences
                            .Peek()
                            .First()
                            .Value
                            .Columns;
                    }

                    return valuesInsertSource
                            .RowValues
                            .First()
                            .ColumnValues
                            .Select(f => f.GetField(null, logger, file))
                            .ToList();
                    
                case SelectInsertSource selectInsertSource:
                    return selectInsertSource
                        .Select
                        .GetFields(logger, file)
                        .ToList();

                case ExecuteInsertSource executeInsertSource:
                    return executeInsertSource
                        .Execute
                        .ExecutableEntity
                        .GetFields(logger, file);

                default:
                    logger.Log(LogLevel.Warning, 
                        LogType.NotSupportedYet,
                        file.Path, 
                        $"\"{insertSource.GetType()}\" insert source is not supported yet.");
                    return new List<Field>();
            }
        }

        public static IList<FieldPairReference> GetFieldPairReferences(
            this InsertSource insertSource,
            ILogger logger,
            SchemaFile file
        )
        {
            switch (insertSource)
            {
                case SelectInsertSource selectInsertSource:
                    return selectInsertSource
                        .Select
                        .GetFieldPairReferences(logger, file)
                        .ToList();

                case ExecuteInsertSource executeInsertSource:
                    return executeInsertSource
                        .Execute
                        .GetFieldPairReferences(logger, file);

                case ValuesInsertSource valuesInsertSource: // no extra pairs from pure values
                default:
                    return new List<FieldPairReference>();
            }
        }
    }
}
