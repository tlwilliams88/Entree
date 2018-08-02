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
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Impl.Logic.Orders;

using Moq;

using Newtonsoft.Json;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Orders
{
    public class OrderQueueLogicTests : BaseDITests
    {
        #region ParseMainframeFile
        public class SendToHost
        {
            [Fact]
            public void NoExceptions()
            {
                // arrange
                const string transactionReady = Constants.MAINFRAME_RECEIVE_STATUS_GO;
                const string received = Constants.MAINFRAME_RECEIVE_STATUS_GOOD_RETURN;
                const string wait = Constants.MAINFRAME_RECEIVE_STATUS_WAITING;
                const string cancelled = Constants.MAINFRAME_RECEIVE_STATUS_CANCELLED;

                List<string> successfulResponseSequence = new List<string>
                {
                    transactionReady, received, wait, received, wait, received
                };
                Queue<string> successfulResponseQueue = new Queue<string>(successfulResponseSequence);

                MockDependents mockDependents = new MockDependents();
                mockDependents.MockOrderSocketConnectionRepository
                    .Setup(m => m.Receive())
                    .Returns(() => successfulResponseQueue.Dequeue());

                IOrderQueueLogic testunit = MakeUnitToBeTested(true, mockDependents);

                // act
                string jsonOrderFile = GetMockData("OrderFile.json");
                OrderFile order = JsonConvert.DeserializeObject<OrderFile>(jsonOrderFile);
                Action sendToHost = () => testunit.SendToHost(order);

                // assert
                sendToHost.Should().NotThrow();
            }

            [Fact]
            public void HandlesTimeout()
            {
                // arrange
                int WSAETIMEDOUT = 10060;

                MockDependents mockDependents = new MockDependents();
                mockDependents.MockOrderSocketConnectionRepository
                    .Setup(m => m.Receive())
                    .Throws(BuildMockException(WSAETIMEDOUT));

                IOrderQueueLogic testunit = MakeUnitToBeTested(true, mockDependents);

                // act
                string jsonOrderFile = GetMockData("OrderFile.json");
                OrderFile order = JsonConvert.DeserializeObject<OrderFile>(jsonOrderFile);
                Action sendToHost = () => testunit.SendToHost(order);

                // assert
                sendToHost.Should()
                    .Throw<TimeoutException>()
                    .WithInnerException<SocketResponseException>()
                    .WithInnerException<IOException>()
                    .WithMessage("*host has failed to respond*");
            }

            [Fact]
            public void HandlesConnectionReset()
            {
                // arrange
                int WSAECONNRESET = 10054;

                MockDependents mockDependents = new MockDependents();
                mockDependents.MockOrderSocketConnectionRepository
                    .Setup(m => m.Receive())
                    .Throws(BuildMockException(WSAECONNRESET));

                IOrderQueueLogic testunit = MakeUnitToBeTested(true, mockDependents);

                // act
                string jsonOrderFile = GetMockData("OrderFile.json");
                OrderFile order = JsonConvert.DeserializeObject<OrderFile>(jsonOrderFile);
                Action sendToHost = () => testunit.SendToHost(order);

                // assert
                sendToHost.Should()
                    .Throw<HostTransactionFailureException>()
                    .WithInnerException<SocketResponseException>()
                    .WithInnerException<IOException>()
                    .WithMessage("*connection was forcibly closed by the remote host*");
            }

            private Exception BuildMockException(int errorCode)
            {
                SocketException socketException = new SocketException(errorCode);
                IOException ioException = new IOException("Unable to read data from the transport connection: " + socketException.Message + ".", socketException);
                SocketResponseException socketResponseException = new SocketResponseException("Error reading from host socket connection", ioException);

                return socketResponseException;
            }
        }
        #endregion


        #region Setup
        public class MockDependents
        {
            public Mock<IOrderSocketConnectionRepository> MockOrderSocketConnectionRepository { get; set; }
            public Mock<IEventLogRepository> MockEventLogRepository { get; set; }
            public Mock<IGenericQueueRepository> MockGenericQueueRepository { get; set; }
            public Mock<ISpecialOrderRepository> MockSpecialOrderRepository { get; set; }

            public MockDependents()
            {
                MockOrderSocketConnectionRepository = new Mock<IOrderSocketConnectionRepository>();
                MockEventLogRepository = new Mock<IEventLogRepository>();
                MockGenericQueueRepository = new Mock<IGenericQueueRepository>();
                MockSpecialOrderRepository = new Mock<ISpecialOrderRepository>();

            }
        }

        private static string GetMockData(string mockDataName)
        {
            StreamReader reader = GetMockDataReader(mockDataName);
            string mockData = reader.ReadToEnd();
            reader.Close();

            return mockData;
        }

        private static StreamReader GetMockDataReader(string mockDataName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();
            var resourceName = typeof(OrderQueueLogicTests).Namespace + "." + mockDataName;

            Stream stream = assembly.GetManifestResourceStream(resourceName);

            StreamReader reader = null;
            if (stream != null)
                reader = new StreamReader(stream);

            return reader;
        }

        private static IOrderQueueLogic MakeUnitToBeTested(bool useAutoFac, MockDependents mockDependents)
        {
            if (useAutoFac)
            {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                RegisterInContainer(ref cb, mockDependents);

                IContainer testcontainer = cb.Build();

                return testcontainer.Resolve<IOrderQueueLogic>();
            }

            OrderQueueLogicImpl testunit = new OrderQueueLogicImpl(
                mockDependents.MockEventLogRepository.Object,
                mockDependents.MockGenericQueueRepository.Object,
                mockDependents.MockOrderSocketConnectionRepository.Object,
                mockDependents.MockSpecialOrderRepository.Object);

            return testunit;
        }

        public static void RegisterInContainer(ref ContainerBuilder cb, MockDependents mockDependents)
        {
            cb.RegisterInstance(mockDependents.MockOrderSocketConnectionRepository.Object)
              .As<IOrderSocketConnectionRepository>();

            cb.RegisterInstance(mockDependents.MockEventLogRepository.Object)
              .As<IEventLogRepository>();

            cb.RegisterInstance(mockDependents.MockGenericQueueRepository.Object)
              .As<IGenericQueueRepository>();

            cb.RegisterInstance(mockDependents.MockSpecialOrderRepository.Object)
              .As<ISpecialOrderRepository>();

        }


        #endregion Setup
    }
}