using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var repo = new Mock<ICustomListDetailsRepository>();

            repo.Setup(d => d.GetCustomListDetails(It.Is<long>(i => i == 1)))
                .Returns(
                    new List<CustomListDetail>() { 
                        new CustomListDetail() {
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
                        new CustomListDetail() {
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
            var repo = new Mock<ICustomListHeadersRepository>();

            repo.Setup(h => h.GetCustomListHeader(It.Is<long>(i => i == 1)))
                .Returns(new CustomListHeader() {
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
                .Returns(new List<CustomListHeader>() {
                    new CustomListHeader() {
                        Active = true,
                        BranchId = "FUT",
                        CustomerNumber = "123456",
                        CreatedUtc = new DateTime(2017, 7, 7, 9, 41, 0, DateTimeKind.Utc),
                        Id = 1,
                        ModifiedUtc = new DateTime(2017, 7, 7, 9, 42, 0, DateTimeKind.Utc),
                        Name = "Fake List Name 1",
                        UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                    },
                    new CustomListHeader() {
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
            var repo = new Mock<ICustomListSharesRepository>();

            repo.Setup(s => s.GetCustomListSharesByHeaderId(It.Is<long>(i => i == 1)))
                .Returns(
                    new List<CustomListShare>(){
                        new CustomListShare() {
                            Active = true,
                            BranchId = "FUT",
                            CreatedUtc = new DateTime(2017, 7, 7, 16, 25, 0, DateTimeKind.Utc),
                            CustomerNumber = "234567",
                            HeaderId = 1,
                            Id = 1,
                            ModifiedUtc = new DateTime(2017, 7, 7, 16, 26, 0, DateTimeKind.Utc)
                        },
                        new CustomListShare() {
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
                    new List<CustomListShare>(){
                        new CustomListShare() {
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
            public void ExistingHeader_ReturnsTheSameHeaderId() {
                // arrange
                var expected = 1;
                var fakeActive = true;
                var fakeCustomer = new UserSelectedContext();
                var fakeId = 1;
                var fakeName = "fake name";
                var fakeUser = new UserProfile();
                var logic = MakeMockLogic();

                // act
                var results = logic.CreateOrUpdateList(fakeUser, fakeCustomer, fakeId, fakeName, fakeActive);

                // assert
                results.Should()
                       .Be(expected);
            }

            [Fact]
            public void NewHeader_ReturnsTheNextAvailableHeaderId() {
                // arrange
                var expected = 1;
                var fakeActive = true;
                var fakeCustomer = new UserSelectedContext();
                var fakeId = 0;
                var fakeName = "fake name";
                var fakeUser = new UserProfile();
                var logic = MakeMockLogic();

                // act
                var results = logic.CreateOrUpdateList(fakeUser, fakeCustomer, fakeId, fakeName, fakeActive);

                // assert
                results.Should()
                       .Be(expected);
            }

            [Fact]
            public void CallingMethod_HitsTheSaveHeaderMethodOnce() {
                // arrange
                var expected = 1;
                var fakeActive = true;
                var fakeCustomer = new UserSelectedContext();
                var fakeId = 1;
                var fakeName = "fake name";
                var fakeUser = new UserProfile();
                var detail = new Mock<ICustomListDetailsRepository>();
                var header = new Mock<ICustomListHeadersRepository>();
                var shares = new Mock<ICustomListSharesRepository>();
                var logic = new CustomListLogicImpl(shares.Object, header.Object, detail.Object);

                // act
                var results = logic.CreateOrUpdateList(fakeUser, fakeCustomer, fakeId, fakeName, fakeActive);

                // assert
                header.Verify(h => h.SaveCustomListHeader(It.IsAny<CustomListHeader>()), Times.Once);
            }
        }
        
        public class DeleteList {
            [Fact]
            public void CallingMethod_HitsTheSaveHeaderMethodOnce() {
                // arrange
                var expected = 1;
                var fakeCustomer = new UserSelectedContext();
                var fakeList = new ListModel() {
                    ListId = 1,
                    Name = "Fake Name"
                };
                var fakeUser = new UserProfile();
                var detail = new Mock<ICustomListDetailsRepository>();
                var header = new Mock<ICustomListHeadersRepository>();
                var shares = new Mock<ICustomListSharesRepository>();
                var logic = new CustomListLogicImpl(shares.Object, header.Object, detail.Object);

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
                var logic = MakeMockLogic();
                var headerId = 1;
                var expected = 1;
                var customer = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var user = new UserProfile() {
                    UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                };

                // act
                var results = logic.GetListModel(user, customer, headerId);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void BadCustomerId_ReturnsExpectedList() {
                // arrange
                var logic = MakeMockLogic();
                var headerId = 1;
                var expected = 1;
                var customer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "999999"
                };
                var user = new UserProfile() {
                    UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                };

                // act
                var results = logic.GetListModel(user, customer, headerId);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void BadHeaderId_ReturnsNull() {
                // arrange
                var logic = MakeMockLogic();
                var headerId = 157;
                var customer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var user = new UserProfile() {
                    UserId = new Guid("9615ef5f-fa2a-4497-a59f-69f34cbe6921")
                };

                // act
                var results = logic.GetListModel(user, customer, headerId);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void BadUser_ReturnsExpectedList() {
                // arrange
                var logic = MakeMockLogic();
                var headerId = 1;
                var expected = 1;
                var customer = new UserSelectedContext() {
                                                             BranchId = "FUT",
                                                             CustomerId = "123456"
                                                         };
                var user = new UserProfile() {
                                                 UserId = new Guid("9615ef5f-fa2a-4497-a59f-69f34cbe6921")
                                             };

                // act
                var results = logic.GetListModel(user, customer, headerId);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedList() {
                // arrange
                var logic = MakeMockLogic();
                var headerId = 1;
                var expected = 1;
                var customer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var user = new UserProfile() {
                    UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                };

                // act
                var results = logic.GetListModel(user, customer, headerId);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedItemCount() {
                // arrange
                var logic = MakeMockLogic();
                var headerId = 1;
                var expected = 2;
                var customer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var user = new UserProfile() {
                    UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                };

                // act
                var results = logic.GetListModel(user, customer, headerId);

                // assert
                results.Items
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedSharedCount() {
                // arrange
                var logic = MakeMockLogic();
                var headerId = 1;
                var expected = 2;
                var customer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var user = new UserProfile() {
                    UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                };

                // act
                var results = logic.GetListModel(user, customer, headerId);

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
                var headerId = 0;
                var logic = MakeMockLogic();

                // act
                var results = logic.ReadList(headerId, true);

                // assert
                results.Should()
                       .BeNull();
            }

            [Fact]
            public void GoodHeaderIdWithoutItems_ReturnsExpectedListModel() {
                // arrange
                var expected = 1;
                var headerId = 1;
                var logic = MakeMockLogic();

                // act
                var result = logic.ReadList(headerId, true);
            
                // assert
                result.ListId
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderIdWithoutItems_ReturnsExpectedItemCount() {
                // arrange
                var expected = 0;
                var headerId = 1;
                var logic = MakeMockLogic();

                // act
                var result = logic.ReadList(headerId, true);

                // assert
                result.Items
                      .Count
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderIdWithItems_ReturnsExpectedListModel() {
                // arrange
                var expected = 1;
                var headerId = 1;
                var logic = MakeMockLogic();

                // act
                var result = logic.ReadList(headerId, false);

                // assert
                result.ListId
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderIdWithItems_ReturnsExpectedItemCount() {
                // arrange
                var expected = 2;
                var headerId = 1;
                var logic = MakeMockLogic();

                // act
                var result = logic.ReadList(headerId, false);

                // assert
                result.Items
                      .Count
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderIdWithoutItems_ReturnsExpectedShareCount() {
                // arrange
                var expected = 0;
                var headerId = 1;
                var logic = MakeMockLogic();

                // act
                var result = logic.ReadList(headerId, true);

                // assert
                result.SharedWith
                      .Count
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodHeaderIdWithItems_ReturnsExpectedShareCount() {
                // arrange
                var expected = 2;
                var headerId = 1;
                var logic = MakeMockLogic();

                // act
                var result = logic.ReadList(headerId, false);

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
                var logic = MakeMockLogic();
                var customer = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var user = new UserProfile();
                var expected = 0;

                // act
                var results = logic.ReadLists(user, customer, false);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void BadCustomerIdWithDetails_ReturnsListWithNoHeaders() {
                // arrange
                var logic = MakeMockLogic();
                var customer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "999999"
                };
                var user = new UserProfile();
                var expected = 0;

                // act
                var results = logic.ReadLists(user, customer, false);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void BadBranchIdWithoutDetails_ReturnsListWithNoHeaders() {
                // arrange
                var logic = MakeMockLogic();
                var customer = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var user = new UserProfile();
                var expected = 0;

                // act
                var results = logic.ReadLists(user, customer, true);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void BadCustomerIdWithoutDetails_ReturnsListWithNoHeaders() {
                // arrange
                var logic = MakeMockLogic();
                var customer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "999999"
                };
                var user = new UserProfile();
                var expected = 0;

                // act
                var results = logic.ReadLists(user, customer, true);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void BadUserWithoutDetails_ReturnsExpectedCountOfTwo() {
                // arrange
                var logic = MakeMockLogic();
                var customer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var user = new UserProfile() {
                    UserId = new Guid("9615ef5f-fa2a-4497-a59f-69f34cbe6921")
                };
                var expected = 2;

                // act
                var results = logic.ReadLists(user, customer, true);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomerWithDetails_ReturnsExpectedCountOfTwo() {
                // arrange
                var logic = MakeMockLogic();
                var customer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var user = new UserProfile();
                var expected = 2;

                // act
                var results = logic.ReadLists(user, customer, false);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomerWithDetails_ReturnsExpectedCountOfTwoItemsForHeaderIdOf1() {
                // arrange
                var logic = MakeMockLogic();
                var customer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var user = new UserProfile();
                var headerId = 1;
                var expected = 2;

                // act
                var results = logic.ReadLists(user, customer, false);
                var header = results.First(h => h.ListId == headerId);

                // assert
                header.Items
                      .Count
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodCustomerWithDetails_ReturnsExpectedCountOfTwoSharesForHeaderIdOf1() {
                // arrange
                var logic = MakeMockLogic();
                var customer = new UserSelectedContext() {
                                                             BranchId = "FUT",
                                                             CustomerId = "123456"
                                                         };
                var user = new UserProfile();
                var headerId = 1;
                var expected = 2;

                // act
                var results = logic.ReadLists(user, customer, false);
                var header = results.First(h => h.ListId == headerId);

                // assert
                header.SharedWith
                      .Count
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodCustomerWithoutDetails_ReturnsExpectedCountOfTwo() {
                // arrange
                var logic = MakeMockLogic();
                var customer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var user = new UserProfile();
                var expected = 2;

                // act
                var results = logic.ReadLists(user, customer, true);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodUserWithoutDetails_ReturnsExpectedCountOfTwo() {
                // arrange
                var logic = MakeMockLogic();
                var customer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                var user = new UserProfile() {
                    UserId = new Guid("c04afdba-90be-4cc9-8ec3-0969463a018c")
                };
                var expected = 2;

                // act
                var results = logic.ReadLists(user, customer, true);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void NullUserWithDetails_ReturnsCountOfTwoHeaders() {
                // arrange
                var logic = MakeMockLogic();
                var customer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = null;
                var expected = 2;

                // act
                var results = logic.ReadLists(user, customer, false);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void NullUserWithoutDetails_ReturnsCountOfTwoHeaders() {
                // arrange
                var logic = MakeMockLogic();
                var customer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "123456"
                };
                UserProfile user = null;
                var expected = 2;

                // act
                var results = logic.ReadLists(user, customer, true);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomerWithSharedList_ReturnsExpectedListId()
            {
                // arrange
                var logic = MakeMockLogic();
                var customer = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "111111"
                };
                var user = new UserProfile();
                var expected = 1;

                // act
                var results = logic.ReadLists(user, customer, false);

                // assert
                results.First()
                       .ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void CustomerWithNoSharedList_DoesNotSeeList()
            {
                // arrange
                var logic = MakeMockLogic();
                var customer = new UserSelectedContext()
                {
                    BranchId = "FUT",
                    CustomerId = "222222"
                };
                var user = new UserProfile();
                var expected = 0;

                // act
                var results = logic.ReadLists(user, customer, false);

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
                var detail = new CustomListDetail();
                var logic = MakeMockLogic();

                // act
                Action act = () => logic.SaveItem(detail);

                // assert
                act.ShouldThrow<ArgumentException>();
            }

            // the list item is what is passed to detailRepo.save
            [Fact]
            public void GoodHeaderId_CallsTheExpectedMethod() {
                // arrange
                var detail = new CustomListDetail() {
                    HeaderId = 17
                };
                var detailRepo = new Mock<ICustomListDetailsRepository>();
                var headerRepo = new Mock<ICustomListHeadersRepository>();
                var sharesRepo = new Mock<ICustomListSharesRepository>();
                var logic = new CustomListLogicImpl(sharesRepo.Object, headerRepo.Object, detailRepo.Object);

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
                var fakeCustomer = new UserSelectedContext();
                var farkModel = new ListModel();
                var fakeUser = new UserProfile();
                var detail = new Mock<ICustomListDetailsRepository>();
                var header = new Mock<ICustomListHeadersRepository>();
                var shares = new Mock<ICustomListSharesRepository>();
                var logic = new CustomListLogicImpl(shares.Object, header.Object, detail.Object);

                // act
                var results = logic.SaveList(fakeUser, fakeCustomer, farkModel);

                // assert
                header.Verify(h => h.SaveCustomListHeader(It.IsAny<CustomListHeader>()), Times.Once);
            }

            [Fact]
            public void GoodHeaderNoDetail_DoesNotCallSaveDetails() {
                // arrange
                var fakeCustomer = new UserSelectedContext();
                var farkModel = new ListModel();
                var fakeUser = new UserProfile();
                var detail = new Mock<ICustomListDetailsRepository>();
                var header = new Mock<ICustomListHeadersRepository>();
                var shares = new Mock<ICustomListSharesRepository>();
                var logic = new CustomListLogicImpl(shares.Object, header.Object, detail.Object);

                // act
                var results = logic.SaveList(fakeUser, fakeCustomer, farkModel);

                // assert
                detail.Verify(d => d.SaveCustomListDetail(It.IsAny<CustomListDetail>()), Times.Never);
            }

            [Fact]
            public void GoodHeaderNoDetail_IfIsDeleteIsTrueSavesWithActiveFlagFalse() {
                // arrange
                var fakeCustomer = new UserSelectedContext();
                var fakeUser = new UserProfile();
                var farkModel = new ListModel() {
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

                var detail = new Mock<ICustomListDetailsRepository>();
                var header = new Mock<ICustomListHeadersRepository>();
                var shares = new Mock<ICustomListSharesRepository>();
                var logic = new CustomListLogicImpl(shares.Object, header.Object, detail.Object);

                // act
                var results = logic.SaveList(fakeUser, fakeCustomer, farkModel);

                // assert
                detail.Verify(x => x.SaveCustomListDetail(It.Is<CustomListDetail>(d => d.Active.Equals(false))), Times.Once);
            }

            [Fact]
            public void GoodHeaderNoDetail_IfIsDeleteIsFalseAndActiveIsFalseSavesWithActiveFlagTrue() {
                // arrange
                var fakeCustomer = new UserSelectedContext();
                var fakeUser = new UserProfile();
                var farkModel = new ListModel() {
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

                var detail = new Mock<ICustomListDetailsRepository>();
                var header = new Mock<ICustomListHeadersRepository>();
                var shares = new Mock<ICustomListSharesRepository>();
                var logic = new CustomListLogicImpl(shares.Object, header.Object, detail.Object);

                // act
                var results = logic.SaveList(fakeUser, fakeCustomer, farkModel);

                // assert
                detail.Verify(x => x.SaveCustomListDetail(It.Is<CustomListDetail>(d => d.Active.Equals(true))), Times.Once);
            }

            [Fact]
            public void GoodHeaderGoodDetail_CallsSaveDetailsTwice() {
                // arrange
                var fakeCustomer = new UserSelectedContext();
                var fakeModel = new ListModel() {
                    ListId = 17,
                    Items = new List<ListItemModel>() {
                        new ListItemModel() {
                            ItemNumber = "123456"
                        },
                        new ListItemModel() {
                            ItemNumber = "234567"
                        }
                    }
                };
                var fakeUser = new UserProfile();
                var detail = new Mock<ICustomListDetailsRepository>();
                var header = new Mock<ICustomListHeadersRepository>();
                var shares = new Mock<ICustomListSharesRepository>();
                var logic = new CustomListLogicImpl(shares.Object, header.Object, detail.Object);

                // act
                var results = logic.SaveList(fakeUser, fakeCustomer, fakeModel);

                // assert
                detail.Verify(d => d.SaveCustomListDetail(It.IsAny<CustomListDetail>()), Times.Exactly(2));
            }


        }
    }
}
