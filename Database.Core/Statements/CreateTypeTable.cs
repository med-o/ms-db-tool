using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.FragmentExtensions;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Types;

namespace Database.Core.Statements
{
    public class CreateTypeTable : Statement<CreateTypeTableStatement>
    {
        public CreateTypeTable(ILogger logger) : base(logger)
        {
        }

        public override SchemaObjectType Type => SchemaObjectType.UserDefinedTableType;

        public override IList<SchemaObject> Create(SchemaFile file)
        {
            var newSchemaList = new List<SchemaObject>();

            foreach (var createTypeTableStatement in Fragments)
            {
                var columns = createTypeTableStatement
                    .Definition
                    .ColumnDefinitions
                    .GetFields(Logger, file)
                    .ToList();

                var dbObject = new UserDefinedTableType()
                {
                    Database = createTypeTableStatement.Name.DatabaseIdentifier?.Value ?? file.Context.Name,
                    Schema = createTypeTableStatement.Name.SchemaIdentifier?.Value ?? SchemaObject.DefaultSchema,
                    Identifier = createTypeTableStatement.Name.BaseIdentifier.Value,
                    File = file,
                    Columns = columns,
                };

                newSchemaList.Add(dbObject);
            }

            return newSchemaList;
        }
    }
}
