using System.IO;
using Database.Core.Settings;

namespace Database.Core.Logging
{
    public class FileLogger : LoggerBase
    {
        public FileLogger(
            ILogEntryFormatter logEntryFormatter,
            ILoggerSettingsRepository loggerSettingsRepository
        ) : base(logEntryFormatter)
        {
            Settings = loggerSettingsRepository.Get();

            lock (lockObj)
            {
                using (var streamWriter = new StreamWriter(GetFilePath(), false))
                {
                    streamWriter.WriteLine(string.Empty);
                    streamWriter.Close();
                }
            }
        }

        public LoggerSettings Settings { get; }

        public override void Log(string message)
        {
            if (!Settings.Verbose) return;

            lock (lockObj)
            {
                using (var streamWriter = new StreamWriter(GetFilePath(), true))
                {
                    streamWriter.WriteLine(message);
                    streamWriter.Close();
                }
            }
        }

        private string GetFilePath()
        {
            var file = new FileInfo($"{Settings.OutputFileLocation}\\{Settings.OutputFileName}");
            file.Directory.Create();
            return file.FullName;
        }
    }
}
