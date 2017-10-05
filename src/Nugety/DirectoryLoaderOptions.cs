using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Nugety
{
    public class DirectoryLoaderOptions
    {
        public DirectoryLoaderOptions(IDirectoryModuleProvider loader)
        {
            Loader = loader;
        }

        public IDirectoryModuleProvider Loader { get; }

        private Collection<string> _directories = new Collection<string>();
        public IEnumerable<string> Directories 
        {
            get { return this._directories; }
        }

        public IDirectoryModuleProvider SetDirectory(params string[] directory)
        {
            foreach (var d in directory)
            {
                this._directories.Add(d);
            }
            return this.Loader;
        }
    }
}