using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Autofac;
using FluentAssertions;
using Moq;
using Xunit;

using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Customers;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.OnlinePayments.Customer;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Interface.OnlinePayments.Log;
using KeithLink.Svc.Core.Interface.OnlinePayments.Payment;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic.Orders;


namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Orders
{
    public class OrderLogicTests : BaseDITests
    {
        #region Setup
        public class MockDependents
        {
            public Mock<ICacheRepository> ICacheRepository { get; set; }
            public Mock<IEventLogRepository> IEventLogRepository { get; set; }
            public Mock<ICatalogLogic> ICatalogLogic { get; set; }
            public Mock<INotesListLogic> INoteLogic { get; set; }
            public Mock<IPriceLogic> IPriceLogic { get; set; }
            public Mock<ICustomerRepository> ICustomerRepository { get; set; }
            public Mock<IKPayInvoiceRepository> IKPayInvoiceRepository { get; set; }
            public Mock<IPurchaseOrderRepository> IPurchaseOrderRepository { get; set; }
            public Mock<IOrderHistoryHeaderRepsitory> IOrderHistoryHeaderRepsitory { get; set; }
            public Mock<IShipDateRepository> IShipDateRepository { get; set; }
            public Mock<IOrderedFromListRepository> IOrderedFromListRepository { get; set; }
            public Mock<IOrderQueueLogic> IOrderQueueLogic { get; set; }

            public static void RegisterInContainer(ref ContainerBuilder cb)
            {
                cb.RegisterInstance(MakeICacheRepository().Object)
                  .As<ICacheRepository>();
                cb.RegisterInstance(MakeIEventLogRepository().Object)
                  .As<IEventLogRepository>();
                cb.RegisterInstance(MakeICatalogLogic().Object)
                  .As<ICatalogLogic>();
                cb.RegisterInstance(MakeINoteLogic().Object)
                  .As<INotesListLogic>();
                cb.RegisterInstance(MakeIPriceLogic().Object)
                  .As<IPriceLogic>();
                cb.RegisterInstance(MakeICustomerRepository().Object)
                  .As<ICustomerRepository>();
                cb.RegisterInstance(MakeIKPayInvoiceRepository().Object)
                  .As<IKPayInvoiceRepository>();
                cb.RegisterInstance(MakeIPurchaseOrderRepository().Object)
                  .As<IPurchaseOrderRepository>();
                cb.RegisterInstance(MakeIOrderHistoryHeaderRepsitory().Object)
                  .As<IOrderHistoryHeaderRepsitory>();
                cb.RegisterInstance(MakeIShipDateRepository().Object)
                  .As<IShipDateRepository>();
                cb.RegisterInstance(MakeIOrderQueueLogic().Object)
                  .As<IOrderQueueLogic>();
                cb.RegisterInstance(MakeIOrderedFromListRepository().Object)
                  .As<IOrderedFromListRepository>();
            }

            public static Mock<IOrderedFromListRepository> MakeIOrderedFromListRepository()
            {
                var mock = new Mock<IOrderedFromListRepository>();

                return mock;
            }

            public static Mock<IOrderQueueLogic> MakeIOrderQueueLogic()
            {
                var mock = new Mock<IOrderQueueLogic>();

                return mock;
            }

            public static Mock<IShipDateRepository> MakeIShipDateRepository()
            {
                var mock = new Mock<IShipDateRepository>();

                mock.Setup(m => m.GetShipDates(It.IsAny<UserSelectedContext>()))
                    .Returns(new ShipDateReturn() {
                                                      ShipDates = new List<ShipDate>() {
                                                                                           new ShipDate() {
                                                                                                              CutOffDateTime = "1/1/1970 23:59",
                                                                                                              Date = "1/1/1970"
                                                                                                          }
                                                                                       }
                                                  });

                return mock;
            }

            public static Mock<IPurchaseOrderRepository> MakeIPurchaseOrderRepository()
            {
                var mock = new Mock<IPurchaseOrderRepository>();

                return mock;
            }

            public static Mock<IPriceLogic> MakeIPriceLogic()
            {
                var mock = new Mock<IPriceLogic>();

                return mock;
            }

            public static Mock<INotesListLogic> MakeINoteLogic()
            {
                var mock = new Mock<INotesListLogic>();

                return mock;
            }

            public static Mock<ICatalogLogic> MakeICatalogLogic()
            {
                var mock = new Mock<ICatalogLogic>();

                return mock;
            }

            public static Mock<IGenericQueueRepository> MakeIGenericQueueRepository() {
                var mock = new Mock<IGenericQueueRepository>();

                return mock;
            }

            public static Mock<IKPayPaymentTransactionRepository> MakeIKPayPaymentTransactionRepository() {
                var mock = new Mock<IKPayPaymentTransactionRepository>();

                return mock;
            }

            public static Mock<IOrderHistoryHeaderRepsitory> MakeIOrderHistoryHeaderRepsitory() {
                var mock = new Mock<IOrderHistoryHeaderRepsitory>();

                mock.Setup(m => m.Read(It.IsAny<Expression<Func<OrderHistoryHeader, bool>>>(),
                                       d => d.OrderDetails))
                    .Returns(new List<OrderHistoryHeader>() {
                                                                new OrderHistoryHeader() {
                                                                                             ControlNumber = "11111111",
                                                                                             OrderStatus = "P",
                                                                                             DeliveryDate = "1/1/1970",
                                                                                             InvoiceNumber = "11111111",
                                                                                             OrderDetails =
                                                                                                     new List<OrderHistoryDetail>() {
                                                                                                                                        new OrderHistoryDetail() {
                                                                                                                                                                     ItemNumber = "111111",
                                                                                                                                                                     TotalShippedWeight = 0,
                                                                                                                                                                     SellPrice = 0,
                                                                                                                                                                     OrderQuantity = 1,
                                                                                                                                                                     ShippedQuantity = 1,
                                                                                                                                                                     LineNumber = 1,
                                                                                                                                                                     CatchWeight = false,
                                                                                                                                                                     ItemDeleted = false,
                                                                                                                                                                     UnitOfMeasure = "C"
                                                                                                                                                                 }
                                                                                                                                    },
                                                                                             CustomerNumber = "123456",
                                                                                             OrderSystem = "B"
                                                                                         }
                                                            }.AsQueryable);

                return mock;
            }

            public static Mock<IKPayLogRepository> MakeIKPayLogRepository() {
                var mock = new Mock<IKPayLogRepository>();

                return mock;
            }

            public static Mock<IKPayInvoiceRepository> MakeIKPayInvoiceRepository() {
                var mock = new Mock<IKPayInvoiceRepository>();

                return mock;
            }

            public static Mock<ICustomerRepository> MakeICustomerRepository() {
                var mock = new Mock<ICustomerRepository>();

                return mock;
            }

            public static Mock<ICustomerBankRepository> MakeICustomerBankRepository() {
                var mock = new Mock<ICustomerBankRepository>();

                return mock;
            }

            public static Mock<IInternalUserAccessRepository> MakeIInternalUserAccessRepository() {
                var mock = new Mock<IInternalUserAccessRepository>();

                return mock;
            }

            public static Mock<IAuditLogRepository> MakeIAuditLogRepository() {
                var mock = new Mock<IAuditLogRepository>();

                return mock;
            }

            public static Mock<IEventLogRepository> MakeIEventLogRepository() {
                var mock = new Mock<IEventLogRepository>();

                return mock;
            }

            public static Mock<ICacheRepository> MakeICacheRepository()
            {
                var mock = new Mock<ICacheRepository>();

                return mock;
            }

        }

        private static IOrderLogic MakeTestsLogic(bool useAutoFac, ref MockDependents mockDependents)
        {
            if (useAutoFac)
            {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                MockDependents.RegisterInContainer(ref cb);

                var testcontainer = cb.Build();

                return testcontainer.Resolve<IOrderLogic>();
            }
            else
            {
                mockDependents = new MockDependents();
                mockDependents.ICacheRepository = MockDependents.MakeICacheRepository();
                mockDependents.ICatalogLogic = MockDependents.MakeICatalogLogic();
                mockDependents.INoteLogic = MockDependents.MakeINoteLogic();
                mockDependents.ICustomerRepository = MockDependents.MakeICustomerRepository();
                mockDependents.IEventLogRepository = MockDependents.MakeIEventLogRepository();
                mockDependents.IOrderQueueLogic = MockDependents.MakeIOrderQueueLogic();
                mockDependents.IOrderedFromListRepository = MockDependents.MakeIOrderedFromListRepository();
                mockDependents.IKPayInvoiceRepository = MockDependents.MakeIKPayInvoiceRepository();
                mockDependents.IPriceLogic = MockDependents.MakeIPriceLogic();
                mockDependents.IPurchaseOrderRepository = MockDependents.MakeIPurchaseOrderRepository();
                mockDependents.IOrderHistoryHeaderRepsitory = MockDependents.MakeIOrderHistoryHeaderRepsitory();
                mockDependents.IShipDateRepository = MockDependents.MakeIShipDateRepository();

                var testunit = new OrderLogicImpl(mockDependents.IPurchaseOrderRepository.Object, mockDependents.ICatalogLogic.Object, mockDependents.INoteLogic.Object,
                                                  mockDependents.ICacheRepository.Object, mockDependents.IOrderQueueLogic.Object, mockDependents.IPriceLogic.Object,
                                                  mockDependents.IEventLogRepository.Object, mockDependents.IShipDateRepository.Object, mockDependents.ICustomerRepository.Object,
                                                  mockDependents.IOrderHistoryHeaderRepsitory.Object, mockDependents.IKPayInvoiceRepository.Object, mockDependents.IOrderedFromListRepository.Object);
                return testunit;
            }
        }
        #endregion

        #region attributes
        #endregion

        #region GetOrder
        public class GetOrder
        {
            [Fact]
            public void WhenGoodBranchAndGoodInvoiceYieldGoodOrderWithItemInCatalog_ResultingDetailIsExpected()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);

                mockDependents.ICatalogLogic.Setup(m => m.GetProductsByIds("FUT", new List<string>() { "111111" }))
                    .Returns(new ProductsReturn()
                    {
                        Products = new List<Product>() {
                                                                                         new Product() {
                                                                                                           ItemNumber = "111111",
                                                                                                           Name = "Fake Name",
                                                                                                           BrandExtendedDescription = "Fake Brand",
                                                                                                           ItemClass = "Fake Class",
                                                                                                           Size = "Fake Size",
                                                                                                           Pack = "2"
                                                                                                       }
                                                                                     }
                    });

                var testBranch = "FUT";
                var testInvoice = "1111111";
                var expected = "Fake Name / 111111 / Fake Brand / Fake Class / 2 / Fake Size";

                // act
                var result = testunit.GetOrder(testBranch, testInvoice);

                // assert
                result.Items
                      .First()
                      .Detail
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void WhenGoodBranchAndGoodInvoiceYieldGoodOrderWithItemNotInCatalog_ResultingDetailIsNull()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);

                mockDependents.ICatalogLogic.Setup(m => m.GetProductsByIds("FUT", new List<string>() { "111111" }))
                    .Returns(new ProductsReturn()
                    {
                        Products = new List<Product>() {}
                    });

                var testBranch = "FUT";
                var testInvoice = "1111111";

                // act
                var result = testunit.GetOrder(testBranch, testInvoice);

                // assert
                result.Items
                      .First()
                      .Detail
                      .Should()
                      .BeNull();
            }

        }
        #endregion
    }
}
