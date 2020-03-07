using System;
using Database.App.Console.Setup;
using Database.Core.Generator;
using Database.Core.IO;
using Database.Core.Validation;

namespace Database.App.Console
{
    class Program
    {

        private const string Header = @"
                        _,.---.---.---.--.._ 
                    _.-' `--.`---.`---'-. _,`--.._
                   /`--._ .'.     `.     `,`-.`-._\
                  ||   \  `.`---.__`__..-`. ,'`-._/
             _  ,`\ `-._\   \    `.    `_.-`-._,``-.
          ,`   `-_ \/ `-.`--.\    _\_.-'\__.-`-.`-._`.
         (_.o> ,--. `._/'--.-`,--`  \_.-'       \`-._ \
          `---'    `._ `---._/__,----`           `-. `-\
                    /_, ,  _..-'                    `-._\
                    \_, \/ ._(
                     \_, \/ ._\
                      `._,\/ ._\
                        `._// ./`-._
                          `-._-_-_.-'";

        static int Main(string[] args)
        {
            try
            {
                System.Console.Out.WriteLine(Header);
                System.Console.Out.WriteLine();

                var container = new Bootstrapper().BootstrapContainer();

                var schemaGenerator = container.Resolve<IDatabaseSchemaGenerator>();
                var schemaValidator = container.Resolve<IValidationEngine>();
                var schemaWriter = container.Resolve<ISchemaWriter>();

                var databaseSchema = schemaGenerator.GenerateSchema();
                schemaValidator.ValidateDefinition(databaseSchema);
                schemaWriter.Write(databaseSchema);

                return 0;
            }
            catch (Exception e)
            {
                System.Console.Out.WriteLine(e.Message);
                return 1;
            }
        }
    }
}
