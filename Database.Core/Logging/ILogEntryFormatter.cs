namespace Database.Core.Logging
{
    public interface ILogEntryFormatter
    {
        string Format(LogEntry entry);
        string Format(SchemaFileLogEntry entry);
        string Format(TSqlFragmentLogEntry entry);
    }
}
