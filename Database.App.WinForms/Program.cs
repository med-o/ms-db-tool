using System;
using System.Windows.Forms;
using Database.App.WinForms.Setup;
using Database.App.WinForms.UI;

namespace Database.App.WinForms
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var container = new Bootstrapper().BootstrapContainer();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var app = container.Resolve<ApplicationForm>();
            Application.Run(app);
        }
    }
}
