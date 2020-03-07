using System.Configuration;

namespace Database.Core.Settings
{
    public class LoggerSettingsAppConfigRepository : ILoggerSettingsRepository
    {
        private LoggerSettings _settings;

        public virtual LoggerSettings Get()
        {
            _settings = _settings ?? new LoggerSettings()
            {
                OutputFileLocation = ConfigurationManager.AppSettings.Get("LogFileLocation"),
                OutputFileName = ConfigurationManager.AppSettings.Get("LogFileName"),
                Verbose = ConfigurationManager.AppSettings.Get("LogIsVerbose")?.Equals("true") ?? true,
            };

            return _settings;
        }
    }
}
