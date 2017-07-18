using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using FluentAssertions;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;
using KeithLink.Svc.Core.Models.Profile;

using Moq;
using Xunit;
using KeithLink.Svc.Core.Models.SiteCatalog;

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

            mockHeaderRepo.Setup(h => h.SaveInventoryValuationListHeader(new InventoryValuationListHeader()
                                                                                                            {
                                                                                                                BranchId = "FUT",
                                                                                                                CustomerNumber = "123456",
                                                                                                                CreatedUtc = It.IsAny<DateTime>(),
                                                                                                                Id = 1,
                                                                                                                ModifiedUtc = It.IsAny<DateTime>()
                                                                                                            }))
                          .Returns(It.Is<long>(l => l == 1));

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

                // act
                var results = testunit.ReadLists(fakeUser, testcontext, false);

                // assert
                results.Count()
                       .Should()
                       .Be(0);
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

                // act
                var results = testunit.ReadLists(fakeUser, testcontext, false);

                // assert
                results.Count()
                       .Should()
                       .Be(0);
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

                // act
                var results = testunit.ReadLists(fakeUser, testcontext, false);

                // assert
                results.Count()
                       .Should()
                       .Be(1);
            }
        }

        public class ReadList
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

                // act
                var results = testunit.ReadList(1, testcontext, false);

                // assert
                results.Should()
                       .BeNull();
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

                // act
                var results = testunit.ReadLists(fakeUser, testcontext, false);

                // assert
                results.Count()
                       .Should()
                       .Be(0);
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

                // act
                var results = testunit.ReadLists(fakeUser, testcontext, false);

                // assert
                results.Count()
                       .Should()
                       .Be(1);
            }
        }

        public class CreateOrUpdateList
        { //tests on this method wouldn't be conclusive of much
        }
    }
}
