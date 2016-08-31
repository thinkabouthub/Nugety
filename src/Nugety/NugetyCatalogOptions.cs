using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nugety
{
    public class NugetyCatalogOptions
    {
        private NugetyCatalog _catalog;
        public NugetyCatalogOptions(NugetyCatalog catalog)
        {
            _catalog = catalog;
        }

        public string FileNameFilterPattern { get; set; }

        public string ModuleNameFilterPattern { get; set; }


        public virtual NugetyCatalog SetFileNameFilterPattern(string pattern)
        {
            this.FileNameFilterPattern = pattern;
            return _catalog;
        }

        public virtual NugetyCatalog SetModuleNameFilterPattern(string pattern)
        {
            this.ModuleNameFilterPattern = pattern;
            return _catalog;
        }
    }
}
