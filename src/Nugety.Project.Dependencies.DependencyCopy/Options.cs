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

        [Option('s', "read", Required = true, HelpText = "Source directory to be processed.")]

        public string Source { get; set; }


        [Option('t', "read", Required = true,    HelpText = "Target directory to move dependency files to.")]

        public string Target { get; set; }


        [Option('b', "read", Required = true,    HelpText = "Main app bin folder to check for dependencies already existing.")]

        public string MainAppBinFolder { get; set; }
    }
}
