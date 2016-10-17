﻿using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Nugety.Tests
{
    public class GetModuleTests
    {
        [Fact] 
        public void Given_FileNameFilterPattern_When_Invalid_Then_NoModuleReturned()
        {
            var modules = new NugetyCatalog()
                .Options.SetModuleFileNameFilter("*Module3.dll")
                .FromDirectory()
                .GetModules<IModuleInitializer>();

            Assert.True(!modules.Any(m => m.Name == "Module2"));
        }

        [Fact]
        public void Given_FileNameFilterPattern_When_Valid_Then_ModuleReturned()
        {
            var modules = new NugetyCatalog()
                .Options.SetModuleFileNameFilter("*Module1.dll")
                .FromDirectory()
                .GetModules<IModuleInitializer>();

            Assert.True(modules.Any(m => m.Name == "Module1"));
        }

        [Fact]
        public void Given_GetManyModules_When_Valid_Then_ModulesReturned()
        {
            var modules = new NugetyCatalog().GetMany
            (
                c => c.FromDirectory().GetModules<IModuleInitializer>("Module1"),
                c => c.FromDirectory().GetModules<IModuleInitializer>("Module3 with dependency3 v0")
            );

            Assert.True(modules.Any(m => m.Name == "Module1"));
            Assert.True(modules.Any(m => m.Name == "Module3 with dependency3 v0"));
        }

        [Fact]
        public void Given_Module_When_Valid_Then_PropertiesPopulated()
        {
            var modules = new NugetyCatalog()
                .FromDirectory()
                .GetModules<IModuleInitializer>("Module1");

            var module = modules.FirstOrDefault(m => m.Name == "Module1");
            Assert.True(module != null);
            Assert.NotNull(module.Catalog);
            Assert.NotNull(module.ModuleInitialiser);
            Assert.True(!string.IsNullOrEmpty(module.Location));
            Assert.NotNull(module.AssemblyInfo);
            Assert.NotNull(module.AssemblyInfo.Assembly);
            Assert.True(!string.IsNullOrEmpty(module.AssemblyInfo.Location));
        }

        [Fact]
        public void Given_ModuleLocation_When_Invalid_Then_ThrowsDirectoryNotFoundException()
        {
            Assert.Throws<DirectoryNotFoundException>(() =>
            {
                var modules = new NugetyCatalog()
                    .FromDirectory("InvalidDirectory")
                    .GetModules<IModuleInitializer>();
            });
        }

        [Fact]
        public void Given_ModuleLocation_When_Valid_Then_ModuleReturned()
        {
            var modules = new NugetyCatalog()
                .FromDirectory()
                .GetModules<IModuleInitializer>();

            Assert.True(modules.Any(m => m.Name == "Module1"));
            Assert.True(modules.Any(m => m.Name == "Module3 with dependency3 v0"));
            Assert.True(modules.Any(m => m.Name == "Module2 without dependency2"));
        }

        [Fact]
        public void Given_ModuleName_When_InValid_Then_ThrowsDirectoryNotFoundException()
        {
            var exception = Assert.Throws<DirectoryNotFoundException>(() =>
            {
                var modules = new NugetyCatalog()
                    .FromDirectory()
                    .GetModules<IModuleInitializer>("InvalidModule1", "InvalidModule2");
            });
            Assert.Equal("Module Directory not found for 'InvalidModule1,InvalidModule2'", exception.Message);
        }

        [Fact]
        public void Given_ModuleName_When_InValidAndValid_Then_ThrowsDirectoryNotFoundException()
        {
            var exception = Assert.Throws<DirectoryNotFoundException>(() =>
            {
                var modules = new NugetyCatalog()
                    .FromDirectory()
                    .GetModules<IModuleInitializer>("Module1", "InvalidModule");
            });
            Assert.Equal("Module Directory not found for 'InvalidModule'", exception.Message);
        }

        [Fact]
        public void Given_ModuleName_When_Valid_Then_ModuleReturned()
        {
            var modules = new NugetyCatalog()
                .FromDirectory()
                .GetModules<IModuleInitializer>("Module1");

            Assert.True(modules.Any(m => m.Name == "Module1"));
        }

        [Fact]
        public void Given_ModuleNameFilterPattern_When_Invalid_Then_NoModuleReturned()
        {
            var modules = new NugetyCatalog()
                .Options.SetModuleNameFilter("Module2")
                .FromDirectory()
                .GetModules<IModuleInitializer>();

            Assert.True(!modules.Any(m => m.Name == "Module2"));
        }

        [Fact]
        public void Given_ModuleNameFilterPattern_When_Valid_Then_ModuleReturned()
        {
            var modules = new NugetyCatalog()
                .Options.SetModuleNameFilter("Module1")
                .FromDirectory()
                .GetModules<IModuleInitializer>();

            Assert.True(modules.Any(m => m.Name == "Module1"));
        }

        [Fact]
        public void Given_ModuleAssemblyLoad_When_FailToResolve_Then_ModuleDiscovered()
        {
            var catalog = new NugetyCatalog();
            var modules = catalog
                .Options.SetModuleFileNameFilter("*Module3.dll")
                .FromDirectory()
                .GetModules<IModuleInitializer>();

            var assembly = Assembly.Load("Nugety.Tests.Module3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=0b0afd28caef48a5");

            Assert.True(!modules.Any(m => m.Name == "Module3"));
        }
    }
}