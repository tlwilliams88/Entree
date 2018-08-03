using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

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
        #region BindSocketToPort
        public class BindSocketToPort
        {
            [Fact]
            public void WhenPortIsFree_HasNoException()
            {
                // arrange
                int WSAEADDRINUSE = 10048;

                MockDependents mockDependents = new MockDependents();

                ISocketListenerRepository testunit = MakeUnitToBeTested(true, mockDependents);

                // act
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Configuration.MainframeConfirmationListeningPort);
                Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Action bindSocketToPort = () => testunit.BindSocketToPort(listener, localEndPoint);

                // assert
                bindSocketToPort.Should().NotThrow();
            }

            [Fact]
            public void WhenPortIsInUse_HasNoException()
            {
                // arrange
                int WSAEADDRINUSE = 10048;

                MockDependents mockDependents = new MockDependents();

                ISocketListenerRepository testunit = MakeUnitToBeTested(true, mockDependents);

                var slowClosinglistener = StartListener(Configuration.MainframeConfirmationListeningPort);     // starts the port in use condition
                Task.Run(() => StopListenerAfterDelay(slowClosinglistener, 30000));     // keep the port in use condition

                // act
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Configuration.MainframeConfirmationListeningPort);
                Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Action bindSocketToPort = () => testunit.BindSocketToPort(listener, localEndPoint);

                // assert
                bindSocketToPort.Should().NotThrow();
            }

            private Socket StartListener(int portNumber)
            {
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
                Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                return listener;
            }

            private void StopListenerAfterDelay(Socket listener, int milliseconds)
            {
                Thread.Sleep(milliseconds);    // hold the port
                listener.Close();
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

            public MockDependents()
            {
                MockEventLogRepository = new Mock<IEventLogRepository>();
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
        }


        #endregion Setup
    }
}