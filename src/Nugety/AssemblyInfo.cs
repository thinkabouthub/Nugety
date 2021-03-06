﻿using System.Reflection;

namespace Nugety
{
    public class AssemblyInfo
    {
        public AssemblyInfo(Assembly assembly, ModuleInfo module = null)
        {
            this.Assembly = assembly;
            this.Location = Assembly.Location;
            this.Module = module;
        }

        public ModuleInfo Module { get; private set; }

        public Assembly Assembly { get; private set; }

        public string Location { get; private set; }
    }
}