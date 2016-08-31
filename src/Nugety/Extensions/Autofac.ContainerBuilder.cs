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
    public static class ContainerBuilderExtensions
    {
        public static void RegisterModules(this ContainerBuilder builder, IEnumerable<Autofac.Module> modules)
        {
            foreach (var m in modules)
            {
                builder.RegisterModule(m);
            }
        }
    }
}
