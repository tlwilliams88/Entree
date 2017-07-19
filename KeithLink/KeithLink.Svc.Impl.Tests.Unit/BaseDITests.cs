using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using FluentAssertions;
using Moq;
using Xunit;

using KeithLink.Svc.Impl.Repository.SmartResolver;

namespace KeithLink.Svc.Impl.Tests.Unit
{
    public class BaseDITests
    {
        public static ContainerBuilder GetTestsContainer()
        {
            return DependencyMapFactory.GetTestsContainer();
        }
    }
}
