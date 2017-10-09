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

        public string Directory { get; protected set; }

        public bool IncludeExecutingDirectory { get; protected set; }

        public virtual IDirectoryModuleProvider SetDirectory(string directory)
        {
            this.Directory = directory;
            return this.Loader;
        }

        public virtual IDirectoryModuleProvider SetIncludeExecutingDirectory(bool include)
        {
            this.IncludeExecutingDirectory = include;
            return this.Loader;
        }
    }
}