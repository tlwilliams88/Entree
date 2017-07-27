using System.Collections.Generic;

using Autofac;
using FluentAssertions;
using Moq;
using Xunit;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.ReminderItems;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic.Lists;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Lists
{
    public class ReminderItemsListLogicTests : BaseDITests
    {
        public class MockDependents {
            public Mock<IRemindersListHeadersRepository> headersRepository { get; set; }
            public Mock<IRemindersListDetailsRepository> detailsRepository { get; set; }
        }
        private static IRemindersListLogic MakeTestsLogic(bool useAutoFac, ref MockDependents mockDependents)
        {
            if (useAutoFac) {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                cb.RegisterInstance(MakeMockHeaderRepo())
                  .As<IRemindersListHeadersRepository>();
                cb.RegisterInstance(MakeMockDetailsRepo())
                  .As<IRemindersListDetailsRepository>();

                var testcontainer = cb.Build();

                return testcontainer.Resolve<IRemindersListLogic>();
            } else {
                mockDependents = new MockDependents();
                mockDependents.headersRepository = new Mock<IRemindersListHeadersRepository>();
                mockDependents.detailsRepository = new Mock<IRemindersListDetailsRepository>();
                var testunit = new ReminderItemsListLogicImpl(mockDependents.headersRepository.Object, mockDependents.detailsRepository.Object);
                return testunit;
            }
        }

        private static IRemindersListHeadersRepository MakeMockHeaderRepo()
        {
            var mockHeaderRepo = new Mock<IRemindersListHeadersRepository>();

            mockHeaderRepo.Setup(h => h.GetReminderItemsHeader(It.Is<UserSelectedContext>(c => c.BranchId == "FUT" &&
                                                                                               c.CustomerId == "123456")))
                          .Returns(new ReminderItemsListHeader()
                          {
                              BranchId = "FUT",
                              CustomerNumber = "123456",
                              Id = 1
                          });

            return mockHeaderRepo.Object;
        }

        private static IRemindersListDetailsRepository MakeMockDetailsRepo()
        {
            var mockDetailsRepo = new Mock<IRemindersListDetailsRepository>();

            mockDetailsRepo.Setup(h => h.GetRemindersDetails(It.Is<long>(l => l == 1)))
                           .Returns(new List<ReminderItemsListDetail>()
                           {
                               new ReminderItemsListDetail() {
                                                             CatalogId = "FUT",
                                                             ItemNumber = "123456",
                                                             Each = false,
                                                             LineNumber = 1,
                                                             Id = 1
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
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac:true, mockDependents: ref mockDependents);
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var fakeUser = new UserProfile();
                var fakeId = 0;

                // act
                var results = testunit.GetListModel(fakeUser, testcontext, fakeId);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void BadCustomerId_ReturnsNull()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: true, mockDependents: ref mockDependents);
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "223456"
                };
                var fakeUser = new UserProfile();
                var fakeId = 0;

                // act
                var results = testunit.GetListModel(fakeUser, testcontext, fakeId);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void GoodCustomerIdAndBranch_ReturnsExpectedList()
            {
                // arrange
                var mockDependents = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: true, mockDependents: ref mockDependents);
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var expectedListId = 1;
                var fakeUser = new UserProfile();
                var fakeId = 0;

                // act
                var results = testunit.GetListModel(fakeUser, testcontext, fakeId);

                // assert
                results.ListId
                       .Should()
                       .Be(expectedListId);
            }
        }

        public class SaveList
        { // works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCustomerIdAndBranch_CallsSaveHeader()
            {
                // arrange
                var mockDependants = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac:false, mockDependents:ref mockDependants);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
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
                testunit.SaveList(fakeUser, testcontext, testList);

                // assert - Always returns what is setup provided the mock is called
                mockDependants.headersRepository.Verify(h => h.SaveReminderListHeader(It.IsAny<ReminderItemsListHeader>()), Times.Once(), "Error updating");
            }
        }

        public class Save
        { // works differently if you want to verify a mock is called; we can't go through autofac
            [Fact]
            public void AnyCustomerIdAndBranch_CallsSaveDetail()
            {
                // arrange
                var mockDependants = new MockDependents();
                var testunit = MakeTestsLogic(useAutoFac: false, mockDependents: ref mockDependants);
                var fakeUser = new UserProfile();
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var testItem =
                    new ReminderItemsListDetail()
                    {
                        CatalogId = "FUT",
                        ItemNumber = "123456",
                        Each = false,
                        LineNumber = 1,
                        HeaderId = 1L
                    };

                // act
                testunit.Save(testcontext, testItem);

                // assert - Always returns what is setup provided the mock is called
                mockDependants.detailsRepository.Verify(h => h.SaveReminderListDetail(It.IsAny<ReminderItemsListDetail>()), Times.Once(), "Error updating");
            }
        }
    }
}
