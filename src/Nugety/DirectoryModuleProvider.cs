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

        public DirectoryLoaderOptions Options => options ?? (options = new DirectoryLoaderOptions(this));

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
                if (!this.Catalog.Modules.Any(m => m.Name == directory.Name))
                {
                    var module = LoadUsingFileName<T>(directory);
                    if (module != null) modules.Add(module);
                }
            }
            return modules;
        }

        public virtual string ParseModuleFileNameFilter(string filter)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                var extension = Path.GetExtension(filter);
                if (string.IsNullOrEmpty(extension))
                {
                    filter = string.Concat(filter, ".dll");
                }
                else if (extension != ".dll")
                {
                    throw new DirectoryNotFoundException($"ModuleFileNameFilterPattern '{filter}' file extension must be empty or of type '.dll'");
                }
            }
            return filter;
        }

        protected virtual ModuleInfo<T> LoadUsingFileName<T>(DirectoryInfo directory)
        {
            var filter = this.ParseModuleFileNameFilter(Catalog.Options.ModuleFileNameFilterPattern);
            foreach (var file in directory.GetFileSystemInfos(!string.IsNullOrEmpty(filter) 
                ? filter
                : "*.dll", 
                SearchOption.AllDirectories))
            {
                var assemblyName = AssemblyName.GetAssemblyName(file.FullName);
                var info = this.LoadAssembly(null, assemblyName);
                if (info != null)
                {
                    var type = this.Catalog.GetModuleInitializer<T>(info.Assembly);
                    if (type != null)
                    {
                        var module = new ModuleInfo<T>(this, directory.Name, new AssemblyInfo(info.Assembly), type);
                        this.Catalog.AddModule(module);
                        return module;
                    }
                }
            }
            return null;
        }

        public virtual IEnumerable<DirectoryInfo> GetModuleDirectories(params string[] name)
        {
            var list = new Collection<DirectoryInfo>();
            if (!Directory.Exists(Options.Location)) throw new DirectoryNotFoundException($"Directory Catalog '{Options.Location}' does not exist");

            var directory = new DirectoryInfo(Options.Location);
            var directories = directory.GetDirectories(
                    !string.IsNullOrEmpty(Catalog.Options.ModuleNameFilterPattern)
                        ? Catalog.Options.ModuleNameFilterPattern
                        : "*", SearchOption.TopDirectoryOnly);
            var notFound = name.Where(n => !directories.Any(d => d.Name == n)).ToList();
            if (notFound.Any()) throw new DirectoryNotFoundException($"Module Directory not found for '{string.Join(",", notFound.ToArray())}'");
            if (name.Length > 0)
            {
                foreach (var n in name)
                {
                    var namedDirectory = directories.FirstOrDefault(d => d.Name == n);
                    if (namedDirectory != null) list.Add(namedDirectory);
                }
            }
            else
            {
                foreach (var d in directories) list.Add(d);
            }
            return list;
        }

        public virtual AssemblyInfo LoadAssembly(ModuleInfo module, AssemblyName name)
        {
            var assemblies = this.Catalog.Domain.GetAssemblies();
            var names = assemblies.Select(a => a.GetName());

            Assembly assembly;
            var loadedName = names.FirstOrDefault(n => n.ToString().Equals(name.ToString()));
            if (loadedName != null)
            {
                assembly = assemblies.FirstOrDefault(a => a.GetName().ToString() == loadedName.ToString());
            }
            else
            {
                assembly = Assembly.Load(name);
            }
            return assembly != null ? new AssemblyInfo(assembly) : null;
        }

        public virtual AssemblyInfo ResolveAssembly(ModuleInfo module, AssemblyName name)
        {
            var directory = new DirectoryInfo(Path.GetDirectoryName(module.Location));
            var filtered = directory.GetFileSystemInfos(string.Concat(name.Name, ".dll"), SearchOption.AllDirectories);
            var assemblyInfo = this.ResolveAssembly(module, name, filtered);

            if (assemblyInfo == null)
            {
                var files = directory.GetFileSystemInfos("*.dll", SearchOption.AllDirectories).Where(f => !filtered.Any(t => t.Name.Equals(f.Name))).ToArray();
                assemblyInfo = this.ResolveAssembly(module, name, files);
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
                foreach (var file in files)
                {
                    var assemblyName = AssemblyName.GetAssemblyName(file.FullName);
                    if (assemblyName.ToString().Equals(name.ToString()))
                    {
                        try
                        {
                            var info = module.ModuleProvider.LoadAssembly(module, assemblyName);
                            if (info != null) return info;
                        }
                        catch
                        {
                            // Consume exception. Assembly failing to load should not fail all assemblies.
                        }
                    }
                }   
            }
            return null;
        }
    }
}