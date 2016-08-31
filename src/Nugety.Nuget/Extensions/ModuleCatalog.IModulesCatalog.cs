using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Autofac;

namespace Nugety
{
    public static class IModulesCatalogExtensions
    {
        /// TODO: location may need to be determined based on target platform
        /// http://lastexitcode.com/projects/NuGet/FileLocations/
        public static INugetModuleLoader FromNuget(this INugetyCatalogProvider catalog, string location = @"%UserProfile%\.nuget\packages")
        {
            return new NugetModuleLoader(catalog).Options.SetLocation(location);
        }
    }
}