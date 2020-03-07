using System.Collections.Generic;
using Database.Core.IO;
using Database.Core.Schema;
using Database.Core.Validation;

namespace Database.Core.Logging
{
    public interface ILogger
    {
        void Log(string message);

        void Log(LogLevel level, string message);

        void Log(LogLevel level, LogType type, string filePath, string message);

        void LogParsingErrors(ParserOutput parserOutput, string filePath);

        void LogValidationErrors(IDictionary<IValidationRule, IList<ValidationResult>> validationResults, SchemaFile file);
    }
}
