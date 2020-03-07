using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.IO;

namespace Database.Core.IO
{
    public class Parser : IParser
    {
        private readonly TSqlParser _scriptParser;

        public Parser()
        {
            // TODO: create factories to abstract the version
            var initialQuotedIdenfifiers = false;
            _scriptParser = new TSql130Parser(initialQuotedIdenfifiers);
        }

        public ParserOutput ParseString(string inputString)
        {
            TSqlScript tsqlScript;
            IList<ParseError> parsingErrors;

            using (StringReader reader = new StringReader(inputString))
            {
                tsqlScript = (TSqlScript)_scriptParser.Parse(reader, out parsingErrors);
            }

            return new ParserOutput()
            {
                TsqlScript = tsqlScript,
                ParsingErrors = parsingErrors
            };
        }

        public ParserOutput ParseFile(string filePath)
        {
            TSqlScript tsqlScript;
            IList<ParseError> parsingErrors;

            using (StreamReader reader = new StreamReader(filePath))
            {
                tsqlScript = (TSqlScript)_scriptParser.Parse(reader, out parsingErrors);
            }

            return new ParserOutput()
            {
                TsqlScript = tsqlScript,
                ParsingErrors = parsingErrors
            };
        }
    }
}
