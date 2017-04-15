using System;
using System.Reflection;

namespace Nugety
{
    public class AssemblyLoadItem
    {
        public AssemblyLoadItem() { }

        public AssemblyLoadItem(AssemblyName name, Assembly assembly = null)
        {
            this.Name = name;
            this.Assembly = assembly;
        }

        public AssemblyName Name { get; set; }

        public Assembly Assembly { get; set; }
    }
}
