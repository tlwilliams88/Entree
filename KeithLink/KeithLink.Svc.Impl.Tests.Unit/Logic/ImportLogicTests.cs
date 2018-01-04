using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Autofac;

using FluentAssertions;

using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface;
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
using KeithLink.Svc.Core.Models.ImportFiles;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic;

using Moq;

using Xunit;

using Product = KeithLink.Svc.Core.Models.SiteCatalog.Product;
using UserProfile = KeithLink.Svc.Core.Models.Profile.UserProfile;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic {
    public class ImportLogicTests : BaseDITests {

        public class ImportOrder
        {
            [Fact]
            public void GoodItem_ReturnsExpectedSuccessMsg() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(true, ref mockDependents);
                var fakeUser = new UserProfile();
                var testContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                var testImportFile = new OrderImportFileModel() {
                    Options = new OrderImportOptions() {
                        FileFormat = FileFormat.CSV,
                        IgnoreFirstLine = false,
                        Contents = FileContentType.ItemOnly,
                        CartName = "TestCart"
                    },
                    Contents = "123456"
                };
                var expected = "Import Successful.";

                // act
                var results = testunit.ImportOrder(fakeUser, testContext, testImportFile);

                // assert
                results.SuccessMessage
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodItemAndBadItem_ReturnsExpectedWarningMsg()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(true, ref mockDependents);
                var fakeUser = new UserProfile();
                var testContext = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                var testImportFile = new OrderImportFileModel()
                {
                    Options = new OrderImportOptions()
                    {
                        FileFormat = FileFormat.CSV,
                        IgnoreFirstLine = false,
                        Contents = FileContentType.ItemOnly,
                        CartName = "TestCart"
                    },
                    Contents = "123456\n234567"
                };
                var expected = "Some items failed to import.  Please check the items in your cart.\r\n";

                // act
                var results = testunit.ImportOrder(fakeUser, testContext, testImportFile);

                // assert
                results.WarningMessage
                       .Should()
                       .Be(expected);
            }

        }

        #region Setup
        public class MockDependents {
            public Mock<ICatalogLogic> CatalogLogic { get; set; }

            public Mock<IEventLogRepository> EventLogRepository { get; set; }

            public Mock<IShoppingCartLogic> ShoppingCartLogic { get; set; }

            public Mock<IPriceLogic> PriceLogic { get; set; }

            public Mock<ICustomInventoryItemsRepository> CustomInventoryItemsRepository { get; set; }

            public Mock<ISiteCatalogService> SiteCatalogService { get; set; }

            public static void RegisterInContainer(ref ContainerBuilder cb) {
                cb.RegisterInstance(MakeICatalogLogic()
                                            .Object)
                  .As<ICatalogLogic>();
                cb.RegisterInstance(MakeIEventLogRepository()
                                            .Object)
                  .As<IEventLogRepository>();
                cb.RegisterInstance(MakeIShoppingCartLogic()
                                            .Object)
                  .As<IShoppingCartLogic>();
                cb.RegisterInstance(MakeIPriceLogic()
                                            .Object)
                  .As<IPriceLogic>();
                cb.RegisterInstance(MakeICustomInventoryItemsRepository()
                                            .Object)
                  .As<ICustomInventoryItemsRepository>();
                cb.RegisterInstance(MakeISiteCatalogService()
                                            .Object)
                  .As<ISiteCatalogService>();
            }

            public static Mock<ICatalogLogic> MakeICatalogLogic() {
                Mock<ICatalogLogic> mock = new Mock<ICatalogLogic>();

                mock.Setup(f => f.GetProductsByIds("FUT", It.IsAny<List<string>>()))
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

            public static Mock<IEventLogRepository> MakeIEventLogRepository()
            {
                Mock<IEventLogRepository> mock = new Mock<IEventLogRepository>();

                return mock;
            }

            public static Mock<IShoppingCartLogic> MakeIShoppingCartLogic() {
                Mock<IShoppingCartLogic> mock = new Mock<IShoppingCartLogic>();

                return mock;
            }

            public static Mock<IPriceLogic> MakeIPriceLogic()
            {
                Mock<IPriceLogic> mock = new Mock<IPriceLogic>();

                mock.Setup(f => f.GetPrices("FUT", "234567", It.IsAny<DateTime>(), It.IsAny<List<Product>>()))
                    .Returns(new PriceReturn()
                    {
                        Prices = new List<Price> {
                            new Price() {
                                ItemNumber = "123456",
                                CasePrice = 1,
                                PackagePrice = 1
                            }
                        }
                    });

                return mock;
            }

            public static Mock<ICustomInventoryItemsRepository> MakeICustomInventoryItemsRepository()
            {
                Mock<ICustomInventoryItemsRepository> mock = new Mock<ICustomInventoryItemsRepository>();

                return mock;
            }

            public static Mock<ISiteCatalogService> MakeISiteCatalogService()
            {
                Mock<ISiteCatalogService> mock = new Mock<ISiteCatalogService>();

                return mock;
            }

        }

        private static IImportLogic MakeTestsLogic(bool useAutoFac, ref MockDependents mockDependents) {
            if (useAutoFac) {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                MockDependents.RegisterInContainer(ref cb);

                IContainer testcontainer = cb.Build();

                return testcontainer.Resolve<IImportLogic>();
            }
            mockDependents = new MockDependents();
            mockDependents.CatalogLogic = MockDependents.MakeICatalogLogic();
            mockDependents.EventLogRepository = MockDependents.MakeIEventLogRepository();
            mockDependents.ShoppingCartLogic = MockDependents.MakeIShoppingCartLogic();
            mockDependents.PriceLogic = MockDependents.MakeIPriceLogic();
            mockDependents.CustomInventoryItemsRepository = MockDependents.MakeICustomInventoryItemsRepository();
            mockDependents.SiteCatalogService = MockDependents.MakeISiteCatalogService();

            ImportLogicImpl testunit = new ImportLogicImpl(mockDependents.CatalogLogic.Object, mockDependents.EventLogRepository.Object, mockDependents.ShoppingCartLogic.Object, 
                                                           mockDependents.PriceLogic.Object, mockDependents.CustomInventoryItemsRepository.Object, mockDependents.SiteCatalogService.Object);
            return testunit;
        }
        #endregion Setup
    }
}