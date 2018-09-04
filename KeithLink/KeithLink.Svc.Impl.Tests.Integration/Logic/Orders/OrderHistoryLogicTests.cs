using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Reflection;

using Autofac;

using FluentAssertions;

using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Orders.History;
using EF = KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Impl.Repository.SmartResolver;
using KeithLink.Svc.Impl.Tests.Integration.Repository;

using Newtonsoft.Json;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Integration.Logic.Orders
{
    public class OrderHistoryLogicTests
    {
        #region ParseMainframeFile
        public class ParseMainframeFile : MigratedDatabaseTest
        {
            [Fact]
            public void OrderHistory_HasValidHeadersAndOrderDate()
            {
                // arrange
                IOrderHistoryLogic testunit = MakeUnitToBeTested();

                // act
                var mockDataReader = GetMockDataReader("OT780FDF-2018206-10273884.TXT");
                OrderHistoryFileReturn orderHistoryFiles = testunit.ParseMainframeFile(mockDataReader);
                mockDataReader.Close();

                // assert
                orderHistoryFiles.Files.Count.Should().Be(40);
                orderHistoryFiles.Files.ForEach(file => CheckFile(file));
            }

        }
        #endregion


        #region ReadOrderFromQueue
        public class ReadOrderFromQueue : MigratedDatabaseTest
        {
            [Fact]
            public void OrderHistory_HasValidHeadersAndOrderDate()
            {
                // arrange
                IOrderHistoryLogic testunit = MakeUnitToBeTested();

                // act
                string jsonOrderHistoryFile = testunit.ReadOrderFromQueue();

                // assert
                OrderHistoryFile orderHistoryFile = JsonConvert.DeserializeObject<OrderHistoryFile>(jsonOrderHistoryFile);
                CheckFile(orderHistoryFile);
            }
        }
        #endregion


        #region ProcessOrder
        public class ProcessOrder : MigratedDatabaseTest
        {
            [Fact]
            public void OrderHistory_ExecutesInReasonableTimeSpan()
            {
                // arrange
                IOrderHistoryLogic testunit = MakeUnitToBeTested();

                // act
                string jsonOrderHistoryFile = GetMockData("OrderHistoryFile.json");
                Action processOrder = () => testunit.ProcessOrder(jsonOrderHistoryFile);

                // assert
                processOrder.ExecutionTime().Should().BeLessOrEqualTo(TimeSpan.FromSeconds(10));
            }

            [Fact]
            public void OrderHistoryWithSubbedAndReplacedItems_ExecutesInReasonableTimeSpan()
            {
                // arrange
                IOrderHistoryLogic testunit = MakeUnitToBeTested();

                // act
                string jsonOrderHistoryFile = GetMockData("OrderHistoryFileWithReplacementItems.json");
                Action processOrder = () => testunit.ProcessOrder(jsonOrderHistoryFile);

                // assert
                processOrder.ExecutionTime().Should().BeLessOrEqualTo(TimeSpan.FromSeconds(10));
            }
        }
        #endregion

        private static void CheckFile(OrderHistoryFile file)
        {
            file.ValidHeader.Should().BeTrue();

            CheckHeader(file.Header);
        }

        private static void CheckHeader(OrderHistoryHeader header)
        {
            if (string.IsNullOrWhiteSpace(header.OrderDateTime) == false)
            {
                DateTime orderDate = DateTime.Parse(header.OrderDateTime);
                DateTime deliveryDate = DateTime.Parse(header.DeliveryDate).AddDays(1).AddSeconds(-1);   // end of day
                orderDate.Should().BeOnOrBefore(deliveryDate);
            }

            header.ErrorStatus.Should().BeTrue("because I said so");
        }


        #region Setup

        private static string GetMockData(string mockDataName)
        {
            StreamReader reader = GetMockDataReader(mockDataName);
            string  mockData = reader.ReadToEnd();
            reader.Close();

            return mockData;
        }

        private static StreamReader GetMockDataReader(string mockDataName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();
            var resourceName = typeof(OrderHistoryLogicTests).Namespace + "." + mockDataName;

            Stream stream = assembly.GetManifestResourceStream(resourceName);

            StreamReader reader = null;
            if (stream != null)
                reader = new StreamReader(stream);

            return reader;
        }

        private static IOrderHistoryLogic MakeUnitToBeTested()
        {
            ContainerBuilder builder = DependencyMapFactory.GetOrderServiceContainer();
            IContainer container = builder.Build();

            IOrderHistoryLogic testunit = container.Resolve<IOrderHistoryLogic>();

            return testunit;
        }

        #endregion Setup
    }
}