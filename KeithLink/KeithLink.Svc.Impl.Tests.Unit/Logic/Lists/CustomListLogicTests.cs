using System;
using System.Collections.Generic;
using System.Linq;

using Autofac;

using FluentAssertions;

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
    public class CustomListLogicTests {
        private static ICustomListLogic MakeMockLogic() {
            ContainerBuilder cb = DependencyMapFactory.GetTestsContainer();

            cb.RegisterInstance(MakeDetailRepo())
              .AsImplementedInterfaces();
            cb.RegisterInstance(MakeHeaderRepo())
              .AsImplementedInterfaces();
            cb.RegisterInstance(MakeShareRepo())
              .AsImplementedInterfaces();

            IContainer diMap = cb.Build();

            return diMap.Resolve<ICustomListLogic>();
        }

        private static ICustomListDetailsRepository MakeDetailRepo() {
            Mock<ICustomListDetailsRepository> repo = new Mock<ICustomListDetailsRepository>();

            repo.Setup(d => d.GetCustomListDetails(It.Is<long>(i => i == 1)))
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

            repo.Setup(d => d.SaveCustomListDetail(It.Is<CustomListDetail>(m => m.Id == 1)))
                .Returns(1);

            return repo.Object;
        }

        private static ICustomListHeadersRepository MakeHeaderRepo() {
            Mock<ICustomListHeadersRepository> repo = new Mock<ICustomListHeadersRepository>();

            repo.Setup(h => h.GetCustomListHeader(It.Is<long>(i => i == 1)))
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

            repo.Setup(h => h.GetCustomListHeadersByCustomer(It.Is<UserSelectedContext>(u => u.BranchId == "FUT" &&
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

            repo.Setup(h => h.SaveCustomListHeader(It.Is<CustomListHeader>(l => l.Id == 1)))
                .Returns(1);
            repo.Setup(h => h.SaveCustomListHeader(It.Is<CustomListHeader>(l => l.Id == 0)))
                .Returns(1);

            return repo.Object;
        }

        private static ICustomListSharesRepository MakeShareRepo() {
            Mock<ICustomListSharesRepository> repo = new Mock<ICustomListSharesRepository>();

            repo.Setup(s => s.GetCustomListSharesByHeaderId(It.Is<long>(i => i == 1)))
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

            repo.Setup(s => s.GetCustomListSharesByCustomer(It.Is<UserSelectedContext>(u => u.CustomerId == "111111")))
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

            return repo.Object;
        }

        public class CreateOrUpdateList {
            [Fact]
            public void CallingMethod_HitsTheSaveHeaderMethodOnce() {
                // arrange
                int expected = 1;
                bool fakeActive = true;
                UserSelectedContext fakeCustomer = new UserSelectedContext();
                int fakeId = 1;
                string fakeName = "fake name";
                UserProfile fakeUser = new UserProfile();
                Mock<ICustomListDetailsRepository> detail = new Mock<ICustomListDetailsRepository>();
                Mock<ICustomListHeadersRepository> header = new Mock<ICustomListHeadersRepository>();
                Mock<ICustomListSharesRepository> shares = new Mock<ICustomListSharesRepository>();
                CustomListLogicImpl logic = new CustomListLogicImpl(shares.Object, header.Object, detail.Object);

                // act
                long results = logic.CreateOrUpdateList(fakeUser, fakeCustomer, fakeId, fakeName, fakeActive);

                // assert
                header.Verify(h => h.SaveCustomListHeader(It.IsAny<CustomListHeader>()), Times.Once);
            }

            [Fact]
            public void ExistingHeader_ReturnsTheSameHeaderId() {
                // arrange
                int expected = 1;
                bool fakeActive = true;
                UserSelectedContext fakeCustomer = new UserSelectedContext();
                int fakeId = 1;
                string fakeName = "fake name";
                UserProfile fakeUser = new UserProfile();
                ICustomListLogic logic = MakeMockLogic();

                // act
                long results = logic.CreateOrUpdateList(fakeUser, fakeCustomer, fakeId, fakeName, fakeActive);

                // assert
                results.Should()
                       .Be(expected);
            }

            [Fact]
            public void NewHeader_ReturnsTheNextAvailableHeaderId() {
                // arrange
                int expected = 1;
                bool fakeActive = true;
                UserSelectedContext fakeCustomer = new UserSelectedContext();
                int fakeId = 0;
                string fakeName = "fake name";
                UserProfile fakeUser = new UserProfile();
                ICustomListLogic logic = MakeMockLogic();

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
                int expected = 1;
                UserSelectedContext fakeCustomer = new UserSelectedContext();
                ListModel fakeList = new ListModel {
                    ListId = 1,
                    Name = "Fake Name"
                };
                UserProfile fakeUser = new UserProfile();
                Mock<ICustomListDetailsRepository> detail = new Mock<ICustomListDetailsRepository>();
                Mock<ICustomListHeadersRepository> header = new Mock<ICustomListHeadersRepository>();
                Mock<ICustomListSharesRepository> shares = new Mock<ICustomListSharesRepository>();
                CustomListLogicImpl logic = new CustomListLogicImpl(shares.Object, header.Object, detail.Object);

                // act
                logic.DeleteList(fakeUser, fakeCustomer, fakeList);

                // assert
                header.Verify(h => h.SaveCustomListHeader(It.IsAny<CustomListHeader>()), Times.Once);
            }
        }

        public class GetListModel {
            [Fact]
            public void BadBranchId_ReturnsExpectedList() {
                // arrange
                ICustomListLogic logic = MakeMockLogic();
                int headerId = 1;
                int expected = 1;
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile {
                    UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                };

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
                ICustomListLogic logic = MakeMockLogic();
                int headerId = 1;
                int expected = 1;
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "999999"
                };
                UserProfile user = new UserProfile {
                    UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                };

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
                ICustomListLogic logic = MakeMockLogic();
                int headerId = 157;
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile {
                    UserId = new Guid("9615ef5f-fa2a-4497-a59f-69f34cbe6921")
                };

                // act
                ListModel results = logic.GetListModel(user, customer, headerId);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void BadUser_ReturnsExpectedList() {
                // arrange
                ICustomListLogic logic = MakeMockLogic();
                int headerId = 1;
                int expected = 1;
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile {
                    UserId = new Guid("9615ef5f-fa2a-4497-a59f-69f34cbe6921")
                };

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
                ICustomListLogic logic = MakeMockLogic();
                int headerId = 1;
                int expected = 2;
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile {
                    UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                };

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
                ICustomListLogic logic = MakeMockLogic();
                int headerId = 1;
                int expected = 1;
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile {
                    UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                };

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
                ICustomListLogic logic = MakeMockLogic();
                int headerId = 1;
                int expected = 2;
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile {
                    UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                };

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
                ICustomListLogic logic = MakeMockLogic();
                UserSelectedContext userSelectedContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

                // act
                ListModel results = logic.ReadList(headerId, userSelectedContext, true);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void GoodHeaderIdWithItems_ReturnsExpectedItemCount() {
                // arrange
                int expected = 2;
                int headerId = 1;
                ICustomListLogic logic = MakeMockLogic();
                UserSelectedContext userSelectedContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

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
                int expected = 1;
                int headerId = 1;
                ICustomListLogic logic = MakeMockLogic();
                UserSelectedContext userSelectedContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

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
                int expected = 2;
                int headerId = 1;
                ICustomListLogic logic = MakeMockLogic();
                UserSelectedContext userSelectedContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

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
                int expected = 0;
                int headerId = 1;
                ICustomListLogic logic = MakeMockLogic();
                UserSelectedContext userSelectedContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

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
                int expected = 1;
                int headerId = 1;
                ICustomListLogic logic = MakeMockLogic();
                UserSelectedContext userSelectedContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

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
                int expected = 0;
                int headerId = 1;
                ICustomListLogic logic = MakeMockLogic();
                UserSelectedContext userSelectedContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };

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
                ICustomListLogic logic = MakeMockLogic();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile();
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
                ICustomListLogic logic = MakeMockLogic();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile();
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
                ICustomListLogic logic = MakeMockLogic();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "999999"
                };
                UserProfile user = new UserProfile();
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
                ICustomListLogic logic = MakeMockLogic();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "999999"
                };
                UserProfile user = new UserProfile();
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
                ICustomListLogic logic = MakeMockLogic();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile {
                    UserId = new Guid("9615ef5f-fa2a-4497-a59f-69f34cbe6921")
                };
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
                ICustomListLogic logic = MakeMockLogic();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "222222"
                };
                UserProfile user = new UserProfile();
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
                ICustomListLogic logic = MakeMockLogic();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile();
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
                ICustomListLogic logic = MakeMockLogic();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile();
                int headerId = 1;
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
                ICustomListLogic logic = MakeMockLogic();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile();
                int headerId = 1;
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
                ICustomListLogic logic = MakeMockLogic();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile();
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
                ICustomListLogic logic = MakeMockLogic();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "111111"
                };
                UserProfile user = new UserProfile();
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
            public void GoodUserWithoutDetails_ReturnsExpectedCountOfTwo() {
                // arrange
                ICustomListLogic logic = MakeMockLogic();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = new UserProfile {
                    UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                };
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
                ICustomListLogic logic = MakeMockLogic();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = null;
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
                ICustomListLogic logic = MakeMockLogic();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = null;
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
                ICustomListLogic logic = MakeMockLogic();

                // act
                Action act = () => logic.SaveItem(detail);

                // assert
                act.ShouldThrow<ArgumentException>();
            }

            // the list item is what is passed to detailRepo.save
            [Fact]
            public void GoodHeaderId_CallsTheExpectedMethod() {
                // arrange
                CustomListDetail detail = new CustomListDetail {
                    HeaderId = 17
                };
                Mock<ICustomListDetailsRepository> detailRepo = new Mock<ICustomListDetailsRepository>();
                Mock<ICustomListHeadersRepository> headerRepo = new Mock<ICustomListHeadersRepository>();
                Mock<ICustomListSharesRepository> sharesRepo = new Mock<ICustomListSharesRepository>();
                CustomListLogicImpl logic = new CustomListLogicImpl(sharesRepo.Object, headerRepo.Object, detailRepo.Object);

                // act
                logic.SaveItem(detail);

                // assert
                detailRepo.Verify(r => r.SaveCustomListDetail(It.IsAny<CustomListDetail>()), Times.Once);
            }
        }

        public class SaveList {
            [Fact]
            public void GoodHeader_CallsHeaderSaveMetodOnce() {
                // arrange
                UserSelectedContext fakeCustomer = new UserSelectedContext();
                ListModel farkModel = new ListModel();
                UserProfile fakeUser = new UserProfile();
                Mock<ICustomListDetailsRepository> detail = new Mock<ICustomListDetailsRepository>();
                Mock<ICustomListHeadersRepository> header = new Mock<ICustomListHeadersRepository>();
                Mock<ICustomListSharesRepository> shares = new Mock<ICustomListSharesRepository>();
                CustomListLogicImpl logic = new CustomListLogicImpl(shares.Object, header.Object, detail.Object);

                // act
                ListModel results = logic.SaveList(fakeUser, fakeCustomer, farkModel);

                // assert
                header.Verify(h => h.SaveCustomListHeader(It.IsAny<CustomListHeader>()), Times.Once);
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
                Mock<ICustomListDetailsRepository> detail = new Mock<ICustomListDetailsRepository>();
                Mock<ICustomListHeadersRepository> header = new Mock<ICustomListHeadersRepository>();
                Mock<ICustomListSharesRepository> shares = new Mock<ICustomListSharesRepository>();
                CustomListLogicImpl logic = new CustomListLogicImpl(shares.Object, header.Object, detail.Object);

                // act
                ListModel results = logic.SaveList(fakeUser, fakeCustomer, fakeModel);

                // assert
                detail.Verify(d => d.SaveCustomListDetail(It.IsAny<CustomListDetail>()), Times.Exactly(2));
            }

            [Fact]
            public void GoodHeaderNoDetail_DoesNotCallSaveDetails() {
                // arrange
                UserSelectedContext fakeCustomer = new UserSelectedContext();
                ListModel farkModel = new ListModel();
                UserProfile fakeUser = new UserProfile();
                Mock<ICustomListDetailsRepository> detail = new Mock<ICustomListDetailsRepository>();
                Mock<ICustomListHeadersRepository> header = new Mock<ICustomListHeadersRepository>();
                Mock<ICustomListSharesRepository> shares = new Mock<ICustomListSharesRepository>();
                CustomListLogicImpl logic = new CustomListLogicImpl(shares.Object, header.Object, detail.Object);

                // act
                ListModel results = logic.SaveList(fakeUser, fakeCustomer, farkModel);

                // assert
                detail.Verify(d => d.SaveCustomListDetail(It.IsAny<CustomListDetail>()), Times.Never);
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

                Mock<ICustomListDetailsRepository> detail = new Mock<ICustomListDetailsRepository>();
                Mock<ICustomListHeadersRepository> header = new Mock<ICustomListHeadersRepository>();
                Mock<ICustomListSharesRepository> shares = new Mock<ICustomListSharesRepository>();
                CustomListLogicImpl logic = new CustomListLogicImpl(shares.Object, header.Object, detail.Object);

                // act
                ListModel results = logic.SaveList(fakeUser, fakeCustomer, farkModel);

                // assert
                detail.Verify(x => x.SaveCustomListDetail(It.Is<CustomListDetail>(d => d.Active.Equals(true))), Times.Once);
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

                Mock<ICustomListDetailsRepository> detail = new Mock<ICustomListDetailsRepository>();
                Mock<ICustomListHeadersRepository> header = new Mock<ICustomListHeadersRepository>();
                Mock<ICustomListSharesRepository> shares = new Mock<ICustomListSharesRepository>();
                CustomListLogicImpl logic = new CustomListLogicImpl(shares.Object, header.Object, detail.Object);

                // act
                ListModel results = logic.SaveList(fakeUser, fakeCustomer, farkModel);

                // assert
                detail.Verify(x => x.SaveCustomListDetail(It.Is<CustomListDetail>(d => d.Active.Equals(false))), Times.Once);
            }
        }
    }
}