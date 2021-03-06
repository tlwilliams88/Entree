﻿using Autofac;

using KeithLink.Svc.Impl.Repository.SmartResolver;

namespace KeithLink.Svc.Impl.Tests.Unit {
    public class BaseDITests {
        public static ContainerBuilder GetTestsContainer() {
            return DependencyMapFactory.GetTestsContainer();
        }
    }
}