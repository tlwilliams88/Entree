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
using KeithLink.Svc.Core.Models.Lists.RecentlyOrdered;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic.Lists;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Lists
{
    public class RecentlyOrderedListLogicTests : BaseDITests
    {
        private static IRecentlyOrderedListLogic MakeTestsObject()
        {
            ContainerBuilder cb = GetTestsContainer();

            // Register mocks
            cb.RegisterInstance(MakeMockHeaderRepo())
              .As<IRecentlyOrderedListHeadersRepository>();
            cb.RegisterInstance(MakeMockDetailsRepo())
              .As<IRecentlyOrderedListDetailsRepository>();

            var testcontainer = cb.Build();

            return testcontainer.Resolve<IRecentlyOrderedListLogic>();
        }

        private static IRecentlyOrderedListHeadersRepository MakeMockHeaderRepo()
        {
            var mockHeaderRepo = new Mock<IRecentlyOrderedListHeadersRepository>();

            mockHeaderRepo.Setup(h => h.GetRecentlyOrderedHeader(It.IsAny<Guid>(),
                                                         It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                                    c.CustomerId == "123456")))
                          .Returns(new RecentlyOrderedListHeader()
                          {
                              BranchId = "FUT",
                              CustomerNumber = "123456",
                              Id = 1
                          });

            return mockHeaderRepo.Object;
        }

        private static IRecentlyOrderedListDetailsRepository MakeMockDetailsRepo()
        {
            var mockDetailsRepo = new Mock<IRecentlyOrderedListDetailsRepository>();

            mockDetailsRepo.Setup(h => h.GetRecentlyOrderedDetails(It.Is<long>(l => l == 1)))
                           .Returns(new List<RecentlyOrderedListDetail>()
                           {
                               new RecentlyOrderedListDetail() {
                                                             CatalogId = "FUT",
                                                             ItemNumber = "123456",
                                                             Each = false,
                                                             LineNumber = 1,
                                                             Id = 1
                                                      }
                           });

            return mockDetailsRepo.Object;
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
                var results = testunit.ReadList(fakeUser, testcontext, headerOnly);

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
                var results = testunit.ReadList(fakeUser, testcontext, headerOnly);

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
                var expectedListId = 1;
                var fakeUser = new UserProfile();
                var headerOnly = false;

                // act
                var results = testunit.ReadList(fakeUser, testcontext, headerOnly);

                // assert
                results.ListId
                       .Should()
                       .Be(expectedListId);
            }
        }

        public class PostRecentOrder
        { // works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCustomerIdAndBranch_CallsSaveDetailEveryTime()
            {
                // arrange
                var mockHeaderRepo = new Mock<IRecentlyOrderedListHeadersRepository>();
                var mockDetailsRepo = new Mock<IRecentlyOrderedListDetailsRepository>();
                var testunit = new RecentlyOrderedListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();
                var testRecentOrder = new RecentNonBEKList()
                {
                    Catalog = "FUT",
                    Items = new List<RecentNonBEKItem>() {
                                                             new RecentNonBEKItem() {
                                                                                        CatalogId="FUT",
                                                                                        ItemNumber = "111111"
                                                                                    }
                                                         }
                };

                // act
                testunit.PostRecentOrder(fakeUser, testcontext, testRecentOrder);

                // assert - Always returns what is setup provided the mock is called
                mockDetailsRepo.Verify(h => h.Save(It.IsAny<RecentlyOrderedListDetail>()), Times.Once(), "Error updating");
            }
        }

        public class DeleteAll
        { // works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void GoodCustomerIdAndBranch_CallsDeleteDetail()
            {
                // arrange
                var mockHeaderRepo = new Mock<IRecentlyOrderedListHeadersRepository>();

                mockHeaderRepo.Setup(h => h.GetRecentlyOrderedHeader(It.IsAny<Guid>(),
                                                             It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                                        c.CustomerId == "123456")))
                              .Returns(new RecentlyOrderedListHeader()
                              {
                                  BranchId = "FUT",
                                  CustomerNumber = "123456",
                                  Id = 1
                              });

                var mockDetailsRepo = new Mock<IRecentlyOrderedListDetailsRepository>();
                var testunit = new RecentlyOrderedListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();

                // act
                testunit.DeleteAll(fakeUser, testcontext);

                // assert - Always returns what is setup provided the mock is called
                mockDetailsRepo.Verify(h => h.DeleteOldRecentlyOrdered(It.IsAny<long>(), It.IsAny<int>()), Times.Once(), "Error updating");
            }

            [Fact]
            public void BadCustomerIdAndBranch_CompletesWithoutError()
            {
                // arrange
                var mockHeaderRepo = new Mock<IRecentlyOrderedListHeadersRepository>();
                var mockDetailsRepo = new Mock<IRecentlyOrderedListDetailsRepository>();
                var testunit = new RecentlyOrderedListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();

                // act
                testunit.DeleteAll(fakeUser, testcontext);

                // assert - Always returns what is setup provided the mock is called
                mockDetailsRepo.Verify(h => h.DeleteOldRecentlyOrdered(It.IsAny<long>(), It.IsAny<int>()), Times.Never, "Error updating");
            }

        }

    }
}
