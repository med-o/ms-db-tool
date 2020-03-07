using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Types;
using Database.Core.Schema.Types.Fields;
using Database.Core.FragmentExtensions;

namespace Database.Core.Statements
{
    public class CreateType : Statement<CreateTypeUddtStatement>
    {
        public CreateType(ILogger logger) : base(logger)
        {
        }

        public override SchemaObjectType Type => SchemaObjectType.UserDefinedType;

        public override IList<SchemaObject> Create(SchemaFile file)
        {
            var newSchemaList = new List<SchemaObject>();

            foreach (var createTypeStatement in Fragments)
            {
                var dbObject = new UserDefinedType()
                {
                    Database = createTypeStatement.Name.DatabaseIdentifier?.Value ?? file.Context.Name,
                    Schema = createTypeStatement.Name.SchemaIdentifier?.Value ?? SchemaObject.DefaultSchema,
                    Identifier = createTypeStatement.Name.BaseIdentifier.Value,
                    File = file,
                    Columns = GetColumns(createTypeStatement, file)
                };

                newSchemaList.Add(dbObject);
            }

            return newSchemaList;
        }

        private List<Field> GetColumns(CreateTypeUddtStatement createTypeStatement, SchemaFile file)
        {
            // TODO : revisit this design, UDT doesn't have columns..
            var name = createTypeStatement.Name.BaseIdentifier?.Value;
            var nullable = createTypeStatement.NullableConstraint?.Nullable ?? false;
            var field = createTypeStatement.DataType.GetField(name, nullable, Logger, file);

            return new List<Field>() { field };
        }
    }
}
