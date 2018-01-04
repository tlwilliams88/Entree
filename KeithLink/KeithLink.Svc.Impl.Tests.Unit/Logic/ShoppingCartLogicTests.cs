using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Autofac;

using FluentAssertions;

using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic;

using Moq;

using Xunit;

using Product = KeithLink.Svc.Core.Models.SiteCatalog.Product;
using UserProfile = KeithLink.Svc.Core.Models.Profile.UserProfile;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic {
    public class ShoppingCartLogicTests : BaseDITests {
        #region CartReport
        public class CartReport {
            [Fact]
            public void AnyCall_CallsBasketLogicRetrieveSharedCustomerBasket() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                Guid testCart = new Guid("dddddddddddddddddddddddddddddddd");
                PagedListModel testPagedListModel = new PagedListModel {
                    Name = "Fake Name"
                };
                PrintListModel testPrintModel = new PrintListModel();

                // act
                MemoryStream results = testunit.CartReport(fakeUser, testContext, testCart, testPagedListModel, testPrintModel);

                // assert
                mockDependents.BasketLogic.Verify(m => m.RetrieveSharedCustomerBasket(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<Guid>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyCall_CallsUserActiveCartLogicGetUserActiveCart() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                Guid testCart = new Guid("dddddddddddddddddddddddddddddddd");
                PagedListModel testPagedListModel = new PagedListModel {
                    Name = "Fake Name"
                };
                PrintListModel testPrintModel = new PrintListModel();

                // act
                MemoryStream results = testunit.CartReport(fakeUser, testContext, testCart, testPagedListModel, testPrintModel);

                // assert
                mockDependents.UserActiveCartLogic.Verify(m => m.GetUserActiveCart(It.IsAny<UserSelectedContext>(), It.IsAny<Guid>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyCall_ProducesStream() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(true, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                Guid testCart = new Guid("dddddddddddddddddddddddddddddddd");
                PagedListModel testPagedListModel = new PagedListModel {
                    Name = "Fake Name"
                };
                PrintListModel testPrintModel = new PrintListModel();

                // act
                MemoryStream results = testunit.CartReport(fakeUser, testContext, testCart, testPagedListModel, testPrintModel);

                // assert
                results.Should()
                       .NotBeNull();
            }
        }
        #endregion CartReport

        #region LookupProductDetails
        public class LookupProductDetails {
            [Fact]
            public void CartWithGoodItem_DetailIsExpected() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(true, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                ShoppingCart testCart = new ShoppingCart {
                    Active = true,
                    BranchId = "FUT",
                    CartId = new Guid("dddddddddddddddddddddddddddddddd"),
                    Name = "Fake Cart Name",
                    Items = new List<ShoppingCartItem> {
                        new ShoppingCartItem {
                            ItemNumber = "123456",
                            CatalogId = "FUT"
                        }
                    }
                };
                string expected = "Fake Name / 123456 / Fake Brand / Fake Class / Fake Pack / Fake Size";

                // act
                testunit.LookupProductDetails(fakeUser, testContext, testCart);

                // assert
                testCart.Items.First()
                        .Detail.Should()
                        .Be(expected);
            }
        }
        #endregion LookupProductDetails

        #region Setup
        public class MockDependents {
            public Mock<ICacheRepository> CacheRepository { get; set; }

            public Mock<ICustomerRepository> CustomerRepository { get; set; }

            public Mock<IBasketRepository> BasketRepository { get; set; }

            public Mock<ICatalogLogic> CatalogLogic { get; set; }

            public Mock<IPriceLogic> PriceLogic { get; set; }

            public Mock<IPurchaseOrderRepository> PurchaseOrderRepository { get; set; }

            public Mock<IGenericQueueRepository> GenericQueueRepository { get; set; }

            public Mock<IBasketLogic> BasketLogic { get; set; }

            public Mock<INotesListLogic> NotesListLogic { get; set; }

            public Mock<IOrderQueueLogic> OrderQueueLogic { get; set; }

            public Mock<IOrderHistoryLogic> OrderHistoryLogic { get; set; }

            public Mock<IAuditLogRepository> AuditLogRepository { get; set; }

            public Mock<IEventLogRepository> EventLogRepository { get; set; }

            public Mock<IUserActiveCartLogic> UserActiveCartLogic { get; set; }

            public Mock<IExternalCatalogRepository> ExternalCatalogRepository { get; set; }

            public Mock<IOrderedFromListRepository> OrderedFromListRepository { get; set; }

            public Mock<IOrderedItemsFromListRepository> OrderedItemsFromListRepository { get; set; }

            public static void RegisterInContainer(ref ContainerBuilder cb) {
                cb.RegisterInstance(MakeICacheRepository().Object)
                  .As<ICacheRepository>();
                cb.RegisterInstance(MakeICustomerRepository().Object)
                  .As<ICustomerRepository>();
                cb.RegisterInstance(MakeIBasketRepository().Object)
                  .As<IBasketRepository>();
                cb.RegisterInstance(MakeICatalogLogic().Object)
                  .As<ICatalogLogic>();
                cb.RegisterInstance(MakeIPriceLogic().Object)
                  .As<IPriceLogic>();
                cb.RegisterInstance(MakeIPurchaseOrderRepository().Object)
                  .As<IPurchaseOrderRepository>();
                cb.RegisterInstance(MakeIGenericQueueRepository().Object)
                  .As<IGenericQueueRepository>();
                cb.RegisterInstance(MakeIBasketLogic().Object)
                  .As<IBasketLogic>();
                cb.RegisterInstance(MakeINotesListLogic().Object)
                  .As<INotesListLogic>();
                cb.RegisterInstance(MakeIOrderQueueLogic().Object)
                  .As<IOrderQueueLogic>();
                cb.RegisterInstance(MakeIOrderHistoryLogic().Object)
                  .As<IOrderHistoryLogic>();
                cb.RegisterInstance(MakeIAuditLogRepository().Object)
                  .As<IAuditLogRepository>();
                cb.RegisterInstance(MakeIEventLogRepository().Object)
                  .As<IEventLogRepository>();
                cb.RegisterInstance(MakeIUserActiveCartLogic().Object)
                  .As<IUserActiveCartLogic>();
                cb.RegisterInstance(MakeIExternalCatalogRepository().Object)
                  .As<IExternalCatalogRepository>();
                cb.RegisterInstance(MakeIOrderedFromListRepository().Object)
                  .As<IOrderedFromListRepository>();
                cb.RegisterInstance(MakeIOrderedItemsFromListRepository().Object)
                  .As<IOrderedItemsFromListRepository>();
            }

            public static Mock<ICacheRepository> MakeICacheRepository() {
                Mock<ICacheRepository> mock = new Mock<ICacheRepository>();

                return mock;
            }

            public static Mock<ICustomerRepository> MakeICustomerRepository() {
                Mock<ICustomerRepository> mock = new Mock<ICustomerRepository>();

                mock.Setup(f => f.GetCustomerByCustomerNumber(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(new Customer());

                return mock;
            }

            public static Mock<IBasketRepository> MakeIBasketRepository() {
                Mock<IBasketRepository> mock = new Mock<IBasketRepository>();

                return mock;
            }

            public static Mock<ICatalogLogic> MakeICatalogLogic() {
                Mock<ICatalogLogic> mock = new Mock<ICatalogLogic>();

                mock.Setup(f => f.GetProductsByIds("fut", It.IsAny<List<string>>()))
                    .Returns(new ProductsReturn {
                        Products = new List<Product> {
                            new Product {
                                ItemNumber = "123456",
                                Name = "Fake Name",
                                BrandExtendedDescription = "Fake Brand",
                                ItemClass = "Fake Class",
                                Size = "Fake Size",
                                Pack = "Fake Pack"
                            }
                        }
                    });

                return mock;
            }

            public static Mock<IPriceLogic> MakeIPriceLogic() {
                Mock<IPriceLogic> mock = new Mock<IPriceLogic>();

                return mock;
            }

            public static Mock<IPurchaseOrderRepository> MakeIPurchaseOrderRepository() {
                Mock<IPurchaseOrderRepository> mock = new Mock<IPurchaseOrderRepository>();

                return mock;
            }

            public static Mock<IGenericQueueRepository> MakeIGenericQueueRepository() {
                Mock<IGenericQueueRepository> mock = new Mock<IGenericQueueRepository>();

                return mock;
            }

            public static Mock<IBasketLogic> MakeIBasketLogic() {
                Mock<IBasketLogic> mock = new Mock<IBasketLogic>();

                Basket returnedBasket = new Basket {
                    Id = "dddddddddddddddddddddddddddddddd",
                    DisplayName = "Fake Name",
                    BranchId = "FUT",
                    RequestedShipDate = "1/1/2017"
                };
                mock.Setup(f => f.RetrieveSharedCustomerBasket(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<Guid>()))
                    .Returns(returnedBasket);

                return mock;
            }

            public static Mock<INotesListLogic> MakeINotesListLogic() {
                Mock<INotesListLogic> mock = new Mock<INotesListLogic>();

                return mock;
            }

            public static Mock<IOrderQueueLogic> MakeIOrderQueueLogic() {
                Mock<IOrderQueueLogic> mock = new Mock<IOrderQueueLogic>();

                return mock;
            }

            public static Mock<IOrderHistoryLogic> MakeIOrderHistoryLogic() {
                Mock<IOrderHistoryLogic> mock = new Mock<IOrderHistoryLogic>();

                return mock;
            }

            public static Mock<IAuditLogRepository> MakeIAuditLogRepository() {
                Mock<IAuditLogRepository> mock = new Mock<IAuditLogRepository>();

                return mock;
            }

            public static Mock<IEventLogRepository> MakeIEventLogRepository() {
                Mock<IEventLogRepository> mock = new Mock<IEventLogRepository>();

                return mock;
            }

            public static Mock<IUserActiveCartLogic> MakeIUserActiveCartLogic() {
                Mock<IUserActiveCartLogic> mock = new Mock<IUserActiveCartLogic>();

                return mock;
            }

            public static Mock<IExternalCatalogRepository> MakeIExternalCatalogRepository() {
                Mock<IExternalCatalogRepository> mock = new Mock<IExternalCatalogRepository>();

                return mock;
            }

            public static Mock<IOrderedFromListRepository> MakeIOrderedFromListRepository() {
                Mock<IOrderedFromListRepository> mock = new Mock<IOrderedFromListRepository>();

                return mock;
            }

            public static Mock<IOrderedItemsFromListRepository> MakeIOrderedItemsFromListRepository()
            {
                Mock<IOrderedItemsFromListRepository> mock = new Mock<IOrderedItemsFromListRepository>();

                return mock;
            }
        }

        private static IShoppingCartLogic MakeTestsLogic(bool useAutoFac, ref MockDependents mockDependents) {
            if (useAutoFac) {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                MockDependents.RegisterInContainer(ref cb);

                IContainer testcontainer = cb.Build();

                return testcontainer.Resolve<IShoppingCartLogic>();
            }
            mockDependents = new MockDependents();
            mockDependents.AuditLogRepository = MockDependents.MakeIAuditLogRepository();
            mockDependents.BasketLogic = MockDependents.MakeIBasketLogic();
            mockDependents.BasketRepository = MockDependents.MakeIBasketRepository();
            mockDependents.CacheRepository = MockDependents.MakeICacheRepository();
            mockDependents.CatalogLogic = MockDependents.MakeICatalogLogic();
            mockDependents.CustomerRepository = MockDependents.MakeICustomerRepository();
            mockDependents.EventLogRepository = MockDependents.MakeIEventLogRepository();
            mockDependents.ExternalCatalogRepository = MockDependents.MakeIExternalCatalogRepository();
            mockDependents.GenericQueueRepository = MockDependents.MakeIGenericQueueRepository();
            mockDependents.NotesListLogic = MockDependents.MakeINotesListLogic();
            mockDependents.OrderHistoryLogic = MockDependents.MakeIOrderHistoryLogic();
            mockDependents.OrderQueueLogic = MockDependents.MakeIOrderQueueLogic();
            mockDependents.OrderedFromListRepository = MockDependents.MakeIOrderedFromListRepository();
            mockDependents.OrderedItemsFromListRepository = MockDependents.MakeIOrderedItemsFromListRepository();
            mockDependents.PriceLogic = MockDependents.MakeIPriceLogic();
            mockDependents.PurchaseOrderRepository = MockDependents.MakeIPurchaseOrderRepository();
            mockDependents.UserActiveCartLogic = MockDependents.MakeIUserActiveCartLogic();

            ShoppingCartLogicImpl testunit = new ShoppingCartLogicImpl(mockDependents.BasketRepository.Object, mockDependents.CatalogLogic.Object, mockDependents.PriceLogic.Object,
                                                                       mockDependents.OrderQueueLogic.Object, mockDependents.PurchaseOrderRepository.Object, mockDependents.GenericQueueRepository.Object,
                                                                       mockDependents.BasketLogic.Object, mockDependents.OrderHistoryLogic.Object, mockDependents.OrderedItemsFromListRepository.Object, 
                                                                       mockDependents.CustomerRepository.Object,
                                                                       mockDependents.AuditLogRepository.Object, mockDependents.NotesListLogic.Object, mockDependents.UserActiveCartLogic.Object,
                                                                       mockDependents.ExternalCatalogRepository.Object, mockDependents.CacheRepository.Object, mockDependents.EventLogRepository.Object,
                                                                       mockDependents.OrderedFromListRepository.Object);
            return testunit;
        }
        #endregion Setup
    }
}