using Database.Core.FragmentExtensions;

namespace Database.Core.Logging
{
    public class PlainTextLogEntryFormatter : ILogEntryFormatter
    {
        public string Format(LogEntry entry)
        {
            return $"[{entry.Level}]: {entry.Message}";
        }

        public string Format(SchemaFileLogEntry entry)
        {
            return $"[{entry.Level}][{entry.Type}]: {entry.Message} ({entry.File})";
        }

        public string Format(TSqlFragmentLogEntry entry)
        {
            return $"[{entry.Level}][{entry.Type}]: {entry.Message} " +
                $" Fragment: \"{entry.Fragment.GetTokenText()}\" [Line:{entry.Fragment.StartLine} Column:{entry.Fragment.StartColumn}]" +
                $"({entry.File})";
        }
    }
}
