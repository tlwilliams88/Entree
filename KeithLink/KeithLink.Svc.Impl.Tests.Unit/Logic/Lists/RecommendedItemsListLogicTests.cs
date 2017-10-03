using System.Collections.Generic;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.RecommendedItems;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic.Lists;

using Autofac;
using FluentAssertions;
using Moq;
using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Lists
{
    public class RecommendedItemsListLogicTests : BaseDITests
    {
        private static IRecommendedItemsListLogic MakeTestsObject()
        {
            ContainerBuilder cb = GetTestsContainer();

            // Register mocks
            cb.RegisterInstance(MakeMockHeaderRepo())
              .As<IRecommendedItemsListHeadersRepository>();
            cb.RegisterInstance(MakeMockDetailsRepo())
              .As<IRecommendedItemsListDetailsRepository>();

            var testcontainer = cb.Build();

            return testcontainer.Resolve<IRecommendedItemsListLogic>();
        }

        private static IRecommendedItemsListLogic MakeTestObject(out Mock<IRecommendedItemsListHeadersRepository> mockHeadersRepository,
                                                               out Mock<IRecommendedItemsListDetailsRepository> mockDetailsRepository)
        {
            mockHeadersRepository = new Mock<IRecommendedItemsListHeadersRepository>();
            mockDetailsRepository = new Mock<IRecommendedItemsListDetailsRepository>();
            var testunit = new RecommendedItemsListLogicImpl(mockHeadersRepository.Object, mockDetailsRepository.Object);
            return testunit;
        }

        private static IRecommendedItemsListHeadersRepository MakeMockHeaderRepo()
        {
            var mockHeaderRepo = new Mock<IRecommendedItemsListHeadersRepository>();

            mockHeaderRepo.Setup(h => h.GetRecommendedItemsHeaderByCustomerNumberBranch(It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                                                                        c.CustomerId == "123456")))
                          .Returns(new RecommendedItemsListHeader()
                          {
                              BranchId = "FUT",
                              CustomerNumber = "123456",
                              Id = 1
                          });

            return mockHeaderRepo.Object;
        }

        private static IRecommendedItemsListDetailsRepository MakeMockDetailsRepo()
        {
            var mockDetailsRepo = new Mock<IRecommendedItemsListDetailsRepository>();

            mockDetailsRepo.Setup(h => h.GetAllByHeader(It.Is<long>(l => l == 1)))
                           .Returns(new List<RecommendedItemsListDetail>()
                           {
                               new RecommendedItemsListDetail() {
                                                             CatalogId = "FUT",
                                                             ItemNumber = "123456",
                                                             Each = false,
                                                             LineNumber = 1,
                                                             Id = 1
                                                      }
                           });

            return mockDetailsRepo.Object;
        }

        public class CreateList
        { // works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCustomerIdAndBranch_CallsSaveHeader()
            {
                // arrange
                Mock<IRecommendedItemsListHeadersRepository> mockHeadersRepo;
                Mock<IRecommendedItemsListDetailsRepository> mockDetailsRepo;
                var testunit = MakeTestObject(out mockHeadersRepo, out mockDetailsRepo);
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

                // act
                testunit.CreateList(testcontext);

                // assert - Always returns what is setup provided the mock is called
                mockHeadersRepo.Verify(h => h.SaveRecommendedItemsHeader(It.IsAny<RecommendedItemsListHeader>()), Times.Once(), "Error updating");
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

        public class SaveDetail
        { // works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCustomerIdAndBranch_CallsSaveDetail()
            {
                // arrange
                Mock<IRecommendedItemsListHeadersRepository> mockHeadersRepo;
                Mock<IRecommendedItemsListDetailsRepository> mockDetailsRepo;
                var testunit = MakeTestObject(out mockHeadersRepo, out mockDetailsRepo);
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var testdetail = new RecommendedItemsListDetail()
                {
                    CatalogId = "FUT",
                    ItemNumber = "123456",
                    Each = false,
                    LineNumber = 1,
                    Id = 1
                };

                // act
                testunit.SaveDetail(testcontext, testdetail);

                // assert - Always returns what is setup provided the mock is called
                mockDetailsRepo.Verify(h => h.Save(It.IsAny<RecommendedItemsListDetail>()), Times.Once(), "Error updating");
            }
        }

        public class DeleteRecommendedItems
        { // works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void GoodCustomerIdAndBranch_CallsDeleteDetail()
            {
                // arrange
                Mock<IRecommendedItemsListHeadersRepository> mockHeadersRepo;
                Mock<IRecommendedItemsListDetailsRepository> mockDetailsRepo;
                var testunit = MakeTestObject(out mockHeadersRepo, out mockDetailsRepo);
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();

                mockHeadersRepo.Setup(h => h.GetRecommendedItemsHeaderByCustomerNumberBranch(It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                                                                            c.CustomerId == "123456")))
                               .Returns(new RecommendedItemsListHeader()
                               {
                                   BranchId = "FUT",
                                   CustomerNumber = "123456",
                                   Id = 1
                               });

                // act
                testunit.DeleteRecommendedItems(fakeUser, testcontext);

                // assert - Always returns what is setup provided the mock is called
                mockDetailsRepo.Verify(h => h.Delete(It.IsAny<long>()), Times.Once(), "Error updating");
            }

            [Fact]
            public void BadCustomerIdAndBranch_CompletesWithoutError()
            {
                // arrange
                Mock<IRecommendedItemsListHeadersRepository> mockHeadersRepo;
                Mock<IRecommendedItemsListDetailsRepository> mockDetailsRepo;
                var testunit = MakeTestObject(out mockHeadersRepo, out mockDetailsRepo);
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();

                // act
                testunit.DeleteRecommendedItems(fakeUser, testcontext);

                // assert - Always returns what is setup provided the mock is called
            }
        }
    }
}