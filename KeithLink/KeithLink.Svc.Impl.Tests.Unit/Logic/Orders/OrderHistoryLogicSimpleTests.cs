using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using FluentAssertions;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History;

using Newtonsoft.Json;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Orders
{
    public class OrderHistoryLogicSimpleTests : BaseDITests
    {
        #region DeserializationTests
        public class DeserializationTests
        {
            [Fact]
            public void Order_HasValidOrderDate()
            {
                // arrange
                string jsonOrderFile = GetMockData("OrderFile.json");

                //expect
                string expectedCustomerNumber = "700766";
                DateTime expectedOrderdate = new DateTime(2018, 07, 19, 16, 34, 51);

                // act
                OrderFile orderFile = JsonConvert.DeserializeObject<OrderFile>(jsonOrderFile);

                // assert
                orderFile.Header.CustomerNumber.Should().Be(expectedCustomerNumber);
                orderFile.Header.OrderCreateDateTime.Should().Be(expectedOrderdate);
            }

            [Fact]
            public void OrderHistory_HasValidOrderDate()
            {
                // arrange
                string jsonOrderHistoryFile = GetMockData("OrderHistoryFile.json");

                //expect
                string expectedCustomerNumber = "734161";
                string expectedOrderdate = new DateTime(2018, 07, 19, 16, 34, 51).ToLongDateFormatWithTime();

                // act
                OrderHistoryFile orderHistoryFile = JsonConvert.DeserializeObject<OrderHistoryFile>(jsonOrderHistoryFile);

                // assert
                orderHistoryFile.Header.CustomerNumber.Should().Be(expectedCustomerNumber);
                orderHistoryFile.Header.OrderDateTime.Should().Be(expectedOrderdate);
            }
        }
        #endregion

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
            var resourceName = typeof(OrderHistoryLogicSimpleTests).Namespace + "." + mockDataName;

            Stream stream = assembly.GetManifestResourceStream(resourceName);

            StreamReader reader = null;
            if (stream != null)
                reader = new StreamReader(stream);

            return reader;
        }

    }
}