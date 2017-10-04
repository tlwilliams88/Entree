using System.Collections.Generic;

using Autofac;

using FluentAssertions;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.RecommendedItems;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic.Lists;

using Moq;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Lists {
    public class RecommendedItemsListLogicTests : BaseDITests {
        private static IRecommendedItemsListLogic MakeTestsObject() {
            ContainerBuilder cb = GetTestsContainer();

            // Register mocks
            cb.RegisterInstance(MakeMockHeaderRepo())
              .As<IRecommendedItemsListHeadersRepository>();
            cb.RegisterInstance(MakeMockDetailsRepo())
              .As<IRecommendedItemsListDetailsRepository>();

            IContainer testcontainer = cb.Build();

            return testcontainer.Resolve<IRecommendedItemsListLogic>();
        }

        private static IRecommendedItemsListLogic MakeTestObject(out Mock<IRecommendedItemsListHeadersRepository> mockHeadersRepository,
                                                                 out Mock<IRecommendedItemsListDetailsRepository> mockDetailsRepository) {
            mockHeadersRepository = new Mock<IRecommendedItemsListHeadersRepository>();
            mockDetailsRepository = new Mock<IRecommendedItemsListDetailsRepository>();
            RecommendedItemsListLogicImpl testunit = new RecommendedItemsListLogicImpl(mockHeadersRepository.Object, mockDetailsRepository.Object);
            return testunit;
        }

        private static IRecommendedItemsListHeadersRepository MakeMockHeaderRepo() {
            Mock<IRecommendedItemsListHeadersRepository> mockHeaderRepo = new Mock<IRecommendedItemsListHeadersRepository>();

            mockHeaderRepo.Setup(h => h.GetRecommendedItemsHeaderByCustomerNumberBranch(It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                                                                        c.CustomerId == "123456")))
                          .Returns(new RecommendedItemsListHeader {
                              BranchId = "FUT",
                              CustomerNumber = "123456",
                              Id = 1
                          });

            return mockHeaderRepo.Object;
        }

        private static IRecommendedItemsListDetailsRepository MakeMockDetailsRepo() {
            Mock<IRecommendedItemsListDetailsRepository> mockDetailsRepo = new Mock<IRecommendedItemsListDetailsRepository>();

            mockDetailsRepo.Setup(h => h.GetAllByHeader(It.Is<long>(l => l == 1)))
                           .Returns(new List<RecommendedItemsListDetail> {
                               new RecommendedItemsListDetail {
                                   CatalogId = "FUT",
                                   ItemNumber = "123456",
                                   Each = false,
                                   LineNumber = 1,
                                   Id = 1
                               }
                           });

            return mockDetailsRepo.Object;
        }

        public class CreateList {
// works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCustomerIdAndBranch_CallsSaveHeader() {
                // arrange
                Mock<IRecommendedItemsListHeadersRepository> mockHeadersRepo;
                Mock<IRecommendedItemsListDetailsRepository> mockDetailsRepo;
                IRecommendedItemsListLogic testunit = MakeTestObject(out mockHeadersRepo, out mockDetailsRepo);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

                // act
                testunit.CreateList(testcontext);

                // assert - Always returns what is setup provided the mock is called
                mockHeadersRepo.Verify(h => h.SaveRecommendedItemsHeader(It.IsAny<RecommendedItemsListHeader>()), Times.Once(), "Error updating");
            }
        }

        public class ReadList {
            [Fact]
            public void BadBranchId_ReturnsNull() {
                // arrange
                IRecommendedItemsListLogic testunit = MakeTestsObject();
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
                IRecommendedItemsListLogic testunit = MakeTestsObject();
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
                IRecommendedItemsListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                int expectedListId = 1;
                UserProfile fakeUser = new UserProfile();
                bool headerOnly = false;

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, headerOnly);

                // assert
                results.ListId
                       .Should()
                       .Be(expectedListId);
            }
        }

        public class SaveDetail {
// works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCustomerIdAndBranch_CallsSaveDetail() {
                // arrange
                Mock<IRecommendedItemsListHeadersRepository> mockHeadersRepo;
                Mock<IRecommendedItemsListDetailsRepository> mockDetailsRepo;
                IRecommendedItemsListLogic testunit = MakeTestObject(out mockHeadersRepo, out mockDetailsRepo);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                RecommendedItemsListDetail testdetail = new RecommendedItemsListDetail {
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

        public class DeleteRecommendedItems {
            [Fact]
            public void BadCustomerIdAndBranch_CompletesWithoutError() {
                // arrange
                Mock<IRecommendedItemsListHeadersRepository> mockHeadersRepo;
                Mock<IRecommendedItemsListDetailsRepository> mockDetailsRepo;
                IRecommendedItemsListLogic testunit = MakeTestObject(out mockHeadersRepo, out mockDetailsRepo);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();

                // act
                testunit.DeleteRecommendedItems(fakeUser, testcontext);

                // assert - Always returns what is setup provided the mock is called
            } // works differently if you want to verify a mock is called; we can't go through autofac

            [Fact]
            public void GoodCustomerIdAndBranch_CallsDeleteDetail() {
                // arrange
                Mock<IRecommendedItemsListHeadersRepository> mockHeadersRepo;
                Mock<IRecommendedItemsListDetailsRepository> mockDetailsRepo;
                IRecommendedItemsListLogic testunit = MakeTestObject(out mockHeadersRepo, out mockDetailsRepo);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();

                mockHeadersRepo.Setup(h => h.GetRecommendedItemsHeaderByCustomerNumberBranch(It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                                                                             c.CustomerId == "123456")))
                               .Returns(new RecommendedItemsListHeader {
                                   BranchId = "FUT",
                                   CustomerNumber = "123456",
                                   Id = 1
                               });

                // act
                testunit.DeleteRecommendedItems(fakeUser, testcontext);

                // assert - Always returns what is setup provided the mock is called
                mockDetailsRepo.Verify(h => h.Delete(It.IsAny<long>()), Times.Once(), "Error updating");
            }
        }
    }
}