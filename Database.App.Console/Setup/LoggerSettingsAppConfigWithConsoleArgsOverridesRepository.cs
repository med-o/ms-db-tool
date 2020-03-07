using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Database.Core.Settings;

namespace Database.App.Console.Setup
{
    public class LoggerSettingsAppConfigWithConsoleArgsOverridesRepository : LoggerSettingsAppConfigRepository
    {
        public override LoggerSettings Get()
        {
            var argsQueue = new Queue<string>(Environment.GetCommandLineArgs());

            // TODO : make this more robust and handle failures gracefully (expects arguments in pairs now
            while (argsQueue.Any())
            {
                var arg = argsQueue.Dequeue();
                switch (arg)
                {
                    case "--LogFileLocation":
                        ConfigurationManager.AppSettings.Set("LogFileLocation", argsQueue.Dequeue());
                        break;
                    case "--LogIsVerbose":
                        ConfigurationManager.AppSettings.Set("LogIsVerbose", argsQueue.Dequeue());
                        break;
                }
            }

            return base.Get();
        }
    }
}
