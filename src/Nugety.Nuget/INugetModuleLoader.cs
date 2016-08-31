using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;


namespace Nugety
{
    public interface INugetModuleLoader : IModuleProvider
    {
        NugetLoaderOptions Options { get; }
    }
}
