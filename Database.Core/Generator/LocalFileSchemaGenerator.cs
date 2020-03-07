using System.Collections.Generic;
using System.Linq;
using Database.Core.Logging;
using Database.Core.Schema;
using Database.Core.Statements;

namespace Database.Core.Generator
{
    public class LocalFileSchemaGenerator : ILocalFileSchemaGenerator
    {
        private readonly ILogger _logger;

        public LocalFileSchemaGenerator(ILogger logger)
        {
            _logger = logger;
        }

        public void AddLocalSchema(SchemaFile file)
        {
            var statements = new List<IStatement>()
            {
                new CreateType(_logger),
                new CreateTypeTable(_logger),
                new CreateTable(_logger),
                new CreateOrAlterView(_logger),
                new CreateOrAlterFunction(_logger),
                new CreateOrAlterStoredProcedure(_logger),
            };

            statements.ForEach(statement => AddToSchema(file, statement));
        }

        private void AddToSchema(SchemaFile file, IStatement statement)
        {
            var statementSchema = statement
                .Collect(file)
                .Create(file)
                .ToDictionary(k => k.GetQualifiedIdentfier());

            file
                .Schema
                .AddRange(statementSchema);
        }
    }
}
