using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nugety.DependencyCopy
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
                    Console.WriteLine("Target directory: " + options.Target);
                    Console.WriteLine("Source directory: " + options.Source);
                    Console.WriteLine("Main app bin directory: " + options.MainAppBinFolder);
                    
                    DependencyUtil.ProcessDependencies(options.Source, options.Target, options.MainAppBinFolder);
                }
                else
                {
                    if (!Directory.Exists(options.Source))
                    {
                        Console.WriteLine("Source directory doesnt exist: " + options.Source);
                    }
                    if (!Directory.Exists(options.MainAppBinFolder))
                    {
                        Console.WriteLine("Main app bin directory doesnt exist: " + options.MainAppBinFolder);
                    }
                }
            }
            else
            {
                Console.WriteLine("Must enter three folder arguments");

                Console.WriteLine("Target directory: " + options.Target);
                Console.WriteLine("Source directory: " + options.Source);
                Console.WriteLine("Main app bin directory: " + options.MainAppBinFolder);

                Console.WriteLine("args list" + args.Length);

                for (int i = 0; i < args.Length; i++)
                {
                    Console.WriteLine("arg " + i + " value is " + args[i]);
                }
            }
        }
    }
}
