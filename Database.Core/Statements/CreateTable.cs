using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.FragmentExtensions;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Types;

namespace Database.Core.Statements
{
    public class CreateTable : Statement<CreateTableStatement>
    {
        public CreateTable(ILogger logger) : base(logger)
        {
        }

        public override SchemaObjectType Type => SchemaObjectType.Table;

        public override IList<SchemaObject> Create(SchemaFile file)
        {
            var newSchemaList = new List<SchemaObject>();

            foreach (var createTableStatement in Fragments)
            {
                var columns = createTableStatement
                    .Definition
                    .ColumnDefinitions
                    .GetFields(Logger, file)
                    .ToList();

                var dbObject = new Table()
                {
                    Database = createTableStatement.SchemaObjectName.DatabaseIdentifier?.Value ?? file.Context.Name,
                    Schema = createTableStatement.SchemaObjectName.SchemaIdentifier?.Value ?? SchemaObject.DefaultSchema,
                    Identifier = createTableStatement.SchemaObjectName.BaseIdentifier.Value,
                    File = file,
                    Columns = columns,
                };

                newSchemaList.Add(dbObject);
            }

            return newSchemaList;
        }
    }
}
