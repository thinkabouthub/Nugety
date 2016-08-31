using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;


namespace Nugety
{
    public class AssemblyInfo
    {
        public AssemblyInfo(Assembly assembly)
        {
            this.Assembly = assembly;
            this.Location = this.Assembly.Location;
        }

        public Assembly Assembly { get; set; }
        public string Location { get; set; }
    }
}
