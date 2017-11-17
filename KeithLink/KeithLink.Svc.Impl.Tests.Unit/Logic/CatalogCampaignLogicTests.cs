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
                        Uri = "test uri.jpg"
                    },
                    new CatalogCampaignHeader() {
                        Active = true,
                        Description = "Mock Description",
                        EndDate = new DateTime(2017, 11, 16, 14, 50, 0, DateTimeKind.Local),
                        Id = 2,
                        Name = "Mock Name",
                        StartDate = new DateTime(2017, 11, 16, 14, 49, 0, DateTimeKind.Local),
                        Uri = "test uri.jpg"
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

        private static ICatalogCampaignLogic MakeLogic(ICatalogCampaignHeaderRepository headerRepo = null, ICatalogCampaignItemRepository itemRepo = null) {
            if(headerRepo == null) { headerRepo = MockHeaderRepo().Object; }
            if(itemRepo == null) { itemRepo = MockItemRepo().Object; }

            return new CatalogCampaignLogicImpl(headerRepo, itemRepo);
        }

        public class AddOrUpdateCampaign { }

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
                var expected = 2;

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
