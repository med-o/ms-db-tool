using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.FragmentExtensions;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Types;

namespace Database.Core.Statements
{
    public class CreateOrAlterView : Statement<ViewStatementBody>
    {
        public CreateOrAlterView(ILogger logger) : base(logger)
        {
        }

        public override SchemaObjectType Type => SchemaObjectType.View;

        public override IList<SchemaObject> Create(SchemaFile file)
        {
            var schemaList = new List<SchemaObject>();

            foreach (var createViewStatement in Fragments)
            {
                var columns = createViewStatement
                    .SelectStatement
                    .GetFields(Logger, file);

                var dbObject = new View
                {
                    Database = createViewStatement.SchemaObjectName.DatabaseIdentifier?.Value ?? file.Context.Name,
                    Schema = createViewStatement.SchemaObjectName.SchemaIdentifier?.Value ?? SchemaObject.DefaultSchema,
                    Identifier = createViewStatement.SchemaObjectName.BaseIdentifier.Value,
                    Columns = columns,
                    File = file,
                };

                schemaList.Add(dbObject);
            }

            return schemaList;
        }
    }
}
