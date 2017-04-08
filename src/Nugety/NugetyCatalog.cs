using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Nugety
{
    public class NugetyCatalog : INugetyCatalogProvider
    {
        private NugetyCatalogOptions options;
        private readonly Collection<ModuleInfo> _modules = new Collection<ModuleInfo>();
        public static readonly object _lock = new object();
        //public ILoggerFactory Logger { get; set; }

        public NugetyCatalog(AppDomain domain)
        {
            this.Domain = domain;
            if (this.Domain != null && this.Options.AssemblySearchModes.HasFlag(AssemblyHeuristicModes.SearchCatalog)) this.Domain.AssemblyResolve += this.Domain_AssemblyResolve;
        }

        public NugetyCatalog() : this(AppDomain.CurrentDomain)
        {
        }

        public static INugetyCatalogProvider Catalog { get; set; }

        private bool disposed = false;

        public AppDomain Domain { get; }

        private readonly Collection<AssemblyName> assembliesFailedToResolve = new Collection<AssemblyName>();

        public IEnumerable<AssemblyName> AssembliesFailedToResolve => this.assembliesFailedToResolve;

        private readonly Collection<AssemblyName> resolveCalledForAssemblies = new Collection<AssemblyName>();

        public IEnumerable<AssemblyName> ResolveCalledForAssemblies => this.resolveCalledForAssemblies;

        public IEnumerable<ModuleInfo> Modules => this._modules;

        public void AddModule(ModuleInfo module)
        {
            lock (_lock)
            {
                this._modules.Add(module);
            }
        }

        public void RemoveModule(ModuleInfo module)
        {
            lock (_lock)
            {
                this._modules.Remove(module);
            }
        }

        public NugetyCatalogOptions Options => options ?? (options = new NugetyCatalogOptions(this));

        public virtual T Load<T>(ModuleInfo module)
        {
            var args = new ModuleCancelEventArgs(module);
            OnModuleLoading(args);
            if (!args.Cancel)
            {
                var instance = (T)module.AssemblyInfo.Assembly.CreateInstance(module.ModuleInitialiser.FullName);
                Debug.WriteLine($"Module Initializer Instance of type '{instance.GetType().FullName}' loaded");
                this.OnModuleLoaded(module, instance);
                return instance;
            }
            return default(T);
        }

        public virtual IEnumerable<T> Load<T>(IEnumerable<ModuleInfo> modules)
        {
            var instances = new Collection<T>();
            foreach (var module in modules)
            {
                var i = this.Load<T>(module);
                if (i != null) instances.Add(i);
            }
            return instances;
        }

        public virtual Type GetModuleInitializer<T>(Assembly assembly)
        {
            return GetModuleInitializer(assembly, typeof(T));
        }

        public virtual Type GetModuleInitializer(Assembly assembly, Type initialiser)
        {
            Type type = null;
            try
            {
                type = assembly.GetTypes().FirstOrDefault(t => !t.GetTypeInfo().IsInterface && initialiser.IsAssignableFrom(t));
                if (type == null)
                {
                    type = assembly.ExportedTypes.FirstOrDefault(t => t == initialiser);
                }
                if (type != null)
                    Debug.WriteLine($"Module Initializer '{type}' of type '{initialiser}' found in Assembly '{assembly}'");
                else
                    Debug.WriteLine($"Module Initializer of type '{initialiser}' not found in Assembly '{assembly}'");

            }
            catch (ReflectionTypeLoadException)
            {
                if (!this.Options.IgnoreLoaderExceptions) throw;
            }
            return type;
        }

        public virtual IEnumerable<ModuleInfo<T>> GetMany<T>(params Func<INugetyCatalogProvider, IEnumerable<ModuleInfo<T>>>[] loaders)
        {
            var loadModules = new List<ModuleInfo<T>>();

            foreach (var l in loaders) loadModules.AddRange(l.Invoke(this));
            return loadModules.AsEnumerable();
        }

        public virtual IDirectoryModuleProvider FromDirectory(string location = "Nugety")
        {
            return new DirectoryModuleProvider(this).Options.SetLocation(location);
        }

        public event EventHandler<AssemblyResolveCancelEventArgs> AssemblyResolve;

        public event EventHandler<AssemblyResolvedEventArgs> AssemblyResolved;

        protected void OnAssemblyResolved(AssemblyResolvedEventArgs args)
        {
            AssemblyResolved?.Invoke(this, args);
        }

        protected void OnAssemblyResolve(AssemblyResolveCancelEventArgs args)
        {
            AssemblyResolve?.Invoke(this, args);
        }

        public event EventHandler<ModuleIntanceEventArgs> ModuleLoaded;

        public event EventHandler<ModuleCancelEventArgs> ModuleLoading;

        protected void OnModuleLoaded(ModuleInfo module, object value)
        {
            ModuleLoaded?.Invoke(this, new ModuleIntanceEventArgs(module, value));
        }

        protected void OnModuleLoading(ModuleCancelEventArgs args)
        {
            ModuleLoading?.Invoke(this, args);
        }

        protected virtual Assembly Domain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                if (this.Options.AssemblySearchModes.HasFlag(AssemblyHeuristicModes.SearchCatalog)) return this.ResolveAssembly(new AssemblyName(args.Name));
            }
            catch (Exception)
            {
                if (!args.Name.Contains(".resources")) throw;
            }
            return null;
        }

        public virtual Assembly ResolveAssembly(AssemblyName name)
        {
            lock (_lock)
            {
                var cancelArgs = new AssemblyResolveCancelEventArgs(name);
                this.OnAssemblyResolve(cancelArgs);
                if (!cancelArgs.Cancel)
                {
                    if (!this.HasResolveBeenCalled(name))
                    {
                        this.AddToResolveCalled(name);

                        Debug.WriteLine($"Resolve Assembly '{name.FullName}'");

                        AssemblyInfo assemblyInfo = null;
                        if (this.Options.AssemblySearchModes.HasFlag(AssemblyHeuristicModes.SearchCatalog)) assemblyInfo = this.ResolveAssemblyFromModules(name);
                        if (assemblyInfo != null && assemblyInfo.Module != null)
                        {
                            Debug.WriteLine($"Assembly '{name.FullName}' found in Module '{assemblyInfo.Module.Name}' at location '{assemblyInfo.Location}'");
                        }
                        if (assemblyInfo == null && this.Options.AssemblySearchModes.HasFlag(AssemblyHeuristicModes.OptimisticRedirect))
                        {
                            assemblyInfo = this.ResolveAssemblyWithRedirect(name);
                            if (assemblyInfo != null)
                            {
                                Debug.WriteLine($"Assembly '{name.FullName}' found using Optimistic Redirect at location '{assemblyInfo.Location}'");
                            }
                        }
                        if (assemblyInfo != null && assemblyInfo.Assembly != null)
                        {
                            var args = new AssemblyResolvedEventArgs(name, assemblyInfo.Module, assemblyInfo);
                            this.OnAssemblyResolved(args);

                            return assemblyInfo.Assembly;
                        }

                        Debug.WriteLine($"Cannot Resolve Assembly '{name.FullName}'");
                        this.AddToResolveFailed(name);
                    }
                    return null;
                }
                return cancelArgs.Assembly;
            }
        }

        protected virtual bool HasResolveFailed(AssemblyName name)
        {
            return this.AssembliesFailedToResolve.Any(n => n.FullName.Equals(name.FullName));
        }

        protected virtual void AddToResolveFailed(AssemblyName name)
        {
            this.assembliesFailedToResolve.Add(name);
        }

        protected virtual bool HasResolveBeenCalled(AssemblyName name)
        {
            return this.ResolveCalledForAssemblies.Any(n => n.FullName.Equals(name.FullName));
        }

        protected virtual void AddToResolveCalled(AssemblyName name)
        {
            this.resolveCalledForAssemblies.Add(name);
        }

        public virtual AssemblyInfo ResolveAssemblyWithRedirect(AssemblyName name)
        {
            lock (_lock)
            {
                var redirectName = new AssemblyName(name.Name);
                var assembly = Assembly.Load(redirectName);
                if (assembly != null)
                {
                    return new AssemblyInfo(assembly);
                }
                else
                {
                    return this.ResolveAssemblyFromModules(redirectName);
                }
            }
        }

        public virtual AssemblyInfo ResolveAssemblyFromModules(AssemblyName name)
        {
            lock (_lock)
            {

                foreach (var module in this.Modules.Where(m => m.AllowAssemblyResolve))
                {
                    var assemblyInfo = module.ModuleProvider.ResolveAssembly(module, name);
                    if (assemblyInfo != null)
                    {
                        return assemblyInfo;
                    }
                }
            }
            return null;
        }

        //public virtual INugetyCatalogProvider UseLoggerFactory(ILoggerFactory loggerFactory)
        //{
        //    this.Logger = loggerFactory;
        //    return this;
        //}

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (this.Domain != null)
                    {
                        this.Domain.AssemblyResolve -= this.Domain_AssemblyResolve;
                    }
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}