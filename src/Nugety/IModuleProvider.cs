using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Nugety
{
    public interface IModuleProvider
    {
        INugetyCatalogProvider Catalog { get; }

        IEnumerable<ModuleInfo<T>> GetModules<T>(params string[] name);
    }
}
