using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Reflection;
using System.Net.Sockets;

using Autofac;

using Castle.Components.DictionaryAdapter;

using FluentAssertions;

using KeithLink.Svc.Core;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Exceptions.Orders;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.Confirmations;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Impl.Repository.EF.Operational;

using Moq;

using Newtonsoft.Json;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Orders
{
    public class ConfirmationLogicTests : BaseDITests
    {
        #region ListenForMainFrameCalls
        public class ListenForMainFrameCalls
        {
            [Fact]
            public void HandlesAddressInUse()
            {
                // arrange
                int WSAEADDRINUSE = 10048;

                MockDependents mockDependents = new MockDependents();
                mockDependents.MockSocketListenerRepository
                    .Setup(m => m.Listen(It.IsAny<int>()))
                    .Throws(BuildMockException(WSAEADDRINUSE));

                IConfirmationLogic testunit = MakeUnitToBeTested(true, mockDependents);

                // act
                Action listenForMainFrameCalls = () => testunit.ListenForMainFrameCalls();

                // assert
                listenForMainFrameCalls.Should().NotThrow();
            }

            private Exception BuildMockException(int errorCode)
            {
                SocketException socketException = new SocketException(errorCode);
                return socketException;
            }
        }
        #endregion


        #region Setup
        public class MockDependents
        {
            public Mock<IEventLogRepository> MockEventLogRepository { get; set; }
            public Mock<ISocketListenerRepository> MockSocketListenerRepository { get; set; }
            public Mock<IGenericQueueRepository> MockGenericQueueRepository { get; set; }
            public Mock<IOrderConversionLogic> MockOrderConversionLogic { get; set; }
            public Mock<IUnitOfWork> MockUnitOfWork { get; set; }
            public Mock<IGenericSubscriptionQueueRepository> MockGenericSubscriptionQueueRepository { get; set; }

            public MockDependents()
            {
                MockEventLogRepository = new Mock<IEventLogRepository>();
                MockSocketListenerRepository = new Mock<ISocketListenerRepository>();
                MockGenericQueueRepository = new Mock<IGenericQueueRepository>();
                MockOrderConversionLogic = new Mock<IOrderConversionLogic>();
                MockUnitOfWork = new Mock<IUnitOfWork>();
                MockGenericSubscriptionQueueRepository = new Mock<IGenericSubscriptionQueueRepository>();

            }
        }

        private static IConfirmationLogic MakeUnitToBeTested(bool useAutoFac, MockDependents mockDependents)
        {
            if (useAutoFac)
            {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                RegisterInContainer(ref cb, mockDependents);

                IContainer testcontainer = cb.Build();

                return testcontainer.Resolve<IConfirmationLogic>();
            }

            ConfirmationLogicImpl testunit = new ConfirmationLogicImpl(
                mockDependents.MockEventLogRepository.Object,
                mockDependents.MockSocketListenerRepository.Object,
                mockDependents.MockGenericQueueRepository.Object,
                mockDependents.MockOrderConversionLogic.Object,
                mockDependents.MockUnitOfWork.Object,
                mockDependents.MockGenericSubscriptionQueueRepository.Object);

            return testunit;
        }

        public static void RegisterInContainer(ref ContainerBuilder cb, MockDependents mockDependents)
        {
            cb.RegisterInstance(mockDependents.MockEventLogRepository.Object)
              .As<IEventLogRepository>();

            cb.RegisterInstance(mockDependents.MockSocketListenerRepository.Object)
              .As<ISocketListenerRepository>();

            cb.RegisterInstance(mockDependents.MockGenericQueueRepository.Object)
              .As<IGenericQueueRepository>();

            cb.RegisterInstance(mockDependents.MockOrderConversionLogic.Object)
              .As<IOrderConversionLogic>();

            cb.RegisterInstance(mockDependents.MockUnitOfWork.Object)
              .As<IUnitOfWork>();

            cb.RegisterInstance(mockDependents.MockGenericSubscriptionQueueRepository.Object)
              .As<IGenericSubscriptionQueueRepository>();

        }


        #endregion Setup
    }
}