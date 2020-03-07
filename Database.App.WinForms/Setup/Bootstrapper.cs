using Castle.Windsor;
using Castle.Windsor.Installer;

namespace Database.App.WinForms.Setup
{
    public class Bootstrapper
    {
        public IWindsorContainer BootstrapContainer()
        {
            var container = new WindsorContainer();

            // Install from this assembly first to register service overrides
            container.Install(FromAssembly.This());
            container.Install(FromAssembly.Named("Database.Core"));

            return container;
        }
    }
}
