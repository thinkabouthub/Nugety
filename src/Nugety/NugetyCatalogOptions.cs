namespace Nugety
{
    public class NugetyCatalogOptions
    {
        private readonly NugetyCatalog _catalog;

        public NugetyCatalogOptions(NugetyCatalog catalog)
        {
            _catalog = catalog;
        }

        public string FileNameFilterPattern { get; set; }

        public string ModuleNameFilterPattern { get; set; }


        public virtual NugetyCatalog SetFileNameFilterPattern(string pattern)
        {
            FileNameFilterPattern = pattern;
            return _catalog;
        }

        public virtual NugetyCatalog SetModuleNameFilterPattern(string pattern)
        {
            ModuleNameFilterPattern = pattern;
            return _catalog;
        }
    }
}