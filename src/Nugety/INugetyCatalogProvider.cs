using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Nugety
{
    public interface INugetyCatalogProvider
    {
        event EventHandler<ModuleCancelEventArgs> ModuleLoading;

        event EventHandler<AssemblyResolveCancelEventArgs> AssemblyResolve;

        event EventHandler<AssemblyResolvedEventArgs> AssemblyResolved;

        NugetyCatalogOptions Options { get; }

        IEnumerable<ModuleInfo<T>> GetMany<T>(params Func<INugetyCatalogProvider, IEnumerable<ModuleInfo<T>>>[] loaders);

        T Load<T>(ModuleInfo module);

        IEnumerable<T> Load<T>(IEnumerable<ModuleInfo> modules);

        IDirectoryModuleProvider FromDirectory(string location = "Nugety");

        Type GetModuleInitializer<T>(Assembly assembly);

        Type GetModuleInitializer(Assembly assembly, Type initialiser);

        IEnumerable<AssemblyName> AssemblyResolveFailed { get; }

        IEnumerable<ModuleInfo> Modules { get; }

        void AddModule(ModuleInfo module);

        void RemoveModule(ModuleInfo module);

        Assembly ResolveAssembly(AssemblyName name);

        AppDomain Domain { get; }

        INugetyCatalogProvider UseLoggerFactory(ILoggerFactory loggerFactory);
    }
}