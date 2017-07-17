using System;
using System.Collections.Generic;
using System.Linq;

using Autofac;
using FluentAssertions;
using Moq;
using Xunit;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Favorites;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.SmartResolver;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Lists
{
    public class FavoritesListLogicTests : BaseDITests
    {
        private static IFavoritesListLogic MakeTestsUnit()
        {
            ContainerBuilder cb = GetTestsContainer();

            // Register mocks
            cb.RegisterInstance(MakeMockHeaderRepo())
              .As<IFavoriteListHeadersRepository>();
            cb.RegisterInstance(MakeMockDetailsRepo())
              .As<IFavoriteListDetailsRepository>();

            var testcontainer = cb.Build();

            return testcontainer.Resolve<IFavoritesListLogic>();
        }

        private static IFavoriteListHeadersRepository MakeMockHeaderRepo()
        {
            var mockHeaderRepo = new Mock<IFavoriteListHeadersRepository>();

            mockHeaderRepo.Setup(h => h.GetFavoritesList(It.IsAny<Guid>(), 
                                                         It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                                    c.CustomerId == "123456")))
                          .Returns(new FavoritesListHeader()
                          {
                              BranchId = "FUT",
                              CustomerNumber = "123456",
                              UserId = It.IsAny<Guid>(),
                              CreatedUtc = It.IsAny<DateTime>(),
                              Id = 1,
                              ModifiedUtc = It.IsAny<DateTime>()
                          });

            mockHeaderRepo.Setup(h => h.SaveFavoriteListHeader(new FavoritesListHeader()
                                                                {
                                                                    BranchId = "FUT",
                                                                    CustomerNumber = "123456",
                                                                    CreatedUtc = new DateTime(2017, 7, 14, 16, 41, 0, DateTimeKind.Utc),
                                                                    ModifiedUtc = new DateTime(2017, 7, 14, 16, 41, 0, DateTimeKind.Utc)
                                                                }))
                          .Returns(It.Is<long>(l => l == 1));

            return mockHeaderRepo.Object;
        }

        private static IFavoriteListDetailsRepository MakeMockDetailsRepo()
        {
            var mockDetailsRepo = new Mock<IFavoriteListDetailsRepository>();

            mockDetailsRepo.Setup(h => h.GetFavoritesListDetails(It.Is<long>(l => l == 1)))
                           .Returns(new List<FavoritesListDetail>()
                           { 
                               new FavoritesListDetail() {
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

            mockDetailsRepo.Setup(h => h.SaveFavoriteListDetail(new FavoritesListDetail()
                                                                {
                                                                    CatalogId = "FUT",
                                                                    ItemNumber = "123456"
                                                                }))
                          .Returns(It.Is<long>(l => l == 1));

            return mockDetailsRepo.Object;
        }

        public class GetFavoritedItemNumbers
        {
            [Fact]
            public void BadBranchId_ReturnsEmptyList()
            {
                // arrange
                var testunit = MakeTestsUnit();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();

                // act
                var results = testunit.GetFavoritedItemNumbers(fakeUser, testcontext);

                // assert
                results.Count()
                       .Should()
                       .Be(0);
            }

            [Fact]
            public void BadCustomerId_ReturnsEmptyList()
            {
                // arrange
                var testunit = MakeTestsUnit();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };
                var fakeUser = new UserProfile();

                // act
                var results = testunit.GetFavoritedItemNumbers(fakeUser, testcontext);

                // assert
                results.Count()
                       .Should()
                       .Be(0);
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsExpectedList()
            {
                // arrange
                var testunit = MakeTestsUnit();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();
                var expected = "123456";

                // act
                var results = testunit.GetFavoritedItemNumbers(fakeUser, testcontext);

                // assert
                results.First()
                       .Should()
                       .Be(expected);
            }
        }

        public class GetFavoritesList
        {
            [Fact]
            public void BadBranchId_ReturnsNull()
            {
                // arrange
                var testunit = MakeTestsUnit();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();

                // act
                var results = testunit.GetFavoritesList(fakeUser.UserId, testcontext, true);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void BadCustomerId_ReturnsNull()
            {
                // arrange
                var testunit = MakeTestsUnit();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };
                var fakeUser = new UserProfile();

                // act
                var results = testunit.GetFavoritesList(fakeUser.UserId, testcontext, true);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void GoodCustomerIdAndBranchHeaderOnly_ReturnsList()
            {
                // arrange
                var testunit = MakeTestsUnit();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();
                var expected = new ListModel() {
                                                   CustomerNumber = "123456",
                                                   BranchId = "FUT",
                                                   ListId=1L,
                                                   Type = ListType.Favorite,
                                                   Name = "Favorites"
                };

                // act
                var results = testunit.GetFavoritesList(fakeUser.UserId, testcontext, true);

                // assert
                results.Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsListWithAnItem()
            {
                // arrange
                var testunit = MakeTestsUnit();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();
                var expected = 1;

                // act
                var results = testunit.GetFavoritesList(fakeUser.UserId, testcontext, false);

                // assert
                results.Items
                       .Count()
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsListExpectedItemNumber()
            {
                // arrange
                var testunit = MakeTestsUnit();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();
                var expected = "123456";

                // act
                var results = testunit.GetFavoritesList(fakeUser.UserId, testcontext, false);

                // assert
                results.Items
                       .First()
                       .ItemNumber
                       .Should()
                       .Be(expected);
            }
        }
    }
}
