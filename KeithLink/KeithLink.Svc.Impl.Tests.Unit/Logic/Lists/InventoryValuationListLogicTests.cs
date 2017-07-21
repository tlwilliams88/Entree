using System;
using System.Collections.Generic;
using System.Linq;

using Autofac;
using FluentAssertions;
using Moq;
using Xunit;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic.Lists;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Lists
{
    public class InventoryValuationListLogicTests : BaseDITests
    {
        private static IInventoryValuationListLogic MakeTestsObject()
        {
            ContainerBuilder cb = GetTestsContainer();

            // Register mocks
            cb.RegisterInstance(MakeMockHeaderRepo())
              .As<IInventoryValuationListHeadersRepository>();
            cb.RegisterInstance(MakeMockDetailsRepo())
              .As<IInventoryValuationListDetailsRepository>();

            var testcontainer = cb.Build();

            return testcontainer.Resolve<IInventoryValuationListLogic>();
        }

        private static IInventoryValuationListHeadersRepository MakeMockHeaderRepo()
        {
            var mockHeaderRepo = new Mock<IInventoryValuationListHeadersRepository>();

            mockHeaderRepo.Setup(h => h.GetInventoryValuationListHeaders(
                                                         It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                                    c.CustomerId == "123456")))
                          .Returns(new List<InventoryValuationListHeader>()
                          {
                              new InventoryValuationListHeader() { 
                                  BranchId = "FUT",
                                  CustomerNumber = "123456",
                                  CreatedUtc = It.IsAny<DateTime>(),
                                  Id = 1,
                                  ModifiedUtc = It.IsAny<DateTime>()
                              }
                          });

            mockHeaderRepo.Setup(h => h.GetInventoryValuationListHeader(It.Is<long>(l => l == 1)))
                          .Returns(new InventoryValuationListHeader() {
                                                                          BranchId = "FUT",
                                                                          CustomerNumber = "123456",
                                                                          CreatedUtc = It.IsAny<DateTime>(),
                                                                          Id = 1,
                                                                          ModifiedUtc = It.IsAny<DateTime>()
                                                                      });

            return mockHeaderRepo.Object;
        }

        private static IInventoryValuationListDetailsRepository MakeMockDetailsRepo()
        {
            var mockDetailsRepo = new Mock<IInventoryValuationListDetailsRepository>();

            mockDetailsRepo.Setup(h => h.GetInventoryValuationDetails(It.Is<long>(l => l == 1)))
                           .Returns(new List<InventoryValuationListDetail>()
                           {
                               new InventoryValuationListDetail() {
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

        public class ReadLists
        {
            [Fact]
            public void BadBranchId_ReturnsEmptyList()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();
                var expectedEmptyListCount = 0;

                // act
                var results = testunit.ReadLists(fakeUser, testcontext, false);

                // assert
                results.Count()
                       .Should()
                       .Be(expectedEmptyListCount);
            }

            [Fact]
            public void BadCustomerId_ReturnsEmptyList()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };
                var fakeUser = new UserProfile();
                var expectedEmptyListCount = 0;

                // act
                var results = testunit.ReadLists(fakeUser, testcontext, false);

                // assert
                results.Count()
                       .Should()
                       .Be(expectedEmptyListCount);
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
                var expectedListId = 1;

                // act
                var results = testunit.ReadLists(fakeUser, testcontext, false);

                // assert
                results.First()
                       .ListId
                       .Should()
                       .Be(expectedListId);
            }
        }

        public class ReadList
        {
            [Fact]
            public void BadBranchId_ReturnsExpectedListId()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var expectedListId = 1;

                // act
                var results = testunit.ReadList(expectedListId, testcontext, false);

                // assert
                results.ListId
                       .Should()
                       .Be(expectedListId);
            }

            [Fact]
            public void BadCustomerId_ReturnsExpectedList()
            {
                // arrange
                var testunit = MakeTestsObject();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };
                var expectedListId = 1;

                // act
                var results = testunit.ReadList(expectedListId, testcontext, false);

                // assert
                results.ListId
                       .Should()
                       .Be(expectedListId);
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

                // act
                var results = testunit.ReadList(expectedListId, testcontext, false);

                // assert
                results.ListId
                       .Should()
                       .Be(expectedListId);
            }
        }

        public class CreateOrUpdateList
        { // works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCustomerIdAndBranch_CallsHeaderRepoSave()
            {
                // arrange
                var mockHeaderRepo = new Mock<IInventoryValuationListHeadersRepository>();
                var mockDetailsRepo = new Mock<IInventoryValuationListDetailsRepository>();
                var testunit = new InventoryValuationListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();
                var fakeId = (long)1;
                var fakeName = "Fake Name";
                var testActive = true;
                var expected = true;

                // act
                var results = testunit.CreateOrUpdateList(fakeUser, testcontext, fakeId, fakeName, testActive);

                // assert - Always returns what is setup provided the mock is called
                mockHeaderRepo.Verify(h => h.SaveInventoryValuationListHeader(It.IsAny<InventoryValuationListHeader>()), Times.Once(), "Error updating");
            }
        }

        public class SaveItem
        { // works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCustomerIdAndBranch_CallsDetailsRepoSaveInventoryValuationDetail()
            {
                // arrange
                var mockHeaderRepo = new Mock<IInventoryValuationListHeadersRepository>();
                var mockDetailsRepo = new Mock<IInventoryValuationListDetailsRepository>();
                var testunit = new InventoryValuationListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();
                var fakeId = (long)1;
                var testItem =
                    new InventoryValuationListDetail()
                    {
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

        public class SaveList
        {
            [Fact]
            public void CallToSaveList_DoesCallHeaderRepo()
            {
                // arrange
                var mockHeaderRepo = new Mock<IInventoryValuationListHeadersRepository>();
                var mockDetailsRepo = new Mock<IInventoryValuationListDetailsRepository>();
                var testunit = new InventoryValuationListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();
                var testList = new ListModel() {
                                                   ListId = 1,
                                                   CustomerNumber = "123456",
                                                   BranchId = "FUT",
                                                   Items = new List<ListItemModel>() {
                                                                                         new ListItemModel() {
                                                                                                                 ItemNumber = "123456"
                                                                                                             }
                                                                                     }
                                               };

                // act
                var results = testunit.SaveList(fakeUser, testcontext, testList);

                // assert
                mockHeaderRepo.Verify(h => h.SaveInventoryValuationListHeader(It.IsAny<InventoryValuationListHeader>()), Times.Once(), "Error updating");
            }

            [Fact]
            public void CallToSaveList_DoesCallDetailRepo()
            {
                // arrange
                var mockHeaderRepo = new Mock<IInventoryValuationListHeadersRepository>();
                var mockDetailsRepo = new Mock<IInventoryValuationListDetailsRepository>();
                var testunit = new InventoryValuationListLogicImpl(mockHeaderRepo.Object, mockDetailsRepo.Object);
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();
                var testList = new ListModel()
                {
                    ListId = 1,
                    CustomerNumber = "123456",
                    BranchId = "FUT",
                    Items = new List<ListItemModel>() {
                                                                                         new ListItemModel() {
                                                                                                                 ItemNumber = "123456"
                                                                                                             }
                                                                                     }
                };

                // act
                var results = testunit.SaveList(fakeUser, testcontext, testList);

                // assert
                mockDetailsRepo.Verify(h => h.SaveInventoryValuationDetail(It.IsAny<InventoryValuationListDetail>()), Times.AtLeastOnce, "Error updating");
            }
        }
    }
}
