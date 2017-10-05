using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Autofac;

using FluentAssertions;

using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.Reports;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Configuration.EF;
using KeithLink.Svc.Core.Models.Customers.EF;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.CustomList;
using KeithLink.Svc.Core.Models.Lists.Favorites;
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;
using KeithLink.Svc.Core.Models.Lists.MandatoryItem;
using KeithLink.Svc.Core.Models.Lists.RecommendedItems;
using KeithLink.Svc.Core.Models.Lists.ReminderItems;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Service.List;

using Moq;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Service.List {
    public class ListServiceTests : BaseDITests {
        #region GetContractInformation
        public class GetContractInformation {
            [Fact]
            public void BadBranchId_ReturnsEmptyList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(true, ref mockDependents);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                int emptyList = 0;
                // act
                Dictionary<string, string> results = testunit.GetContractInformation(testcontext);

                // assert
                results.Count
                       .Should()
                       .Be(emptyList);
            }

            [Fact]
            public void BadCustomerId_ReturnsEmptyList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(true, ref mockDependents);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "999999"
                };
                int emptyList = 0;
                // act
                Dictionary<string, string> results = testunit.GetContractInformation(testcontext);

                // assert
                results.Count
                       .Should()
                       .Be(emptyList);
            }

            [Fact]
            public void GoodCustomer_CallsContractListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                string expectedItemNumber = "123456";
                string expectedCategory = "Fake Category";

                // act
                Dictionary<string, string> results = testunit.GetContractInformation(testcontext);

                // assert
                mockDependents.ContractListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(),
                                                                            It.IsAny<UserSelectedContext>(),
                                                                            It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedItem() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(true, ref mockDependents);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                string expectedItemNumber = "123456";

                // act
                Dictionary<string, string> results = testunit.GetContractInformation(testcontext);

                // assert
                results.First()
                       .Key
                       .Should()
                       .Be(expectedItemNumber);
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedItemWithExpectedCategory() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(true, ref mockDependents);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                string expectedItemNumber = "123456";
                string expectedCategory = "Fake Category";

                // act
                Dictionary<string, string> results = testunit.GetContractInformation(testcontext);

                // assert
                results[expectedItemNumber]
                        .Should()
                        .Be(expectedCategory);
            }
        }
        #endregion GetContractInformation

        #region GetItemsHistoryList
        public class GetItemsHistoryList {
            [Fact]
            public void AnyContext_CallsItemHistoryRepositoryRead() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                List<string> testitems = new List<string> {"123456"};
                int emptyList = 0;
                // act
                ItemHistory[] results = testunit.GetItemsHistoryList(testcontext, testitems.ToArray());

                // assert
                mockDependents.ItemHistoryRepository.Verify(m => m.Read(It.IsAny<Expression<Func<ItemHistory, bool>>>()), Times.Once, "not called");
            }
        }
        #endregion GetItemsHistoryList

        #region GetNotesHash
        public class GetNotesHash {
            [Fact]
            public void AnyContext_CallsNotesGetList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);

                UserSelectedContext testContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

                // act
                Dictionary<string, ListItemModel> results = testunit.GetNotesHash(testContext);

                // assert
                mockDependents.NotesListLogic.Verify(m => m.GetList(It.IsAny<UserSelectedContext>()), Times.Once, "not called");
            }
        }
        #endregion GetNotesHash

        #region GetFavoritesHash
        public class GetFavoritesHash {
            [Fact]
            public void AnyContext_CallsFavoritesGetList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);

                UserSelectedContext testContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

                UserProfile testProfile = new UserProfile {
                    UserId = new Guid()
                };

                // act
                Dictionary<string, ListItemModel> results = testunit.GetFavoritesHash(testProfile, testContext);

                // assert
                mockDependents.FavoritesListLogic.Verify(m => m.GetFavoritesList(
                                                                                 It.Is<Guid>(g => g.Equals(testProfile.UserId)),
                                                                                 It.Is<UserSelectedContext>(
                                                                                                            x => x.BranchId.Equals(testContext.BranchId) &&
                                                                                                                 x.CustomerId.Equals(testContext.CustomerId)),
                                                                                 false),
                                                         Times.Once, "not called");
            }
        }
        #endregion GetFavoritesHash

        #region ReadListByType
        public class ReadListByType {
            [Fact]
            public void AnyUserAnyContextWithTypeContract_CallsContractListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Contract;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, testListType, 1);

                // assert
                mockDependents.ContractListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(),
                                                                            It.IsAny<UserSelectedContext>(),
                                                                            It.IsAny<long>()),
                                                        Times.Exactly(2),
                                                        "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeCustom_CallsCustomListLogicReadLists() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Custom;

                // act
                List<ListModel> results = testunit.ReadListByType(fakeUser, testcontext, testListType);

                // assert
                mockDependents.CustomListLogic.Verify(m => m.ReadLists(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeFavorites_CallsFavoritesListLogicGetFavoritesList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Favorite;

                // act
                List<ListModel> results = testunit.ReadListByType(fakeUser, testcontext, testListType);

                // assert
                mockDependents.FavoritesListLogic.Verify(m => m.GetFavoritesList(It.IsAny<Guid>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeInventoryValuation_CallsInventoryValuationListLogicReadLists() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.InventoryValuation;

                // act
                List<ListModel> results = testunit.ReadListByType(fakeUser, testcontext, testListType);

                // assert
                mockDependents.InventoryValuationListLogic.Verify(m => m.ReadLists(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeMandatory_CallsMandatoryItemsListLogicReadList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Mandatory;

                // act
                List<ListModel> results = testunit.ReadListByType(fakeUser, testcontext, testListType);

                // assert
                mockDependents.MandatoryItemsListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeNotes_CallsNotesListLogicGetList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Custom;

                // act
                List<ListModel> results = testunit.ReadListByType(fakeUser, testcontext, testListType);

                // assert
                mockDependents.NotesListLogic.Verify(m => m.GetList(It.IsAny<UserSelectedContext>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeRecentlyOrdered_CallsRecentlyOrderedListLogicReadList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.RecentlyOrdered;

                // act
                List<ListModel> results = testunit.ReadListByType(fakeUser, testcontext, testListType);

                // assert
                mockDependents.RecentlyOrderedListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeRecentlyViewed_CallsRecentlyViewedListLogicReadList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.RecentlyViewed;

                // act
                List<ListModel> results = testunit.ReadListByType(fakeUser, testcontext, testListType);

                // assert
                mockDependents.RecentlyViewedListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeReminder_CallsRecommendedItemsListLogicReadList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.RecommendedItems;

                // act
                List<ListModel> results = testunit.ReadListByType(fakeUser, testcontext, testListType);

                // assert
                mockDependents.RecommendedItemsListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeReminder_CallsRemindersListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Reminder;

                // act
                List<ListModel> results = testunit.ReadListByType(fakeUser, testcontext, testListType);

                // assert
                mockDependents.RemindersListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), false), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeWorksheet_CallsHistoryListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Worksheet;

                // act
                List<ListModel> results = testunit.ReadListByType(fakeUser, testcontext, testListType);

                // assert
                mockDependents.HistoryListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), false), Times.Once, "not called");
            }
        }
        #endregion ReadListByType

        #region ReadUserList
        public class ReadUserList {
            [Fact]
            public void AnyUserAnyContext_CallsContractListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                bool testHeadersOnly = false;

                // act
                List<ListModel> results = testunit.ReadUserList(fakeUser, testcontext, testHeadersOnly);

                // assert
                mockDependents.ContractListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()),
                                                        Times.AtLeastOnce, "not called");
            }

            [Fact]
            public void AnyUserAnyContext_CallsCustomListLogicReadLists() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                bool testHeadersOnly = false;

                // act
                List<ListModel> results = testunit.ReadUserList(fakeUser, testcontext, testHeadersOnly);

                // assert
                mockDependents.CustomListLogic.Verify(m => m.ReadLists(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContext_CallsFavoritesListLogicGetFavoritesList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                bool testHeadersOnly = false;

                // act
                List<ListModel> results = testunit.ReadUserList(fakeUser, testcontext, testHeadersOnly);

                // assert
                mockDependents.FavoritesListLogic.Verify(m => m.GetFavoritesList(It.IsAny<Guid>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Exactly(3), "not called");
            }

            [Fact]
            public void AnyUserAnyContext_CallsHistoryListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                bool testHeadersOnly = false;

                // act
                List<ListModel> results = testunit.ReadUserList(fakeUser, testcontext, testHeadersOnly);

                // assert
                mockDependents.HistoryListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), testHeadersOnly), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContext_CallsMandatoryItemsListLogicReadList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                bool testHeadersOnly = false;

                // act
                List<ListModel> results = testunit.ReadUserList(fakeUser, testcontext, testHeadersOnly);

                // assert
                mockDependents.MandatoryItemsListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContext_CallsRecommendedItemsListLogicReadList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                bool testHeadersOnly = false;

                // act
                List<ListModel> results = testunit.ReadUserList(fakeUser, testcontext, testHeadersOnly);

                // assert
                mockDependents.RecommendedItemsListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContext_CallsRemindersListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                bool testHeadersOnly = false;

                // act
                List<ListModel> results = testunit.ReadUserList(fakeUser, testcontext, testHeadersOnly);

                // assert
                mockDependents.RemindersListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), false), Times.Once, "not called");
            }
        }
        #endregion ReadUserList

        #region ReadLabels
        public class ReadLabels {
            [Fact]
            public void AnyUserAnyContext_CallsCustomListLogicReadLists() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };

                // act
                List<string> results = testunit.ReadLabels(fakeUser, testcontext);

                // assert
                mockDependents.CustomListLogic.Verify(m => m.ReadLists(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContext_CallsFavoritesListLogicGetFavoritesList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };

                // act
                List<string> results = testunit.ReadLabels(fakeUser, testcontext);

                // assert
                mockDependents.FavoritesListLogic.Verify(m => m.GetFavoritesList(It.IsAny<Guid>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }
        }
        #endregion ReadLabels

        #region ReadList
        public class ReadList {
            [Fact]
            public void AnyUserAnyContextWithTypeContract_CallsContractListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Contract;
                long testId = 0;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                mockDependents.ContractListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeCustom_CallsCustomListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Custom;
                long testId = 0;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                mockDependents.CustomListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeFavorites_CallsFavoritesListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Favorite;
                long testId = 0;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                mockDependents.FavoritesListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeInventoryValuation_CallsInventoryValuationListLogicReadList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.InventoryValuation;
                long testId = 0;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                mockDependents.InventoryValuationListLogic.Verify(m => m.ReadList(It.IsAny<long>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeMandatory_CallsMandatoryItemsListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Mandatory;
                long testId = 0;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                mockDependents.MandatoryItemsListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeNotes_CallsNotesListLogicGetList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Notes;
                long testId = 0;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                mockDependents.NotesListLogic.Verify(m => m.GetList(It.IsAny<UserSelectedContext>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeRecentlyViewed_CallsRecentlyViewedListLogicReadList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.RecentlyViewed;
                long testId = 0;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                mockDependents.RecentlyViewedListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeReminder_CallsRecommendedItemsListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.RecommendedItems;
                long testId = 0;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                mockDependents.RecommendedItemsListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeReminder_CallsRemindersListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Reminder;
                long testId = 0;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                mockDependents.RemindersListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeWorksheet_CallsHistoryListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Worksheet;
                long testId = 0;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                mockDependents.HistoryListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void GoodCustomer_ReturnsExpectedItemWithExpectedCategory() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(true, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                string expectedItemNumber = "123456";
                string expectedCategory = "Fake Category";
                ListType testListType = ListType.Contract;
                long testId = 0;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, testListType, testId);

                // assert
                results.Items.First()
                       .Category
                       .Should()
                       .Be(expectedCategory);
            }
        }
        #endregion ReadList

        #region ReadPagedList
        public class ReadPagedList {
            [Fact]
            public void AnyUserAnyContextWithTypeContract_CallsContractListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Contract;
                long testId = 0;
                PagingModel testPaging = new PagingModel();

                // act
                PagedListModel results = testunit.ReadPagedList(fakeUser, testcontext, testListType, testId, testPaging);

                // assert
                mockDependents.ContractListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(),
                                                                            It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.AtLeastOnce, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeCustom_CallsCustomListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Custom;
                long testId = 0;
                PagingModel testPaging = new PagingModel();

                // act
                PagedListModel results = testunit.ReadPagedList(fakeUser, testcontext, testListType, testId, testPaging);

                // assert
                mockDependents.CustomListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeFavorites_CallsFavoritesListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Favorite;
                long testId = 0;
                PagingModel testPaging = new PagingModel();

                // act
                PagedListModel results = testunit.ReadPagedList(fakeUser, testcontext, testListType, testId, testPaging);

                // assert
                mockDependents.FavoritesListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeInventoryValuation_CallsInventoryValuationListLogicReadList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.InventoryValuation;
                long testId = 0;
                PagingModel testPaging = new PagingModel();

                // act
                PagedListModel results = testunit.ReadPagedList(fakeUser, testcontext, testListType, testId, testPaging);

                // assert
                mockDependents.InventoryValuationListLogic.Verify(m => m.ReadList(It.IsAny<long>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeMandatory_CallsMandatoryItemsListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Mandatory;
                long testId = 0;
                PagingModel testPaging = new PagingModel();

                // act
                PagedListModel results = testunit.ReadPagedList(fakeUser, testcontext, testListType, testId, testPaging);

                // assert
                mockDependents.MandatoryItemsListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeNotes_CallsNotesListLogicGetList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Notes;
                long testId = 0;
                PagingModel testPaging = new PagingModel();

                // act
                PagedListModel results = testunit.ReadPagedList(fakeUser, testcontext, testListType, testId, testPaging);

                // assert
                mockDependents.NotesListLogic.Verify(m => m.GetList(It.IsAny<UserSelectedContext>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeRecentlyViewed_CallsRecentlyViewedListLogicReadList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.RecentlyViewed;
                long testId = 0;
                PagingModel testPaging = new PagingModel();

                // act
                PagedListModel results = testunit.ReadPagedList(fakeUser, testcontext, testListType, testId, testPaging);

                // assert
                mockDependents.RecentlyViewedListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeReminder_CallsRecommendedItemsListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.RecommendedItems;
                long testId = 0;
                PagingModel testPaging = new PagingModel();

                // act
                PagedListModel results = testunit.ReadPagedList(fakeUser, testcontext, testListType, testId, testPaging);

                // assert
                mockDependents.RecommendedItemsListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeReminder_CallsRemindersListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Reminder;
                long testId = 0;
                PagingModel testPaging = new PagingModel();

                // act
                PagedListModel results = testunit.ReadPagedList(fakeUser, testcontext, testListType, testId, testPaging);

                // assert
                mockDependents.RemindersListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeWorksheet_CallsHistoryListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Worksheet;
                long testId = 0;
                PagingModel testPaging = new PagingModel();

                // act
                PagedListModel results = testunit.ReadPagedList(fakeUser, testcontext, testListType, testId, testPaging);

                // assert
                mockDependents.HistoryListLogic.Verify(m => m.GetListModel(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>()), Times.Once, "not called");
            }
        }
        #endregion ReadPagedList

        #region ReadRecent
        public class ReadRecent {
            [Fact]
            public void AnyUserAnyContext_CallsRecentlyViewedListLogicReadList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };

                // act
                List<RecentItem> results = testunit.ReadRecent(fakeUser, testcontext);

                // assert
                mockDependents.RecentlyViewedListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }
        }
        #endregion ReadRecent

        #region ReadRecentOrder
        public class ReadRecentOrder {
            [Fact]
            public void AnyUserAnyContext_CallsRecentlyOrderedListLogicReadList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                string testCatalog = "UNFI";

                // act
                RecentNonBEKList results = testunit.ReadRecentOrder(fakeUser, testcontext, testCatalog);

                // assert
                mockDependents.RecentlyOrderedListLogic.Verify(m => m.ReadList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()), Times.Once, "not called");
            }
        }
        #endregion ReadRecentOrder

        #region UpdateList
        public class UpdateList {
            [Fact]
            public void AnyUserAnyContextWithTypeCustom_CallsCustomListLogicSaveList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Custom;
                ListModel testModel = new ListModel {
                    ListId = 17,
                    Items = new List<ListItemModel> {
                        new ListItemModel {
                            ItemNumber = "123456"
                        },
                        new ListItemModel {
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
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Favorite;
                ListModel testModel = new ListModel {
                    ListId = 17,
                    Items = new List<ListItemModel> {
                        new ListItemModel {
                            ItemNumber = "123456"
                        },
                        new ListItemModel {
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
            public void AnyUserAnyContextWithTypeInventoryValuation_CallsInventoryValuationListLogicSaveList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.InventoryValuation;
                ListModel testModel = new ListModel {
                    ListId = 17,
                    Items = new List<ListItemModel> {
                        new ListItemModel {
                            ItemNumber = "123456"
                        },
                        new ListItemModel {
                            ItemNumber = "234567"
                        }
                    }
                };

                // act
                testunit.UpdateList(fakeUser, testcontext, testListType, testModel);

                // assert
                mockDependents.InventoryValuationListLogic.Verify(m => m.SaveList(fakeUser, testcontext, testModel), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeMandatory_CallsMandatoryItemsListLogicSaveList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Mandatory;
                ListModel testModel = new ListModel {
                    ListId = 17,
                    Items = new List<ListItemModel> {
                        new ListItemModel {
                            ItemNumber = "123456"
                        },
                        new ListItemModel {
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
            public void AnyUserAnyContextWithTypeReminder_CallsRemindersListLogicSaveList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Reminder;
                ListModel testModel = new ListModel {
                    ListId = 17,
                    Items = new List<ListItemModel> {
                        new ListItemModel {
                            ItemNumber = "123456"
                        },
                        new ListItemModel {
                            ItemNumber = "234567"
                        }
                    }
                };

                // act
                testunit.UpdateList(fakeUser, testcontext, testListType, testModel);

                // assert
                mockDependents.RemindersListLogic.Verify(m => m.SaveList(fakeUser, testcontext, testModel), Times.Once, "not called");
            }
        }
        #endregion UpdateList

        #region SaveItem
        public class SaveItem {
            [Fact]
            public void AnyUserAnyContextWithTypeCustom_CallsCustomListLogicSaveItem() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Custom;
                long testHeaderId = 1;
                ListItemModel testListItem = new ListItemModel {
                    ItemNumber = "123456",
                    CatalogId = "BEK"
                };

                // act
                testunit.SaveItem(fakeUser, testcontext, testListType, testHeaderId, testListItem);

                // assert
                mockDependents.CustomListLogic.Verify(m => m.SaveItem(It.IsAny<CustomListDetail>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeFavorites_CallsFavoritesListLogicSave() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Favorite;
                long testHeaderId = 1;
                ListItemModel testListItem = new ListItemModel {
                    ItemNumber = "123456",
                    CatalogId = "BEK"
                };

                // act
                testunit.SaveItem(fakeUser, testcontext, testListType, testHeaderId, testListItem);

                // assert
                mockDependents.FavoritesListLogic.Verify(m => m.Save(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<FavoritesListDetail>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeInventoryValuation_CallsInventoryValuationListLogicSaveItem() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.InventoryValuation;
                long testHeaderId = 1;
                ListItemModel testListItem = new ListItemModel {
                    ItemNumber = "123456",
                    CatalogId = "BEK"
                };

                // act
                testunit.SaveItem(fakeUser, testcontext, testListType, testHeaderId, testListItem);

                // assert
                mockDependents.InventoryValuationListLogic.Verify(m => m.SaveItem(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<long>(), It.IsAny<InventoryValuationListDetail>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeMandatory_CallsMandatoryItemsListLogicSaveDetail() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Mandatory;
                long testHeaderId = 1;
                ListItemModel testListItem = new ListItemModel {
                    ItemNumber = "123456",
                    CatalogId = "BEK"
                };

                // act
                testunit.SaveItem(fakeUser, testcontext, testListType, testHeaderId, testListItem);

                // assert
                mockDependents.MandatoryItemsListLogic.Verify(m => m.SaveDetail(It.IsAny<UserSelectedContext>(), It.IsAny<MandatoryItemsListDetail>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeNotes_CallsNotesListLogicSaveNote() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Notes;
                long testHeaderId = 1;
                ListItemModel testListItem = new ListItemModel {
                    ItemNumber = "123456",
                    CatalogId = "BEK"
                };

                // act
                testunit.SaveItem(fakeUser, testcontext, testListType, testHeaderId, testListItem);

                // assert
                mockDependents.NotesListLogic.Verify(m => m.SaveNote(It.IsAny<UserSelectedContext>(), It.IsAny<ListItemModel>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeRecommendedItems_CallsRecommendedItemsListLogicSaveDetail() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.RecommendedItems;
                long testHeaderId = 1;
                ListItemModel testListItem = new ListItemModel {
                    ItemNumber = "123456",
                    CatalogId = "BEK"
                };

                // act
                testunit.SaveItem(fakeUser, testcontext, testListType, testHeaderId, testListItem);

                // assert
                mockDependents.RecommendedItemsListLogic.Verify(m => m.SaveDetail(It.IsAny<UserSelectedContext>(), It.IsAny<RecommendedItemsListDetail>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeReminder_CallsRemindersListLogicSave() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Reminder;
                long testHeaderId = 1;
                ListItemModel testListItem = new ListItemModel {
                    ItemNumber = "123456",
                    CatalogId = "BEK"
                };

                // act
                testunit.SaveItem(fakeUser, testcontext, testListType, testHeaderId, testListItem);

                // assert
                mockDependents.RemindersListLogic.Verify(m => m.Save(It.IsAny<UserSelectedContext>(), It.IsAny<ReminderItemsListDetail>()), Times.Once, "not called");
            }

            [Fact]
            public void TryToAddItemWithPosition0_ExpectedToChangePosition() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(true, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Custom;
                long testHeaderId = 1;
                ListItemModel testListItem = new ListItemModel {
                    ItemNumber = "123456",
                    CatalogId = "BEK"
                };
                int expected = 1;

                // act
                testunit.SaveItem(fakeUser, testcontext, testListType, testHeaderId, testListItem);

                // assert
                testListItem.Position.Should()
                            .Be(expected);
            }

            [Fact]
            public void TryToAddItemWithPosition1_ExpectedSamePosition() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(true, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Custom;
                long testHeaderId = 1;
                ListItemModel testListItem = new ListItemModel {
                    ItemNumber = "123456",
                    CatalogId = "BEK",
                    Position = 1
                };
                int expected = 1;

                // act
                testunit.SaveItem(fakeUser, testcontext, testListType, testHeaderId, testListItem);

                // assert
                testListItem.Position.Should()
                            .Be(expected);
            }
        }
        #endregion SaveItem

        #region SaveItems
        public class SaveItems {
            [Fact]
            public void AnyUserAnyContextWithTypeCustom_CallsCustomListLogicSaveItem() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Custom;
                long testHeaderId = 1;
                List<ListItemModel> testListItems = new List<ListItemModel> {
                    new ListItemModel {
                        ItemNumber = "123456",
                        CatalogId = "BEK"
                    },
                    new ListItemModel {
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
            public void AnyUserAnyContextWithTypeFavorites_CallsFavoritesListLogicSave() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Favorite;
                long testHeaderId = 1;
                List<ListItemModel> testListItems = new List<ListItemModel> {
                    new ListItemModel {
                        ItemNumber = "123456",
                        CatalogId = "BEK"
                    },
                    new ListItemModel {
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
            public void AnyUserAnyContextWithTypeInventoryValuation_CallsInventoryValuationListLogicSaveItem() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.InventoryValuation;
                long testHeaderId = 1;
                List<ListItemModel> testListItems = new List<ListItemModel> {
                    new ListItemModel {
                        ItemNumber = "123456",
                        CatalogId = "BEK"
                    },
                    new ListItemModel {
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
            public void AnyUserAnyContextWithTypeMandatory_CallsMandatoryItemsListLogicSaveDetail() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Mandatory;
                long testHeaderId = 1;
                List<ListItemModel> testListItems = new List<ListItemModel> {
                    new ListItemModel {
                        ItemNumber = "123456",
                        CatalogId = "BEK"
                    },
                    new ListItemModel {
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
            public void AnyUserAnyContextWithTypeNotes_CallsNotesListLogicSaveNote() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Notes;
                long testHeaderId = 1;
                List<ListItemModel> testListItems = new List<ListItemModel> {
                    new ListItemModel {
                        ItemNumber = "123456",
                        CatalogId = "BEK"
                    },
                    new ListItemModel {
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
            public void AnyUserAnyContextWithTypeRecommendedItems_CallsRecommendedItemsListLogiSaveDetailc() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.RecommendedItems;
                long testHeaderId = 1;
                List<ListItemModel> testListItems = new List<ListItemModel> {
                    new ListItemModel {
                        ItemNumber = "123456",
                        CatalogId = "BEK"
                    },
                    new ListItemModel {
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
            public void AnyUserAnyContextWithTypeReminder_CallsRemindersListLogicSave() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Reminder;
                long testHeaderId = 1;
                List<ListItemModel> testListItems = new List<ListItemModel> {
                    new ListItemModel {
                        ItemNumber = "123456",
                        CatalogId = "BEK"
                    },
                    new ListItemModel {
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
            public void TryToAddItemWithPosition0_ExpectedToChangePosition() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(true, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Custom;
                long testHeaderId = 1;
                List<ListItemModel> testListItems = new List<ListItemModel> {
                    new ListItemModel {
                        ItemNumber = "123456",
                        CatalogId = "BEK"
                    }
                };
                int expected = 1;

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
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(true, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Custom;
                long testHeaderId = 1;
                List<ListItemModel> testListItems = new List<ListItemModel> {
                    new ListItemModel {
                        ItemNumber = "123456",
                        CatalogId = "BEK",
                        Position = 1
                    }
                };
                int expected = 1;

                // act
                testunit.SaveItems(fakeUser, testcontext, testListType, testHeaderId, testListItems);

                // assert
                testListItems.First()
                             .Position.Should()
                             .Be(expected);
            }
        }
        #endregion SaveItems

        #region CreateList
        public class CreateList {
            [Fact]
            public void AnyUserAnyContextWithTypeCustom_CallsCustomListLogicCreateOrUpdateList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Custom;
                ListModel testModel = new ListModel {
                    ListId = 17,
                    Name = "Fake Name",
                    Items = new List<ListItemModel> {
                        new ListItemModel {
                            ItemNumber = "123456"
                        },
                        new ListItemModel {
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
            public void AnyUserAnyContextWithTypeFavorite_CallsFavoritesListLogicCreateList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Favorite;
                ListModel testModel = new ListModel {
                    Items = new List<ListItemModel>()
                };

                // act
                testunit.CreateList(fakeUser, testcontext, testListType, testModel);

                // assert
                mockDependents.FavoritesListLogic.Verify(m => m.CreateList(fakeUser, testcontext), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeInventoryValuation_CallsInventoryValuationListLogicCreateOrUpdateList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.InventoryValuation;
                ListModel testModel = new ListModel {
                    ListId = 17,
                    Items = new List<ListItemModel> {
                        new ListItemModel {
                            ItemNumber = "123456"
                        },
                        new ListItemModel {
                            ItemNumber = "234567"
                        }
                    }
                };

                // act
                testunit.CreateList(fakeUser, testcontext, testListType, testModel);

                // assert
                mockDependents.InventoryValuationListLogic.Verify(m => m.CreateOrUpdateList(fakeUser, testcontext, 0, testModel.Name, It.IsAny<bool>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContextWithTypeMandatory_CallsMandatoryItemsListLogicSaveList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Mandatory;
                ListModel testModel = new ListModel {
                    ListId = 17,
                    Items = new List<ListItemModel> {
                        new ListItemModel {
                            ItemNumber = "123456"
                        },
                        new ListItemModel {
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
            public void AnyUserAnyContextWithTypeReminder_CallsRemindersListLogicSaveList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Reminder;
                ListModel testModel = new ListModel {
                    ListId = 17,
                    Items = new List<ListItemModel> {
                        new ListItemModel {
                            ItemNumber = "123456"
                        },
                        new ListItemModel {
                            ItemNumber = "234567"
                        }
                    }
                };

                // act
                testunit.CreateList(fakeUser, testcontext, testListType, testModel);

                // assert
                mockDependents.RemindersListLogic.Verify(m => m.SaveList(fakeUser, testcontext, testModel), Times.Once, "not called");
            }
        }
        #endregion CreateList

        #region CopyList
        public class CopyList {
            [Fact]
            public void AnyUserAnyContext_CallsCustomListLogicGetListModel() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                ListCopyShareModel copyListModel = new ListCopyShareModel {
                    ListId = 1,
                    Customers = new List<Customer> {
                        new Customer {
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
        #endregion CopyList

        #region DeleteList
        public class DeleteList {
            [Fact]
            public void AnyUserAnyContextWithTypeCustom_CallsCustomListLogicDeleteList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Custom;
                ListModel testModel = new ListModel {
                    ListId = 17,
                    Items = new List<ListItemModel> {
                        new ListItemModel {
                            ItemNumber = "123456"
                        },
                        new ListItemModel {
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
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.InventoryValuation;
                ListModel testModel = new ListModel {
                    ListId = 17,
                    Items = new List<ListItemModel> {
                        new ListItemModel {
                            ItemNumber = "123456"
                        },
                        new ListItemModel {
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
        #endregion DeleteList

        #region ReadRecommendedItemsList
        public class ReadRecommendedItemsList {
            [Fact]
            public void AnyUserAnyContext_CallsRecommendedItemsListLogicReadList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserSelectedContext testcontext = new UserSelectedContext {
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
        #endregion ReadRecommendedItemsList

        #region MarkFavoritesAndAddNotes_ReturnsListModel
        public class MarkFavoritesAndAddNotes_ReturnsListModel {
            [Fact]
            public void AnyUserAnyContext_CallsFavoritesListLogicGetFavoritesList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListModel testModel = new ListModel {
                    ListId = 17,
                    Items = new List<ListItemModel> {
                        new ListItemModel {
                            ItemNumber = "123456"
                        },
                        new ListItemModel {
                            ItemNumber = "234567"
                        }
                    }
                };

                // act
                ListModel list = testunit.MarkFavoritesAndAddNotes(fakeUser, testModel, testcontext);

                // assert
                mockDependents.FavoritesListLogic.Verify(m => m.GetFavoritesList(It.IsAny<Guid>(),
                                                                                 It.IsAny<UserSelectedContext>(),
                                                                                 false), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContext_CallsNotesListLogicGetList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                ListModel testModel = new ListModel {
                    ListId = 17,
                    Items = new List<ListItemModel> {
                        new ListItemModel {
                            ItemNumber = "123456"
                        },
                        new ListItemModel {
                            ItemNumber = "234567"
                        }
                    }
                };

                // act
                ListModel list = testunit.MarkFavoritesAndAddNotes(fakeUser, testModel, testcontext);

                // assert
                mockDependents.NotesListLogic.Verify(m => m.GetList(It.IsAny<UserSelectedContext>()), Times.Once, "not called");
            }
        }
        #endregion MarkFavoritesAndAddNotes_ReturnsListModel

        #region MarkFavoritesAndAddNotes_ReturnsListOfProduct
        public class MarkFavoritesAndAddNotes_ReturnsListOfProduct {
            [Fact]
            public void AnyUserAnyContext_CallsFavoritesListLogicGetFavoritesList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                List<Product> testModel = new List<Product> {
                    new Product {
                        ItemNumber = "123456"
                    },
                    new Product {
                        ItemNumber = "234567"
                    }
                };

                // act
                List<Product> list = testunit.MarkFavoritesAndAddNotes(fakeUser, testModel, testcontext);

                // assert
                mockDependents.FavoritesListLogic.Verify(m => m.GetFavoritesList(It.IsAny<Guid>(),
                                                                                 It.IsAny<UserSelectedContext>(),
                                                                                 false), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContext_CallsNotesListLogicGetList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                List<Product> testModel = new List<Product> {
                    new Product {
                        ItemNumber = "123456"
                    },
                    new Product {
                        ItemNumber = "234567"
                    }
                };

                // act
                List<Product> list = testunit.MarkFavoritesAndAddNotes(fakeUser, testModel, testcontext);

                // assert
                mockDependents.NotesListLogic.Verify(m => m.GetList(It.IsAny<UserSelectedContext>()), Times.Once, "not called");
            }
        }
        #endregion MarkFavoritesAndAddNotes_ReturnsListOfProduct

        #region GetBarcodeForList
        public class GetBarcodeForList {
            [Fact]
            public void AnyUserAnyContext_CallsItemBarcodeRepositoryLogicGetBarcodeForList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile testuser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Custom;
                long testId = 1;

                // act
                testunit.GetBarcodeForList(testuser, testcontext, testListType, testId);

                // assert
                mockDependents.ItemBarcodeRepository.Verify(m => m.GetBarcodeForList(It.IsAny<ListModel>()), Times.Once, "not called");
            }
        }
        #endregion GetBarcodeForList

        #region AddCustomInventory
        public class AddCustomInventory {
            [Fact]
            public void AnyUserAnyContext_CallsCustomInventoryItemsRepositoryGet() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile testuser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Custom;
                long testId = 1;
                long testCustomInventoryId = 1;

                // act
                testunit.AddCustomInventory(testuser, testcontext, testListType, testId, testCustomInventoryId);

                // assert
                mockDependents.CustomInventoryItemsRepository.Verify(m => m.Get(It.IsAny<long>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyUserAnyContext_CallsCustomListLogicSaveList() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile testuser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Custom;
                long testId = 1;
                long testCustomInventoryId = 1;

                // act
                testunit.AddCustomInventory(testuser, testcontext, testListType, testId, testCustomInventoryId);

                // assert
                mockDependents.CustomListLogic.Verify(m => m.SaveList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<ListModel>()), Times.Once, "not called");
            }
        }
        #endregion AddCustomInventory

        #region AddCustomInventoryItems
        public class AddCustomInventoryItems {
            [Fact]
            public void AnyUserAnyContext_CallsCustomInventoryItemsRepositoryGetItemsByItemIds() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testunit = MakeTestsService(false, ref mockDependents);
                UserProfile testuser = new UserProfile();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                ListType testListType = ListType.Custom;
                long testId = 1;
                List<long> testCustomInventoryId = new List<long> {1};

                // act
                testunit.AddCustomInventoryItems(testuser, testcontext, testListType, testId, testCustomInventoryId);

                // assert
                mockDependents.CustomInventoryItemsRepository.Verify(m => m.GetItemsByItemIds(It.IsAny<List<long>>()), Times.Once, "not called");
            }
        }
        #endregion AddCustomInventoryItems

        #region DeleteItem
        public class DeleteItem {
            [Fact]
            public void BadItemNumber_DoesNotCallSaveItem() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testUnit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile {
                    UserId = new Guid("b514a6c2-6f07-48f1-a26c-650f28337b01")
                };
                UserSelectedContext fakeCustomer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                ListType fakeType = ListType.Favorite;
                int fakeId = 1;
                string fakeItemNumber = "999999";

                // act
                testUnit.DeleteItem(fakeUser, fakeCustomer, fakeType, fakeId, fakeItemNumber);

                // assert
                mockDependents.FavoritesListLogic.Verify(f => f.Save(It.IsAny<UserProfile>(),
                                                                     It.IsAny<UserSelectedContext>(),
                                                                     It.IsAny<FavoritesListDetail>()),
                                                         Times.Never);
            }

            [Fact]
            public void GoodItemNumber_DeletesTheSpecifiedFavoriteItem() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testUnit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile {
                    UserId = new Guid("b514a6c2-6f07-48f1-a26c-650f28337b01")
                };
                UserSelectedContext fakeCustomer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                ListType fakeType = ListType.Favorite;
                int fakeId = 1;
                string fakeItemNumber = "234567";

                // act
                testUnit.DeleteItem(fakeUser, fakeCustomer, fakeType, fakeId, fakeItemNumber);

                // assert
                mockDependents.FavoritesListLogic.Verify(f => f.Save(It.IsAny<UserProfile>(),
                                                                     It.IsAny<UserSelectedContext>(),
                                                                     It.Is<FavoritesListDetail>(d => d.ItemNumber == fakeItemNumber)),
                                                         Times.Once);
            }
        }
        #endregion DeleteItem

        #region DeleteItems
        public class DeleteItems {
            [Fact]
            public void EmptyItemNumberList_DoesNotCallTheSaveMethod() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testUnit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile {
                    UserId = new Guid("b514a6c2-6f07-48f1-a26c-650f28337b01")
                };
                UserSelectedContext fakeCustomer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                ListType fakeType = ListType.Favorite;
                int fakeId = 1;
                List<string> fakeItemNumbers = new List<string>();

                // act
                testUnit.DeleteItems(fakeUser, fakeCustomer, fakeType, fakeId, fakeItemNumbers);

                // assert
                mockDependents.FavoritesListLogic.Verify(f => f.Save(It.IsAny<UserProfile>(),
                                                                     It.IsAny<UserSelectedContext>(),
                                                                     It.IsAny<FavoritesListDetail>()),
                                                         Times.Never);
            }

            [Fact]
            public void GoodItemNumbers_DeletesTheSpecifiedFavoriteItems() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IListService testUnit = MakeTestsService(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile {
                    UserId = new Guid("b514a6c2-6f07-48f1-a26c-650f28337b01")
                };
                UserSelectedContext fakeCustomer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                ListType fakeType = ListType.Favorite;
                int fakeId = 1;
                List<string> fakeItemNumbers = new List<string> {"234567", "345678"};

                // act
                testUnit.DeleteItems(fakeUser, fakeCustomer, fakeType, fakeId, fakeItemNumbers);

                // assert
                mockDependents.FavoritesListLogic.Verify(f => f.Save(It.IsAny<UserProfile>(),
                                                                     It.IsAny<UserSelectedContext>(),
                                                                     It.Is<FavoritesListDetail>(d => fakeItemNumbers.Contains(d.ItemNumber))),
                                                         Times.Exactly(2));
            }
        }
        #endregion DeleteItems

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
                Mock<IRecentlyViewedListLogic> mock = new Mock<IRecentlyViewedListLogic>();

                return mock;
            }

            public static Mock<IFavoritesListLogic> MakeMockFavoritesListLogic() {
                Mock<IFavoritesListLogic> mock = new Mock<IFavoritesListLogic>();

                mock.Setup(f => f.GetListModel(It.IsAny<UserProfile>(),
                                               It.IsAny<UserSelectedContext>(),
                                               It.Is<long>(i => i == 1)))
                    .Returns(new ListModel {
                        BranchId = "FUT",
                        CustomerNumber = "123456",
                        Items = new List<ListItemModel> {
                            new ListItemModel {
                                ItemNumber = "123456"
                            },
                            new ListItemModel {
                                ItemNumber = "234567"
                            },
                            new ListItemModel {
                                ItemNumber = "345678"
                            }
                        }
                    });

                return mock;
            }

            public static Mock<IHistoryListLogic> MakeMockHistoryListLogic() {
                Mock<IHistoryListLogic> mock = new Mock<IHistoryListLogic>();

                return mock;
            }

            public static Mock<IContractListLogic> MakeMockContractListLogic() {
                Mock<IContractListLogic> mock = new Mock<IContractListLogic>();

                mock.Setup(h => h.GetListModel(It.IsAny<UserProfile>(),
                                               It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                               c.CustomerId == "123456"),
                                               It.IsAny<long>()))
                    .Returns(new ListModel {
                        ListId = 1,
                        CustomerNumber = "123456",
                        BranchId = "FUT",
                        Items = new List<ListItemModel> {
                            new ListItemModel {
                                ItemNumber = "123456",
                                Category = "Fake Category"
                            }
                        }
                    });

                return mock;
            }

            public static Mock<IRemindersListLogic> MakeMockRemindersListLogic() {
                Mock<IRemindersListLogic> mock = new Mock<IRemindersListLogic>();

                mock.Setup(h => h.SaveList(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<ListModel>()))
                    .Returns(new ListModel {
                        ListId = 1
                    });

                return mock;
            }

            public static Mock<IInventoryValuationListLogic> MakeMockInventoryValuationListLogic() {
                Mock<IInventoryValuationListLogic> mock = new Mock<IInventoryValuationListLogic>();

                return mock;
            }

            public static Mock<IMandatoryItemsListLogic> MakeMockMandatoryItemsListLogic() {
                Mock<IMandatoryItemsListLogic> mock = new Mock<IMandatoryItemsListLogic>();

                return mock;
            }

            public static Mock<ICustomInventoryItemsRepository> MakeMockCustomInventoryItemRepository() {
                Mock<ICustomInventoryItemsRepository> mock = new Mock<ICustomInventoryItemsRepository>();

                mock.Setup(h => h.Get(1))
                    .Returns(new CustomInventoryItem {
                        ItemNumber = "666666",
                        Name = "double bad",
                        CasePrice = 0,
                        PackagePrice = 0
                    });

                return mock;
            }

            public static Mock<IRecommendedItemsListLogic> MakeMockRecommendedItemsListLogic() {
                Mock<IRecommendedItemsListLogic> mock = new Mock<IRecommendedItemsListLogic>();

                mock.Setup(h => h.ReadList(It.IsAny<UserProfile>(),
                                           It.IsAny<UserSelectedContext>(),
                                           It.IsAny<bool>()))
                    .Returns(new ListModel {
                        ListId = 1,
                        CustomerNumber = "123456",
                        BranchId = "FUT",
                        Items = new List<ListItemModel> {
                            new ListItemModel {
                                ItemNumber = "123456",
                                Each = false,
                                Category = "Fake Category"
                            }
                        }
                    });

                return mock;
            }

            public static Mock<IRecentlyOrderedListLogic> MakeMockRecentlyOrderedListLogic() {
                Mock<IRecentlyOrderedListLogic> mock = new Mock<IRecentlyOrderedListLogic>();

                return mock;
            }

            public static Mock<INotesListLogic> MakeMockNotesListLogic() {
                Mock<INotesListLogic> mock = new Mock<INotesListLogic>();

                mock.Setup(l => l.GetList(It.IsAny<UserSelectedContext>()))
                    .Returns(new ListModel {
                        ListId = 1,
                        CustomerNumber = "123456",
                        BranchId = "FUT",
                        Items = new List<ListItemModel> {
                            new ListItemModel {
                                ItemNumber = "123456",
                                Notes = "Test Note"
                            }
                        }
                    });

                return mock;
            }

            public static Mock<ICustomListLogic> MakeMockCustomListLogic() {
                Mock<ICustomListLogic> mock = new Mock<ICustomListLogic>();

                mock.Setup(h => h.GetListModel(It.IsAny<UserProfile>(),
                                               It.IsAny<UserSelectedContext>(),
                                               It.Is<long>(i => i == 1)))
                    .Returns(new ListModel {
                        ListId = 1,
                        CustomerNumber = "123456",
                        BranchId = "FUT",
                        Items = new List<ListItemModel> {
                            new ListItemModel {
                                ItemNumber = "123456",
                                Category = "Fake Category",
                                Each = false
                            }
                        }
                    });

                mock.Setup(h => h.ReadList(It.Is<long>(i => i == 1), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()))
                    .Returns(new ListModel {
                        ListId = 1,
                        CustomerNumber = "123456",
                        BranchId = "FUT",
                        Items = new List<ListItemModel> {
                            new ListItemModel {
                                ItemNumber = "123456",
                                Category = "Fake Category",
                                Each = false
                            }
                        }
                    });

                mock.Setup(h => h.ReadLists(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<bool>()))
                    .Returns(new List<ListModel> {
                        new ListModel {
                            ListId = 1,
                            CustomerNumber = "123456",
                            BranchId = "FUT",
                            Items = new List<ListItemModel> {
                                new ListItemModel {
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
                    .Returns(new ListModel {
                        ListId = 2,
                        CustomerNumber = "123456",
                        BranchId = "FUT"
                    });

                mock.Setup(h => h.CreateOrUpdateList(It.IsAny<UserProfile>(),
                                                     It.IsAny<UserSelectedContext>(),
                                                     0,
                                                     It.IsAny<string>(),
                                                     It.IsAny<bool>()))
                    .Returns(2);

                return mock;
            }

            public static Mock<IEventLogRepository> MakeMockEventLogRepository() {
                Mock<IEventLogRepository> mock = new Mock<IEventLogRepository>();

                return mock;
            }

            public static Mock<IItemBarcodeImageRepository> MakeMockItemBarcodeImageRepository() {
                Mock<IItemBarcodeImageRepository> mock = new Mock<IItemBarcodeImageRepository>();

                return mock;
            }

            public static Mock<IProductImageRepository> MakeMockProductImageRepository() {
                Mock<IProductImageRepository> mock = new Mock<IProductImageRepository>();

                return mock;
            }

            public static Mock<IPriceLogic> MakeMockPriceLogic() {
                Mock<IPriceLogic> mock = new Mock<IPriceLogic>();

                return mock;
            }

            public static Mock<IItemHistoryRepository> MakeMockItemHistoryRepository() {
                Mock<IItemHistoryRepository> mock = new Mock<IItemHistoryRepository>();

                return mock;
            }

            public static Mock<IExternalCatalogRepository> MakeMockExternalCatalogRepository() {
                Mock<IExternalCatalogRepository> mock = new Mock<IExternalCatalogRepository>();

                mock.Setup(h => h.ReadAll())
                    .Returns(new List<ExternalCatalog> {
                        new ExternalCatalog {
                            BekBranchId = "XXX",
                            ExternalBranchId = "multi-X"
                        }
                    });

                return mock;
            }

            public static Mock<ICatalogLogic> MakeMockCatalogLogic() {
                Mock<ICatalogLogic> mock = new Mock<ICatalogLogic>();

                return mock;
            }

            public static Mock<ICacheListLogic> MakeMockCacheListHelper() {
                Mock<ICacheListLogic> mock = new Mock<ICacheListLogic>();

                return mock;
            }
        }

        private static IListService MakeTestsService(bool useAutoFac, ref MockDependents mockDependents) {
            if (useAutoFac) {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                MockDependents.RegisterInContainer(ref cb);

                IContainer testcontainer = cb.Build();

                return testcontainer.Resolve<IListService>();
            }
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

            ListServiceImpl testunit = new ListServiceImpl(mockDependents.HistoryListLogic.Object, mockDependents.CatalogLogic.Object, mockDependents.NotesListLogic.Object,
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
        #endregion Setup
    }
}