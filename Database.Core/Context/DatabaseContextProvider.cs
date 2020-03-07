using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Database.Core.Logging;
using Database.Core.Visitors;

namespace Database.Core.Context
{
    // TODO : multiple responsibilities? split into 2 classes and create facory if you add context from SSMS dropdown

    public class DatabaseContextProvider : IDatabaseContextProvider
    {
        private readonly ILogger _logger;

        public DatabaseContextProvider(ILogger logger)
        {
            _logger = logger;
        }

        public IList<DatabaseContext> Get(string fileContent, TSqlScript script)
        {
            var contexts = GetFromUseStatement(script);
            if (contexts != null)
            {
                return contexts;
            }

            _logger.Log(LogLevel.Error, 
                LogType.MissingDatabaseContext,
                "",
                "Database context can't be determined. There is no USE statement. Returning \"Unknown\".");

            return new List<DatabaseContext>() {
                new DatabaseContext() {
                    Name = "Unkown",
                    Type = DatabaseContextType.NotSpecified
                }
            };
        }

        private IList<DatabaseContext> GetFromUseStatement(TSqlScript script)
        {
            var visitor = new UseStatementVisitor();
            script.Accept(visitor);
            if (visitor.Fragments.Count > 2)
            {
                // TODO : create a validation rule as well
                _logger.Log(LogLevel.Warning, $"More than one USE statement detected, first value is used to determine database context.");
            }

            var useStatementContext = visitor
                .Fragments
                .Select(x => new DatabaseContext()
                {
                    Type = DatabaseContextType.UseStatement,
                    Name = x.DatabaseName.Value
                })
                .FirstOrDefault();

            return useStatementContext != null
                ? new List<DatabaseContext>() { useStatementContext }
                : null;
        }
    }
}
