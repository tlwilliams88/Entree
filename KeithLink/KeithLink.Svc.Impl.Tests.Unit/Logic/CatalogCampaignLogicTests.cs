using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;
using Moq;
using Xunit;

using KeithLink.Svc.Core.Interface.Marketing;
using KeithLink.Svc.Core.Models.Marketing;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic {
    public class CatalogCampaignLogicTests {
        private static Mock<ICatalogCampaignHeaderRepository> MockHeaderRepo() {
            Mock<ICatalogCampaignHeaderRepository> repo = new Mock<ICatalogCampaignHeaderRepository>();

            repo.Setup(r => r.GetHeader(It.Is<int>(i => i == 1)))
                .Returns(new CatalogCampaignHeader() { 
                    Active = true,
                    Description = "Mock Description",
                    EndDate = new DateTime(2017, 11, 16, 14, 50, 0, DateTimeKind.Local),
                    Id = 1,
                    Name = "Mock Name",
                    StartDate = new DateTime(2017, 11, 16, 14, 49, 0, DateTimeKind.Local),
                    Uri = "Mock Uri.jpg"
                });

            repo.Setup(r => r.GetByUri(It.Is<string>(s => s == "test uri.jpg")))
                .Returns(new CatalogCampaignHeader() {
                    Active = true,
                    Description = "Mock Description",
                    EndDate = new DateTime(2017, 11, 16, 14, 50, 0, DateTimeKind.Local),
                    Id = 1,
                    Name = "Mock Name",
                    StartDate = new DateTime(2017, 11, 16, 14, 49, 0, DateTimeKind.Local),
                    Uri = "test uri.jpg"
                });

            repo.Setup(r => r.GetAll())
                .Returns(new List<CatalogCampaignHeader>() {
                    new CatalogCampaignHeader() {
                        Active = true,
                        Description = "Mock Description",
                        EndDate = new DateTime(2017, 11, 16, 14, 50, 0, DateTimeKind.Local),
                        Id = 1,
                        Name = "Mock Name",
                        StartDate = new DateTime(2017, 11, 16, 14, 49, 0, DateTimeKind.Local),
                        Uri = "test uri.jpg",
                        HasFilter = true
                    },
                    new CatalogCampaignHeader() {
                        Active = true,
                        Description = "Mock Description",
                        EndDate = new DateTime(2017, 11, 16, 14, 50, 0, DateTimeKind.Local),
                        Id = 2,
                        Name = "Mock Name",
                        StartDate = new DateTime(2017, 11, 16, 14, 49, 0, DateTimeKind.Local),
                        Uri = "test uri.jpg"
                    },
                    new CatalogCampaignHeader() {
                        Active = true,
                        Description = "Mock Description",
                        EndDate = new DateTime(2017, 11, 16, 14, 50, 0, DateTimeKind.Local),
                        Id = 3,
                        Name = "Mock Name",
                        StartDate = new DateTime(2017, 11, 16, 14, 49, 0, DateTimeKind.Local),
                        Uri = "number three.jpg",
                        HasFilter = true
                    }
                });

            return repo;
        }

        private static Mock<ICatalogCampaignHeaderRepository> MockEmptyHeaderRepo() {
            Mock<ICatalogCampaignHeaderRepository> repo = new Mock<ICatalogCampaignHeaderRepository>();

            repo.Setup(r => r.GetAll())
                .Returns(new List<CatalogCampaignHeader>() {});

            return repo;
        }

        private static Mock<ICatalogCampaignHeaderRepository> MockAllFilteredyHeaderRepo() {
            Mock<ICatalogCampaignHeaderRepository> repo = new Mock<ICatalogCampaignHeaderRepository>();

            repo.Setup(r => r.GetAll())
                .Returns(new List<CatalogCampaignHeader>() { new CatalogCampaignHeader() {
                    Active = true,
                    Description = "Mock Description",
                    EndDate = new DateTime(2017, 11, 16, 14, 50, 0, DateTimeKind.Local),
                    Id = 1,
                    Name = "Mock Name",
                    StartDate = new DateTime(2017, 11, 16, 14, 49, 0, DateTimeKind.Local),
                    Uri = "test uri.jpg",
                    HasFilter = true
                }
            });

            return repo;
        }

        private static Mock<ICatalogCampaignHeaderRepository> MockNoFilteredyHeaderRepo() {
            Mock<ICatalogCampaignHeaderRepository> repo = new Mock<ICatalogCampaignHeaderRepository>();

            repo.Setup(r => r.GetAll())
                .Returns(new List<CatalogCampaignHeader>() { new CatalogCampaignHeader() {
                    Active = true,
                    Description = "Mock Description",
                    EndDate = new DateTime(2017, 11, 16, 14, 50, 0, DateTimeKind.Local),
                    Id = 1,
                    Name = "Mock Name",
                    StartDate = new DateTime(2017, 11, 16, 14, 49, 0, DateTimeKind.Local),
                    Uri = "test uri.jpg",
                    HasFilter = true
                }
            });

            return repo;
        }

        private static Mock<ICatalogCampaignItemRepository> MockItemRepo() { 
            Mock<ICatalogCampaignItemRepository> repo = new Mock<ICatalogCampaignItemRepository>();

            repo.Setup(r => r.GetByCampaign(It.Is<long>(i => i == 1)))
                .Returns(new List<CatalogCampaignItem>() { 
                    new CatalogCampaignItem() {
                        Id = 15,
                        Active = true,
                        ItemNumber = "123456",
                        CatalogCampaignHeaderId = 1
                    },
                    new CatalogCampaignItem() {
                        Id = 16,
                        Active = true,
                        ItemNumber = "123457",
                        CatalogCampaignHeaderId = 1
                    },
                    new CatalogCampaignItem() {
                        Id = 17,
                        Active = true,
                        ItemNumber = "123458",
                        CatalogCampaignHeaderId = 1
                    }
                });

            return repo;
        }

        private static Mock<ICampaignCustomerRepository> MockCustomerRepo() {
            Mock<ICampaignCustomerRepository> repo = new Mock<ICampaignCustomerRepository>();

            repo.Setup(r => r.GetAllCustomersByCampaign(It.Is<long>(i => i == 1)))
                .Returns(
                    new List<CampaignCustomer>() {
                        new CampaignCustomer() {
                            BranchId = "FDF",
                            CampaignId = 1,
                            CustomerNumber = "123456",
                        }
                    }
                );
            repo.Setup(r => r.GetAllCustomersByCampaign(It.Is<long>(i => i == 3)))
                .Returns(
                    new List<CampaignCustomer>() {
                        new CampaignCustomer() {
                            BranchId = "FDF",
                            CampaignId = 3,
                            CustomerNumber = "234567",
                        }
                    }
                );

            return repo;
        }

        private static ICatalogCampaignLogic MakeLogic(ICatalogCampaignHeaderRepository headerRepo = null, ICatalogCampaignItemRepository itemRepo = null,
                                                       ICampaignCustomerRepository customerRepository = null) {
            if(headerRepo == null) { headerRepo = MockHeaderRepo().Object; }
            if(itemRepo == null) { itemRepo = MockItemRepo().Object; }
            if(customerRepository == null) { customerRepository = MockCustomerRepo().Object; }

            return new CatalogCampaignLogicImpl(headerRepo, itemRepo, customerRepository);
        }

        public class AddOrUpdateCampaign { }

        public class GetAllAvailableCampaigns {
            [Fact]
            public void BadCustomer_ReturnsSingleRecord() {
                // arrange
                var logic = MakeLogic();
                var context = new UserSelectedContext() {
                    BranchId = "FDF",
                    CustomerId = "999999"
                };
                var expected = 1;

                // act
                var test = logic.GetAllAvailableCampaigns(context);

                // assert
                test.campaigns
                    .Count
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodCustomer_ReturnsTwoRecords() {
                // arrange
                var logic = MakeLogic();
                var context = new UserSelectedContext() {
                    BranchId = "FDF",
                    CustomerId = "123456"
                };
                var expected = 2;

                // act
                var test = logic.GetAllAvailableCampaigns(context);

                // assert
                test.campaigns
                    .Count
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void BadCustomer_CallsGetAllheadersOnce() {
                // arrange
                var repo = MockHeaderRepo();
                var logic = MakeLogic(headerRepo: repo.Object);
                var context = new UserSelectedContext() {
                    BranchId = "FDF",
                    CustomerId = "999999"
                };
                var expected = 1;

                // act
                var test = logic.GetAllAvailableCampaigns(context);

                // assert
                repo.Verify(r => r.GetAll(), 
                            Times.Exactly(expected));
            }

            [Fact]
            public void GoodCustomer_CallsGetAllheadersOnce() {
                // arrange
                var repo = MockHeaderRepo();
                var logic = MakeLogic(headerRepo: repo.Object);
                var context = new UserSelectedContext() {
                    BranchId = "FDF",
                    CustomerId = "123456"
                };
                var expected = 1;

                // act
                var test = logic.GetAllAvailableCampaigns(context);

                // assert
                repo.Verify(r => r.GetAll(),
                            Times.Exactly(expected));
            }

            [Fact]
            public void GoodCustomer_CallsGetCustomersTwice() {
                // arrange
                var repo = MockCustomerRepo();
                var logic = MakeLogic(customerRepository: repo.Object);
                var context = new UserSelectedContext() {
                    BranchId = "FDF",
                    CustomerId = "123456"
                };
                var expected = 2;

                // act
                var test = logic.GetAllAvailableCampaigns(context);

                // assert
                repo.Verify(r => r.GetAllCustomersByCampaign(It.IsAny<long>()),
                            Times.Exactly(expected));
            }

            [Fact]
            public void BadCustomer_CallsGetCustomersTwice() {
                // arrange
                var repo = MockCustomerRepo();
                var logic = MakeLogic(customerRepository: repo.Object);
                var context = new UserSelectedContext() {
                    BranchId = "FDF",
                    CustomerId = "999999"
                };
                var expected = 2;

                // act
                var test = logic.GetAllAvailableCampaigns(context);

                // assert
                repo.Verify(r => r.GetAllCustomersByCampaign(It.IsAny<long>()),
                            Times.Exactly(expected));
            }

            [Fact]
            public void NoCampaigns_ReturnsNoResults() {
                // arrange
                var repo = MockEmptyHeaderRepo();
                var logic = MakeLogic(headerRepo: repo.Object);
                var context = new UserSelectedContext() {
                    BranchId = "FDF",
                    CustomerId = "123456"
                };
                var expected = 0;

                // act
                var test = logic.GetAllAvailableCampaigns(context);

                // assert
                test.campaigns
                    .Count
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void OnlyFilteredCampaignsWithGoodCustomer_ReturnsOneResult() {
                // arrange
                var repo = MockAllFilteredyHeaderRepo();
                var logic = MakeLogic(headerRepo: repo.Object);
                var context = new UserSelectedContext() {
                    BranchId = "FDF",
                    CustomerId = "123456"
                };
                var expected = 1;

                // act
                var test = logic.GetAllAvailableCampaigns(context);

                // assert
                test.campaigns
                    .Count
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void OnlyFilteredCampaignsWithBadCustomer_ReturnsNoResults() {
                // arrange
                var repo = MockAllFilteredyHeaderRepo();
                var logic = MakeLogic(headerRepo: repo.Object);
                var context = new UserSelectedContext() {
                    BranchId = "FDF",
                    CustomerId = "999999"
                };
                var expected = 0;

                // act
                var test = logic.GetAllAvailableCampaigns(context);

                // assert
                test.campaigns
                    .Count
                    .Should()
                    .Be(expected);
            }
        }

        public class GetAllCampaigns {
            [Fact]
            public void GoodCallWithoutItems_CallsHeaderGetAllOnce() {
                // arrange
                var headerRepo = MockHeaderRepo();
                var logic = MakeLogic(headerRepo: headerRepo.Object);
                var includeItems = false;
                var expected = 1;

                // act
                logic.GetAllCampaigns(includeItems);

                // assert
                headerRepo.Verify(r => r.GetAll(),
                                  Times.Exactly(expected));
            }

            [Fact]
            public void GoodCallWithItems_CallsHeaderGetAllOnce() {
                // arrange
                var headerRepo = MockHeaderRepo();
                var logic = MakeLogic(headerRepo: headerRepo.Object);
                var includeItems = true;
                var expected = 1;

                // act
                logic.GetAllCampaigns(includeItems);

                // assert
                headerRepo.Verify(r => r.GetAll(),
                                  Times.Exactly(expected));
            }

            [Fact]
            public void GoodCallWithoutItems_DoesNotCallGetItems() {
                // arrange
                var itemRepo = MockItemRepo();
                var logic = MakeLogic(itemRepo: itemRepo.Object);
                var includeItems = false;
                var expected = 0;

                // act
                logic.GetAllCampaigns(includeItems);

                // assert
                itemRepo.Verify(r => r.GetByCampaign(It.IsAny<long>()),
                                     Times.Exactly(expected));
            }

            [Fact]
            public void GoodCallWithoutItems_CallsGetByCampaignTwice() {
                // arrange
                var itemRepo = MockItemRepo();
                var logic = MakeLogic(itemRepo: itemRepo.Object);
                var includeItems = true;
                var expected = 3;

                // act
                logic.GetAllCampaigns(includeItems);

                // assert
                itemRepo.Verify(r => r.GetByCampaign(It.IsAny<long>()),
                                Times.Exactly(expected));
            }
        }

        public class GetCampaign {
            [Fact]
            public void BadIdWithNoItems_ThrowsException() {
                // arrange
                var logic = MakeLogic();
                var id = 999;
                var includeItems = false;

                // act
                Action act = () => logic.GetCampaign(id, includeItems);

                // assert
                act.ShouldThrow<NullReferenceException>();
            }

            [Fact]
            public void BadIdWithItems_ThrowsException() {
                // arrange
                var logic = MakeLogic();
                var id = 999;
                var includeItems = true;

                // act
                Action act = () => logic.GetCampaign(id, includeItems);

                // assert
                act.ShouldThrow<NullReferenceException>();
            }

            [Fact]
            public void GoodIdNoItems_CallsGetHeaderOnce() {
                // arrange
                var headerRepo = MockHeaderRepo();
                var logic = MakeLogic(headerRepo: headerRepo.Object);
                var id = 1;
                var includeItems = false;
                var expected = 1;

                // act
                var test = logic.GetCampaign(id, includeItems);

                // assert
                headerRepo.Verify(r => r.GetHeader(It.Is<int>(i => i == 1)),
                                  Times.Exactly(expected));
            }

            [Fact]
            public void GoodIdNoItems_DoesNotCallItemRepo() {
                // arrange
                var itemRepo = MockItemRepo();
                var logic = MakeLogic(itemRepo: itemRepo.Object);
                var id = 1;
                var includeItems = false;
                var expected = 0;

                // act
                var test = logic.GetCampaign(id, includeItems);

                // assert
                itemRepo.Verify(r => r.GetByCampaign(It.Is<int>(i => i == 1)), 
                                Times.Exactly(expected));
            }

            [Fact]
            public void GoodIdWithItems_CallsItemRepoOnce() {
                // arrange
                var itemRepo = MockItemRepo();
                var logic = MakeLogic(itemRepo: itemRepo.Object);
                var id = 1;
                var includeItems = true;
                var expected = 1;

                // act
                var test = logic.GetCampaign(id, includeItems);

                // assert
                itemRepo.Verify(r => r.GetByCampaign(It.Is<long>(i => i == 1)),
                                Times.Exactly(expected));
            }
        }

        public class GetCampaignByUri {
            [Fact]
            public void BadUrlWithNoItems_ThrowsException() {
                // arrange
                var logic = MakeLogic();
                var uri = "bad uri";
                var includeItems = false;

                // act
                Action act = () => logic.GetCampaignByUri(uri, includeItems);

                // assert
                act.ShouldThrow<NullReferenceException>();
            }

            [Fact]
            public void BadIdWithItems_ThrowsException() {
                // arrange
                var logic = MakeLogic();
                var uri = "bad uri";
                var includeItems = true;

                // act
                Action act = () => logic.GetCampaignByUri(uri, includeItems);

                // assert
                act.ShouldThrow<NullReferenceException>();
            }

            [Fact]
            public void GoodIdNoItems_CallsGetHeaderOnce() {
                // arrange
                var headerRepo = MockHeaderRepo();
                var logic = MakeLogic(headerRepo: headerRepo.Object);
                var uri = "test uri.jpg";
                var includeItems = false;
                var expected = 1;

                // act
                var test = logic.GetCampaignByUri(uri, includeItems);

                // assert
                headerRepo.Verify(r => r.GetByUri(It.Is<string>(s => s == uri)),
                                  Times.Exactly(expected));
            }

            [Fact]
            public void GoodIdNoItems_DoesNotCallItemRepo() {
                // arrange
                var itemRepo = MockItemRepo();
                var logic = MakeLogic(itemRepo: itemRepo.Object);
                var uri = "test uri.jpg";
                var includeItems = false;
                var expected = 0;

                // act
                var test = logic.GetCampaignByUri(uri, includeItems);

                // assert
                itemRepo.Verify(r => r.GetByCampaign(It.Is<int>(i => i == 1)),
                                Times.Exactly(expected));
            }

            [Fact]
            public void GoodIdWithItems_CallsItemRepoOnce() {
                // arrange
                var itemRepo = MockItemRepo();
                var logic = MakeLogic(itemRepo: itemRepo.Object);
                var uri = "test uri.jpg";
                var includeItems = true;
                var expected = 1;

                // act
                var test = logic.GetCampaignByUri(uri, includeItems);

                // assert
                itemRepo.Verify(r => r.GetByCampaign(It.Is<long>(i => i == 1)),
                                Times.Exactly(expected));
            }
        }
    }
}
