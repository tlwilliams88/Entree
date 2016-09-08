using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Enumerations.Dependencies
{
    public enum DependencyInstanceType
    {
        None,
        InstancePerLifetimeScope,
        InstancePerRequest,
        InstancePerDependency,
        SingleInstance
    }
}