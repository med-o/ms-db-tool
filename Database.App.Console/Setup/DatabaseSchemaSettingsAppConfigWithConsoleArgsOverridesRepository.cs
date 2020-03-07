using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Database.Core.Settings;

namespace Database.App.Console.Setup
{
    public class DatabaseSchemaSettingsAppConfigWithConsoleArgsOverridesRepository : DatabaseSchemaSettingsAppConfigRepository
    {
        public override DatabaseSchemaSettings Get()
        {
            var argsQueue = new Queue<string>(Environment.GetCommandLineArgs());

            // TODO : make this more robust and handle failures gracefully (expects arguments in pairs now
            while (argsQueue.Any())
            {
                var arg = argsQueue.Dequeue();
                switch (arg)
                {
                    case "--SchemaDefinitionInputFilesLocation":
                        ConfigurationManager.AppSettings.Set("SchemaDefinitionInputFilesLocation", argsQueue.Dequeue());
                        break;
                    case "--SchemaDefinitionOutputFilesLocation":
                        ConfigurationManager.AppSettings.Set("SchemaDefinitionOutputFilesLocation", argsQueue.Dequeue());
                        break;
                    case "--GenerateSchemaDefinitionFiles":
                        ConfigurationManager.AppSettings.Set("GenerateSchemaDefinitionFiles", argsQueue.Dequeue());
                        break;
                }
            }

            return base.Get();
        }
    }
}
