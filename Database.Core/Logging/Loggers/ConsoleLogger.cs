using System;

namespace Database.Core.Logging
{
    /// <summary>
    /// Writes output into the console, content can then be redirected to text box as well
    /// </summary>
    public class ConsoleLogger : LoggerBase
    {
        public ConsoleLogger(ILogEntryFormatter logEntryFormatter) : base(logEntryFormatter)
        {
        }

        public override void Log(string message)
        {
            lock (lockObj)
            {
                Console.WriteLine(message);
            }
        }
    }
}
