using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Database.Core.Validation.Settings;

namespace Database.Core.Validation
{
    public class ValidationEngineInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component
                .For<IValidationEngine>()
                .ImplementedBy<ValidationEngine>()
                .LifestyleTransient()
            );

            container.Register(Classes
                .FromThisAssembly()
                .BasedOn<IValidationRule>()
                .WithServiceAllInterfaces()
                .LifestyleTransient()
            );

            container.Register(Component
                .For<IValidationRulesFactory>()
                .ImplementedBy<ValidationRulesFactory>()
                .LifestyleSingleton()
            );

            container.Register(Component
                .For<IValidationRuleSettingsRepository>()
                .ImplementedBy<ValidationRuleSettingsDatabaseRepository>()
                .LifestyleSingleton()
            );
        }
    }
}
