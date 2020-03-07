using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.FragmentExtensions;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Schema.Types;

namespace Database.Core.Statements
{
    public class SelectDataSet : Statement<SelectStatement>
    {
        public SelectDataSet(ILogger logger) : base(logger)
        {
        }

        public override SchemaObjectType Type => SchemaObjectType.DerivedTable;

        public override IList<SchemaObject> Create(SchemaFile file)
        {
            foreach (var selectStatement in Fragments)
            {
                var columns = selectStatement.GetFields(Logger, file);

                if (columns.Any())
                {
                    // TODO : add data set to local schema
                    var select = new DerivedTable()
                    {
                        Columns = columns,
                        Database = SchemaObject.TempDb,
                        Schema = SchemaObject.DefaultSchema,
                        File = file,
                        Identifier = "DataSet"
                    };

                    file
                        .LocalSchema
                        .Add(new KeyValuePair<string, SchemaObject>(select.GetQualifiedIdentfier(), select));
                }
            }

            // we don't return it because it would go to global schema, keep it local
            return new List<SchemaObject>();
        }
    }
}
