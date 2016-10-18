using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Nugety.Tests
{
    public class ResolveAssemblyTests
    {
        [Fact]
        public void Given_AssemblyResolve_When_DependencyProbeFailed_Then_DependencyFoundWithModule()
        {
            using (var catalog = new NugetyCatalog())
            {
                var modules = catalog
                    .Options.SetModuleFileNameFilter("*Module5.dll")
                    .FromDirectory()
                    .GetModules<IModuleInitializer>("Module5");

                var instances = modules.Load();

                instances.FirstOrDefault().ConfigureServices(null);
                Assert.True(!instances.OfType<InvalidInterface>().Any());
            }          
        }

        [Fact]
        public void Given_ModuleLoaded_When_AssemblyLoad_Then_AssemblyDiscovered()
        {
            using (var catalog = new NugetyCatalog())
            {
                var modules = catalog
                    .Options.SetModuleFileNameFilter("*Module6.dll")
                    .FromDirectory()
                    .GetModules<IModuleInitializer>().Load();

                var assembly = Assembly.Load("Nugety.Tests.Module6, Version=1.0.0.0, Culture=neutral, PublicKeyToken=0b0afd28caef48a5");

                Assert.NotNull(assembly);
            }
        }
    }
}