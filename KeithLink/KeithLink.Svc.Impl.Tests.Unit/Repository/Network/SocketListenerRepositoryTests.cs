using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Reflection;
using System.Net;
using System.Net.Sockets;

using Autofac;

using Castle.Components.DictionaryAdapter;

using FluentAssertions;

using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Impl.Repository.Network;

using Moq;

using Newtonsoft.Json;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Repository.Network
{
    public class SocketListenerRepositoryTests : BaseDITests
    {
        #region Listen
        public class Listen
        {
            [Fact]
            public void WhenPortIsInUse_HasNoException()
            {
                // arrange
                int WSAEADDRINUSE = 10048;

                MockDependents mockDependents = new MockDependents();
                mockDependents.MockSocket
                    .Setup(m => m.Bind(It.IsAny<IPEndPoint>()))
                    .Throws(BuildMockException(WSAEADDRINUSE));

                ISocketListenerRepository testunit = MakeUnitToBeTested(true, mockDependents);

                // act
                Action listen = () => testunit.Listen(Configuration.MainframeConfirmationListeningPort);
                listen.Invoke();

                // assert
                mockDependents.MockSocket
                    .Verify(m => m.Bind(It.IsAny<IPEndPoint>()), Times.Once, "not called.");
                listen.Should().NotThrow();
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
            public Mock<Socket> MockSocket { get; set; }

            public MockDependents()
            {
                MockEventLogRepository = new Mock<IEventLogRepository>();
                MockSocket = new Mock<Socket>();
            }
        }

        private static ISocketListenerRepository MakeUnitToBeTested(bool useAutoFac, MockDependents mockDependents)
        {
            if (useAutoFac)
            {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                RegisterInContainer(ref cb, mockDependents);

                IContainer testcontainer = cb.Build();

                return testcontainer.Resolve<ISocketListenerRepository>();
            }

            SocketListenerRepositoryImpl testunit = new SocketListenerRepositoryImpl(
                mockDependents.MockEventLogRepository.Object);

            return testunit;
        }

        public static void RegisterInContainer(ref ContainerBuilder cb, MockDependents mockDependents)
        {
            cb.RegisterInstance(mockDependents.MockEventLogRepository.Object)
              .As<IEventLogRepository>();

            cb.RegisterInstance(mockDependents.MockSocket.Object)
              .As<Socket>();
        }


        #endregion Setup
    }
}