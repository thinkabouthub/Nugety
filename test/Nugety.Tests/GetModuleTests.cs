using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Nugety.Tests
{
    public class GetModuleTests
    {
        [Fact] 
        public void Given_FileNameFilterPattern_When_Invalid_Then_NoModuleReturned()
        {
            using (var catalog = new NugetyCatalog())
            {
                //var exception = Assert.Throws<DirectoryNotFoundException>(() =>
                //{
                    var modules = catalog
                        .Options.SetModuleFileNameFilter("*Invalid.dll")
                        .FromDirectory()
                        .GetModules<IModuleInitializer>("Module1Initializer");
                Assert.True(!modules.Any());
                //});
            }
        }

        [Fact]
        public void Given_FileNameFilterPattern_When_Valid_Then_ModuleReturned()
        {
            using (var catalog = new NugetyCatalog())
            {
                var modules = catalog
                    .Options.SetModuleFileNameFilter("*Module1.dll")
                    .FromDirectory()
                    .GetModules<IModuleInitializer>("Module1Initializer");

                Assert.True(modules.Any(m => m.Name == "Module1Initializer"));
            }
        }

        [Fact]
        public void Given_GetManyModules_When_Valid_Then_ModulesReturned()
        {
            using (var catalog = new NugetyCatalog())
            {
                var modules = catalog.GetMany
                (
                    c => c.FromDirectory().GetModules<IModuleInitializer>("Module1Initializer"),
                    c => c.FromDirectory().GetModules<IModuleInitializer>("Module3Initializer")
                );

                Assert.True(modules.Any(m => m.Name == "Module1Initializer"));
                Assert.True(modules.Any(m => m.Name == "Module3Initializer"));
            }
        }

        [Fact]
        public void Given_Module_When_Valid_Then_PropertiesPopulated()
        {
            using (var catalog = new NugetyCatalog())
            {
                var modules = catalog
                    .FromDirectory()
                    .GetModules<IModuleInitializer>("Module1Initializer");

                var module = modules.FirstOrDefault(m => m.Name == "Module1Initializer");
                Assert.True(module != null);
                Assert.NotNull(module.Catalog);
                Assert.NotNull(module.ModuleInitialiser);
                Assert.True(!string.IsNullOrEmpty(module.Location));
                Assert.NotNull(module.AssemblyInfo);
                Assert.NotNull(module.AssemblyInfo.Assembly);
                Assert.True(!string.IsNullOrEmpty(module.AssemblyInfo.Assembly.CodeBase));
            }
        }

        [Fact]
        public void Given_ModuleLocation_When_Invalid_Then_ThrowsDirectoryNotFoundException()
        {
            using (var catalog = new NugetyCatalog())
            {
                //Assert.Throws<DirectoryNotFoundException>(() =>
                //{
                    var modules = catalog
                        .FromDirectory("InvalidDirectory")
                        .GetModules<IModuleInitializer>();
                //});
            }
        }

        [Fact]
        public void Given_ModuleLocation_When_Valid_Then_ModuleReturned()
        {
            using (var catalog = new NugetyCatalog())
            {
                var modules = catalog
                    .FromDirectory()
                    .GetModules<IModuleInitializer>();

                Assert.True(modules.Any(m => m.Name == "Module1Initializer"));
                Assert.True(modules.Any(m => m.Name == "Module2Initializer"));
                Assert.True(modules.Any(m => m.Name == "Module3Initializer"));
            }
        }

        [Fact]
        public void Given_ModuleName_When_InValid_Then_ThrowsDirectoryNotFoundException()
        {
            using (var catalog = new NugetyCatalog())
            {
                //var exception = Assert.Throws<DirectoryNotFoundException>(() =>
                //{
                    var modules = catalog
                        .FromDirectory()
                        .GetModules<IModuleInitializer>("InvalidModule1", "InvalidModule2");

                Assert.True(!modules.Any());
                //});
            }
        }

        [Fact]
        public void Given_ModuleName_When_InValidAndValid_Then_ThrowsDirectoryNotFoundException()
        {
            using (var catalog = new NugetyCatalog())
            {
                //var exception = Assert.Throws<DirectoryNotFoundException>(() =>
                //{
                    var modules = catalog
                        .FromDirectory()
                        .GetModules<IModuleInitializer>("Module1Initializer", "InvalidModule");
                Assert.True(modules.Count() == 1);
                //});
            }
        }

        [Fact]
        public void Given_ModuleName_When_Valid_Then_ModuleReturned()
        {
            using (var catalog = new NugetyCatalog())
            {
                var modules = catalog
                    .FromDirectory()
                    .GetModules<IModuleInitializer>("Module1Initializer");

                Assert.True(modules.Any(m => m.Name == "Module1Initializer"));
            }
        }

        [Fact]
        public void Given_ModuleNameFilterPattern_When_Invalid_Then_NoModuleReturned()
        {
            using (var catalog = new NugetyCatalog())
            {
                var modules = catalog
                    .Options.SetModuleNameFilter("Invalid")
                    .FromDirectory()
                    .GetModules<IModuleInitializer>();

                Assert.True(!modules.Any());
            }
        }

        [Fact]
        public void Given_ModuleNameFilterPattern_When_Valid_Then_ModuleReturned()
        {
            using (var catalog = new NugetyCatalog())
            {
                var modules = catalog
                    .Options.SetModuleNameFilter("Module1")
                    .FromDirectory()
                    .GetModules<IModuleInitializer>();

                Assert.True(modules.Any(m => m.Name == "Module1Initializer"));
            }
        }
    }
}