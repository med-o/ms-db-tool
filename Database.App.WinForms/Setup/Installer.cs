using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Database.App.WinForms.UI;
using Database.Core.Logging;
using Database.Core.Validation.Settings;

namespace Database.App.WinForms.Setup
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.AddFacility<TypedFactoryFacility>();
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));

            container.Register(Component.For<ApplicationForm>());

            container.Register(Component
                .For<ILogger>()
                .ImplementedBy<ConsoleLogger>() // output to console is redirected to a text box
                //.ImplementedBy<ExceptionThrowingLogger>()
                .IsDefault()
                .LifestyleTransient()
            );

            // TODO : remove this override once it's hooked up to DB
            container.Register(Component
                .For<IValidationRuleSettingsRepository>()
                .ImplementedBy<ValidationRuleSettingsHardcodedRepository>()
                .IsDefault()
                .LifestyleTransient()
            );

            // NOTE : output from console is redirected with these components
            container.Register(Component
                .For<RichTextBoxStreamWriter>()
            );
            container.Register(Component
                .For<IRichTextBoxStreamWriterFactory>()
                .AsFactory()
            );
        }
    }
}
