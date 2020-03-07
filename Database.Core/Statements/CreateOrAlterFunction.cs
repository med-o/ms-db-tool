using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.FragmentExtensions;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Types;

namespace Database.Core.Statements
{
    public class CreateOrAlterFunction : Statement<FunctionStatementBody>
    {
        // TODO : will I need to split this for scalar and table functions?
        public CreateOrAlterFunction(ILogger logger) : base(logger)
        {
        }

        public override SchemaObjectType Type => SchemaObjectType.Function;

        public override IList<SchemaObject> Create(SchemaFile file)
        {
            var schemaList = new List<SchemaObject>();

            foreach (var createFunctionStatement in Fragments)
            {
                var parameters = createFunctionStatement.Parameters.GetParameters(Logger, file);
                var columns = createFunctionStatement.ReturnType.GetFields(Logger, file);
                var dataSets = createFunctionStatement
                    .StatementList
                    ?.Statements
                    .CollectLocalSchema(Logger, file)
                    .ToList();

                var dbObject = new Function()
                {
                    File = file,
                    Database = createFunctionStatement.Name.DatabaseIdentifier?.Value ?? file.Context.Name,
                    Schema = createFunctionStatement.Name.SchemaIdentifier?.Value ?? SchemaObject.DefaultSchema,
                    Identifier = createFunctionStatement.Name.BaseIdentifier.Value,
                    Columns = columns,
                    Parameters = parameters,
                };

                schemaList.Add(dbObject);
            }

            return schemaList;
        }
    }
}
