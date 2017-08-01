using System;
using System.Collections.Generic;
using System.Linq;

using Autofac;

using FluentAssertions;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Favorites;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic.Lists;

using Moq;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Lists {
    public class FavoritesListLogicTests : BaseDITests {
        private static IFavoritesListLogic MakeTestsUnit() {
            ContainerBuilder cb = GetTestsContainer();

            // Register mocks
            cb.RegisterInstance(MakeMockHeaderRepo())
              .As<IFavoriteListHeadersRepository>();
            cb.RegisterInstance(MakeMockDetailsRepo())
              .As<IFavoriteListDetailsRepository>();

            IContainer testcontainer = cb.Build();

            return testcontainer.Resolve<IFavoritesListLogic>();
        }

        private static IFavoriteListHeadersRepository MakeMockHeaderRepo() {
            Mock<IFavoriteListHeadersRepository> mockHeaderRepo = new Mock<IFavoriteListHeadersRepository>();

            mockHeaderRepo.Setup(h => h.GetFavoritesList(It.IsAny<Guid>(),
                                                         It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                                         c.CustomerId == "123456")))
                          .Returns(new FavoritesListHeader {
                              BranchId = "FUT",
                              CustomerNumber = "123456",
                              UserId = It.IsAny<Guid>(),
                              CreatedUtc = It.IsAny<DateTime>(),
                              Id = 1,
                              ModifiedUtc = It.IsAny<DateTime>()
                          });

            mockHeaderRepo.Setup(h => h.SaveFavoriteListHeader(new FavoritesListHeader {
                              BranchId = "FUT",
                              CustomerNumber = "123456",
                              CreatedUtc = new DateTime(2017, 7, 14, 16, 41, 0, DateTimeKind.Utc),
                              ModifiedUtc = new DateTime(2017, 7, 14, 16, 41, 0, DateTimeKind.Utc)
                          }))
                          .Returns(It.Is<long>(l => l == 1));

            return mockHeaderRepo.Object;
        }

        private static IFavoriteListDetailsRepository MakeMockDetailsRepo() {
            Mock<IFavoriteListDetailsRepository> mockDetailsRepo = new Mock<IFavoriteListDetailsRepository>();

            mockDetailsRepo.Setup(h => h.GetFavoritesListDetails(It.Is<long>(l => l == 1)))
                           .Returns(new List<FavoritesListDetail> {
                               new FavoritesListDetail {
                                   CatalogId = "FUT",
                                   ItemNumber = "123456",
                                   Each = false,
                                   LineNumber = 1,
                                   Active = true,
                                   CreatedUtc = new DateTime(2017, 7, 14, 16, 41, 0, DateTimeKind.Utc),
                                   Id = 1,
                                   ModifiedUtc = new DateTime(2017, 7, 14, 16, 41, 0, DateTimeKind.Utc)
                               }
                           });

            mockDetailsRepo.Setup(h => h.SaveFavoriteListDetail(new FavoritesListDetail {
                               CatalogId = "FUT",
                               ItemNumber = "123456"
                           }))
                           .Returns(It.Is<long>(l => l == 1));

            return mockDetailsRepo.Object;
        }

        public class GetFavoritedItemNumbers {
            [Fact]
            public void BadBranchId_ReturnsEmptyList() {
                // arrange
                IFavoritesListLogic testunit = MakeTestsUnit();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                int expectedEmptyListCount = 0;

                // act
                List<string> results = testunit.GetFavoritedItemNumbers(fakeUser, testcontext);

                // assert
                results.Count()
                       .Should()
                       .Be(expectedEmptyListCount);
            }

            [Fact]
            public void BadCustomerId_ReturnsEmptyList() {
                // arrange
                IFavoritesListLogic testunit = MakeTestsUnit();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };
                UserProfile fakeUser = new UserProfile();
                int expectedEmptyListCount = 0;

                // act
                List<string> results = testunit.GetFavoritedItemNumbers(fakeUser, testcontext);

                // assert
                results.Count()
                       .Should()
                       .Be(expectedEmptyListCount);
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsExpectedList() {
                // arrange
                IFavoritesListLogic testunit = MakeTestsUnit();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                string expected = "123456";

                // act
                List<string> results = testunit.GetFavoritedItemNumbers(fakeUser, testcontext);

                // assert
                results.First()
                       .Should()
                       .Be(expected);
            }
        }

        public class GetFavoritesList {
            [Fact]
            public void BadBranchId_ReturnsNull() {
                // arrange
                IFavoritesListLogic testunit = MakeTestsUnit();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();

                // act
                ListModel results = testunit.GetFavoritesList(fakeUser.UserId, testcontext, true);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void BadCustomerId_ReturnsNull() {
                // arrange
                IFavoritesListLogic testunit = MakeTestsUnit();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };
                UserProfile fakeUser = new UserProfile();

                // act
                ListModel results = testunit.GetFavoritesList(fakeUser.UserId, testcontext, true);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsListExpectedItemNumber() {
                // arrange
                IFavoritesListLogic testunit = MakeTestsUnit();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                string expected = "123456";

                // act
                ListModel results = testunit.GetFavoritesList(fakeUser.UserId, testcontext, false);

                // assert
                results.Items
                       .First()
                       .ItemNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsListWithAnItem() {
                // arrange
                IFavoritesListLogic testunit = MakeTestsUnit();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                int expectedCount = 1;

                // act
                ListModel results = testunit.GetFavoritesList(fakeUser.UserId, testcontext, false);

                // assert
                results.Items
                       .Count()
                       .Should()
                       .Be(expectedCount);
            }

            [Fact]
            public void GoodCustomerIdAndBranchHeaderOnly_ReturnsListWithExpectedId() {
                // arrange
                IFavoritesListLogic testunit = MakeTestsUnit();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                int expectedId = 1;

                // act
                ListModel results = testunit.GetFavoritesList(fakeUser.UserId, testcontext, true);

                // assert
                results.ListId
                       .Should()
                       .Be(expectedId);
            }
        }

        public class GetListModel {
            [Fact]
            public void BadBranchId_ReturnsNull() {
                // arrange
                IFavoritesListLogic testunit = MakeTestsUnit();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                int fakeId = 0;

                // act
                ListModel results = testunit.GetListModel(fakeUser, testcontext, fakeId);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void BadCustomerId_ReturnsNull() {
                // arrange
                IFavoritesListLogic testunit = MakeTestsUnit();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };
                UserProfile fakeUser = new UserProfile();
                int fakeId = 0;

                // act
                ListModel results = testunit.GetListModel(fakeUser, testcontext, fakeId);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsListExpectedItemNumber() {
                // arrange
                IFavoritesListLogic testunit = MakeTestsUnit();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                string expected = "123456";
                int fakeId = 0;

                // act
                ListModel results = testunit.GetListModel(fakeUser, testcontext, fakeId);

                // assert
                results.Items
                       .First()
                       .ItemNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsListWithAnItem() {
                // arrange
                IFavoritesListLogic testunit = MakeTestsUnit();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                int expectedCount = 1;
                int fakeId = 0;

                // act
                ListModel results = testunit.GetListModel(fakeUser, testcontext, fakeId);

                // assert
                results.Items
                       .Count()
                       .Should()
                       .Be(expectedCount);
            }

            [Fact]
            public void GoodCustomerIdAndBranchHeaderOnly_ReturnsListWithExpectedId() {
                // arrange
                IFavoritesListLogic testunit = MakeTestsUnit();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                int expectedId = 1;
                int fakeId = 0;

                // act
                ListModel results = testunit.GetListModel(fakeUser, testcontext, fakeId);

                // assert
                results.ListId
                       .Should()
                       .Be(expectedId);
            }
        }

        public class SaveList {
// works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCustomerIdAndBranch_CallsSaveHeaderEveryTime() {
                // arrange
                Mock<IFavoriteListHeadersRepository> mockHeaderRepo = new Mock<IFavoriteListHeadersRepository>();
                Mock<IFavoriteListDetailsRepository> mockDetailsRepo = new Mock<IFavoriteListDetailsRepository>();
                FavoritesListLogicImpl testunit = new FavoritesListLogicImpl(mockDetailsRepo.Object, mockHeaderRepo.Object);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                ListModel testList = new ListModel {
                    ListId = 1,
                    CustomerNumber = "123456",
                    BranchId = "FUT",
                    Items = new List<ListItemModel> {
                        new ListItemModel {
                            ItemNumber = "123456"
                        }
                    }
                };
                // act
                testunit.SaveList(fakeUser, testcontext, testList);

                // assert - Always returns what is setup provided the mock is called
                mockDetailsRepo.Verify(h => h.SaveFavoriteListDetail(It.IsAny<FavoritesListDetail>()), Times.Once(), "Error updating");
            }

            [Fact]
            public void AnyCustomerIdAndBranch_WhenIsDeleteIsTrueDeleteIsCalledWithListItemId() {
                // arrange
                Mock<IFavoriteListHeadersRepository> mockHeaderRepo = new Mock<IFavoriteListHeadersRepository>();
                Mock<IFavoriteListDetailsRepository> mockDetailsRepo = new Mock<IFavoriteListDetailsRepository>();
                FavoritesListLogicImpl testunit = new FavoritesListLogicImpl(mockDetailsRepo.Object, mockHeaderRepo.Object);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                ListModel testList = new ListModel {
                    ListId = 1,
                    CustomerNumber = "123456",
                    BranchId = "FUT",
                    Name = "Favorites",
                    IsContractList = false,
                    ReadOnly = false,
                    IsFavorite = true,
                    IsWorksheet = false,
                    IsReminder = false,
                    IsShared = false,
                    IsSharing = false,
                    IsMandatory = false,
                    IsRecommended = false,
                    Items = new List<ListItemModel> {
                        new ListItemModel {
                            Active = true,
                            ListItemId = 2,
                            IsDelete = true,
                            CatalogId = "FDF",
                            ItemNumber = "123456"
                        }
                    }
                };
                // act
                testunit.SaveList(fakeUser, testcontext, testList);

                // assert - Always returns what is setup provided the mock is called
                mockDetailsRepo.Verify(
                       x => x.SaveFavoriteListDetail(It.Is<FavoritesListDetail>(d => d.Active.Equals(false))),
                       Times.Once(),
                       "Error updating");
            }
        }

        public class CreateList
        {
            // works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCustomerIdAndBranch_CallsSaveHeaderEveryTime()
            {
                // arrange
                Mock<IFavoriteListHeadersRepository> mockHeaderRepo = new Mock<IFavoriteListHeadersRepository>();
                Mock<IFavoriteListDetailsRepository> mockDetailsRepo = new Mock<IFavoriteListDetailsRepository>();
                FavoritesListLogicImpl testunit = new FavoritesListLogicImpl(mockDetailsRepo.Object, mockHeaderRepo.Object);
                UserSelectedContext testcontext = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();

                // act
                testunit.CreateList(fakeUser, testcontext);

                // assert - Always returns what is setup provided the mock is called
                mockHeaderRepo.Verify(h => h.SaveFavoriteListHeader(It.IsAny<FavoritesListHeader>()), Times.Once(), "Error updating");
            }
        }

        public class Save {
// works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCustomerIdAndBranch_CallsSaveDetailEveryTime() {
                // arrange
                Mock<IFavoriteListHeadersRepository> mockHeaderRepo = new Mock<IFavoriteListHeadersRepository>();
                Mock<IFavoriteListDetailsRepository> mockDetailsRepo = new Mock<IFavoriteListDetailsRepository>();
                FavoritesListLogicImpl testunit = new FavoritesListLogicImpl(mockDetailsRepo.Object, mockHeaderRepo.Object);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                FavoritesListDetail testDetail = new FavoritesListDetail {
                            CatalogId = "FUT",
                            ItemNumber = "123456",
                            Each = false,
                            LineNumber = 1,
                            Active = true,
                            CreatedUtc = new DateTime(2017, 7, 14, 16, 41, 0, DateTimeKind.Utc),
                            Id = 1,
                            ModifiedUtc = new DateTime(2017, 7, 14, 16, 41, 0, DateTimeKind.Utc)
                        }
                        ;
                // act
                testunit.Save(fakeUser, testcontext, testDetail);

                // assert - Always returns what is setup provided the mock is called
                mockDetailsRepo.Verify(h => h.SaveFavoriteListDetail(testDetail), Times.Once(), "Error updating");
            }
        }
    }
}