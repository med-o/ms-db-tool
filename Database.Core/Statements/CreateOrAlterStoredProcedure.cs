using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.FragmentExtensions;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Types;
using Database.Core.Schema.Types.Fields;

namespace Database.Core.Statements
{
    public class CreateOrAlterStoredProcedure : Statement<ProcedureStatementBody>
    {
        public CreateOrAlterStoredProcedure(ILogger logger) : base(logger)
        {
        }

        public override SchemaObjectType Type => SchemaObjectType.StoredProcedure;

        public override IList<SchemaObject> Create(SchemaFile file)
        {
            var schemaList = new List<SchemaObject>();

            foreach (var createProcedureStatement in Fragments)
            {
                var parameters = createProcedureStatement
                    .Parameters
                    .GetParameters(Logger, file);

                var dataSets = createProcedureStatement
                    .StatementList
                    .Statements
                    .CollectLocalSchema(Logger, file)
                    .ToList();

                // TODO : this needs to change to capture multiple datasets
                // TODO : also flag when there are no data sets being returned
                var columns = dataSets
                    .FirstOrDefault()
                    ?.Columns ?? new List<Field>();

                var dbObject = new StoredProcedure()
                {
                    File = file,
                    Database = createProcedureStatement.ProcedureReference.Name.DatabaseIdentifier?.Value ?? file.Context.Name,
                    Schema = createProcedureStatement.ProcedureReference.Name.SchemaIdentifier?.Value ?? SchemaObject.DefaultSchema,
                    Identifier = createProcedureStatement.ProcedureReference.Name.BaseIdentifier.Value,
                    Columns = columns,
                    Parameters = parameters,
                };

                schemaList.Add(dbObject);
            }

            return schemaList;
        }
    }
}
