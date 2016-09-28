using System.IO;
using System.Linq;
using System.Reflection;
using Nugety.Tests.Common;
using Xunit;

namespace Nugety.Tests
{
    public class GetModuleWithDependenciesTests
    {
        [Fact]
        public void Given_Dependency_When_DoesNotExist_Then_ThrowsException()
        {
            Assert.Throws<FileNotFoundException>(() =>
            {
                var modules = new NugetyCatalog()
                    .FromDirectory()
                    .GetModules<IModuleInitializer>("Module2 without dependency2");

                var instance = modules.Load().OfType<IDependencyVersion>().FirstOrDefault();
                var type = instance.GetDependencyType();
            });
        }

        [Fact]
        public void Given_Dependency_When_Exists_Then_ModuleReturned()
        {
            var modules = new NugetyCatalog()
                .FromDirectory()
                .GetModules<IModuleInitializer>("Module3 with dependency3 v0");

            var instances = modules.Load();

            Assert.True(modules.Any(m => m.Name == "Module3 with dependency3 v0"));
            Assert.True(instances.OfType<IModuleInitializer>().Any());
        }

        [Fact]
        public void Given_TwoModules_When_HasSameDependencyDifferentVersion_Then_BothVersionsLoad()
        {
            var modules = new NugetyCatalog()
                .FromDirectory()
                .GetModules<IModuleInitializer>("Module3 with dependency3 v0", "Module4 with dependency3 v1");

            Assert.True(modules.Count() == 2);
            var instances = modules.Load().OfType<IDependencyVersion>();
            Assert.True(instances.Count() == 2);

            var names = instances.Select(i => i.GetDependencyType().GetTypeInfo().AssemblyQualifiedName);
            Assert.True(names.Any(n => n.Contains("1.0.0.0")), "Module3 did not load Dependency2 version '1.0.0.0'");
            Assert.True(names.Any(n => n.Contains("1.0.1.0")), "Module4 did not load Dependency2 version '1.0.1.0'");
        }
    }
}