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
using KeithLink.Svc.Core.Models.Lists.RecentlyViewed;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic.Lists;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Lists
{
    public class RecentlyViewedListLogicTests : BaseDITests
    {
        private static IRecentlyViewedListLogic MakeTestsObject()
        {
            ContainerBuilder cb = GetTestsContainer();

            // Register mocks
            cb.RegisterInstance(MakeMockHeaderRepo())
              .As<IRecentlyViewedListHeadersRepository>();
            cb.RegisterInstance(MakeMockDetailsRepo())
              .As<IRecentlyViewedListDetailsRepository>();

            var testcontainer = cb.Build();

            return testcontainer.Resolve<IRecentlyViewedListLogic>();
        }

        private static IRecentlyViewedListLogic MakeTestObject(out Mock<IRecentlyViewedListHeadersRepository> mockHeadersRepository, 
                                                               out Mock<IRecentlyViewedListDetailsRepository> mockDetailsRepository)
        {
            mockHeadersRepository = new Mock<IRecentlyViewedListHeadersRepository>();
            mockDetailsRepository = new Mock<IRecentlyViewedListDetailsRepository>();
            var testunit = new RecentlyViewedListLogicImpl(mockHeadersRepository.Object, mockDetailsRepository.Object);
            return testunit;
        }

        private static IRecentlyViewedListHeadersRepository MakeMockHeaderRepo()
        {
            var mockHeaderRepo = new Mock<IRecentlyViewedListHeadersRepository>();

            mockHeaderRepo.Setup(h => h.GetRecentlyViewedHeader(It.IsAny<Guid>(),
                                                         It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                                    c.CustomerId == "123456")))
                          .Returns(new RecentlyViewedListHeader()
                          {
                              BranchId = "FUT",
                              CustomerNumber = "123456",
                              Id = 1
                          });

            return mockHeaderRepo.Object;
        }

        private static IRecentlyViewedListDetailsRepository MakeMockDetailsRepo()
        {
            var mockDetailsRepo = new Mock<IRecentlyViewedListDetailsRepository>();

            mockDetailsRepo.Setup(h => h.GetRecentlyViewedDetails(It.Is<long>(l => l == 1)))
                           .Returns(new List<RecentlyViewedListDetail>()
                           {
                               new RecentlyViewedListDetail() {
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

        public class PostRecentView
        { // works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCustomerIdAndBranch_CallsSaveDetailEveryTime()
            {
                // arrange
                Mock<IRecentlyViewedListHeadersRepository> mockHeadersRepo;
                Mock<IRecentlyViewedListDetailsRepository> mockDetailsRepo;
                var testunit = MakeTestObject(out mockHeadersRepo, out mockDetailsRepo);
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();
                var testRecentViewedItemNumber = "111111";

                // act
                testunit.PostRecentView(fakeUser, testcontext, testRecentViewedItemNumber);

                // assert - Always returns what is setup provided the mock is called
                mockDetailsRepo.Verify(h => h.Save(It.IsAny<RecentlyViewedListDetail>()), Times.Once(), "Error updating");
            }
        }

        public class DeleteAll
        { // works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void GoodCustomerIdAndBranch_CallsDeleteOldRecentlyViewedDetail()
            {
                // arrange
                Mock<IRecentlyViewedListHeadersRepository> mockHeadersRepo;
                Mock<IRecentlyViewedListDetailsRepository> mockDetailsRepo;
                var testunit = MakeTestObject(out mockHeadersRepo, out mockDetailsRepo);

                mockHeadersRepo.Setup(h => h.GetRecentlyViewedHeader(It.IsAny<Guid>(),
                                                                     It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                                                c.CustomerId == "123456")))
                              .Returns(new RecentlyViewedListHeader()
                              {
                                  BranchId = "FUT",
                                  CustomerNumber = "123456",
                                  Id = 1
                              });

                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();

                // act
                testunit.DeleteAll(fakeUser, testcontext);

                // assert - Always returns what is setup provided the mock is called
                mockDetailsRepo.Verify(h => h.DeleteOldRecentlyViewed(It.IsAny<long>(), It.IsAny<int>()), Times.Once(), "Error updating");
            }

            [Fact]
            public void BadCustomerIdAndBranch_CompletesWithoutError()
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
                testunit.DeleteAll(fakeUser, testcontext);

                // assert - Always returns what is setup provided the mock is called
            }

        }

    }
}
