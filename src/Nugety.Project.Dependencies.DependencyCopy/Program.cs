﻿using System;
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
            //TODO maybe put something in here that allows the user to do a /help swith
            if (args.Length == 3)
            {
                var source = args[0];
                var target = args[1];
                var mainAppBinFolder = args[2];
                if (Directory.Exists(source) && Directory.Exists(mainAppBinFolder))
                {
                    DirectoryUtil.ProcessDependencies(source, target, mainAppBinFolder);
                }
            }
            else
            {
                Console.WriteLine("Must enter three folder arguments");
            }
        }
    }
}