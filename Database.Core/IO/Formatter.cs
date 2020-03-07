using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Database.Core.IO
{
    public class Formatter : IFormatter
    {
        private readonly ISqlScriptGeneratorFactory _generatorFactory;

        public Formatter(ISqlScriptGeneratorFactory generatorFactory)
        {
            _generatorFactory = generatorFactory;
        }

        public string FormatSql(TSqlScript tsqlScript, SqlVersion version)
        {
            String script;

            _generatorFactory
                .Get(version)
                .GenerateScript(tsqlScript, out script);

            return script;
        }
    }
}
