using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nugety.Tests.Common
{
    public interface IDependencyVersion
    {
        Type GetDependencyType();
    }
}
