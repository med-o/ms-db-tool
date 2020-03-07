using System.Diagnostics;

namespace Database.Core.Logging
{
    /// <summary>
    /// Trace logger writes into output windown in VS
    /// </summary>
    public class TraceLogger : LoggerBase, ILogger
    {
        public TraceLogger(ILogEntryFormatter logEntryFormatter) : base(logEntryFormatter)
        {
        }

        public override void Log(string message)
        {
            lock (lockObj)
            {
                Trace.WriteLine(message);
            }
        }
    }
}
