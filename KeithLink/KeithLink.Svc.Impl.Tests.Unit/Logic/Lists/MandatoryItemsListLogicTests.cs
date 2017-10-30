using System.Collections.Generic;
using System.Linq;

using Autofac;

using FluentAssertions;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.MandatoryItem;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic.Lists;

using Moq;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Lists {
    public class MandatoryItemsListLogicTests : BaseDITests {
        private static IMandatoryItemsListLogic MakeTestsObject() {
            ContainerBuilder cb = GetTestsContainer();

            // Register mocks
            cb.RegisterInstance(MakeMockHeaderRepo())
              .As<IMandatoryItemsListHeadersRepository>();
            cb.RegisterInstance(MakeMockDetailsRepo())
              .As<IMandatoryItemsListDetailsRepository>();

            IContainer testcontainer = cb.Build();

            return testcontainer.Resolve<IMandatoryItemsListLogic>();
        }

        private static IMandatoryItemsListHeadersRepository MakeMockHeaderRepo() {
            Mock<IMandatoryItemsListHeadersRepository> mockHeaderRepo = new Mock<IMandatoryItemsListHeadersRepository>();

            mockHeaderRepo.Setup(h => h.GetListHeaderForCustomer(
                                                                 It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                                                 c.CustomerId == "123456")))
                          .Returns(new MandatoryItemsListHeader {
                              BranchId = "FUT",
                              CustomerNumber = "123456",
                              Id = 1
                          });

            return mockHeaderRepo.Object;
        }

        private static IMandatoryItemsListDetailsRepository MakeMockDetailsRepo() {
            Mock<IMandatoryItemsListDetailsRepository> mockDetailsRepo = new Mock<IMandatoryItemsListDetailsRepository>();

            mockDetailsRepo.Setup(h => h.GetAllByHeader(It.Is<long>(l => l == 1)))
                           .Returns(new List<MandatoryItemsListDetail> {
                               new MandatoryItemsListDetail {
                                   CatalogId = "FUT",
                                   ItemNumber = "123456",
                                   Each = false,
                                   LineNumber = 1,
                                   Id = 1
                               }
                           });

            return mockDetailsRepo.Object;
        }

        public class GetMandatoryItemNumbers {
            [Fact]
            public void BadBranchId_ReturnsEmptyList() {
                // arrange
                IMandatoryItemsListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };

                int emptylist = 0;
                UserProfile fakeUser = new UserProfile();

                // act
                List<string> results = testunit.GetMandatoryItemNumbers(fakeUser, testcontext);

                // assert
                results.Count()
                       .Should()
                       .Be(emptylist);
            }

            [Fact]
            public void BadCustomerId_ReturnsEmptyList() {
                // arrange
                IMandatoryItemsListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };

                int emptylist = 0;
                UserProfile fakeUser = new UserProfile();

                // act
                List<string> results = testunit.GetMandatoryItemNumbers(fakeUser, testcontext);

                // assert
                results.Count()
                       .Should()
                       .Be(emptylist);
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsExpectedList() {
                // arrange
                IMandatoryItemsListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

                string expectedItem = "123456";
                UserProfile fakeUser = new UserProfile();

                // act
                List<string> results = testunit.GetMandatoryItemNumbers(fakeUser, testcontext);

                // assert
                results.First()
                       .Should()
                       .Be(expectedItem);
            }
        }

        public class GetListModel {
            [Fact]
            public void BadBranchId_ReturnsNull() {
                // arrange
                IMandatoryItemsListLogic testunit = MakeTestsObject();
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
                IMandatoryItemsListLogic testunit = MakeTestsObject();
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
            public void GoodCustomerIdAndBranch_ReturnsExpectedList() {
                // arrange
                IMandatoryItemsListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                int fakeId = 0;
                int expectedListId = 1;

                // act
                ListModel results = testunit.GetListModel(fakeUser, testcontext, fakeId);

                // assert
                results.ListId
                       .Should()
                       .Be(expectedListId);
            }
        }

        public class ReadList {
            [Fact]
            public void BadBranchId_ReturnsNull() {
                // arrange
                IMandatoryItemsListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, false);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void BadCustomerId_ReturnsNull() {
                // arrange
                IMandatoryItemsListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };
                UserProfile fakeUser = new UserProfile();

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, false);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsExpectedList() {
                // arrange
                IMandatoryItemsListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

                int expected = 1;
                UserProfile fakeUser = new UserProfile();

                // act
                ListModel results = testunit.ReadList(fakeUser, testcontext, false);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }
        }

        public class CreateOrUpdateList {
            // works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCustomerIdAndBranch_CallsSaveHeaderEveryTime() {
                // arrange
                Mock<IMandatoryItemsListHeadersRepository> mockHeaderRepo = new Mock<IMandatoryItemsListHeadersRepository>();
                Mock<IMandatoryItemsListDetailsRepository> mockDetailsRepo = new Mock<IMandatoryItemsListDetailsRepository>();
                MandatoryItemsListLogicImpl testunit = new MandatoryItemsListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                MandatoryItemsListDetail testDetail = new MandatoryItemsListDetail {
                    CatalogId = "FUT",
                    ItemNumber = "123456",
                    Each = false,
                    LineNumber = 1,
                    Id = 1
                };

                // act
                testunit.SaveDetail(testcontext, testDetail);

                // assert - Always returns what is setup provided the mock is called
                mockDetailsRepo.Verify(h => h.Save(It.IsAny<MandatoryItemsListDetail>()), Times.Once(), "Error updating");
            }
        }

        public class DeleteMandatoryItems {
            // works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCustomerIdAndBranch_CallsDeleteDetail() {
                // arrange
                Mock<IMandatoryItemsListHeadersRepository> mockHeaderRepo = new Mock<IMandatoryItemsListHeadersRepository>();
                Mock<IMandatoryItemsListDetailsRepository> mockDetailsRepo = new Mock<IMandatoryItemsListDetailsRepository>();
                MandatoryItemsListLogicImpl testunit = new MandatoryItemsListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                int testId = 1;
                MandatoryItemsListDetail testDetail = new MandatoryItemsListDetail {
                    CatalogId = "FUT",
                    ItemNumber = "123456",
                    Each = false,
                    LineNumber = 1,
                    Id = testId
                };

                // act
                testunit.DeleteMandatoryItems(testDetail);

                // assert - Always returns what is setup provided the mock is called
                mockDetailsRepo.Verify(h => h.Delete(testId), Times.Once(), "Error updating");
            }
        }

        public class SaveList {
            [Fact]
            public void AnyCustomerIdAndBranch_WhenIsDeleteIsFalseAndActiveIsFalseSetsActiveToTrue() {
                // arrange
                Mock<IMandatoryItemsListHeadersRepository> mockHeaderRepo = new Mock<IMandatoryItemsListHeadersRepository>();
                Mock<IMandatoryItemsListDetailsRepository> mockDetailsRepo = new Mock<IMandatoryItemsListDetailsRepository>();
                MandatoryItemsListLogicImpl testunit = new MandatoryItemsListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeProfile = new UserProfile();
                ListModel testList = new ListModel {
                    ListId = 1,
                    BranchId = "FUT",
                    Items = new List<ListItemModel> {
                        new ListItemModel {
                            ListItemId = 1,
                            ItemNumber = "123456",
                            Active = false,
                            IsDelete = false
                        }
                    }
                };

                // act
                testunit.SaveList(fakeProfile, testcontext, testList);

                // assert - Always returns what is setup provided the mock is called
                mockDetailsRepo.Verify(h => h.Save(It.Is<MandatoryItemsListDetail>(d => d.Active.Equals(true))), Times.Once(), "Error updating");
            }

            [Fact]
            public void AnyCustomerIdAndBranch_WhenIsDeleteIsTrueAndActiveIsTrueSetsActiveToFalse() {
                // arrange
                Mock<IMandatoryItemsListHeadersRepository> mockHeaderRepo = new Mock<IMandatoryItemsListHeadersRepository>();
                Mock<IMandatoryItemsListDetailsRepository> mockDetailsRepo = new Mock<IMandatoryItemsListDetailsRepository>();
                MandatoryItemsListLogicImpl testunit = new MandatoryItemsListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeProfile = new UserProfile();
                ListModel testList = new ListModel {
                    ListId = 1,
                    BranchId = "FUT",
                    Items = new List<ListItemModel> {
                        new ListItemModel {
                            ListItemId = 1,
                            ItemNumber = "123456",
                            Active = true,
                            IsDelete = true
                        }
                    }
                };

                // act
                testunit.SaveList(fakeProfile, testcontext, testList);

                // assert - Always returns what is setup provided the mock is called
                mockDetailsRepo.Verify(h => h.Save(It.Is<MandatoryItemsListDetail>(d => d.Active.Equals(false))), Times.Once(), "Error updating");
            }
        }

        public class CreateList {
            // works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCustomerIdAndBranch_Completes() {
                // arrange
                Mock<IMandatoryItemsListHeadersRepository> mockHeaderRepo = new Mock<IMandatoryItemsListHeadersRepository>();
                Mock<IMandatoryItemsListDetailsRepository> mockDetailsRepo = new Mock<IMandatoryItemsListDetailsRepository>();
                MandatoryItemsListLogicImpl testunit = new MandatoryItemsListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                // act
                testunit.CreateList(fakeUser, testcontext);

                // assert - Always returns what is setup provided the mock is called
                mockHeaderRepo.Verify(h => h.SaveMandatoryItemsHeader(It.IsAny<MandatoryItemsListHeader>()), Times.Once(), "Error updating");
            }
        }
    }
}