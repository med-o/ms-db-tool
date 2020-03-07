using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Database.Core.FragmentExtensions;

namespace Database.Core.Logging
{
    public class JsonLogEntryFormatter : ILogEntryFormatter
    {
        public string Format(LogEntry entry)
        {
            var transformedEntry = new
            {
                time = DateTime.Now,
                level = entry.Level,
                message = entry.Message,
            };

            return JsonConvert.SerializeObject(transformedEntry, Formatting.None, new StringEnumConverter());
        }

        public string Format(SchemaFileLogEntry entry)
        {
            var transformedEntry = new
            {
                time = DateTime.Now,
                level = entry.Level,
                type = entry.Type,
                message = entry.Message,
                file = entry.File,
            };

            return JsonConvert.SerializeObject(transformedEntry, Formatting.None, new StringEnumConverter());
        }

        public string Format(TSqlFragmentLogEntry entry)
        {
            var transformedEntry = new
            {
                time = DateTime.Now,
                level = entry.Level,
                type = entry.Type,
                message = entry.Message,
                fragment = entry.Fragment.GetTokenText(),
                line = entry.Fragment.StartLine,
                column = entry.Fragment.StartColumn,
                file = entry.File,
            };

            return JsonConvert.SerializeObject(transformedEntry, Formatting.None, new StringEnumConverter());
        }
    }
}
