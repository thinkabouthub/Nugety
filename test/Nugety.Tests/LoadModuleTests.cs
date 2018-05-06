using System.IO;
using System.Linq;
using Xunit;

namespace Nugety.Tests
{
    public interface InvalidInterface
    {
    }

    public class LoadModuleTests
    {
        [Fact]
        public void Given_Initializer_When_Invalid_Then_NoInstancesReturned()
        {
            using (var catalog = new NugetyCatalog())
            {
                //var exception = Assert.Throws<DirectoryNotFoundException>(() =>
                //{
                    var modules = catalog
                        .FromDirectory()
                        .GetModules<InvalidInterface>("Module1");
                Assert.True(!modules.Any());
                //});
            }
        }

        [Fact]
        public void Given_Initializer_When_Valid_Then_ModuleInstanceReturned()
        {
            using (var catalog = new NugetyCatalog())
            {
                var modules = catalog
                    .FromDirectory()
                    .GetModules<IModuleInitializer>("Module1Initializer");

                var instances = modules.Load();
                Assert.True(instances.OfType<IModuleInitializer>().Any());
            }
        }
    }
}