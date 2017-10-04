using System;
using System.Collections.Generic;
using System.Linq;

using Autofac;

using FluentAssertions;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.History;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using Moq;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Lists {
    public class HistoryListLogicTests : BaseDITests {
        private static IHistoryListLogic MakeTestsObject() {
            ContainerBuilder cb = GetTestsContainer();

            // Register mocks
            cb.RegisterInstance(MakeMockHeaderRepo())
              .As<IHistoryListHeaderRepository>();
            cb.RegisterInstance(MakeMockDetailsRepo())
              .As<IHistoryListDetailRepository>();

            IContainer testcontainer = cb.Build();

            return testcontainer.Resolve<IHistoryListLogic>();
        }

        private static IHistoryListHeaderRepository MakeMockHeaderRepo() {
            Mock<IHistoryListHeaderRepository> mockHeaderRepo = new Mock<IHistoryListHeaderRepository>();

            mockHeaderRepo.Setup(h => h.GetHistoryListHeader(
                                                             It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                                             c.CustomerId == "123456")))
                          .Returns(new HistoryListHeader {
                              BranchId = "FUT",
                              CustomerNumber = "123456",
                              CreatedUtc = It.IsAny<DateTime>(),
                              Id = 1,
                              ModifiedUtc = It.IsAny<DateTime>()
                          });

            return mockHeaderRepo.Object;
        }

        private static IHistoryListDetailRepository MakeMockDetailsRepo() {
            Mock<IHistoryListDetailRepository> mockDetailsRepo = new Mock<IHistoryListDetailRepository>();

            mockDetailsRepo.Setup(h => h.GetAllHistoryDetails(It.Is<long>(l => l == 1)))
                           .Returns(new List<HistoryListDetail> {
                               new HistoryListDetail {
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

        public class GetListModel {
            [Fact]
            public void BadBranchId_ReturnsNull() {
                // arrange
                IHistoryListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();

                // act
                ListModel results = testunit.GetListModel(fakeUser, testcontext, 0);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void BadCustomerId_ReturnsNull() {
                // arrange
                IHistoryListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };
                UserProfile fakeUser = new UserProfile();

                // act
                ListModel results = testunit.GetListModel(fakeUser, testcontext, 0);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsExpectedList() {
                // arrange
                IHistoryListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                string expected = "123456";

                // act
                ListModel results = testunit.GetListModel(fakeUser, testcontext, 0);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }
        }

        public class ReadList {
            [Fact]
            public void BadBranchId_ReturnsNull() {
                // arrange
                IHistoryListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                bool headerOnly = false;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, headerOnly);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void BadCustomerId_ReturnsNull() {
                // arrange
                IHistoryListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };
                UserProfile fakeUser = new UserProfile();
                bool headerOnly = false;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, headerOnly);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsExpectedList() {
                // arrange
                IHistoryListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                string expected = "123456";
                bool headerOnly = false;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, headerOnly);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomerIdAndBranchHeaderOnlyFalse_ReturnsExpectedNumberOfItems() {
                // arrange
                IHistoryListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                int expected = 1;
                bool headerOnly = false;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, headerOnly);

                // assert
                results.Items
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomerIdAndBranchHeaderOnlyTrue_ReturnsExpectedNumberOfItems() {
                // arrange
                IHistoryListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                int expected = 0;
                bool headerOnly = true;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, headerOnly);

                // assert
                results.Items
                       .Count
                       .Should()
                       .Be(expected);
            }
        }

        public class ItemsInHistoryList {
            [Fact]
            public void BadBranchId_ReturnsListWithInListItemsAndInHistoryFalse() {
                // arrange
                IHistoryListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                List<string> InList = new List<string> {"123456"};
                bool expected = false;

                // act
                List<InHistoryReturnModel> results = testunit.ItemsInHistoryList(testcontext, InList);

                // assert
                results.First()
                       .InHistory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void BadCustomerId_ReturnsListWithInListItemsAndInHistoryFalse() {
                // arrange
                IHistoryListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };
                List<string> InList = new List<string> {"123456"};
                bool expected = false;

                // act
                List<InHistoryReturnModel> results = testunit.ItemsInHistoryList(testcontext, InList);

                // assert
                results.First()
                       .InHistory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsListWithInListItemsAndInHistoryTrue() {
                // arrange
                IHistoryListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                List<string> InList = new List<string> {"123456"};
                bool expected = true;

                // act
                List<InHistoryReturnModel> results = testunit.ItemsInHistoryList(testcontext, InList);

                // assert
                results.First()
                       .InHistory
                       .Should()
                       .Be(expected);
            }
        }
    }
}