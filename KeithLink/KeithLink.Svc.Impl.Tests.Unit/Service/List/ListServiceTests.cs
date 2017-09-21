using System.Collections.Generic;

using Autofac;

using FluentAssertions;

using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Configurations;

using Moq;

using Xunit;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.Reports;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.ReminderItems;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic.Lists;
using KeithLink.Common.Core.Interfaces.Logging;

using System;
using System.Linq;
using System.Linq.Expressions;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Configuration.EF;
using KeithLink.Svc.Core.Models.Customers.EF;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Lists.CustomList;
using KeithLink.Svc.Core.Models.Lists.Favorites;
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;
using KeithLink.Svc.Core.Models.Lists.MandatoryItem;
using KeithLink.Svc.Core.Models.Lists.Notes;
using KeithLink.Svc.Core.Models.Lists.RecommendedItems;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Impl.Service.List;

namespace KeithLink.Svc.Impl.Tests.Unit.Service.List {
    public class ListServiceTests : BaseDITests {
        #region Setup
        public class MockDependents {
            public Mock<ICacheListLogic> CacheListHelper { get; set; }

            public Mock<IContractListLogic> ContractListLogic { get; set; }

            public Mock<IHistoryListLogic> HistoryListLogic { get; set; }

            public Mock<IFavoritesListLogic> FavoritesListLogic { get; set; }

            public Mock<IRecentlyViewedListLogic> RecentlyViewedListLogic { get; set; }

            public Mock<IRecentlyOrderedListLogic> RecentlyOrderedListLogic { get; set; }

            public Mock<IRecommendedItemsListLogic> RecommendedItemsListLogic { get; set; }

            public Mock<IMandatoryItemsListLogic> MandatoryItemsListLogic { get; set; }

            public Mock<IInventoryValuationListLogic> InventoryValuationListLogic { get; set; }

            public Mock<IRemindersListLogic> RemindersListLogic { get; set; }

            public Mock<ICustomListLogic> CustomListLogic { get; set; }

            public Mock<INotesListLogic> NotesListLogic { get; set; }

            public Mock<ICatalogLogic> CatalogLogic { get; set; }

            public Mock<IExternalCatalogRepository> ExternalCatalogRepository { get; set; }

            public Mock<IItemHistoryRepository> ItemHistoryRepository { get; set; }

            public Mock<IPriceLogic> PriceLogic { get; set; }

            public Mock<IProductImageRepository> ProductImageRepository { get; set; }

            public Mock<IItemBarcodeImageRepository> ItemBarcodeRepository { get; set; }

            public Mock<IEventLogRepository> EventLogRepository { get; set; }

            public Mock<ICustomInventoryItemsRepository> CustomInventoryItemsRepository { get; set; }

            public static void RegisterInContainer(ref ContainerBuilder cb) {
                cb.RegisterInstance(MakeMockRecentlyViewedListLogic()
                                            .Object)
                  .As<IRecentlyViewedListLogic>();
                cb.RegisterInstance(MakeMockRecentlyOrderedListLogic()
                                            .Object)
                  .As<IRecentlyOrderedListLogic>();
                cb.RegisterInstance(MakeMockRecommendedItemsListLogic()
                                            .Object)
                  .As<IRecommendedItemsListLogic>();
                cb.RegisterInstance(MakeMockMandatoryItemsListLogic()
                                            .Object)
                  .As<IMandatoryItemsListLogic>();
                cb.RegisterInstance(MakeMockInventoryValuationListLogic()
                                            .Object)
                  .As<IInventoryValuationListLogic>();
                cb.RegisterInstance(MakeMockRemindersListLogic()
                                            .Object)
                  .As<IRemindersListLogic>();
                cb.RegisterInstance(MakeMockCustomListLogic()
                                            .Object)
                  .As<ICustomListLogic>();
                cb.RegisterInstance(MakeMockNotesListLogic()
                                            .Object)
                  .As<INotesListLogic>();
                cb.RegisterInstance(MakeMockCatalogLogic()
                                            .Object)
                  .As<ICatalogLogic>();
                cb.RegisterInstance(MakeMockExternalCatalogRepository()
                                            .Object)
                  .As<IExternalCatalogRepository>();
                cb.RegisterInstance(MakeMockItemHistoryRepository()
                                            .Object)
                  .As<IItemHistoryRepository>();
                cb.RegisterInstance(MakeMockPriceLogic()
                                            .Object)
                  .As<IPriceLogic>();
                cb.RegisterInstance(MakeMockProductImageRepository()
                                            .Object)
                  .As<IProductImageRepository>();
                cb.RegisterInstance(MakeMockItemBarcodeImageRepository()
                                            .Object)
                  .As<IItemBarcodeImageRepository>();
                cb.RegisterInstance(MakeMockEventLogRepository()
                                            .Object)
                  .As<IEventLogRepository>();
                cb.RegisterInstance(MakeMockCacheListHelper()
                                            .Object)
                  .As<ICacheListLogic>();
                cb.RegisterInstance(MakeMockContractListLogic()
                                            .Object)
                  .As<IContractListLogic>();
                cb.RegisterInstance(MakeMockHistoryListLogic()
                                            .Object)
                  .As<IHistoryListLogic>();
                cb.RegisterInstance(MakeMockFavoritesListLogic()
                                            .Object)
                  .As<IFavoritesListLogic>();
                cb.RegisterInstance(MakeMockCustomInventoryItemRepository()
                                            .Object)
                  .As<ICustomInventoryItemsRepository>();
            }

            public static Mock<IRecentlyViewedListLogic> MakeMockRecentlyViewedListLogic() {
                var mock = new Mock<IRecentlyViewedListLogic>();

                return mock;
            }

            public static Mock<IFavoritesListLogic> MakeMockFavoritesListLogic() {
                var mock = new Mock<IFavoritesListLogic>();

                mock.Setup(f => f.GetListModel(It.IsAny<UserProfile>(),
                                               It.IsAny<UserSelectedContext>(),
                                               It.Is<long>(i => i == 1)))
                    .Returns(new ListModel() {
                        BranchId = "FUT",
                        CustomerNumber = "123456",
                        Items = new List<ListItemModel>() {
                            new ListItemModel() {
                                ItemNumber = "123456"
                            },
                            new ListItemModel() {
                                ItemNumber = "234567"
                            },
                            new ListItemModel() {
                                ItemNumber = "345678"
                            }
                        }
                    });

                return mock;
            }

            public static Mock<IHistoryListLogic> MakeMockHistoryListLogic() {
                var mock = new Mock<IHistoryListLogic>();

                return mock;
            }

            public static Mock<IContractListLogic> MakeMockContractListLogic() {
                var mock = new Mock<IContractListLogic>();

                mock.Setup(h => h.GetListModel(It.IsAny<UserProfile>(),
                                               It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                               c.CustomerId == "123456"),
                                               It.IsAny<long>()))
                    .Returns(new ListModel() {
                        ListId = 1,
                        CustomerNumber = "123456",
                        BranchId = "FUT",
                        Items = new List<ListItemModel>() {
                            new ListItemModel() {
                                ItemNumber = "123456",
                                Category = "Fake Category"
                            }
                        }
                    });

                return mock;
            }

            public static Mock<IRemindersListLogic> MakeMockRemindersListLogic() {
                var mock = new Mock<IRemindersListLogic>();

                mock.Setup(h => h.SaveList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<ListModel>()))
                    .Returns(new ListModel() {
                        ListId = 1
                    });


                return mock;
            }

            public static Mock<IInventoryValuationListLogic> MakeMockInventoryValuationListLogic() {
                var mock = new Mock<IInventoryValuationListLogic>();

                return mock;
            }

            public static Mock<IMandatoryItemsListLogic> MakeMockMandatoryItemsListLogic() {
                var mock = new Mock<IMandatoryItemsListLogic>();

                return mock;
            }

            public static Mock<ICustomInventoryItemsRepository> MakeMockCustomInventoryItemRepository() {
                var mock = new Mock<ICustomInventoryItemsRepository>();

                mock.Setup(h => h.Get(1))
                    .Returns(new CustomInventoryItem() {
                        ItemNumber = "666666",
                        Name = "double bad",
                        CasePrice = 0,
                        PackagePrice = 0
                    });

                return mock;
            }

            public static Mock<IRecommendedItemsListLogic> MakeMockRecommendedItemsListLogic() {
                var mock = new Mock<IRecommendedItemsListLogic>();

                mock.Setup(h => h.ReadList(It.IsAny<UserProfile>(),
                                           It.IsAny<UserSelectedContext>(),
                                           It.IsAny<bool>()))
                    .Returns(new ListModel() {
                        ListId = 1,
                        CustomerNumber = "123456",
                        BranchId = "FUT",
                        Items = new List<ListItemModel>() {
                            new ListItemModel() {
                                ItemNumber = "123456",
                                Each = false,
                                Category = "Fake Category"
                            }
                        }
                    });

                return mock;
            }

            public static Mock<IRecentlyOrderedListLogic> MakeMockRecentlyOrderedListLogic() {
                var mock = new Mock<IRecentlyOrderedListLogic>();

                return mock;
            }

            public static Mock<INotesListLogic> MakeMockNotesListLogic() {
                var mock = new Mock<INotesListLogic>();

                mock.Setup(l => l.GetList(It.IsAny<UserSelectedContext>()))
                    .Returns(new ListModel() {
                        ListId = 1,
                        CustomerNumber = "123456",
                        BranchId = "FUT",
                        Items = new List<ListItemModel>() {
                            new ListItemModel() {
                                ItemNumber = "123456",
                                Notes = "Test Note"
                            }
                        }
                    });
                    

                return mock;
            }

            public static Mock<ICustomListLogic> MakeMockCustomListLogic() {
                var mock = new Mock<ICustomListLogic>();

                mock.Setup(h => h.GetListModel(It.IsAny<UserProfile>(),
                                               It.IsAny<UserSelectedContext>(),
                                               It.Is<long>(i => i == 1)))
                    .Returns(new ListModel() {
                        ListId = 1,
                        CustomerNumber = "123456",
                        BranchId = "FUT",
                        Items = new List<ListItemModel>() {
                            new ListItemModel() {
                                ItemNumber = "123456",
                                Category = "Fake Category",
                                Each = false
                            }
                        }
                    });

                mock.Setup(h => h.ReadList(It.Is<long>(i => i == 1), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()))
                    .Returns(new ListModel() {
                        ListId = 1,
                        CustomerNumber = "123456",
                        BranchId = "FUT",
                        Items = new List<ListItemModel>() {
                            new ListItemModel() {
                                ItemNumber = "123456",
                                Category = "Fake Category",
                                Each = false
                            }
                        }
                    });

                mock.Setup(h => h.ReadLists(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()))
                    .Returns(new List<ListModel>() {
                        new ListModel() {
                            ListId = 1,
                            CustomerNumber = "123456",
                            BranchId = "FUT",
                            Items = new List<ListItemModel>() {
                                new ListItemModel() {
                                    ItemNumber = "123456",
                                    Category = "Fake Category",
                                    Each = false
                                }
                            }
                        }
                    });

                mock.Setup(h => h.GetListModel(It.IsAny<UserProfile>(),
                                               It.IsAny<UserSelectedContext>(),
                                               It.Is<long>(i => i == 2)))
                    .Returns(new ListModel() {
                        ListId = 2,
                        CustomerNumber = "123456",
                        BranchId = "FUT"
                    });

                mock.Setup(h => h.CreateOrUpdateList(It.IsAny<UserProfile>(),
                                                     It.IsAny<UserSelectedContext>(),
                                                     0,
                                                     It.IsAny<string>(),
                                                     It.IsAny<bool>()))
                    .Returns((long) 2);

                return mock;
            }

            public static Mock<IEventLogRepository> MakeMockEventLogRepository() {
                var mock = new Mock<IEventLogRepository>();

                return mock;
            }

            public static Mock<IItemBarcodeImageRepository> MakeMockItemBarcodeImageRepository() {
                var mock = new Mock<IItemBarcodeImageRepository>();

                return mock;
            }

            public static Mock<IProductImageRepository> MakeMockProductImageRepository() {
                var mock = new Mock<IProductImageRepository>();

                return mock;
            }

            public static Mock<IPriceLogic> MakeMockPriceLogic() {
                var mock = new Mock<IPriceLogic>();

                return mock;
            }

            public static Mock<IItemHistoryRepository> MakeMockItemHistoryRepository() {
                var mock = new Mock<IItemHistoryRepository>();

                return mock;
            }

            public static Mock<IExternalCatalogRepository> MakeMockExternalCatalogRepository() {
                var mock = new Mock<IExternalCatalogRepository>();

                mock.Setup(h => h.ReadAll())
                    .Returns(new List<ExternalCatalog>() {
                        new ExternalCatalog() {
                            BekBranchId = "XXX",
                            ExternalBranchId = "multi-X"
                        }
                    });

                return mock;
            }

            public static Mock<ICatalogLogic> MakeMockCatalogLogic() {
                var mock = new Mock<ICatalogLogic>();

                return mock;
            }

            public static Mock<ICacheListLogic> MakeMockCacheListHelper() {
                var mock = new Mock<ICacheListLogic>();

                return mock;
            }
        }

        private static IListService MakeTestsService(bool useAutoFac, ref MockDependents mockDependents) {
            if (useAutoFac) {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                MockDependents.RegisterInContainer(ref cb);

                var testcontainer = cb.Build();

                return testcontainer.Resolve<IListService>();
            } else {
                mockDependents = new MockDependents();
                mockDependents.CacheListHelper = MockDependents.MakeMockCacheListHelper();
                mockDependents.CatalogLogic = MockDependents.MakeMockCatalogLogic();
                mockDependents.ContractListLogic = MockDependents.MakeMockContractListLogic();
                mockDependents.CustomListLogic = MockDependents.MakeMockCustomListLogic();
                mockDependents.EventLogRepository = MockDependents.MakeMockEventLogRepository();
                mockDependents.ExternalCatalogRepository = MockDependents.MakeMockExternalCatalogRepository();
                mockDependents.FavoritesListLogic = MockDependents.MakeMockFavoritesListLogic();
                mockDependents.HistoryListLogic = MockDependents.MakeMockHistoryListLogic();
                mockDependents.InventoryValuationListLogic = MockDependents.MakeMockInventoryValuationListLogic();
                mockDependents.ItemBarcodeRepository = MockDependents.MakeMockItemBarcodeImageRepository();
                mockDependents.ItemHistoryRepository = MockDependents.MakeMockItemHistoryRepository();
                mockDependents.MandatoryItemsListLogic = MockDependents.MakeMockMandatoryItemsListLogic();
                mockDependents.NotesListLogic = MockDependents.MakeMockNotesListLogic();
                mockDependents.PriceLogic = MockDependents.MakeMockPriceLogic();
                mockDependents.ProductImageRepository = MockDependents.MakeMockProductImageRepository();
                mockDependents.RecentlyOrderedListLogic = MockDependents.MakeMockRecentlyOrderedListLogic();
                mockDependents.RecentlyViewedListLogic = MockDependents.MakeMockRecentlyViewedListLogic();
                mockDependents.RecommendedItemsListLogic = MockDependents.MakeMockRecommendedItemsListLogic();
                mockDependents.RemindersListLogic = MockDependents.MakeMockRemindersListLogic();
                mockDependents.CustomInventoryItemsRepository = MockDependents.MakeMockCustomInventoryItemRepository();

                var testunit = new ListServiceImpl(mockDependents.HistoryListLogic.Object, mockDependents.CatalogLogic.Object, mockDependents.NotesListLogic.Object,
                                                   mockDependents.ItemHistoryRepository.Object, mockDependents.FavoritesListLogic.Object, mockDependents.PriceLogic.Object,
                                                   mockDependents.RecentlyViewedListLogic.Object, mockDependents.RecentlyOrderedListLogic.Object,
                                                   mockDependents.RecommendedItemsListLogic.Object, mockDependents.RemindersListLogic.Object,
                                                   mockDependents.ProductImageRepository.Object, mockDependents.ExternalCatalogRepository.Object,
                                                   mockDependents.ItemBarcodeRepository.Object, mockDependents.MandatoryItemsListLogic.Object,
                                                   mockDependents.InventoryValuationListLogic.Object, mockDependents.ContractListLogic.Object,
                                                   mockDependents.CustomListLogic.Object,
                                                   mockDependents.EventLogRepository.Object, mockDependents.CustomInventoryItemsRepository.Object,
                                                   mockDependents.CacheListHelper.Object);
                return testunit;
            }
        }
        #endregion

        #region GetContractInformation
        public class GetContractInformation {
            [Fact]
            public void BadBranchId_ReturnsEmptyList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: true, mockDependents: ref mockDependents);
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var emptyList = 0;
                // act
                var results = testunit.GetContractInformation(testcontext);

                // assert
                results.Count
                       .Should()
                       .Be(emptyList);
            }

            [Fact]
            public void BadCustomerId_ReturnsEmptyList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: true, mockDependents: ref mockDependents);
                var testcontext = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "999999"
                };
                var emptyList = 0;
                // act
                var results = testunit.GetContractInformation(testcontext);

                // assert
                results.Count
                       .Should()
                       .Be(emptyList);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedItem() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: true, mockDependents: ref mockDependents);
                var testcontext = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var expectedItemNumber = "123456";

                // act
                var results = testunit.GetContractInformation(testcontext);

                // assert
                results.First()
                       .Key
                       .Should()
                       .Be(expectedItemNumber);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedItemWithExpectedCategory() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: true, mockDependents: ref mockDependents);
                var testcontext = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var expectedItemNumber = "123456";
                var expectedCategory = "Fake Category";

                // act
                var results = testunit.GetContractInformation(testcontext);

                // assert
                results[expectedItemNumber]
                        .Should()
                        .Be(expectedCategory);
            }

            [Fact]
            public void GoodCustomer_CallsContractListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var testcontext = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var expectedItemNumber = "123456";
                var expectedCategory = "Fake Category";

                // act
                var results = testunit.GetContractInformation(testcontext);

                // assert
                mockDependents.ContractListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(),
                                                                            It.IsAny<UserSelectedContext>(),
                                                                            It.IsAny<long>()), Times.Once, "not called");
            }
        }
        #endregion

        #region GetItemsHistoryList
        public class GetItemsHistoryList {
            [Fact]
            public void AnyContext_CallsItemHistoryRepositoryRead() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testitems = new List<string>() {"123456"};
                var emptyList = 0;
                // act
                var results = testunit.GetItemsHistoryList(testcontext, testitems.ToArray());

                // assert
                mockDependents.ItemHistoryRepository.Verify(m => m.Read(It.IsAny<Expression<Func<ItemHistory, bool>>>()), Times.Once, "not called");
            }
        }
        #endregion

        #region ReadListByType
        public class ReadListByType {
            [Fact]
            public void AnyUserAnyContextWithTypeCustom_CallsCustomListLogicReadLists() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Custom;

                // act
                var results = testunit.ReadListByType(fakeUser, testcontext, testListType);

                // assert
                mockDependents.CustomListLogic.Verify(m => m.ReadLists(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeFavorites_CallsFavoritesListLogicGetFavoritesList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Favorite;

                // act
                var results = testunit.ReadListByType(fakeUser, testcontext, testListType);

                // assert
                mockDependents.FavoritesListLogic.Verify(m => m.GetFavoritesList(It.IsAny<Guid>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeContract_CallsContractListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var testListType = ListType.Contract;

                // act
                var results = testunit.ReadList(fakeUser, testcontext, testListType, 1);

                // assert
                mockDependents.ContractListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(),
                                                                            It.IsAny<UserSelectedContext>(),
                                                                            It.IsAny<long>()),
                                                        Times.Exactly(2),
                                                        "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeNotes_CallsNotesListLogicGetList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var testListType = ListType.Custom;

                // act
                var results = testunit.ReadListByType(fakeUser, testcontext, testListType);

                // assert
                mockDependents.NotesListLogic.Verify(m => m.GetList(It.IsAny<UserSelectedContext>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeWorksheet_CallsHistoryListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var testListType = ListType.Worksheet;

                // act
                var results = testunit.ReadList(fakeUser, testcontext, testListType, 1);
                
                // assert
                mockDependents.HistoryListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeReminder_CallsRemindersListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Reminder;

                // act
                var results = testunit.ReadListByType(fakeUser, testcontext, testListType);

                // assert
                mockDependents.RemindersListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), false), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeMandatory_CallsMandatoryItemsListLogicReadList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Mandatory;

                // act
                var results = testunit.ReadListByType(fakeUser, testcontext, testListType);

                // assert
                mockDependents.MandatoryItemsListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeReminder_CallsRecommendedItemsListLogicReadList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.RecommendedItems;

                // act
                var results = testunit.ReadListByType(fakeUser, testcontext, testListType);

                // assert
                mockDependents.RecommendedItemsListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeInventoryValuation_CallsInventoryValuationListLogicReadLists() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.InventoryValuation;

                // act
                var results = testunit.ReadListByType(fakeUser, testcontext, testListType);

                // assert
                mockDependents.InventoryValuationListLogic.Verify(m => m.ReadLists(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeRecentlyOrdered_CallsRecentlyOrderedListLogicReadList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.RecentlyOrdered;

                // act
                var results = testunit.ReadListByType(fakeUser, testcontext, testListType);

                // assert
                mockDependents.RecentlyOrderedListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeRecentlyViewed_CallsRecentlyViewedListLogicReadList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.RecentlyViewed;

                // act
                var results = testunit.ReadListByType(fakeUser, testcontext, testListType);

                // assert
                mockDependents.RecentlyViewedListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }
        }
        #endregion

        #region ReadUserList
        public class ReadUserList {
            [Fact]
            public void AnyUserAnyContext_CallsCustomListLogicReadLists() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testHeadersOnly = false;

                // act
                var results = testunit.ReadUserList(fakeUser, testcontext, testHeadersOnly);

                // assert
                mockDependents.CustomListLogic.Verify(m => m.ReadLists(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContext_CallsFavoritesListLogicGetFavoritesList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testHeadersOnly = false;

                // act
                var results = testunit.ReadUserList(fakeUser, testcontext, testHeadersOnly);

                // assert
                mockDependents.FavoritesListLogic.Verify(m => m.GetFavoritesList(It.IsAny<Guid>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Exactly(3), "not called");
            }

            [Fact]
            public void AnyUserAnyContext_CallsContractListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testHeadersOnly = false;

                // act
                var results = testunit.ReadUserList(fakeUser, testcontext, testHeadersOnly);

                // assert
                mockDependents.ContractListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()),
                                                        Times.AtLeastOnce, "not called");
            }

            [Fact]
            public void AnyUserAnyContext_CallsHistoryListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testHeadersOnly = false;

                // act
                var results = testunit.ReadUserList(fakeUser, testcontext, testHeadersOnly);

                // assert
                mockDependents.HistoryListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), testHeadersOnly), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContext_CallsRemindersListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testHeadersOnly = false;

                // act
                var results = testunit.ReadUserList(fakeUser, testcontext, testHeadersOnly);

                // assert
                mockDependents.RemindersListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), false), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContext_CallsMandatoryItemsListLogicReadList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testHeadersOnly = false;

                // act
                var results = testunit.ReadUserList(fakeUser, testcontext, testHeadersOnly);

                // assert
                mockDependents.MandatoryItemsListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContext_CallsRecommendedItemsListLogicReadList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testHeadersOnly = false;

                // act
                var results = testunit.ReadUserList(fakeUser, testcontext, testHeadersOnly);

                // assert
                mockDependents.RecommendedItemsListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }
        }
        #endregion

        #region ReadLabels
        public class ReadLabels {
            [Fact]
            public void AnyUserAnyContext_CallsCustomListLogicReadLists() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };

                // act
                var results = testunit.ReadLabels(fakeUser, testcontext);

                // assert
                mockDependents.CustomListLogic.Verify(m => m.ReadLists(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContext_CallsFavoritesListLogicGetFavoritesList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };

                // act
                var results = testunit.ReadLabels(fakeUser, testcontext);

                // assert
                mockDependents.FavoritesListLogic.Verify(m => m.GetFavoritesList(It.IsAny<Guid>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }
        }
        #endregion

        #region ReadList
        public class ReadList {
            [Fact]
            public void AnyUserAnyContextWithTypeCustom_CallsCustomListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Custom;
                var testId = (long) 0;

                // act
                var results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                mockDependents.CustomListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeFavorites_CallsFavoritesListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Favorite;
                var testId = (long) 0;

                // act
                var results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                mockDependents.FavoritesListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeContract_CallsContractListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Contract;
                var testId = (long) 0;

                // act
                var results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                mockDependents.ContractListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedItemWithExpectedCategory() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: true, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var expectedItemNumber = "123456";
                var expectedCategory = "Fake Category";
                var testListType = ListType.Contract;
                var testId = (long) 0;

                // act
                var results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                results.Items.First()
                       .Category
                       .Should()
                       .Be(expectedCategory);
            }

            [Fact]
            public void AnyUserAnyContextWithTypeNotes_CallsNotesListLogicGetList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var testListType = ListType.Notes;
                var testId = (long) 0;

                // act
                var results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                mockDependents.NotesListLogic.Verify(m => m.GetList(It.IsAny<UserSelectedContext>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeWorksheet_CallsHistoryListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Worksheet;
                var testId = (long) 0;

                // act
                var results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                mockDependents.HistoryListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeReminder_CallsRemindersListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Reminder;
                var testId = (long) 0;

                // act
                var results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                mockDependents.RemindersListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeMandatory_CallsMandatoryItemsListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Mandatory;
                var testId = (long) 0;

                // act
                var results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                mockDependents.MandatoryItemsListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeReminder_CallsRecommendedItemsListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.RecommendedItems;
                var testId = (long) 0;

                // act
                var results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                mockDependents.RecommendedItemsListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeInventoryValuation_CallsInventoryValuationListLogicReadList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.InventoryValuation;
                var testId = (long) 0;

                // act
                var results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                mockDependents.InventoryValuationListLogic.Verify(m => m.ReadList(It.IsAny<long>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeRecentlyViewed_CallsRecentlyViewedListLogicReadList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.RecentlyViewed;
                var testId = (long) 0;

                // act
                var results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                mockDependents.RecentlyViewedListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }
        }
        #endregion

        #region ReadPagedList
        public class ReadPagedList {
            [Fact]
            public void AnyUserAnyContextWithTypeCustom_CallsCustomListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Custom;
                var testId = (long) 0;
                var testPaging = new PagingModel() { };

                // act
                var results = testunit.ReadPagedList(fakeUser, testcontext, testListType, testId, testPaging);

                // assert
                mockDependents.CustomListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeFavorites_CallsFavoritesListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Favorite;
                var testId = (long) 0;
                var testPaging = new PagingModel() { };

                // act
                var results = testunit.ReadPagedList(fakeUser, testcontext, testListType, testId, testPaging);

                // assert
                mockDependents.FavoritesListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeContract_CallsContractListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Contract;
                var testId = (long) 0;
                var testPaging = new PagingModel() { };

                // act
                var results = testunit.ReadPagedList(fakeUser, testcontext, testListType, testId, testPaging);

                // assert
                mockDependents.ContractListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(),
                                                                            It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.AtLeastOnce, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeNotes_CallsNotesListLogicGetList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Notes;
                var testId = (long) 0;
                var testPaging = new PagingModel() { };

                // act
                var results = testunit.ReadPagedList(fakeUser, testcontext, testListType, testId, testPaging);

                // assert
                mockDependents.NotesListLogic.Verify(m => m.GetList(It.IsAny<UserSelectedContext>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeWorksheet_CallsHistoryListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Worksheet;
                var testId = (long) 0;
                var testPaging = new PagingModel() { };

                // act
                var results = testunit.ReadPagedList(fakeUser, testcontext, testListType, testId, testPaging);

                // assert
                mockDependents.HistoryListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeReminder_CallsRemindersListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Reminder;
                var testId = (long) 0;
                var testPaging = new PagingModel() { };

                // act
                var results = testunit.ReadPagedList(fakeUser, testcontext, testListType, testId, testPaging);

                // assert
                mockDependents.RemindersListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeMandatory_CallsMandatoryItemsListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Mandatory;
                var testId = (long) 0;
                var testPaging = new PagingModel() { };

                // act
                var results = testunit.ReadPagedList(fakeUser, testcontext, testListType, testId, testPaging);

                // assert
                mockDependents.MandatoryItemsListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeReminder_CallsRecommendedItemsListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.RecommendedItems;
                var testId = (long) 0;
                var testPaging = new PagingModel() { };

                // act
                var results = testunit.ReadPagedList(fakeUser, testcontext, testListType, testId, testPaging);

                // assert
                mockDependents.RecommendedItemsListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeInventoryValuation_CallsInventoryValuationListLogicReadList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.InventoryValuation;
                var testId = (long) 0;
                var testPaging = new PagingModel() { };

                // act
                var results = testunit.ReadPagedList(fakeUser, testcontext, testListType, testId, testPaging);

                // assert
                mockDependents.InventoryValuationListLogic.Verify(m => m.ReadList(It.IsAny<long>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeRecentlyViewed_CallsRecentlyViewedListLogicReadList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.RecentlyViewed;
                var testId = (long) 0;
                var testPaging = new PagingModel() { };

                // act
                var results = testunit.ReadPagedList(fakeUser, testcontext, testListType, testId, testPaging);

                // assert
                mockDependents.RecentlyViewedListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }
        }
        #endregion

        #region ReadRecent
        public class ReadRecent {
            [Fact]
            public void AnyUserAnyContext_CallsRecentlyViewedListLogicReadList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };

                // act
                var results = testunit.ReadRecent(fakeUser, testcontext);

                // assert
                mockDependents.RecentlyViewedListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }
        }
        #endregion

        #region ReadRecentOrder
        public class ReadRecentOrder {
            [Fact]
            public void AnyUserAnyContext_CallsRecentlyOrderedListLogicReadList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testCatalog = "UNFI";

                // act
                var results = testunit.ReadRecentOrder(fakeUser, testcontext, testCatalog);

                // assert
                mockDependents.RecentlyOrderedListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }
        }
        #endregion

        #region UpdateList
        public class UpdateList {
            [Fact]
            public void AnyUserAnyContextWithTypeCustom_CallsCustomListLogicSaveList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Custom;
                var testModel = new ListModel() {
                    ListId = 17,
                    Items = new List<ListItemModel>() {
                        new ListItemModel() {
                            ItemNumber = "123456"
                        },
                        new ListItemModel() {
                            ItemNumber = "234567"
                        }
                    }
                };

                // act
                testunit.UpdateList(fakeUser, testcontext, testListType, testModel);

                // assert
                mockDependents.CustomListLogic.Verify(m => m.SaveList(fakeUser, testcontext, testModel), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeFavorite_CallsFavoritesListLogicSaveList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Favorite;
                var testModel = new ListModel() {
                    ListId = 17,
                    Items = new List<ListItemModel>() {
                        new ListItemModel() {
                            ItemNumber = "123456"
                        },
                        new ListItemModel() {
                            ItemNumber = "234567"
                        }
                    }
                };

                // act
                testunit.UpdateList(fakeUser, testcontext, testListType, testModel);

                // assert
                mockDependents.FavoritesListLogic.Verify(m => m.SaveList(fakeUser, testcontext, testModel), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeReminder_CallsRemindersListLogicSaveList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Reminder;
                var testModel = new ListModel() {
                    ListId = 17,
                    Items = new List<ListItemModel>() {
                        new ListItemModel() {
                            ItemNumber = "123456"
                        },
                        new ListItemModel() {
                            ItemNumber = "234567"
                        }
                    }
                };

                // act
                testunit.UpdateList(fakeUser, testcontext, testListType, testModel);

                // assert
                mockDependents.RemindersListLogic.Verify(m => m.SaveList(fakeUser, testcontext, testModel), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeMandatory_CallsMandatoryItemsListLogicSaveList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Mandatory;
                var testModel = new ListModel() {
                    ListId = 17,
                    Items = new List<ListItemModel>() {
                        new ListItemModel() {
                            ItemNumber = "123456"
                        },
                        new ListItemModel() {
                            ItemNumber = "234567"
                        }
                    }
                };

                // act
                testunit.UpdateList(fakeUser, testcontext, testListType, testModel);

                // assert
                mockDependents.MandatoryItemsListLogic.Verify(m => m.SaveList(fakeUser, testcontext, testModel), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeInventoryValuation_CallsInventoryValuationListLogicSaveList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.InventoryValuation;
                var testModel = new ListModel() {
                    ListId = 17,
                    Items = new List<ListItemModel>() {
                        new ListItemModel() {
                            ItemNumber = "123456"
                        },
                        new ListItemModel() {
                            ItemNumber = "234567"
                        }
                    }
                };

                // act
                testunit.UpdateList(fakeUser, testcontext, testListType, testModel);

                // assert
                mockDependents.InventoryValuationListLogic.Verify(m => m.SaveList(fakeUser, testcontext, testModel), Times.Once, "not called");
            }
        }
        #endregion

        #region SaveItem
        public class SaveItem {
            [Fact]
            public void AnyUserAnyContextWithTypeFavorites_CallsFavoritesListLogicSave() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Favorite;
                var testHeaderId = (long) 1;
                var testListItem = new ListItemModel() {
                    ItemNumber = "123456",
                    CatalogId = "BEK"
                };

                // act
                testunit.SaveItem(fakeUser, testcontext, testListType, testHeaderId, testListItem);

                // assert
                mockDependents.FavoritesListLogic.Verify(m => m.Save(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<FavoritesListDetail>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeNotes_CallsNotesListLogicSaveNote() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Notes;
                var testHeaderId = (long) 1;
                var testListItem = new ListItemModel() {
                    ItemNumber = "123456",
                    CatalogId = "BEK"
                };

                // act
                testunit.SaveItem(fakeUser, testcontext, testListType, testHeaderId, testListItem);

                // assert
                mockDependents.NotesListLogic.Verify(m => m.SaveNote(It.IsAny<UserSelectedContext>(), It.IsAny<ListItemModel>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeReminder_CallsRemindersListLogicSave() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Reminder;
                var testHeaderId = (long) 1;
                var testListItem = new ListItemModel() {
                    ItemNumber = "123456",
                    CatalogId = "BEK"
                };

                // act
                testunit.SaveItem(fakeUser, testcontext, testListType, testHeaderId, testListItem);

                // assert
                mockDependents.RemindersListLogic.Verify(m => m.Save(It.IsAny<UserSelectedContext>(), It.IsAny<ReminderItemsListDetail>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeMandatory_CallsMandatoryItemsListLogicSaveDetail() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Mandatory;
                var testHeaderId = (long) 1;
                var testListItem = new ListItemModel() {
                    ItemNumber = "123456",
                    CatalogId = "BEK"
                };

                // act
                testunit.SaveItem(fakeUser, testcontext, testListType, testHeaderId, testListItem);

                // assert
                mockDependents.MandatoryItemsListLogic.Verify(m => m.SaveDetail(It.IsAny<UserSelectedContext>(), It.IsAny<MandatoryItemsListDetail>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeRecommendedItems_CallsRecommendedItemsListLogicSaveDetail() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.RecommendedItems;
                var testHeaderId = (long) 1;
                var testListItem = new ListItemModel() {
                    ItemNumber = "123456",
                    CatalogId = "BEK"
                };

                // act
                testunit.SaveItem(fakeUser, testcontext, testListType, testHeaderId, testListItem);

                // assert
                mockDependents.RecommendedItemsListLogic.Verify(m => m.SaveDetail(It.IsAny<UserSelectedContext>(), It.IsAny<RecommendedItemsListDetail>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeInventoryValuation_CallsInventoryValuationListLogicSaveItem() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.InventoryValuation;
                var testHeaderId = (long) 1;
                var testListItem = new ListItemModel() {
                    ItemNumber = "123456",
                    CatalogId = "BEK"
                };

                // act
                testunit.SaveItem(fakeUser, testcontext, testListType, testHeaderId, testListItem);

                // assert
                mockDependents.InventoryValuationListLogic.Verify(m => m.SaveItem(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>(), It.IsAny<InventoryValuationListDetail>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeCustom_CallsCustomListLogicSaveItem() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Custom;
                var testHeaderId = (long) 1;
                var testListItem = new ListItemModel() {
                    ItemNumber = "123456",
                    CatalogId = "BEK"
                };

                // act
                testunit.SaveItem(fakeUser, testcontext, testListType, testHeaderId, testListItem);

                // assert
                mockDependents.CustomListLogic.Verify(m => m.SaveItem(It.IsAny<CustomListDetail>()), Times.Once, "not called");
            }

            [Fact]
            public void TryToAddItemWithPosition0_ExpectedToChangePosition() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: true, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Custom;
                var testHeaderId = (long) 1;
                var testListItem = new ListItemModel() {
                    ItemNumber = "123456",
                    CatalogId = "BEK"
                };
                var expected = 1;

                // act
                testunit.SaveItem(fakeUser, testcontext, testListType, testHeaderId, testListItem);

                // assert
                testListItem.Position.Should()
                            .Be(expected);
            }

            [Fact]
            public void TryToAddItemWithPosition1_ExpectedSamePosition() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: true, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Custom;
                var testHeaderId = (long) 1;
                var testListItem = new ListItemModel() {
                    ItemNumber = "123456",
                    CatalogId = "BEK",
                    Position = 1
                };
                var expected = 1;

                // act
                testunit.SaveItem(fakeUser, testcontext, testListType, testHeaderId, testListItem);

                // assert
                testListItem.Position.Should()
                            .Be(expected);
            }
        }
        #endregion

        #region SaveItems
        public class SaveItems {
            [Fact]
            public void AnyUserAnyContextWithTypeFavorites_CallsFavoritesListLogicSave() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Favorite;
                var testHeaderId = (long) 1;
                var testListItems = new List<ListItemModel>() {
                    new ListItemModel() {
                        ItemNumber = "123456",
                        CatalogId = "BEK"
                    },
                    new ListItemModel() {
                        ItemNumber = "234567",
                        CatalogId = "BEK"
                    }
                };

                // act
                testunit.SaveItems(fakeUser, testcontext, testListType, testHeaderId, testListItems);

                // assert
                mockDependents.FavoritesListLogic.Verify(m => m.Save(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<FavoritesListDetail>()),
                                                         Times.AtLeastOnce, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeNotes_CallsNotesListLogicSaveNote() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Notes;
                var testHeaderId = (long) 1;
                var testListItems = new List<ListItemModel>() {
                    new ListItemModel() {
                        ItemNumber = "123456",
                        CatalogId = "BEK"
                    },
                    new ListItemModel() {
                        ItemNumber = "234567",
                        CatalogId = "BEK"
                    }
                };

                // act
                testunit.SaveItems(fakeUser, testcontext, testListType, testHeaderId, testListItems);

                // assert
                mockDependents.NotesListLogic.Verify(m => m.SaveNote(It.IsAny<UserSelectedContext>(), It.IsAny<ListItemModel>()), Times.AtLeastOnce, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeReminder_CallsRemindersListLogicSave() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Reminder;
                var testHeaderId = (long) 1;
                var testListItems = new List<ListItemModel>() {
                    new ListItemModel() {
                        ItemNumber = "123456",
                        CatalogId = "BEK"
                    },
                    new ListItemModel() {
                        ItemNumber = "234567",
                        CatalogId = "BEK"
                    }
                };

                // act
                testunit.SaveItems(fakeUser, testcontext, testListType, testHeaderId, testListItems);

                // assert
                mockDependents.RemindersListLogic.Verify(m => m.Save(It.IsAny<UserSelectedContext>(), It.IsAny<ReminderItemsListDetail>()), Times.AtLeastOnce, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeMandatory_CallsMandatoryItemsListLogicSaveDetail() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Mandatory;
                var testHeaderId = (long) 1;
                var testListItems = new List<ListItemModel>() {
                    new ListItemModel() {
                        ItemNumber = "123456",
                        CatalogId = "BEK"
                    },
                    new ListItemModel() {
                        ItemNumber = "234567",
                        CatalogId = "BEK"
                    }
                };

                // act
                testunit.SaveItems(fakeUser, testcontext, testListType, testHeaderId, testListItems);

                // assert
                mockDependents.MandatoryItemsListLogic.Verify(m => m.SaveDetail(It.IsAny<UserSelectedContext>(), It.IsAny<MandatoryItemsListDetail>()), Times.AtLeastOnce, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeRecommendedItems_CallsRecommendedItemsListLogiSaveDetailc() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.RecommendedItems;
                var testHeaderId = (long) 1;
                var testListItems = new List<ListItemModel>() {
                    new ListItemModel() {
                        ItemNumber = "123456",
                        CatalogId = "BEK"
                    },
                    new ListItemModel() {
                        ItemNumber = "234567",
                        CatalogId = "BEK"
                    }
                };

                // act
                testunit.SaveItems(fakeUser, testcontext, testListType, testHeaderId, testListItems);

                // assert
                mockDependents.RecommendedItemsListLogic.Verify(m => m.SaveDetail(It.IsAny<UserSelectedContext>(), It.IsAny<RecommendedItemsListDetail>()), Times.AtLeastOnce, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeInventoryValuation_CallsInventoryValuationListLogicSaveItem() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.InventoryValuation;
                var testHeaderId = (long) 1;
                var testListItems = new List<ListItemModel>() {
                    new ListItemModel() {
                        ItemNumber = "123456",
                        CatalogId = "BEK"
                    },
                    new ListItemModel() {
                        ItemNumber = "234567",
                        CatalogId = "BEK"
                    }
                };

                // act
                testunit.SaveItems(fakeUser, testcontext, testListType, testHeaderId, testListItems);

                // assert
                mockDependents.InventoryValuationListLogic.Verify(m => m.SaveItem(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>(), It.IsAny<InventoryValuationListDetail>()), Times.AtLeastOnce, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeCustom_CallsCustomListLogicSaveItem() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Custom;
                var testHeaderId = (long) 1;
                var testListItems = new List<ListItemModel>() {
                    new ListItemModel() {
                        ItemNumber = "123456",
                        CatalogId = "BEK"
                    },
                    new ListItemModel() {
                        ItemNumber = "234567",
                        CatalogId = "BEK"
                    }
                };

                // act
                testunit.SaveItems(fakeUser, testcontext, testListType, testHeaderId, testListItems);

                // assert
                mockDependents.CustomListLogic.Verify(m => m.SaveItem(It.IsAny<CustomListDetail>()), Times.AtLeastOnce, "not called");
            }

            [Fact]
            public void TryToAddItemWithPosition0_ExpectedToChangePosition() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: true, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Custom;
                var testHeaderId = (long) 1;
                var testListItems = new List<ListItemModel>() {
                    new ListItemModel() {
                        ItemNumber = "123456",
                        CatalogId = "BEK"
                    }
                };
                var expected = 1;

                // act
                testunit.SaveItems(fakeUser, testcontext, testListType, testHeaderId, testListItems);

                // assert
                testListItems.First()
                             .Position.Should()
                             .Be(expected);
            }

            [Fact]
            public void TryToAddItemWithPosition1_ExpectedSamePosition() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: true, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Custom;
                var testHeaderId = (long) 1;
                var testListItems = new List<ListItemModel>() {
                    new ListItemModel() {
                        ItemNumber = "123456",
                        CatalogId = "BEK",
                        Position = 1
                    }
                };
                var expected = 1;

                // act
                testunit.SaveItems(fakeUser, testcontext, testListType, testHeaderId, testListItems);

                // assert
                testListItems.First()
                             .Position.Should()
                             .Be(expected);
            }
        }
        #endregion

        #region CreateList
        public class CreateList {
            [Fact]
            public void AnyUserAnyContextWithTypeCustom_CallsCustomListLogicCreateOrUpdateList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Custom;
                var testModel = new ListModel() {
                    ListId = 17,
                    Name = "Fake Name",
                    Items = new List<ListItemModel>() {
                        new ListItemModel() {
                            ItemNumber = "123456"
                        },
                        new ListItemModel() {
                            ItemNumber = "234567"
                        }
                    }
                };

                // act
                testunit.CreateList(fakeUser, testcontext, testListType, testModel);

                // assert
                mockDependents.CustomListLogic.Verify(m => m.CreateOrUpdateList(fakeUser, testcontext, 0, testModel.Name, It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeReminder_CallsRemindersListLogicSaveList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Reminder;
                var testModel = new ListModel() {
                    ListId = 17,
                    Items = new List<ListItemModel>() {
                        new ListItemModel() {
                            ItemNumber = "123456"
                        },
                        new ListItemModel() {
                            ItemNumber = "234567"
                        }
                    }
                };

                // act
                testunit.CreateList(fakeUser, testcontext, testListType, testModel);

                // assert
                mockDependents.RemindersListLogic.Verify(m => m.SaveList(fakeUser, testcontext, testModel), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeFavorite_CallsFavoritesListLogicCreateList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Favorite;
                var testModel = new ListModel() {
                    Items = new List<ListItemModel>() { }
                };

                // act
                testunit.CreateList(fakeUser, testcontext, testListType, testModel);

                // assert
                mockDependents.FavoritesListLogic.Verify(m => m.CreateList(fakeUser, testcontext), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeMandatory_CallsMandatoryItemsListLogicSaveList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Mandatory;
                var testModel = new ListModel() {
                    ListId = 17,
                    Items = new List<ListItemModel>() {
                        new ListItemModel() {
                            ItemNumber = "123456"
                        },
                        new ListItemModel() {
                            ItemNumber = "234567"
                        }
                    }
                };

                // act
                testunit.CreateList(fakeUser, testcontext, testListType, testModel);

                // assert
                mockDependents.MandatoryItemsListLogic.Verify(m => m.CreateList(fakeUser, testcontext), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeInventoryValuation_CallsInventoryValuationListLogicCreateOrUpdateList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.InventoryValuation;
                var testModel = new ListModel() {
                    ListId = 17,
                    Items = new List<ListItemModel>() {
                        new ListItemModel() {
                            ItemNumber = "123456"
                        },
                        new ListItemModel() {
                            ItemNumber = "234567"
                        }
                    }
                };

                // act
                testunit.CreateList(fakeUser, testcontext, testListType, testModel);

                // assert
                mockDependents.InventoryValuationListLogic.Verify(m => m.CreateOrUpdateList(fakeUser, testcontext, 0, testModel.Name, It.IsAny<bool>()), Times.Once, "not called");
            }
        }
        #endregion

        #region CopyList
        public class CopyList {
            [Fact]
            public void AnyUserAnyContext_CallsCustomListLogicGetListModel() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var copyListModel = new ListCopyShareModel() {
                    ListId = 1,
                    Customers = new List<Customer>() {
                        new Customer() {
                            CustomerBranch = "FUT",
                            CustomerNumber = "234567"
                        }
                    }
                };

                // act
                testunit.CopyList(fakeUser, testcontext, copyListModel);

                // assert
                mockDependents.CustomListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Exactly(4), "not called");
            }
        }
        #endregion

        #region DeleteList
        public class DeleteList {
            [Fact]
            public void AnyUserAnyContextWithTypeCustom_CallsCustomListLogicDeleteList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.Custom;
                var testModel = new ListModel() {
                    ListId = 17,
                    Items = new List<ListItemModel>() {
                        new ListItemModel() {
                            ItemNumber = "123456"
                        },
                        new ListItemModel() {
                            ItemNumber = "234567"
                        }
                    }
                };

                // act
                testunit.DeleteList(fakeUser, testcontext, testListType, testModel);

                // assert
                mockDependents.CustomListLogic.Verify(m => m.DeleteList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<ListModel>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeInventoryValuation_CallsInventoryValuationListLogicDeleteList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testListType = ListType.InventoryValuation;
                var testModel = new ListModel() {
                    ListId = 17,
                    Items = new List<ListItemModel>() {
                        new ListItemModel() {
                            ItemNumber = "123456"
                        },
                        new ListItemModel() {
                            ItemNumber = "234567"
                        }
                    }
                };

                // act
                testunit.DeleteList(fakeUser, testcontext, testListType, testModel);

                // assert
                mockDependents.InventoryValuationListLogic.Verify(m => m.CreateOrUpdateList(It.IsAny<UserProfile>(),
                                                                                            It.IsAny<UserSelectedContext>(),
                                                                                            It.IsAny<long>(),
                                                                                            It.IsAny<string>(),
                                                                                            false), Times.Once, "not called");
            }
        }
        #endregion

        #region ReadRecommendedItemsList
        public class ReadRecommendedItemsList {
            [Fact]
            public void AnyUserAnyContext_CallsRecommendedItemsListLogicReadList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var testcontext = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

                // act
                testunit.ReadRecommendedItemsList(testcontext);

                // assert
                mockDependents.RecommendedItemsListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()),
                                                                Times.Once, "not called");
            }
        }
        #endregion

        #region MarkFavoritesAndAddNotes_ReturnsListModel
        public class MarkFavoritesAndAddNotes_ReturnsListModel {
            [Fact]
            public void AnyUserAnyContext_CallsNotesListLogicGetList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testModel = new ListModel() {
                    ListId = 17,
                    Items = new List<ListItemModel>() {
                        new ListItemModel() {
                            ItemNumber = "123456"
                        },
                        new ListItemModel() {
                            ItemNumber = "234567"
                        }
                    }
                };

                // act
                var list = testunit.MarkFavoritesAndAddNotes(fakeUser, testModel, testcontext);

                // assert
                mockDependents.NotesListLogic.Verify(m => m.GetList(It.IsAny<UserSelectedContext>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContext_CallsFavoritesListLogicGetFavoritesList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testModel = new ListModel() {
                    ListId = 17,
                    Items = new List<ListItemModel>() {
                        new ListItemModel() {
                            ItemNumber = "123456"
                        },
                        new ListItemModel() {
                            ItemNumber = "234567"
                        }
                    }
                };

                // act
                var list = testunit.MarkFavoritesAndAddNotes(fakeUser, testModel, testcontext);

                // assert
                mockDependents.FavoritesListLogic.Verify(m => m.GetFavoritesList(It.IsAny<Guid>(),
                                                                                 It.IsAny<UserSelectedContext>(),
                                                                                 false), Times.Once, "not called");
            }
        }
        #endregion

        #region MarkFavoritesAndAddNotes_ReturnsListOfProduct
        public class MarkFavoritesAndAddNotes_ReturnsListOfProduct {
            [Fact]
            public void AnyUserAnyContext_CallsNotesListLogicGetList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testModel = new List<Product> {
                    new Product() {
                        ItemNumber = "123456"
                    },
                    new Product() {
                        ItemNumber = "234567"
                    }
                };

                // act
                var list = testunit.MarkFavoritesAndAddNotes(fakeUser, testModel, testcontext);

                // assert
                mockDependents.NotesListLogic.Verify(m => m.GetList(It.IsAny<UserSelectedContext>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContext_CallsFavoritesListLogicGetFavoritesList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testModel = new List<Product> {
                    new Product() {
                        ItemNumber = "123456"
                    },
                    new Product() {
                        ItemNumber = "234567"
                    }
                };

                // act
                var list = testunit.MarkFavoritesAndAddNotes(fakeUser, testModel, testcontext);

                // assert
                mockDependents.FavoritesListLogic.Verify(m => m.GetFavoritesList(It.IsAny<Guid>(),
                                                                                 It.IsAny<UserSelectedContext>(),
                                                                                 false), Times.Once, "not called");
            }
        }
        #endregion

        #region GetBarcodeForList
        public class GetBarcodeForList {
            [Fact]
            public void AnyUserAnyContext_CallsItemBarcodeRepositoryLogicGetBarcodeForList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var testuser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var testListType = ListType.Custom;
                var testId = (long) 1;

                // act
                testunit.GetBarcodeForList(testuser, testcontext, testListType, testId);

                // assert
                mockDependents.ItemBarcodeRepository.Verify(m => m.GetBarcodeForList(It.IsAny<ListModel>()), Times.Once, "not called");
            }
        }
        #endregion

        #region AddCustomInventory
        public class AddCustomInventory {
            [Fact]
            public void AnyUserAnyContext_CallsCustomInventoryItemsRepositoryGet() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var testuser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var testListType = ListType.Custom;
                var testId = (long) 1;
                var testCustomInventoryId = (long) 1;

                // act
                testunit.AddCustomInventory(testuser, testcontext, testListType, testId, testCustomInventoryId);

                // assert
                mockDependents.CustomInventoryItemsRepository.Verify(m => m.Get(It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContext_CallsCustomListLogicSaveList() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var testuser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var testListType = ListType.Custom;
                var testId = (long) 1;
                var testCustomInventoryId = (long) 1;

                // act
                testunit.AddCustomInventory(testuser, testcontext, testListType, testId, testCustomInventoryId);

                // assert
                mockDependents.CustomListLogic.Verify(m => m.SaveList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<ListModel>()), Times.Once, "not called");
            }
        }
        #endregion

        #region AddCustomInventoryItems
        public class AddCustomInventoryItems {
            [Fact]
            public void AnyUserAnyContext_CallsCustomInventoryItemsRepositoryGetItemsByItemIds() {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsService(useAutoFac: false, mockDependents: ref mockDependents);
                var testuser = new UserProfile();
                var testcontext = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var testListType = ListType.Custom;
                var testId = (long) 1;
                var testCustomInventoryId = new List<long>() {(long) 1};

                // act
                testunit.AddCustomInventoryItems(testuser, testcontext, testListType, testId, testCustomInventoryId);

                // assert
                mockDependents.CustomInventoryItemsRepository.Verify(m => m.GetItemsByItemIds(It.IsAny<List<long>>()), Times.Once, "not called");
            }
        }
        #endregion

        #region DeleteItem
        public class DeleteItem {
            [Fact]
            public void GoodItemNumber_DeletesTheSpecifiedFavoriteItem() {
                // arrange
                var mockDependents = new MockDependents();
                var testUnit = MakeTestsService(false, ref mockDependents);
                var fakeUser = new UserProfile {
                    UserId = new Guid("b514a6c2-6f07-48f1-a26c-650f28337b01")
                };
                var fakeCustomer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeType = ListType.Favorite;
                var fakeId = 1;
                var fakeItemNumber = "234567";

                // act
                testUnit.DeleteItem(fakeUser, fakeCustomer, fakeType, fakeId, fakeItemNumber);

                // assert
                mockDependents.FavoritesListLogic.Verify(f => f.Save(It.IsAny<UserProfile>(),
                                                                     It.IsAny<UserSelectedContext>(),
                                                                     It.Is<FavoritesListDetail>(d => d.ItemNumber == fakeItemNumber)),
                                                         Times.Once);
            }

            [Fact]
            public void BadItemNumber_DoesNotCallSaveItem() {
                // arrange
                var mockDependents = new MockDependents();
                var testUnit = MakeTestsService(false, ref mockDependents);
                var fakeUser = new UserProfile {
                    UserId = new Guid("b514a6c2-6f07-48f1-a26c-650f28337b01")
                };
                var fakeCustomer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeType = ListType.Favorite;
                var fakeId = 1;
                var fakeItemNumber = "999999";

                // act
                testUnit.DeleteItem(fakeUser, fakeCustomer, fakeType, fakeId, fakeItemNumber);

                // assert
                mockDependents.FavoritesListLogic.Verify(f => f.Save(It.IsAny<UserProfile>(),
                                                                     It.IsAny<UserSelectedContext>(),
                                                                     It.IsAny<FavoritesListDetail>()),
                                                         Times.Never);
            }
        }
        #endregion

        #region DeleteItems
        public class DeleteItems {
            [Fact]
            public void GoodItemNumbers_DeletesTheSpecifiedFavoriteItems() {
                // arrange
                var mockDependents = new MockDependents();
                var testUnit = MakeTestsService(false, ref mockDependents);
                var fakeUser = new UserProfile {
                    UserId = new Guid("b514a6c2-6f07-48f1-a26c-650f28337b01")
                };
                var fakeCustomer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeType = ListType.Favorite;
                var fakeId = 1;
                var fakeItemNumbers = new List<string> {"234567", "345678"};

                // act
                testUnit.DeleteItems(fakeUser, fakeCustomer, fakeType, fakeId, fakeItemNumbers);

                // assert
                mockDependents.FavoritesListLogic.Verify(f => f.Save(It.IsAny<UserProfile>(),
                                                                     It.IsAny<UserSelectedContext>(),
                                                                     It.Is<FavoritesListDetail>(d => fakeItemNumbers.Contains(d.ItemNumber))),
                                                         Times.Exactly(2));
            }

            [Fact]
            public void EmptyItemNumberList_DoesNotCallTheSaveMethod() {
                // arrange
                var mockDependents = new MockDependents();
                var testUnit = MakeTestsService(false, ref mockDependents);
                var fakeUser = new UserProfile {
                    UserId = new Guid("b514a6c2-6f07-48f1-a26c-650f28337b01")
                };
                var fakeCustomer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeType = ListType.Favorite;
                var fakeId = 1;
                var fakeItemNumbers = new List<string>();

                // act
                testUnit.DeleteItems(fakeUser, fakeCustomer, fakeType, fakeId, fakeItemNumbers);

                // assert
                mockDependents.FavoritesListLogic.Verify(f => f.Save(It.IsAny<UserProfile>(),
                                                                     It.IsAny<UserSelectedContext>(),
                                                                     It.IsAny<FavoritesListDetail>()),
                                                         Times.Never);
            }
        }
        #endregion
    }
}