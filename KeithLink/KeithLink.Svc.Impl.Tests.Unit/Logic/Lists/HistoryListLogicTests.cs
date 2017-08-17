using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using FluentAssertions;
using Moq;
using Xunit;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.History;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Lists
{
    public class HistoryListLogicTests : BaseDITests
    {
        private static IHistoryListLogic MakeTestsObject()
        {
            ContainerBuilder cb = GetTestsContainer();

            // Register mocks
            cb.RegisterInstance(MakeMockHeaderRepo())
              .As<IHistoryListHeaderRepository>();
            cb.RegisterInstance(MakeMockDetailsRepo())
              .As<IHistoryListDetailRepository>();

            var testcontainer = cb.Build();

            return testcontainer.Resolve<IHistoryListLogic>();
        }

        private static IHistoryListHeaderRepository MakeMockHeaderRepo()
        {
            var mockHeaderRepo = new Mock<IHistoryListHeaderRepository>();

            mockHeaderRepo.Setup(h => h.GetHistoryListHeader(
                                                         It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                                    c.CustomerId == "123456")))
                          .Returns(new HistoryListHeader()
                          {
                              BranchId = "FUT",
                              CustomerNumber = "123456",
                              CreatedUtc = It.IsAny<DateTime>(),
                              Id = 1,
                              ModifiedUtc = It.IsAny<DateTime>()
                          });

            return mockHeaderRepo.Object;
        }

        private static IHistoryListDetailRepository MakeMockDetailsRepo()
        {
            var mockDetailsRepo = new Mock<IHistoryListDetailRepository>();

            mockDetailsRepo.Setup(h => h.GetAllHistoryDetails(It.Is<long>(l => l == 1)))
                           .Returns(new List<HistoryListDetail>()
                           {
                               new HistoryListDetail() {
                                                             CatalogId = "FUT",
                                                             ItemNumber = "123456",
                                                             Each = false,
                                                             LineNumber = 1,
                                                             CreatedUtc = new DateTime(2017, 7, 14, 16, 41, 0, DateTimeKind.Utc),
                                                             Id = 1,
                                                             ModifiedUtc = new DateTime(2017, 7, 14, 16, 41, 0, DateTimeKind.Utc)
                                                         }
                           });

            return mockDetailsRepo.Object;
        }

        public class GetListModel
        {
            [Fact]
            public void BadBranchId_ReturnsNull()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();

                // act
                var results = testunit.GetListModel(fakeUser, testcontext, 0);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void BadCustomerId_ReturnsNull()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };
                var fakeUser = new UserProfile();

                // act
                var results = testunit.GetListModel(fakeUser, testcontext, 0);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsExpectedList()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();
                var expected = "123456";

                // act
                var results = testunit.GetListModel(fakeUser, testcontext, 0);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }
        }

        public class ReadList
        {
            [Fact]
            public void BadBranchId_ReturnsNull()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();
                var headerOnly = false;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, headerOnly);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void BadCustomerId_ReturnsNull()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };
                var fakeUser = new UserProfile();
                var headerOnly = false;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, headerOnly);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsExpectedList()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();
                var expected = "123456";
                var headerOnly = false;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, headerOnly);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomerIdAndBranchHeaderOnlyFalse_ReturnsExpectedNumberOfItems()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();
                var expected = 1;
                var headerOnly = false;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, headerOnly);

                // assert
                results.Items
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomerIdAndBranchHeaderOnlyTrue_ReturnsExpectedNumberOfItems()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();
                var expected = 0;
                var headerOnly = true;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, headerOnly);

                // assert
                results.Items
                       .Count
                       .Should()
                       .Be(expected);
            }
        }

        public class ItemsInHistoryList
        {
            [Fact]
            public void BadBranchId_ReturnsListWithInListItemsAndInHistoryFalse()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var InList = new List<string>() { "123456" };
                var expected = false;

                // act
                var results = testunit.ItemsInHistoryList(testcontext, InList);

                // assert
                results.First()
                       .InHistory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void BadCustomerId_ReturnsListWithInListItemsAndInHistoryFalse()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };
                var InList = new List<string>() { "123456" };
                var expected = false;

                // act
                var results = testunit.ItemsInHistoryList(testcontext, InList);

                // assert
                results.First()
                       .InHistory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsListWithInListItemsAndInHistoryTrue()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var InList = new List<string>() { "123456" };
                var expected = true;

                // act
                var results = testunit.ItemsInHistoryList(testcontext, InList);

                // assert
                results.First()
                       .InHistory
                       .Should()
                       .Be(expected);
            }
        }
    }
}
