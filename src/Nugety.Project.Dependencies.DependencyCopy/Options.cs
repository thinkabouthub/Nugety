using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nugety.Project.Dependencies
{
    class Options
    {

        [Option('s', "read", Required = true, HelpText = "Input files to be processed.")]

        public string Source { get; set; }


        [Option('t', "read", Required = true,    HelpText = "Input files to be processed.")]

        public string Target { get; set; }


        [Option('b', "read", Required = true,    HelpText = "Input files to be processed.")]

        public string MainAppBinFolder { get; set; }
    }
}
