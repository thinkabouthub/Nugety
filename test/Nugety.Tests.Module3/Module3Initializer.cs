using System;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nugety.Tests.Common;
using Nugety.Tests.Dependency3;

namespace Nugety.Tests.Module3
{
    [Name("Module3Initializer")]
    public class Module3Initializer : Module, IModuleInitializer, IDependencyVersion
    {
        public Type GetDependencyType()
        {
            var test = new Class1();
            return test.GetType();
        }

        public bool ConfigureServices(IServiceCollection services, IMvcBuilder builder, IServiceProvider provider = null)
        {
            return true;
        }

        public bool Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            return false;
        }

        protected override void Load(ContainerBuilder builder)
        {
        }
    }
}