using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Nugety;
using Autofac;
using Nugety.Tests.Common;

namespace Nugety.Tests.Module3
{
    public class ModuleInitializer : Autofac.Module, IModuleInitializer, IDependencyVersion
    {
        public ModuleInitializer()
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
        }

        public bool ConfigureServices(IServiceCollection services, IMvcBuilder builder, IServiceProvider provider = null)
        {
            return true;
        }

        public bool Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            return false;
        }

        public Type GetDependencyType()
        {
            var test = new Dependency3.Class1();
            return test.GetType();
        }
    }
}
