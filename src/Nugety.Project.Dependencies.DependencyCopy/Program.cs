using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nugety.Project.Dependencies
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            var valid = CommandLine.Parser.Default.ParseArguments(args, options);
            //TODO maybe put something in here that allows the user to do a /help switch

            if (valid)
            {
                Console.WriteLine("Beginning Dependency Processing.");
                
                if (Directory.Exists(options.Source) && Directory.Exists(options.MainAppBinFolder))
                {
                    DependencyUtil.ProcessDependencies(options.Source, options.Target, options.MainAppBinFolder);
                }
            }
            else
            {
                Console.WriteLine("Must enter three folder arguments");
            }
        }
    }
}
