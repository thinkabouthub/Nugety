using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Nugety
{
    public class DirectoryModuleProvider : IDirectoryModuleProvider
    {
        private DirectoryLoaderOptions options;

        public DirectoryModuleProvider(INugetyCatalogProvider catalog)
        {
            Catalog = catalog;
        }

        public INugetyCatalogProvider Catalog { get; }

        public DirectoryLoaderOptions Options
        {
            get { return options ?? (options = new DirectoryLoaderOptions(this)); }
        }

        public virtual IEnumerable<ModuleInfo<T>> GetModules<T>(params string[] name)
        {
            return LoadFromDirectory<T>(name);
        }

        protected virtual IEnumerable<ModuleInfo<T>> LoadFromDirectory<T>(params string[] name)
        {
            var modules = new List<ModuleInfo<T>>();
            var directories = GetModuleDirectories(name);
            foreach (var directory in directories)
            {
                var module = LoadUsingFileName<T>(directory);
                if (module != null) modules.Add(module);
            }
            return modules;
        }

        protected virtual ModuleInfo<T> LoadUsingFileName<T>(DirectoryInfo directory)
        {
            var names = this.Catalog.Domain.GetAssemblies().Select(a => a.GetName());
            foreach (var file in directory.GetFileSystemInfos(!string.IsNullOrEmpty(Catalog.Options.ModuleFileNameFilterPattern) 
                ? Catalog.Options.ModuleFileNameFilterPattern 
                : "*.dll", 
                SearchOption.AllDirectories))
            { 
                if (!names.Any(n => n == AssemblyName.GetAssemblyName(file.FullName)))
                {
                    var info = this.LoadAssembly(null, file.FullName);
                    if (info != null)
                    {
                        var module = new ModuleInfo<T>(this, directory.Name, new AssemblyInfo(info.Assembly));
                        var type = this.Catalog.GetModuleInitializer<T>(info.Assembly);
                        if (type != null)
                        {
                            module.AddModuleInitialiser(type);
                            this.Catalog.AddModule(module);
                            return module;
                        }
                    }
                }
            }
            return null;
        }

        public virtual IEnumerable<DirectoryInfo> GetModuleDirectories(params string[] name)
        {
            var list = new Collection<DirectoryInfo>();
            if (!Directory.Exists(Options.Location))
                throw new DirectoryNotFoundException(Options.Location);

            var directory = new DirectoryInfo(Options.Location);
            var directories = directory.GetDirectories(
                    !string.IsNullOrEmpty(Catalog.Options.ModuleNameFilterPattern)
                        ? Catalog.Options.ModuleNameFilterPattern
                        : "*", SearchOption.TopDirectoryOnly);
            var notFound = name.Where(n => !directories.Any(d => d.Name == n)).ToList();
            if (notFound.Any()) throw new DirectoryNotFoundException(string.Format("Module Directory not found for '{0}'", string.Join(",", notFound.ToArray())));
            if (name.Length > 0)
                foreach (var n in name)
                {
                    var namedDirectory = directories.FirstOrDefault(d => d.Name == n);
                    if (namedDirectory != null) list.Add(namedDirectory);
                }
            else
            {
                foreach (var d in directories) list.Add(d);
            }
            return list;
        }

        public virtual AssemblyInfo LoadAssembly(ModuleInfo module, string location)
        {
            var name = AssemblyName.GetAssemblyName(location);
            var assembly = Assembly.Load(name);
            return assembly != null ? new AssemblyInfo(assembly) : null;
        }

        public virtual AssemblyInfo LoadAssembly(ModuleInfo module, AssemblyName name)
        {
            var directory = new DirectoryInfo(Path.GetDirectoryName(module.Location));
            var filtered = directory.GetFileSystemInfos(string.Concat(name.Name, ".dll"), SearchOption.AllDirectories);
            var assemblyInfo = ResolveAssembly(module, name, filtered);

            if (assemblyInfo == null)
            {
                var files = directory.GetFileSystemInfos("*.dll", SearchOption.AllDirectories).Where(f => !filtered.Any(t => t.Name.Equals(f.Name))).ToArray();
                assemblyInfo = ResolveAssembly(module, name, files);
            }
            if (assemblyInfo != null)
            {
                module.AddAssembly(assemblyInfo);
            }
            return assemblyInfo;
        }

        protected virtual AssemblyInfo ResolveAssembly(ModuleInfo module, AssemblyName name, FileSystemInfo[] files)
        {
            if (files.Any())
            { 
                var assemblies = this.Catalog.Domain.GetAssemblies();
                foreach (var file in files)
                {
                    var assemblyName = AssemblyName.GetAssemblyName(file.FullName);
                    var dependency = module.Assemblies.FirstOrDefault(d => d.Assembly.GetName().ToString().Equals(assemblyName.ToString()));
                    if (dependency == null)
                    {
                        if (!assemblies.Any(c => c.GetName().ToString().Equals(assemblyName.ToString())))
                        {
                            try
                            {
                                var info = this.LoadAssembly(module, file.FullName);
                                if (info != null) dependency = new AssemblyInfo(info.Assembly);
                            }
                            catch
                            {
                                // Consume exception. Assembly failing to load should not fail all assemblies.
                            }
                        }
                    }
                    if (dependency != null && dependency.Assembly.GetName().ToString().Equals(name.ToString()))
                    {
                        return dependency;
                    }
                }   
            }
            return null;
        }
    }
}