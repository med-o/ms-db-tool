using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Database.Core.Logging;
using Database.Core.Settings;
using Database.Core.Validation.Settings;

namespace Database.App.Console.Setup
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));

            // TODO : remove this override once it's hooked up to DB
            container.Register(Component
                .For<IValidationRuleSettingsRepository>()
                .ImplementedBy<ValidationRuleSettingsHardcodedRepository>()
                .IsDefault()
                .LifestyleTransient()
            );

            container.Register(Component
                .For<IDatabaseSchemaSettingRepository>()
                .ImplementedBy<DatabaseSchemaSettingsAppConfigWithConsoleArgsOverridesRepository>()
                .LifestyleSingleton()
            );

            container.Register(Component
                .For<ILoggerSettingsRepository>()
                .ImplementedBy<LoggerSettingsAppConfigWithConsoleArgsOverridesRepository>()
                .LifestyleSingleton()
            );

            container.Register(Component
                .For<ILogger>()
                .ImplementedBy<FileLogger>()
                .LifestyleSingleton()
            );

            container.Register(Component
                .For<ILogEntryFormatter>()
                //.ImplementedBy<PlainTextLogEntryFormatter>()
                .ImplementedBy<JsonLogEntryFormatter>()
                .LifestyleSingleton()
            );

        }
    }
}
