using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Reflection;

using Autofac;

using Castle.Components.DictionaryAdapter;

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

using Moq;

using Newtonsoft.Json;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Orders
{
    public class OrderHistoryLogicTests : BaseDITests
    {
        #region ParseMainframeFile
        public class ParseMainframeFile
        {
            [Fact]
            public void WhenMainframeFileContainsOrderHistory_HasValidHeadersAndOrderDate()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IOrderHistoryLogic testunit = MakeUnitToBeTested(true, mockDependents);

                // act
                var mockDataReader = GetMockDataReader("OT780FDF-2018206-10273884.TXT");
                OrderHistoryFileReturn orderHistoryFiles = testunit.ParseMainframeFile(mockDataReader);
                mockDataReader.Close();

                // assert
                orderHistoryFiles.Files.Count.Should().Be(40);
                orderHistoryFiles.Files.ForEach(file => CheckFile(file));
            }

            [Fact]
            public void WhenMainframeFileDoesNotContainOrderHistory_HasValidHeadersAndOrderDate()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IOrderHistoryLogic testunit = MakeUnitToBeTested(true, mockDependents);

                // act
                var mockDataReader = new StringReader(string.Empty);
                OrderHistoryFileReturn orderHistoryFiles = testunit.ParseMainframeFile(mockDataReader);
                mockDataReader.Close();

                // assert
                orderHistoryFiles.Files.Count.Should().Be(0);
            }

        }
        #endregion


        #region ReadOrderFromQueue
        public class ReadOrderFromQueue
        {
            [Fact]
            public void OrderHistory_HasValidHeadersAndOrderDate()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IOrderHistoryLogic testunit = MakeUnitToBeTested(false, mockDependents);

                // act
                string jsonOrderHistoryFile = testunit.ReadOrderFromQueue();

                // assert
                mockDependents.MockGenericQueueRepository
                   .Verify(m => m.ConsumeFromQueue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once, "not called.");

                OrderHistoryFile orderHistoryFile = JsonConvert.DeserializeObject<OrderHistoryFile>(jsonOrderHistoryFile);
                CheckFile(orderHistoryFile);
            }

        }
        #endregion


        #region ProcessOrder
        public class ProcessOrder
        {
            [Fact]
            public void OrderHistory_IsSentToRepositoryWithValidOrderDate()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IOrderHistoryLogic testunit = MakeUnitToBeTested(false, mockDependents);

                //expect
                string expectedOrderdate = new DateTime(2018, 07, 19, 16, 34, 51).ToLongDateFormatWithTime();

                // act
                string jsonOrderHistoryFile = GetMockData("OrderHistoryFile.json");
                testunit.ProcessOrder(jsonOrderHistoryFile);

                // assert
                mockDependents.MockOrderHistoryHeaderRepository
                    .Verify(m => m.CreateOrUpdate(It.Is<EF.OrderHistoryHeader>(header => header.OrderDateTime == expectedOrderdate)), Times.Once, "not called with expected order date.");

                //mockDependents.MockOrderHistoryHeaderRepository.VerifyAll();
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
            DateTime orderDate = DateTime.Parse(header.OrderDateTime);
            DateTime deliveryDate = DateTime.Parse(header.DeliveryDate).AddDays(1).AddSeconds(-1);   // end of day
            orderDate.Should().BeOnOrBefore(deliveryDate);

            header.ErrorStatus.Should().BeTrue("because I said so");
        }


        #region Setup
        public class MockDependents
        {
            public Mock<IOrderHistoryHeaderRepsitory> MockOrderHistoryHeaderRepository { get; set; }
            public Mock<IPurchaseOrderRepository> MockPurchaseOrderRepository { get; set; }
            public Mock<IKPayInvoiceRepository> MockKPayInvoiceRepository { get; set; }
            public Mock<ICatalogLogic> MockCatalogLogic { get; set; }
            public Mock<IOrderHistoryDetailRepository> MockOrderHistoryDetailRepository { get; set; }
            public Mock<IUnitOfWork> MockUnitOfWork { get; set; }
            public Mock<IEventLogRepository> MockEventLogRepository { get; set; }
            public Mock<IGenericQueueRepository> MockGenericQueueRepository { get; set; }
            public Mock<IOrderConversionLogic> MockOrderConversionLogic { get; set; }
            public Mock<ICustomerRepository> MockCustomerRepository { get; set; }
            public Mock<ISocketListenerRepository> MockSocketListenerRepository { get; set; }
            public Mock<IGenericSubscriptionQueueRepository> MockGenericSubscriptionQueueRepository { get; set; }

            public MockDependents()
            {
                MockOrderHistoryHeaderRepository = new Mock<IOrderHistoryHeaderRepsitory>();
                MockPurchaseOrderRepository = new Mock<IPurchaseOrderRepository>();
                MockKPayInvoiceRepository = new Mock<IKPayInvoiceRepository>();
                MockCatalogLogic = new Mock<ICatalogLogic>();
                MockOrderHistoryDetailRepository = new Mock<IOrderHistoryDetailRepository>();
                MockUnitOfWork = new Mock<IUnitOfWork>();
                MockEventLogRepository = new Mock<IEventLogRepository>();
                MockGenericQueueRepository = new Mock<IGenericQueueRepository>();
                MockOrderConversionLogic = new Mock<IOrderConversionLogic>();
                MockCustomerRepository = new Mock<ICustomerRepository>();
                MockSocketListenerRepository = new Mock<ISocketListenerRepository>();
                MockGenericSubscriptionQueueRepository = new Mock<IGenericSubscriptionQueueRepository>();

                MockGenericQueueRepository
                    .Setup(m => m.ConsumeFromQueue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(GetMockData("OrderHistoryFile.json"));

                MockOrderHistoryHeaderRepository
                    .Setup(m => m.Read(It.IsAny<Expression<Func<EF.OrderHistoryHeader, bool>>>(),
                                       d => d.OrderDetails))
                    .Returns(GetMockOrderHistoryHeaderList().AsQueryable);

                MockOrderHistoryHeaderRepository
                    .Setup(m => m.ReadByConfirmationNumber(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(GetMockOrderHistoryHeaderList());

                MockOrderHistoryHeaderRepository
                    .Setup(m => m.ReadForInvoice(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(GetMockOrderHistoryHeaderList());

                MockOrderHistoryHeaderRepository
                    .Setup(m => m.CreateOrUpdate(It.IsAny<EF.OrderHistoryHeader>()));


                MockCatalogLogic
                    .Setup(m => m.GetProductsByIds(It.IsAny<string>(), It.IsAny<List<string>>()))
                    .Returns(GetMockProducts);

                MockCustomerRepository
                    .Setup(x => x.GetCustomerByCustomerNumber(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(GetMockCustomer());
            }
        }

        private static List<EF.OrderHistoryHeader> GetMockOrderHistoryHeaderList()
        {
            var headerList = new List<EF.OrderHistoryHeader>
            {
                new EF.OrderHistoryHeader
                {
                    ControlNumber = "11111111",
                    OrderStatus = "P",
                    DeliveryDate = "1/1/1970",
                    InvoiceNumber = "11111111",
                    OrderDetails =
                            new List<EF.OrderHistoryDetail> {
                                new EF.OrderHistoryDetail {
                                    ItemNumber = "111111",
                                    TotalShippedWeight = 0,
                                    SellPrice = (decimal)200.00,
                                    OrderQuantity = 1,
                                    ShippedQuantity = 1,
                                    LineNumber = 1,
                                    CatchWeight = false,
                                    ItemDeleted = false,
                                    UnitOfMeasure = "C"
                                }
                            },
                    CustomerNumber  = "123456",
                    OrderSystem     = "B",
                    OrderSubtotal   = (decimal)200.00,
                    OrderDateTime   = new DateTime(2018, 07, 04, 18, 00, 00, DateTimeKind.Local).ToLongDateFormatWithTime(),
                    CreatedUtc      = new DateTime(2018, 07, 21, 05, 00, 00, DateTimeKind.Utc),
                    ModifiedUtc     = new DateTime(2018, 07, 22, 09, 33, 00, DateTimeKind.Utc),
                }
            };

            return headerList;
        }

        private static ProductsReturn GetMockProducts()
        {
            var products = new ProductsReturn
            {
                Products = new List<Product>
                {
                    new Product
                    {
                        ItemNumber = "111111",
                        Name = "Fake Name",
                        BrandExtendedDescription = "Fake Brand",
                        ItemClass = "Fake Class",
                        Size = "Fake Size",
                        Pack = "2"
                    }
                }
            };

            return products;
        }

        private static Customer GetMockCustomer()
        {
            var customer = new Customer()
            {
                CustomerNumber = "111111",
                CustomerName = "Test Customer",
                DisplayName = "Test Display Name",
                CustomerBranch = "FUT",
                NationalOrRegionalAccountNumber = "AAA",
                DsrNumber = "000",
                DsmNumber = "000",
                Dsr = new Dsr()
                {
                    Name = "Test DSR",
                    DsrNumber = "000",
                    Branch = "FUT",
                    ImageUrl = "Fake",
                    PhoneNumber = "888-888-8888",
                    EmailAddress = "fake@benekeith.com"
                },
                ContractId = null,
                IsPoRequired = false,
                IsPowerMenu = false,
                CustomerId = new Guid("00000000-0000-0000-0000-000000000000"),
                AccountId = new Guid("00000000-0000-0000-0000-000000000000"),
                Address = new Address()
                {
                    City = "Fake City",
                    PostalCode = "00000",
                    RegionCode = "",
                    StreetAddress = "1234 Test St."
                },
                Phone = "888-888-8888",
                Email = "customer@benekeith.com",
                PointOfContact = "Fake",
                TermCode = "Net 30",
                TermDescription = "Net 30",
                KPayCustomer = true,
                NationalId = "00",
                NationalNumber = "00",
                NationalSubNumber = "00",
                RegionalNumber = "00",
                IsKeithNetCustomer = true,
                NationalIdDesc = "",
                NationalNumberSubDesc = "",
                RegionalIdDesc = "",
                RegionalNumberDesc = "",
                CanMessage = true,
                CanViewPricing = true,
                CanViewUNFI = true,
                CustomerUsers = new List<UserProfile>()
                {

                }
            };

            return customer;
        }

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

        private static IOrderHistoryLogic MakeUnitToBeTested(bool useAutoFac, MockDependents mockDependents)
        {
            if (useAutoFac)
            {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                RegisterInContainer(ref cb, mockDependents);

                IContainer testcontainer = cb.Build();

                return testcontainer.Resolve<IOrderHistoryLogic>();
            }

            OrderHistoryLogicImpl testunit = new OrderHistoryLogicImpl(
                mockDependents.MockOrderHistoryHeaderRepository.Object,
                mockDependents.MockPurchaseOrderRepository.Object,
                mockDependents.MockKPayInvoiceRepository.Object,
                mockDependents.MockCatalogLogic.Object,
                mockDependents.MockOrderHistoryDetailRepository.Object,
                mockDependents.MockUnitOfWork.Object,
                mockDependents.MockEventLogRepository.Object,
                mockDependents.MockGenericQueueRepository.Object,
                mockDependents.MockOrderConversionLogic.Object, 
                mockDependents.MockCustomerRepository.Object,
                mockDependents.MockSocketListenerRepository.Object,
                mockDependents.MockGenericSubscriptionQueueRepository.Object);

            return testunit;
        }

        public static void RegisterInContainer(ref ContainerBuilder cb, MockDependents mockDependents)
        {
            cb.RegisterInstance(mockDependents.MockOrderHistoryHeaderRepository.Object)
              .As<IOrderHistoryHeaderRepsitory>();

            cb.RegisterInstance(mockDependents.MockPurchaseOrderRepository.Object)
              .As<IPurchaseOrderRepository>();

            cb.RegisterInstance(mockDependents.MockKPayInvoiceRepository.Object)
              .As<IKPayInvoiceRepository>();

            cb.RegisterInstance(mockDependents.MockCatalogLogic.Object)
              .As<ICatalogLogic>();

            cb.RegisterInstance(mockDependents.MockOrderHistoryDetailRepository.Object)
              .As<IOrderHistoryDetailRepository>();

            cb.RegisterInstance(mockDependents.MockUnitOfWork.Object)
              .As<IUnitOfWork>();

            cb.RegisterInstance(mockDependents.MockEventLogRepository.Object)
              .As<IEventLogRepository>();

            cb.RegisterInstance(mockDependents.MockGenericQueueRepository.Object)
              .As<IGenericQueueRepository>();

            cb.RegisterInstance(mockDependents.MockOrderConversionLogic.Object)
              .As<IOrderConversionLogic>();

            cb.RegisterInstance(mockDependents.MockCustomerRepository.Object)
              .As<ICustomerRepository>();

            cb.RegisterInstance(mockDependents.MockSocketListenerRepository.Object)
              .As<ISocketListenerRepository>();

            cb.RegisterInstance(mockDependents.MockGenericSubscriptionQueueRepository.Object)
              .As<IGenericSubscriptionQueueRepository>();

        }


        #endregion Setup
    }
}