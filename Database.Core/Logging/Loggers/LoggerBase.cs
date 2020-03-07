using System.Collections.Generic;
using System.Linq;
using System.Text;
using Database.Core.IO;
using Database.Core.Schema;
using Database.Core.Validation;

namespace Database.Core.Logging
{
    public abstract class LoggerBase : ILogger
    {
        private readonly ILogEntryFormatter _logEntryFormatter;

        public LoggerBase(ILogEntryFormatter logEntryFormatter)
        {
            _logEntryFormatter = logEntryFormatter;
        }

        protected readonly object lockObj = new object();

        public abstract void Log(string message);

        public void Log(LogLevel level, string message)
        {
            Log(_logEntryFormatter.Format(new LogEntry()
            {
                Level = level,
                Message = message
            }));
        }

        public void Log(LogLevel level, LogType type, string filePath, string message)
        {
            Log(_logEntryFormatter.Format(new SchemaFileLogEntry()
            {
                Level = level,
                Type = type,
                Message = message,
                File = filePath,
            }));
        }

        public virtual void LogParsingErrors(ParserOutput parserOutput, string filePath)
        {
            foreach (var error in parserOutput.ParsingErrors)
            {
                Log(_logEntryFormatter.Format(new SchemaFileLogEntry()
                {
                    Level = LogLevel.Error,
                    Type = LogType.ParsingError,
                    Message = $"{error.Message} [Line:{error.Line} Column:{error.Column}]",
                    File = filePath,
                }));
            }
        }

        public virtual void LogValidationErrors(IDictionary<IValidationRule, IList<ValidationResult>> validationResults, SchemaFile file)
        {
            foreach (var validationResult in validationResults)
            {
                var validationRule = validationResult.Key;
                var results = validationResult.Value;

                if (results.Any())
                {
                    foreach (var result in results)
                    {
                        var messageBuilder = new StringBuilder(validationRule.Settings.Label);

                        if (!string.IsNullOrWhiteSpace(result.Message))
                        {
                            messageBuilder.Append($" - {result.Message}");
                        }

                        Log(_logEntryFormatter.Format(new TSqlFragmentLogEntry()
                        {
                            Level = validationRule.Settings.Level,
                            Type = LogType.Validation,
                            Message = messageBuilder.ToString(),
                            File = file.Path,
                            Fragment = result.Fragment
                        }));
                    }
                }
            }
        }
    }
}
