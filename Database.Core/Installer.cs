using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Database.Core.Context;
using Database.Core.Generator;
using Database.Core.IO;
using Database.Core.Logging;
using Database.Core.Settings;
using Database.Core.Statements;

namespace Database.Core
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // output to trace by default, override in consuming app if needed
            container.Register(Component
                .For<ILogger>()
                .ImplementedBy<TraceLogger>()
                .LifestyleTransient()
            );

            container.Register(Component
                .For<ILogEntryFormatter>()
                .ImplementedBy<PlainTextLogEntryFormatter>()
                .LifestyleTransient()
            );

            container.Register(Component
                .For<IDatabaseContextProvider>()
                .ImplementedBy<DatabaseContextProvider>()
                .LifestyleTransient()
            );

            container.Register(Component
                .For<IDatabaseSchemaGenerator>()
                .ImplementedBy<DatabaseSchemaGenerator>()
                .LifestyleTransient()
            );

            container.Register(Component
                .For<ILocalFileSchemaGenerator>()
                .ImplementedBy<LocalFileSchemaGenerator>()
                .LifestyleTransient()
            );

            container.Register(Component
                .For<IParser>()
                .ImplementedBy<Parser>()
                .LifestyleTransient()
            );

            container.Register(Component
                .For<ISqlScriptGeneratorFactory>()
                .ImplementedBy<SqlScriptGeneratorFactory>()
                .LifestyleSingleton()
            );

            container.Register(Component
                .For<IFormatter>()
                .ImplementedBy<Formatter>()
                .LifestyleTransient()
            );

            container.Register(Component
                .For<ISchemaWriter>()
                .ImplementedBy<XmlSchemaWriter>()
                .LifestyleTransient()
            );

            container.Register(Classes
                .FromThisAssembly()
                .BasedOn<IStatement>()
                .WithServiceAllInterfaces()
                .LifestyleTransient());

            // TODO : hook up DB instead of app.config?
            container.Register(Component
                .For<IDatabaseSchemaSettingRepository>()
                .ImplementedBy<DatabaseSchemaSettingsAppConfigRepository>()
                .LifestyleSingleton()
            );

            container.Register(Component
                .For<ILoggerSettingsRepository>()
                .ImplementedBy<LoggerSettingsAppConfigRepository>()
                .LifestyleSingleton()
            );
        }
    }
}
