using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Nugety
{
    public class ModuleInfo<T> : ModuleInfo
    {
        public ModuleInfo(IModuleProvider provider, string name, AssemblyInfo assemblyInfo, Type moduleInitialiser) : base(provider, name, assemblyInfo, moduleInitialiser)
        {
        }

        public ModuleInfo(IModuleProvider provider, AssemblyInfo assembly, Type moduleInitialiser = null)
            : base(provider, assembly, moduleInitialiser)
        {
        }
    }

    public class ModuleInfo
    {
        public static readonly object _lock = new object();

        private readonly Collection<AssemblyInfo> _assemblies = new Collection<AssemblyInfo>();

        public ModuleInfo(IModuleProvider provider, string name, AssemblyInfo assemblyInfo, Type moduleInitialiser) : this(provider, assemblyInfo, moduleInitialiser)
        {
            this.Name = name;
        }

        public ModuleInfo(IModuleProvider provider, AssemblyInfo assembly, Type moduleInitialiser)
        {
            this.ModuleProvider = provider;
            this.Name = assembly.Assembly.GetName().Name;
            this.AssemblyInfo = assembly;
            this.AddModuleInitialiser(moduleInitialiser);
            this.AllowAssemblyResolve = true;
        }

        public INugetyCatalogProvider Catalog => this.ModuleProvider?.Catalog; 

        public IModuleProvider ModuleProvider { get; private set; }

        public Type ModuleInitialiser { get; private set; }

        public string Name { get; }

        public bool AllowAssemblyResolve { get; set; }

        public string Location => new Uri(AssemblyInfo.Assembly.CodeBase).LocalPath; 

        public AssemblyInfo AssemblyInfo { get; }

        protected virtual void AddModuleInitialiser(Type type)
        {
            this.ModuleInitialiser = type;
        }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(Name) ? Name : base.ToString();
        }
    }
}