﻿using System.Collections.Generic;
using System.Reflection;

namespace Nugety
{
    public class NugetModuleLoader : INugetModuleLoader
    {
        private NugetLoaderOptions options;

        public NugetModuleLoader(INugetyCatalogProvider catalog)
        {
            Catalog = catalog;
        }

        public INugetyCatalogProvider Catalog { get; }

        public NugetLoaderOptions Options
        {
            get { return options ?? (options = new NugetLoaderOptions(this)); }
        }

        public virtual IEnumerable<ModuleInfo<T>> GetModules<T>(params string[] name)
        {
            return null;
        }

        public virtual Assembly LoadAssembly(string location)
        {
            return null;
        }

        public virtual AssemblyInfo LoadAssembly(ModuleInfo module, AssemblyName name)
        {
            return null;
        }
    }
}