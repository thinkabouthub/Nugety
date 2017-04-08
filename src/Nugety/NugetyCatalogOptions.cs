namespace Nugety
{
    public class NugetyCatalogOptions
    {
        private readonly NugetyCatalog _catalog;

        public NugetyCatalogOptions(NugetyCatalog catalog)
        {
            _catalog = catalog;
            this.IgnoreLoaderExceptions = true;
            this.AssemblySearchModes = AssemblySearchModes.SearchCatalog | AssemblySearchModes.FileName |
                                       AssemblySearchModes.AssemblyName;
        }

        public string ModuleFileNameFilterPattern { get; set; }

        public string ModuleNameFilterPattern { get; set; }

        public AssemblySearchModes AssemblySearchModes { get; set; }

        public bool IgnoreLoaderExceptions { get; set; }

        public virtual NugetyCatalog SetModuleFileNameFilter(string pattern)
        {
            this.ModuleFileNameFilterPattern = pattern;
            return _catalog;
        }

        public virtual NugetyCatalog SetModuleNameFilter(string pattern)
        {
            this.ModuleNameFilterPattern = pattern;
            return _catalog;
        }

        public virtual NugetyCatalog SetIgnoreLoaderExceptions(bool ignoreLoaderExceptions)
        {
            this.IgnoreLoaderExceptions = ignoreLoaderExceptions;
            return _catalog;
        }

        public virtual NugetyCatalog SetAssemblySearchMode(AssemblySearchModes modes)
        {
            this.AssemblySearchModes = modes;
            return _catalog;
        }
    }
}