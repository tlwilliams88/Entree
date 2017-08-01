using System;
using System.Collections.Generic;
using System.Linq;

using Autofac;

using FluentAssertions;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic.Lists;

using Moq;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Lists {
    public class InventoryValuationListLogicTests : BaseDITests {
        private static IInventoryValuationListLogic MakeTestsObject() {
            ContainerBuilder cb = GetTestsContainer();

            // Register mocks
            cb.RegisterInstance(MakeMockHeaderRepo())
              .As<IInventoryValuationListHeadersRepository>();
            cb.RegisterInstance(MakeMockDetailsRepo())
              .As<IInventoryValuationListDetailsRepository>();

            IContainer testcontainer = cb.Build();

            return testcontainer.Resolve<IInventoryValuationListLogic>();
        }

        private static IInventoryValuationListHeadersRepository MakeMockHeaderRepo() {
            Mock<IInventoryValuationListHeadersRepository> mockHeaderRepo = new Mock<IInventoryValuationListHeadersRepository>();

            mockHeaderRepo.Setup(h => h.GetInventoryValuationListHeaders(
                                                                         It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                                                         c.CustomerId == "123456")))
                          .Returns(new List<InventoryValuationListHeader> {
                              new InventoryValuationListHeader {
                                  BranchId = "FUT",
                                  CustomerNumber = "123456",
                                  CreatedUtc = It.IsAny<DateTime>(),
                                  Id = 1,
                                  ModifiedUtc = It.IsAny<DateTime>()
                              }
                          });

            mockHeaderRepo.Setup(h => h.GetInventoryValuationListHeader(It.Is<long>(l => l == 1)))
                          .Returns(new InventoryValuationListHeader {
                              BranchId = "FUT",
                              CustomerNumber = "123456",
                              CreatedUtc = It.IsAny<DateTime>(),
                              Id = 1,
                              ModifiedUtc = It.IsAny<DateTime>()
                          });

            return mockHeaderRepo.Object;
        }

        private static IInventoryValuationListDetailsRepository MakeMockDetailsRepo() {
            Mock<IInventoryValuationListDetailsRepository> mockDetailsRepo = new Mock<IInventoryValuationListDetailsRepository>();

            mockDetailsRepo.Setup(h => h.GetInventoryValuationDetails(It.Is<long>(l => l == 1)))
                           .Returns(new List<InventoryValuationListDetail> {
                               new InventoryValuationListDetail {
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

        public class ReadLists {
            [Fact]
            public void BadBranchId_ReturnsEmptyList() {
                // arrange
                IInventoryValuationListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                int expectedEmptyListCount = 0;

                // act
                List<ListModel> results = testunit.ReadLists(fakeUser, testcontext, false);

                // assert
                results.Count()
                       .Should()
                       .Be(expectedEmptyListCount);
            }

            [Fact]
            public void BadCustomerId_ReturnsEmptyList() {
                // arrange
                IInventoryValuationListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };
                UserProfile fakeUser = new UserProfile();
                int expectedEmptyListCount = 0;

                // act
                List<ListModel> results = testunit.ReadLists(fakeUser, testcontext, false);

                // assert
                results.Count()
                       .Should()
                       .Be(expectedEmptyListCount);
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsExpectedList() {
                // arrange
                IInventoryValuationListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                int expectedListId = 1;

                // act
                List<ListModel> results = testunit.ReadLists(fakeUser, testcontext, false);

                // assert
                results.First()
                       .ListId
                       .Should()
                       .Be(expectedListId);
            }
        }

        public class ReadList {
            [Fact]
            public void BadBranchId_ReturnsExpectedListId() {
                // arrange
                IInventoryValuationListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                int expectedListId = 1;

                // act
                ListModel results = testunit.ReadList(expectedListId, testcontext, false);

                // assert
                results.ListId
                       .Should()
                       .Be(expectedListId);
            }

            [Fact]
            public void BadCustomerId_ReturnsExpectedList() {
                // arrange
                IInventoryValuationListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };
                int expectedListId = 1;

                // act
                ListModel results = testunit.ReadList(expectedListId, testcontext, false);

                // assert
                results.ListId
                       .Should()
                       .Be(expectedListId);
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsExpectedList() {
                // arrange
                IInventoryValuationListLogic testunit = MakeTestsObject();
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                int expectedListId = 1;

                // act
                ListModel results = testunit.ReadList(expectedListId, testcontext, false);

                // assert
                results.ListId
                       .Should()
                       .Be(expectedListId);
            }
        }

        public class CreateOrUpdateList {
// works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCustomerIdAndBranch_CallsHeaderRepoSave() {
                // arrange
                Mock<IInventoryValuationListHeadersRepository> mockHeaderRepo = new Mock<IInventoryValuationListHeadersRepository>();
                Mock<IInventoryValuationListDetailsRepository> mockDetailsRepo = new Mock<IInventoryValuationListDetailsRepository>();
                InventoryValuationListLogicImpl testunit = new InventoryValuationListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                long fakeId = 1;
                string fakeName = "Fake Name";
                bool testActive = true;
                bool expected = true;

                // act
                long results = testunit.CreateOrUpdateList(fakeUser, testcontext, fakeId, fakeName, testActive);

                // assert - Always returns what is setup provided the mock is called
                mockHeaderRepo.Verify(h => h.SaveInventoryValuationListHeader(It.IsAny<InventoryValuationListHeader>()), Times.Once(), "Error updating");
            }
        }

        public class SaveItem {
// works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCustomerIdAndBranch_CallsDetailsRepoSaveInventoryValuationDetail() {
                // arrange
                Mock<IInventoryValuationListHeadersRepository> mockHeaderRepo = new Mock<IInventoryValuationListHeadersRepository>();
                Mock<IInventoryValuationListDetailsRepository> mockDetailsRepo = new Mock<IInventoryValuationListDetailsRepository>();
                InventoryValuationListLogicImpl testunit = new InventoryValuationListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
                UserSelectedContext testcontext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile fakeUser = new UserProfile();
                long fakeId = 1;
                InventoryValuationListDetail testItem =
                        new InventoryValuationListDetail {
                            CatalogId = "FUT",
                            ItemNumber = "123456",
                            Each = false,
                            LineNumber = 1,
                            HeaderId = 1L
                        };

                // act
                testunit.SaveItem(fakeUser, testcontext, fakeId, testItem);

                // assert - Always returns what is setup provided the mock is called
                mockDetailsRepo.Verify(h => h.SaveInventoryValuationDetail(testItem), Times.Once(), "Error updating");
            }
        }

        public class SaveList {
            [Fact]
            public void CallToSaveList_DoesCallDetailRepo() {
                // arrange
                Mock<IInventoryValuationListHeadersRepository> mockHeaderRepo = new Mock<IInventoryValuationListHeadersRepository>();
                Mock<IInventoryValuationListDetailsRepository> mockDetailsRepo = new Mock<IInventoryValuationListDetailsRepository>();
                InventoryValuationListLogicImpl testunit = new InventoryValuationListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
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
                ListModel results = testunit.SaveList(fakeUser, testcontext, testList);

                // assert
                mockDetailsRepo.Verify(h => h.SaveInventoryValuationDetail(It.IsAny<InventoryValuationListDetail>()), Times.AtLeastOnce, "Error updating");
            }

            [Fact]
            public void CallToSaveList_DoesCallHeaderRepo() {
                // arrange
                Mock<IInventoryValuationListHeadersRepository> mockHeaderRepo = new Mock<IInventoryValuationListHeadersRepository>();
                Mock<IInventoryValuationListDetailsRepository> mockDetailsRepo = new Mock<IInventoryValuationListDetailsRepository>();
                InventoryValuationListLogicImpl testunit = new InventoryValuationListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
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
                ListModel results = testunit.SaveList(fakeUser, testcontext, testList);

                // assert
                mockHeaderRepo.Verify(h => h.SaveInventoryValuationListHeader(It.IsAny<InventoryValuationListHeader>()), Times.Once(), "Error updating");
            }

            [Fact]
            public void CallToSaveList_DoesSetActiveFlagToFalseProperly() {
                // arrange
                Mock<IInventoryValuationListHeadersRepository> mockHeaderRepo = new Mock<IInventoryValuationListHeadersRepository>();
                Mock<IInventoryValuationListDetailsRepository> mockDetailsRepo = new Mock<IInventoryValuationListDetailsRepository>();
                InventoryValuationListLogicImpl testunit = new InventoryValuationListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
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
                            ItemNumber = "123456",
                            Active = true,
                            IsDelete =  true
                        }
                    }
                };

                // act
                ListModel results = testunit.SaveList(fakeUser, testcontext, testList);

                // assert
                mockDetailsRepo.Verify(h => h.SaveInventoryValuationDetail(It.Is<InventoryValuationListDetail>(d => d.Active.Equals(false))), Times.Once(), "Error updating");
            }

            [Fact]
            public void CallToSaveList_DoesSetActiveFlagToTrueProperly() {
                // arrange
                Mock<IInventoryValuationListHeadersRepository> mockHeaderRepo = new Mock<IInventoryValuationListHeadersRepository>();
                Mock<IInventoryValuationListDetailsRepository> mockDetailsRepo = new Mock<IInventoryValuationListDetailsRepository>();
                InventoryValuationListLogicImpl testunit = new InventoryValuationListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
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
                            ItemNumber = "123456",
                            Active = true,
                            IsDelete =  false
                        }
                    }
                };

                // act
                ListModel results = testunit.SaveList(fakeUser, testcontext, testList);

                // assert
                mockDetailsRepo.Verify(h => h.SaveInventoryValuationDetail(It.Is<InventoryValuationListDetail>(d => d.Active.Equals(true))), Times.Once(), "Error updating");
            }

            [Fact]
            public void CallToSaveList_DoesSetActiveFlagToTrueWhenIsDeleteFalseAndActiveFalse() {
                // arrange
                Mock<IInventoryValuationListHeadersRepository> mockHeaderRepo = new Mock<IInventoryValuationListHeadersRepository>();
                Mock<IInventoryValuationListDetailsRepository> mockDetailsRepo = new Mock<IInventoryValuationListDetailsRepository>();
                InventoryValuationListLogicImpl testunit = new InventoryValuationListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
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
                            ItemNumber = "123456",
                            Active = false,
                            IsDelete =  false
                        }
                    }
                };

                // act
                ListModel results = testunit.SaveList(fakeUser, testcontext, testList);

                // assert
                mockDetailsRepo.Verify(h => h.SaveInventoryValuationDetail(It.Is<InventoryValuationListDetail>(d => d.Active.Equals(true))), Times.Once(), "Error updating");
            }

        }
    }
}