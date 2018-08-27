using System;
using System.Collections.Generic;
using System.Linq;

using Autofac;

using FluentAssertions;

using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.CustomList;
using KeithLink.Svc.Core.Models.Lists.CustomListShares;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic.Lists;
using KeithLink.Svc.Impl.Repository.SmartResolver;

using Moq;

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic.Lists {
    public class CustomListLogicTests
    {
        public class MockDependents
        {
            public Mock<ICustomListDetailsRepository> MockDetailRepo;
            public Mock<ICustomListHeadersRepository> MockHeaderRepo;
            public Mock<ICustomListSharesRepository> MockShareRepo;
            public Mock<IEventLogRepository> MockLogRepo;

            public MockDependents()
            {
                MockDetailRepo = MockDetailRepo();
                MockHeaderRepo = MockHeaderRepo();
                MockShareRepo = MockShareRepo();
                MockLogRepo = MockLogRepo();
            }
        }

        private static List<Mock> GetMocks(MockDependents mockDependents)
        {
            List<Mock> mocks = new List<Mock>
            {
                mockDependents.MockDetailRepo,
                mockDependents.MockHeaderRepo,
                mockDependents.MockShareRepo,
                mockDependents.MockLogRepo,
            };

            return mocks;
        }

        private static ICustomListLogic MakeMockLogic(MockDependents mockDependents)
        {
            ContainerBuilder cb = DependencyMapFactory.GetTestsContainer();

            List<Mock> mocks = GetMocks(mockDependents);

            foreach (var mock in mocks)
            {
                cb.RegisterInstance(mock.Object)
                  .AsImplementedInterfaces();
            }

            IContainer diMap = cb.Build();

            return diMap.Resolve<ICustomListLogic>();
        }

        private static Mock<ICustomListDetailsRepository> MockDetailRepo() {
            Mock<ICustomListDetailsRepository> mock = new Mock<ICustomListDetailsRepository>();

            mock.Setup(d => d.GetCustomListDetails(It.Is<long>(i => i == 1)))
                .Returns(
                         new List<CustomListDetail> {
                             new CustomListDetail {
                                 Active = true,
                                 CatalogId = "FUT",
                                 CustomInventoryItemId = 100,
                                 Each = false,
                                 HeaderId = 1,
                                 Id = 1,
                                 ItemNumber = "123456",
                                 Label = "Fake Label",
                                 LineNumber = 1,
                                 Par = 12,
                                 CreatedUtc = new DateTime(2017, 7, 6, 16, 50, 0, DateTimeKind.Utc),
                                 ModifiedUtc = new DateTime(2017, 7, 6, 16, 51, 0, DateTimeKind.Utc)
                             },
                             new CustomListDetail {
                                 Active = true,
                                 CatalogId = "FUT",
                                 CustomInventoryItemId = 101,
                                 Each = false,
                                 HeaderId = 1,
                                 Id = 2,
                                 ItemNumber = "234567",
                                 Label = "Fake Label",
                                 LineNumber = 2,
                                 Par = 10,
                                 CreatedUtc = new DateTime(2017, 7, 6, 16, 50, 0, DateTimeKind.Utc),
                                 ModifiedUtc = new DateTime(2017, 7, 6, 16, 51, 0, DateTimeKind.Utc)
                             }
                         }
                        );

            mock.Setup(d => d.SaveCustomListDetail(It.Is<CustomListDetail>(m => m.Id == 1)))
                .Returns(1);

            return mock;
        }

        private static Mock<ICustomListHeadersRepository> MockHeaderRepo() {
            Mock<ICustomListHeadersRepository> mock = new Mock<ICustomListHeadersRepository>();

            mock.Setup(h => h.GetCustomListHeader(It.Is<long>(i => i == 1)))
                .Returns(new CustomListHeader {
                    Active = true,
                    BranchId = "FUT",
                    CustomerNumber = "123456",
                    CreatedUtc = new DateTime(2017, 7, 7, 9, 41, 0, DateTimeKind.Utc),
                    Id = 1,
                    ModifiedUtc = new DateTime(2017, 7, 7, 9, 42, 0, DateTimeKind.Utc),
                    Name = "Fake List Name",
                    UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                });

            mock.Setup(h => h.GetCustomListHeadersByCustomer(It.Is<UserSelectedContext>(u => u.BranchId == "FUT" &&
                                                                                             u.CustomerId == "123456")))
                .Returns(new List<CustomListHeader> {
                    new CustomListHeader {
                        Active = true,
                        BranchId = "FUT",
                        CustomerNumber = "123456",
                        CreatedUtc = new DateTime(2017, 7, 7, 9, 41, 0, DateTimeKind.Utc),
                        Id = 1,
                        ModifiedUtc = new DateTime(2017, 7, 7, 9, 42, 0, DateTimeKind.Utc),
                        Name = "Fake List Name 1",
                        UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                    },
                    new CustomListHeader {
                        Active = true,
                        BranchId = "FUT",
                        CustomerNumber = "123456",
                        CreatedUtc = new DateTime(2017, 7, 7, 9, 41, 0, DateTimeKind.Utc),
                        Id = 2,
                        ModifiedUtc = new DateTime(2017, 7, 7, 9, 42, 0, DateTimeKind.Utc),
                        Name = "Fake List Name 2",
                        UserId = new Guid("0ef15b1c-7e36-44eb-b389-5b8441d5a3ae")
                    }
                });

            mock.Setup(h => h.SaveCustomListHeader(It.Is<CustomListHeader>(l => l.Id == 1)))
                .Returns(1);
            mock.Setup(h => h.SaveCustomListHeader(It.Is<CustomListHeader>(l => l.Id == 0)))
                .Returns(1);

            return mock;
        }

        private static Mock<ICustomListSharesRepository> MockShareRepo() {
            Mock<ICustomListSharesRepository> mock = new Mock<ICustomListSharesRepository>();

            mock.Setup(s => s.GetCustomListSharesByHeaderId(It.Is<long>(i => i == 1)))
                .Returns(
                         new List<CustomListShare> {
                             new CustomListShare {
                                 Active = true,
                                 BranchId = "FUT",
                                 CreatedUtc = new DateTime(2017, 7, 7, 16, 25, 0, DateTimeKind.Utc),
                                 CustomerNumber = "234567",
                                 HeaderId = 1,
                                 Id = 1,
                                 ModifiedUtc = new DateTime(2017, 7, 7, 16, 26, 0, DateTimeKind.Utc)
                             },
                             new CustomListShare {
                                 Active = true,
                                 BranchId = "FUT",
                                 CreatedUtc = new DateTime(2017, 7, 7, 16, 25, 0, DateTimeKind.Utc),
                                 CustomerNumber = "345678",
                                 HeaderId = 1,
                                 Id = 2,
                                 ModifiedUtc = new DateTime(2017, 7, 7, 16, 26, 0, DateTimeKind.Utc)
                             }
                         }
                        );

            mock.Setup(s => s.GetCustomListSharesByCustomer(It.Is<UserSelectedContext>(u => u.CustomerId == "111111")))
                .Returns(
                         new List<CustomListShare> {
                             new CustomListShare {
                                 Active = true,
                                 BranchId = "FUT",
                                 CreatedUtc = new DateTime(2017, 7, 7, 16, 25, 0, DateTimeKind.Utc),
                                 CustomerNumber = "234567",
                                 HeaderId = 1,
                                 Id = 1,
                                 ModifiedUtc = new DateTime(2017, 7, 7, 16, 26, 0, DateTimeKind.Utc)
                             }
                         }
                        );

            mock.Setup(s => s.GetCustomListSharesByCustomer(It.Is<UserSelectedContext>(u => u.CustomerId == "333333")))
                .Returns(
                         new List<CustomListShare> {
                             new CustomListShare {
                                 Active = true,
                                 BranchId = "FUT",
                                 CreatedUtc = new DateTime(2017, 7, 7, 16, 25, 0, DateTimeKind.Utc),
                                 CustomerNumber = "234567",
                                 HeaderId = 888,
                                 Id = 1,
                                 ModifiedUtc = new DateTime(2017, 7, 7, 16, 26, 0, DateTimeKind.Utc)
                             }
                         }
                        );

            return mock;
        }

        private static Mock<IEventLogRepository> MockLogRepo()
        {
            Mock<IEventLogRepository> mock = new Mock<IEventLogRepository>();

            mock.Setup(s => s.WriteWarningLog(It.IsAny<string>()));

            return mock;
        }

        public class CreateOrUpdateList {
            [Fact]
            public void CallingMethod_HitsTheSaveHeaderMethodOnce() {
                // arrange
                bool fakeActive = true;
                UserSelectedContext fakeCustomer = new UserSelectedContext();
                int fakeId = 1;
                string fakeName = "fake name";
                UserProfile fakeUser = new UserProfile();

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 1;

                // act
                long results = logic.CreateOrUpdateList(fakeUser, fakeCustomer, fakeId, fakeName, fakeActive);

                // assert
                mockDependents.MockHeaderRepo
                    .Verify(h => h.SaveCustomListHeader(It.IsAny<CustomListHeader>()), Times.Once);
            }

            [Fact]
            public void ExistingHeader_ReturnsTheSameHeaderId() {
                // arrange
                bool fakeActive = true;
                UserSelectedContext fakeCustomer = new UserSelectedContext();
                int fakeId = 1;
                string fakeName = "fake name";
                UserProfile fakeUser = new UserProfile();

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 1;

                // act
                long results = logic.CreateOrUpdateList(fakeUser, fakeCustomer, fakeId, fakeName, fakeActive);

                // assert
                results.Should()
                       .Be(expected);
            }

            [Fact]
            public void NewHeader_ReturnsTheNextAvailableHeaderId() {
                // arrange
                bool fakeActive = true;
                UserSelectedContext fakeCustomer = new UserSelectedContext();
                int fakeId = 0;
                string fakeName = "fake name";
                UserProfile fakeUser = new UserProfile();

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 1;

                // act
                long results = logic.CreateOrUpdateList(fakeUser, fakeCustomer, fakeId, fakeName, fakeActive);

                // assert
                results.Should()
                       .Be(expected);
            }
        }

        public class DeleteList {
            [Fact]
            public void CallingMethod_HitsTheSaveHeaderMethodOnce() {
                // arrange
                UserSelectedContext fakeCustomer = new UserSelectedContext();
                ListModel fakeList = new ListModel {
                    ListId = 1,
                    Name = "Fake Name"
                };
                UserProfile fakeUser = new UserProfile();

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 1;

                // act
                logic.DeleteList(fakeUser, fakeCustomer, fakeList);

                // assert
                mockDependents.MockHeaderRepo.Verify(h => h.SaveCustomListHeader(It.IsAny<CustomListHeader>()), Times.Once);
            }
        }

        public class GetListModel {
            [Fact]
            public void BadBranchId_ReturnsExpectedList() {
                // arrange
                int headerId = 1;
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile {
                    UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                };

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 1;

                // act
                ListModel results = logic.GetListModel(user, customer, headerId);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void BadCustomerId_ReturnsExpectedList() {
                // arrange
                int headerId = 1;
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "999999"
                };
                UserProfile user = new UserProfile {
                    UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                };

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 1;

                // act
                ListModel results = logic.GetListModel(user, customer, headerId);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void BadHeaderId_ReturnsNull() {
                // arrange
                int headerId = 157;
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile {
                    UserId = new Guid("9615ef5f-fa2a-4497-a59f-69f34cbe6921")
                };

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // act
                ListModel results = logic.GetListModel(user, customer, headerId);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void BadUser_ReturnsExpectedList() {
                // arrange
                int headerId = 1;
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile {
                    UserId = new Guid("9615ef5f-fa2a-4497-a59f-69f34cbe6921")
                };

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 1;

                // act
                ListModel results = logic.GetListModel(user, customer, headerId);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedItemCount() {
                // arrange
                int headerId = 1;
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile {
                    UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                };

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 2;

                // act
                ListModel results = logic.GetListModel(user, customer, headerId);

                // assert
                results.Items
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedList() {
                // arrange
                int headerId = 1;
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile {
                    UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                };

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 1;

                // act
                ListModel results = logic.GetListModel(user, customer, headerId);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedSharedCount() {
                // arrange
                int headerId = 1;
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile {
                    UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                };

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 2;

                // act
                ListModel results = logic.GetListModel(user, customer, headerId);

                // assert
                results.SharedWith
                       .Count
                       .Should()
                       .Be(expected);
            }
        }

        public class ReadList {
            [Fact]
            public void BadHeaderIdWithoutItems_ReturnsNull() {
                // arrange
                int headerId = 0;
                UserSelectedContext userSelectedContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // act
                ListModel results = logic.ReadList(headerId, userSelectedContext, true);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void GoodHeaderIdWithItems_ReturnsExpectedItemCount() {
                // arrange
                int headerId = 1;
                UserSelectedContext userSelectedContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 2;

                // act
                ListModel result = logic.ReadList(headerId, userSelectedContext, false);

                // assert
                result.Items
                      .Count
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderIdWithItems_ReturnsExpectedListModel() {
                // arrange
                int headerId = 1;
                UserSelectedContext userSelectedContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 1;

                // act
                ListModel result = logic.ReadList(headerId, userSelectedContext, false);

                // assert
                result.ListId
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderIdWithItems_ReturnsExpectedShareCount() {
                // arrange
                int headerId = 1;
                UserSelectedContext userSelectedContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 2;

                // act
                ListModel result = logic.ReadList(headerId, userSelectedContext, false);

                // assert
                result.SharedWith
                      .Count
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderIdWithoutItems_ReturnsExpectedItemCount() {
                // arrange
                int headerId = 1;
                UserSelectedContext userSelectedContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 0;

                // act
                ListModel result = logic.ReadList(headerId, userSelectedContext, true);

                // assert
                result.Items
                      .Count
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderIdWithoutItems_ReturnsExpectedListModel() {
                // arrange
                int headerId = 1;
                UserSelectedContext userSelectedContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 1;

                // act
                ListModel result = logic.ReadList(headerId, userSelectedContext, true);

                // assert
                result.ListId
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderIdWithoutItems_ReturnsExpectedShareCount() {
                // arrange
                int headerId = 1;
                UserSelectedContext userSelectedContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 0;

                // act
                ListModel result = logic.ReadList(headerId, userSelectedContext, true);

                // assert
                result.SharedWith
                      .Count
                      .Should()
                      .Be(expected);
            }
        }

        public class ReadLists {
            [Fact]
            public void BadBranchIdWithDetails_ReturnsListWithNoHeaders() {
                // arrange
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile();

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 0;

                // act
                List<ListModel> results = logic.ReadLists(user, customer, false);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void BadBranchIdWithoutDetails_ReturnsListWithNoHeaders() {
                // arrange
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile();

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 0;

                // act
                List<ListModel> results = logic.ReadLists(user, customer, true);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void BadCustomerIdWithDetails_ReturnsListWithNoHeaders() {
                // arrange
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "999999"
                };
                UserProfile user = new UserProfile();

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 0;

                // act
                List<ListModel> results = logic.ReadLists(user, customer, false);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void BadCustomerIdWithoutDetails_ReturnsListWithNoHeaders() {
                // arrange
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "999999"
                };
                UserProfile user = new UserProfile();

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 0;

                // act
                List<ListModel> results = logic.ReadLists(user, customer, true);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void BadUserWithoutDetails_ReturnsExpectedCountOfTwo() {
                // arrange
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile {
                    UserId = new Guid("9615ef5f-fa2a-4497-a59f-69f34cbe6921")
                };

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 2;

                // act
                List<ListModel> results = logic.ReadLists(user, customer, true);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void CustomerWithNoSharedList_DoesNotSeeList() {
                // arrange
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "222222"
                };
                UserProfile user = new UserProfile();

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 0;

                // act
                List<ListModel> results = logic.ReadLists(user, customer, false);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomerWithDetails_ReturnsExpectedCountOfTwo() {
                // arrange
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile();

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 2;

                // act
                List<ListModel> results = logic.ReadLists(user, customer, false);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomerWithDetails_ReturnsExpectedCountOfTwoItemsForHeaderIdOf1() {
                // arrange
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile();
                int headerId = 1;

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 2;

                // act
                List<ListModel> results = logic.ReadLists(user, customer, false);
                ListModel header = results.First(h => h.ListId == headerId);

                // assert
                header.Items
                      .Count
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodCustomerWithDetails_ReturnsExpectedCountOfTwoSharesForHeaderIdOf1() {
                // arrange
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile();
                int headerId = 1;

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 2;

                // act
                List<ListModel> results = logic.ReadLists(user, customer, false);
                ListModel header = results.First(h => h.ListId == headerId);

                // assert
                header.SharedWith
                      .Count
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodCustomerWithoutDetails_ReturnsExpectedCountOfTwo() {
                // arrange
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile();

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 2;

                // act
                List<ListModel> results = logic.ReadLists(user, customer, true);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomerWithSharedList_ReturnsExpectedListId() {
                // arrange
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "111111"
                };
                UserProfile user = new UserProfile();

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 1;

                // act
                List<ListModel> results = logic.ReadLists(user, customer, false);

                // assert
                results.First()
                       .ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomerWithBrokenSharedList_WritesWarningToLog()
            {
                // arrange
                UserSelectedContext customer = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "333333"
                };
                UserProfile user = new UserProfile();

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // act
                List<ListModel> results = logic.ReadLists(user, customer, false);

                // assert
                mockDependents.MockLogRepo.Verify(log => log.WriteWarningLog(It.IsAny<string>()), Times.Once);
            }

            [Fact]
            public void GoodUserWithoutDetails_ReturnsExpectedCountOfTwo() {
                // arrange
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile {
                    UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                };

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 2;

                // act
                List<ListModel> results = logic.ReadLists(user, customer, true);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void NullUserWithDetails_ReturnsCountOfTwoHeaders() {
                // arrange
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = null;

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 2;

                // act
                List<ListModel> results = logic.ReadLists(user, customer, false);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void NullUserWithoutDetails_ReturnsCountOfTwoHeaders() {
                // arrange
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = null;

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // expect
                int expected = 2;

                // act
                List<ListModel> results = logic.ReadLists(user, customer, true);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }
        }

        public class SaveItem {
            // reads header id from the listitem
            [Fact]
            public void BadHeaderId_ThrowsArgumentException() {
                // arrange
                CustomListDetail detail = new CustomListDetail();

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // act
                Action act = () => logic.SaveItem(detail);

                // assert
                act.Should().Throw<ArgumentException>();
            }

            // the list item is what is passed to detailRepo.save
            [Fact]
            public void GoodHeaderId_CallsTheExpectedMethod() {
                // arrange
                CustomListDetail detail = new CustomListDetail {
                    HeaderId = 17
                };

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // act
                logic.SaveItem(detail);

                // assert
                mockDependents.MockDetailRepo.Verify(r => r.SaveCustomListDetail(It.IsAny<CustomListDetail>()), Times.Once);
            }
        }

        public class SaveList {
            [Fact]
            public void GoodHeader_CallsHeaderSaveMetodOnce() {
                // arrange
                UserSelectedContext fakeCustomer = new UserSelectedContext();
                ListModel farkModel = new ListModel();
                UserProfile fakeUser = new UserProfile();

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // act
                ListModel results = logic.SaveList(fakeUser, fakeCustomer, farkModel);

                // assert
                mockDependents.MockHeaderRepo.Verify(h => h.SaveCustomListHeader(It.IsAny<CustomListHeader>()), Times.Once);
            }

            [Fact]
            public void GoodHeaderGoodDetail_CallsSaveDetailsTwice() {
                // arrange
                UserSelectedContext fakeCustomer = new UserSelectedContext();
                ListModel fakeModel = new ListModel {
                    ListId = 17,
                    Items = new List<ListItemModel> {
                        new ListItemModel {
                            ItemNumber = "123456"
                        },
                        new ListItemModel {
                            ItemNumber = "234567"
                        }
                    }
                };
                UserProfile fakeUser = new UserProfile();

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // act
                ListModel results = logic.SaveList(fakeUser, fakeCustomer, fakeModel);

                // assert
                mockDependents.MockDetailRepo.Verify(d => d.SaveCustomListDetail(It.IsAny<CustomListDetail>()), Times.Exactly(2));
            }

            [Fact]
            public void GoodHeaderNoDetail_DoesNotCallSaveDetails() {
                // arrange
                UserSelectedContext fakeCustomer = new UserSelectedContext();
                ListModel farkModel = new ListModel();
                UserProfile fakeUser = new UserProfile();

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // act
                ListModel results = logic.SaveList(fakeUser, fakeCustomer, farkModel);

                // assert
                mockDependents.MockDetailRepo.Verify(d => d.SaveCustomListDetail(It.IsAny<CustomListDetail>()), Times.Never);
            }

            [Fact]
            public void GoodHeaderNoDetail_IfIsDeleteIsFalseAndActiveIsFalseSavesWithActiveFlagTrue() {
                // arrange
                UserSelectedContext fakeCustomer = new UserSelectedContext();
                UserProfile fakeUser = new UserProfile();
                ListModel farkModel = new ListModel {
                    ListId = 1,
                    CustomerNumber = "123456",
                    BranchId = "FUT",
                    Name = "Custom",
                    Items = new List<ListItemModel> {
                        new ListItemModel {
                            Active = false,
                            ListItemId = 2,
                            IsDelete = false,
                            CatalogId = "FDF",
                            ItemNumber = "123456"
                        }
                    }
                };

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // act
                ListModel results = logic.SaveList(fakeUser, fakeCustomer, farkModel);

                // assert
                mockDependents.MockDetailRepo.Verify(x => x.SaveCustomListDetail(It.Is<CustomListDetail>(d => d.Active.Equals(true))), Times.Once);
            }

            [Fact]
            public void GoodHeaderNoDetail_IfIsDeleteIsTrueSavesWithActiveFlagFalse() {
                // arrange
                UserSelectedContext fakeCustomer = new UserSelectedContext();
                UserProfile fakeUser = new UserProfile();
                ListModel farkModel = new ListModel {
                    ListId = 1,
                    CustomerNumber = "123456",
                    BranchId = "FUT",
                    Name = "Custom",
                    Items = new List<ListItemModel> {
                        new ListItemModel {
                            Active = true,
                            ListItemId = 2,
                            IsDelete = true,
                            CatalogId = "FDF",
                            ItemNumber = "123456"
                        }
                    }
                };

                MockDependents mockDependents = new MockDependents();
                ICustomListLogic logic = MakeMockLogic(mockDependents);

                // act
                ListModel results = logic.SaveList(fakeUser, fakeCustomer, farkModel);

                // assert
                mockDependents.MockDetailRepo.Verify(x => x.SaveCustomListDetail(It.Is<CustomListDetail>(d => d.Active.Equals(false))), Times.Once);
            }
        }
    }
}