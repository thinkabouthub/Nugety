using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nugety
{
    [Flags]
    public enum AssemblyProbingModes
    {
        Optimistic = 1,
        Pessimistic = 2,
        OptimisticRedirect = 4
    }

    public interface INugetyCatalogProvider : IDisposable
    {
        event EventHandler<ModuleCancelEventArgs> ModuleLoading;

        event EventHandler<AssemblyResolveCancelEventArgs> AssemblyResolve;

        event EventHandler<AssemblyResolvedEventArgs> AssemblyResolved;

        NugetyCatalogOptions Options { get; }

        IEnumerable<ModuleInfo<T>> GetMany<T>(params Func<INugetyCatalogProvider, IEnumerable<ModuleInfo<T>>>[] loaders);

        T Load<T>(ModuleInfo module);

        IEnumerable<T> Load<T>(IEnumerable<ModuleInfo> modules);

        IDirectoryModuleProvider FromDirectory(string location = "Nugety");

        IDirectoryModuleProvider FromDirectories(params string[] location);

        Type GetModuleInitializer<T>(Assembly assembly);

        Type GetModuleInitializer(Assembly assembly, Type initialiser);

        IEnumerable<AssemblyLoadItem> AssemblyResolveHistory { get; }

        IEnumerable<ModuleInfo> Modules { get; }

        void AddModule(ModuleInfo module);

        void RemoveModule(ModuleInfo module);

        Assembly ResolveAssembly(AssemblyName name);

        AssemblyInfo ResolveAssemblyWithRedirect(AssemblyName name);

        AssemblyInfo ResolveAssemblyFromModules(AssemblyName name);

        AppDomain Domain { get; }

        //INugetyCatalogProvider UseLoggerFactory(ILoggerFactory loggerFactory);
    }
}