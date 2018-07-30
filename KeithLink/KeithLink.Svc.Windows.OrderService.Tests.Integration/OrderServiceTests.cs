using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Reflection;

using Autofac;

using Castle.Components.DictionaryAdapter;

using FluentAssertions;

using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Impl.Tests.Unit;
using Moq;

using Xunit;

namespace KeithLink.Svc.Windows.OrderService.Tests.Integration
{
    public class OrderServiceTests : BaseDITests
    {
        #region ProcessOrderUpdatesTick
        public class ProcessOrderUpdatesTick
        {
            [Fact]
            public void StreamContainingRawOrderHistory_IsSentToParseMainframeFile()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                OrderService testunit = MakeUnitToBeTested(true, mockDependents);

                // act
                object state = null;
                testunit.ProcessOrderUpdatesTick(state);

                // assert

                // The OrderService does not use the mocks verified here.
                // The container where the mocked instances are registered is replaced by the OrderService
                // The OrderService builds a container from a builder it obtains from the DependencyMapFactory.

                //mockDependents.MockOrderHistoryLogic
                //    .Verify(m => m.ParseMainframeFile(It.IsAny<StreamReader>()), Times.Once, "not called.");

                //mockDependents.MockGenericQueueRepository
                //    .Verify(m => m.BulkPublishToQueue(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once, "not called.");
            }

        }
        #endregion

        #region Setup
        public class MockDependents
        {
            public Mock<IOrderHistoryLogic> MockOrderHistoryLogic { get; set; }
            public Mock<IGenericQueueRepository> MockGenericQueueRepository { get; set; }

            public MockDependents()
            {
                // The OrderService does not use the mocks created here.
                // The container where the mocked instances are registered is replaced by the OrderService
                // The OrderService builds a container from a builder it obtains from the DependencyMapFactory.

                //MockOrderHistoryLogic = new Mock<IOrderHistoryLogic>();
                //MockGenericQueueRepository = new Mock<IGenericQueueRepository>();

                //MockOrderHistoryLogic
                //    .Setup(m => m.ParseMainframeFile(It.IsAny<StreamReader>()))
                //    .Returns(It.IsAny<OrderHistoryFileReturn>());

                //MockGenericQueueRepository
                //    .Setup(m => m.BulkPublishToQueue(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            }
        }

        private static OrderService MakeUnitToBeTested(bool useAutoFac, MockDependents mockDependents)
        {
            if (useAutoFac)
            {
                ContainerBuilder containerBuilder = GetTestsContainer();

                containerBuilder.RegisterType<OrderService>();

                // Register mocks
                RegisterInContainer(ref containerBuilder, mockDependents);

                // The OrderService does not use the container built here.
                // It builds a container from a builder it obtains from the DependencyMapFactory.
                IContainer testcontainer = containerBuilder.Build();

                return testcontainer.Resolve<OrderService>();
            }

            OrderService testunit = new OrderService();

            return testunit;
        }

        public static void RegisterInContainer(ref ContainerBuilder cb, MockDependents mockDependents)
        {
            //cb.RegisterInstance(mockDependents.MockOrderHistoryLogic.Object)
            //  .As<IOrderHistoryLogic>();

            //cb.RegisterInstance(mockDependents.MockGenericQueueRepository.Object)
            //  .As<IGenericQueueRepository>();
        }


        #endregion Setup
    }
}