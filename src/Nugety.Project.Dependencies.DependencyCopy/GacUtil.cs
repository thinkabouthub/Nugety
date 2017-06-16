using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Nugety.Project.Dependencies
{
    public static class GacUtil
    {
        public static bool IsAssemblyInGAC(string assemblyFullName)
        {
            try
            {
                return Assembly.ReflectionOnlyLoad(assemblyFullName)
                               .GlobalAssemblyCache;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsAssemblyInGAC(Assembly assembly)
        {
            return assembly.GlobalAssemblyCache;
        }

    }
}
