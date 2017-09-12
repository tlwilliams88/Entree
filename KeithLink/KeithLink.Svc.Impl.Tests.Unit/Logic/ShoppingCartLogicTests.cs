using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
using KeithLink.Svc.Core.Models.SiteCatalog;
using Product = KeithLink.Svc.Core.Models.SiteCatalog.Product;
using UserProfile = KeithLink.Svc.Core.Models.Profile.UserProfile;
using KeithLink.Svc.Impl.Logic;

using Autofac;
using FluentAssertions;
using Moq;
using Xunit;

using CommerceServer.Foundation;

using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.ShoppingCart;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic
{
    public class ShoppingCartLogicTests : BaseDITests
    {
        #region Setup
        public class MockDependents
        {
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

            public static void RegisterInContainer(ref ContainerBuilder cb)
            {
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
            }

            public static Mock<ICacheRepository> MakeICacheRepository()
            {
                var mock = new Mock<ICacheRepository>();

                return mock;
            }

            public static Mock<ICustomerRepository> MakeICustomerRepository()
            {
                var mock = new Mock<ICustomerRepository>();

                mock.Setup(f => f.GetCustomerByCustomerNumber(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(new Customer());

                return mock;
            }

            public static Mock<IBasketRepository> MakeIBasketRepository()
            {
                var mock = new Mock<IBasketRepository>();

                return mock;
            }

            public static Mock<ICatalogLogic> MakeICatalogLogic()
            {
                var mock = new Mock<ICatalogLogic>();

                mock.Setup(f => f.GetProductsByIds("fut", It.IsAny<List<string>>()))
                    .Returns(new ProductsReturn() {
                                                      Products = new List<Product>() {
                                                                                         new Product() {
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

            public static Mock<IPriceLogic> MakeIPriceLogic()
            {
                var mock = new Mock<IPriceLogic>();

                return mock;
            }

            public static Mock<IPurchaseOrderRepository> MakeIPurchaseOrderRepository()
            {
                var mock = new Mock<IPurchaseOrderRepository>();

                return mock;
            }

            public static Mock<IGenericQueueRepository> MakeIGenericQueueRepository()
            {
                var mock = new Mock<IGenericQueueRepository>();

                return mock;
            }

            public static Mock<IBasketLogic> MakeIBasketLogic()
            {
                var mock = new Mock<IBasketLogic>();

                var returnedBasket = new Basket() {
                                                      Id = "dddddddddddddddddddddddddddddddd",
                                                      DisplayName = "Fake Name",
                                                      BranchId = "FUT",
                                                      RequestedShipDate = "1/1/2017"
                                                  };
                mock.Setup(f => f.RetrieveSharedCustomerBasket(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<Guid>()))
                    .Returns(returnedBasket);

                return mock;
            }

            public static Mock<INotesListLogic> MakeINotesListLogic()
            {
                var mock = new Mock<INotesListLogic>();

                return mock;
            }

            public static Mock<IOrderQueueLogic> MakeIOrderQueueLogic()
            {
                var mock = new Mock<IOrderQueueLogic>();

                return mock;
            }

            public static Mock<IOrderHistoryLogic> MakeIOrderHistoryLogic()
            {
                var mock = new Mock<IOrderHistoryLogic>();

                return mock;
            }

            public static Mock<IAuditLogRepository> MakeIAuditLogRepository()
            {
                var mock = new Mock<IAuditLogRepository>();

                return mock;
            }

            public static Mock<IEventLogRepository> MakeIEventLogRepository()
            {
                var mock = new Mock<IEventLogRepository>();

                return mock;
            }

            public static Mock<IUserActiveCartLogic> MakeIUserActiveCartLogic()
            {
                var mock = new Mock<IUserActiveCartLogic>();

                return mock;
            }

            public static Mock<IExternalCatalogRepository> MakeIExternalCatalogRepository()
            {
                var mock = new Mock<IExternalCatalogRepository>();

                return mock;
            }

            public static Mock<IOrderedFromListRepository> MakeIOrderedFromListRepository()
            {
                var mock = new Mock<IOrderedFromListRepository>();

                return mock;
            }

        }

        private static IShoppingCartLogic MakeTestsLogic(bool useAutoFac, ref MockDependents mockDependents)
        {
            if (useAutoFac)
            {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                MockDependents.RegisterInContainer(ref cb);

                var testcontainer = cb.Build();

                return testcontainer.Resolve<IShoppingCartLogic>();
            }
            else
            {
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
                mockDependents.PriceLogic = MockDependents.MakeIPriceLogic();
                mockDependents.PurchaseOrderRepository = MockDependents.MakeIPurchaseOrderRepository();
                mockDependents.UserActiveCartLogic = MockDependents.MakeIUserActiveCartLogic();

                var testunit = new ShoppingCartLogicImpl(mockDependents.BasketRepository.Object, mockDependents.CatalogLogic.Object, mockDependents.PriceLogic.Object,
                                                         mockDependents.OrderQueueLogic.Object, mockDependents.PurchaseOrderRepository.Object, mockDependents.GenericQueueRepository.Object,
                                                         mockDependents.BasketLogic.Object, mockDependents.OrderHistoryLogic.Object, mockDependents.CustomerRepository.Object,
                                                         mockDependents.AuditLogRepository.Object, mockDependents.NotesListLogic.Object, mockDependents.UserActiveCartLogic.Object,
                                                         mockDependents.ExternalCatalogRepository.Object, mockDependents.CacheRepository.Object, mockDependents.EventLogRepository.Object,
                                                         mockDependents.OrderedFromListRepository.Object);
                return testunit;
            }
        }
        #endregion

        #region attributes
        #endregion

        #region CartReport
        public class CartReport
        {
            [Fact]
            public void AnyCall_CallsBasketLogicRetrieveSharedCustomerBasket()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testContext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                var testCart = new Guid("dddddddddddddddddddddddddddddddd");
                var testPagedListModel = new PagedListModel()
                {
                    Name = "Fake Name"
                };
                var testPrintModel = new PrintListModel();

                // act
                var results = testunit.CartReport(fakeUser, testContext, testCart, testPagedListModel, testPrintModel);

                // assert
                mockDependents.BasketLogic.Verify(m => m.RetrieveSharedCustomerBasket(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<Guid>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyCall_CallsUserActiveCartLogicGetUserActiveCart()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testContext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                var testCart = new Guid("dddddddddddddddddddddddddddddddd");
                var testPagedListModel = new PagedListModel() {
                                                                  Name="Fake Name"
                                                              };
                var testPrintModel = new PrintListModel();

                // act
                var results = testunit.CartReport(fakeUser, testContext, testCart, testPagedListModel, testPrintModel);

                // assert
                mockDependents.UserActiveCartLogic.Verify(m => m.GetUserActiveCart(It.IsAny<UserSelectedContext>(), It.IsAny<Guid>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyCall_ProducesStream()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: true, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testContext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                var testCart = new Guid("dddddddddddddddddddddddddddddddd");
                var testPagedListModel = new PagedListModel()
                {
                    Name = "Fake Name"
                };
                var testPrintModel = new PrintListModel();

                // act
                var results = testunit.CartReport(fakeUser, testContext, testCart, testPagedListModel, testPrintModel);

                // assert
                results.Should()
                       .NotBeNull();
            }
        }
        #endregion

        #region LookupProductDetails
        public class LookupProductDetails
        {
            [Fact]
            public void CartWithGoodItem_DetailIsExpected()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: true, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testContext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                var testCart = new ShoppingCart() {
                                                      Active = true,
                                                      BranchId = "FUT",
                                                      CartId = new Guid("dddddddddddddddddddddddddddddddd"),
                                                      Name = "Fake Cart Name",
                                                      Items = new List<ShoppingCartItem>() {
                                                                                               new ShoppingCartItem() {
                                                                                                                          ItemNumber="123456",
                                                                                                                          CatalogId = "FUT"
                                                                                                                      }
                                                                                           }
                };
                var expected = "Fake Name / 123456 / Fake Brand / Fake Class / Fake Pack / Fake Size";

                // act
                testunit.LookupProductDetails(fakeUser, testContext, testCart);

                // assert
                testCart.Items.First()
                        .Detail.Should()
                        .Be(expected);
            }

        }
        #endregion
    }
}
