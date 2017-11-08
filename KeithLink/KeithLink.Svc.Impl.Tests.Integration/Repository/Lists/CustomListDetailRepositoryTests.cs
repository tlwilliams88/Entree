using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.CustomList;
using KeithLink.Svc.Impl.Repository.SmartResolver;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Lists {
    public class CustomListDetailRepositoryTests {
        private static ICustomListDetailsRepository MakeRepo() {
            IContainer dimap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return dimap.Resolve<ICustomListDetailsRepository>();
        }

        public class GetCustomListDetails : MigratedDatabaseTest {
            [Fact]
            public void GoodHeaderId_ReturnsExpectedCount() {
                // arrange
                var expected = 2;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListDetails(headerId);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodRecord_HasExpectedHeaderId() {
                // arrange
                var expected = 1;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListDetails(headerId);

                // assert
                results[0].HeaderId
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void GoodRecord_HasExpectedItemNumber() {
                // arrange
                var expected = "123456";
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListDetails(headerId);

                // assert
                results[0].ItemNumber
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void GoodRecord_HasExpectedEach() {
                // arrange
                var expected = true;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListDetails(headerId);

                // assert
                results[0].Each
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void GoodRecord_HasExpectedPar() {
                // arrange
                var expected = 1;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListDetails(headerId);

                // assert
                results[0].Par
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void GoodRecord_HasExpectedLabel() {
                // arrange
                var expected = "Fake Label";
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListDetails(headerId);

                // assert
                results[0].Label
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void GoodRecord_HasExpectedCatalogId() {
                // arrange
                var expected = "FDF";
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListDetails(headerId);

                // assert
                results[0].CatalogId
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void GoodRecord_HasExpectedCustomInventoryId() {
                // arrange
                var expected = 0;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListDetails(headerId);

                // assert
                results[0].CustomInventoryItemId
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void GoodRecord_HasExpectedActive() {
                // arrange
                var expected = true;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListDetails(headerId);

                // assert
                results[0].Active
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void GoodRecord_HasExpectedCreatedUtc() {
                // arrange
                var expected = new DateTime(2017, 6, 26, 15, 37, 0, DateTimeKind.Utc);
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListDetails(headerId);

                // assert
                results[0].CreatedUtc
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void GoodRecord_HasExpectedModifiedUtc() {
                // arrange
                var expected = new DateTime(2017, 6, 26, 15, 38, 0, DateTimeKind.Utc);
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListDetails(headerId);

                // assert
                results[0].ModifiedUtc
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void PartialRecord_HasNullItemNumber() {
                // arrange
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListDetails(headerId);

                // assert
                results[1]
                    .ItemNumber
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void PartialRecord_HasNullEach() {
                // arrange
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListDetails(headerId);

                // assert
                results[1]
                    .Each
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void PartialRecord_HasNullLabel() {
                // arrange
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListDetails(headerId);

                // assert
                results[1]
                    .Label
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void PartialRecord_HasNullCatalogId() {
                // arrange
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListDetails(headerId);

                // assert
                results[1]
                    .CatalogId
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void PartialRecord_HasNullCustomInventoryItemId() {
                // arrange
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetCustomListDetails(headerId);

                // assert
                results[1]
                    .CustomInventoryItemId
                    .Should()
                    .BeNull();
            }

        }

        public class SaveCustomListDetail : MigratedDatabaseTest {
            private static CustomListDetail MakeDetail() {
                return new CustomListDetail() {
                    Active = true,
                    CatalogId = "FDF",
                    CustomInventoryItemId = 15,
                    Each = true,
                    HeaderId = 1,
                    ItemNumber = "100123",
                    Label = "Fake Label",
                    LineNumber = 300,
                    Par = 17
                };
            }

            [Fact]
            public void GoodDetail_DeleteSetsExpectedActive() {
                // arrange
                var detail = MakeDetail();
                var repo = MakeRepo();
                var expected = 0;

                // act
                detail.Active = false;
                var id = repo.SaveCustomListDetail(detail);
                var results = repo.GetCustomListDetails(detail.HeaderId);

                // assert
                results.Count(x => x.Id.Equals(id))
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesExpectedActive() {
                // arrange
                var detail = MakeDetail();
                var expected = true;
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveCustomListDetail(detail);
                var results = repo.GetCustomListDetails(detail.HeaderId);

                // assert
                results.First(d => d.Id == detailId)
                       .Active
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesExpectedCatalogId() {
                // arrange
                var detail = MakeDetail();
                var expected = "FDF";
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveCustomListDetail(detail);
                var results = repo.GetCustomListDetails(detail.HeaderId);

                // assert
                results.First(d => d.Id == detailId)
                       .CatalogId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesExpectedCustomInventoryItemId() {
                // arrange
                var detail = MakeDetail();
                var expected = 15;
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveCustomListDetail(detail);
                var results = repo.GetCustomListDetails(detail.HeaderId);

                // assert
                results.First(d => d.Id == detailId)
                       .CustomInventoryItemId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesExpectedEach() {
                // arrange
                var detail = MakeDetail();
                var expected = true;
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveCustomListDetail(detail);
                var results = repo.GetCustomListDetails(detail.HeaderId);

                // assert
                results.First(d => d.Id == detailId)
                       .Each
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesExpectedHeaderId() {
                // arrange
                var detail = MakeDetail();
                var expected = 1;
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveCustomListDetail(detail);
                var results = repo.GetCustomListDetails(detail.HeaderId);

                // assert
                results.First(d => d.Id == detailId)
                       .HeaderId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesExpectedItemNumber() {
                // arrange
                var detail = MakeDetail();
                var expected = "100123";
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveCustomListDetail(detail);
                var results = repo.GetCustomListDetails(detail.HeaderId);

                // assert
                results.First(d => d.Id == detailId)
                       .ItemNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesExpectedLabel() {
                // arrange
                var detail = MakeDetail();
                var expected = "Fake Label";
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveCustomListDetail(detail);
                var results = repo.GetCustomListDetails(detail.HeaderId);

                // assert
                results.First(d => d.Id == detailId)
                       .Label
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesExpectedLineNumber() {
                // arrange
                var detail = MakeDetail();
                var expected = 300;
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveCustomListDetail(detail);
                var results = repo.GetCustomListDetails(detail.HeaderId);

                // assert
                results.First(d => d.Id == detailId)
                       .LineNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesExpectedPar() {
                // arrange
                var detail = MakeDetail();
                var expected = 17;
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveCustomListDetail(detail);
                var results = repo.GetCustomListDetails(detail.HeaderId);

                // assert
                results.First(d => d.Id == detailId)
                       .Par
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void NullCatalogId_ReturnsNull() {
                // arrange
                var detail = new CustomListDetail() {
                    Active = true,
                    CatalogId = null,
                    CustomInventoryItemId = 15,
                    Each = true,
                    HeaderId = 1,
                    ItemNumber = "100123",
                    Label = "Fake Label",
                    LineNumber = 300,
                    Par = 17
                };
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveCustomListDetail(detail);
                var results = repo.GetCustomListDetails(detail.HeaderId);

                // assert
                results.First(d => d.Id == detailId)
                       .CatalogId
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void NullCustomInventoryItemId_ReturnsNull() {
                // arrange
                var detail = new CustomListDetail() {
                    Active = true,
                    CatalogId = "FDF",
                    CustomInventoryItemId = null,
                    Each = true,
                    HeaderId = 1,
                    ItemNumber = "100123",
                    Label = "Fake Label",
                    LineNumber = 300,
                    Par = 17
                };
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveCustomListDetail(detail);
                var results = repo.GetCustomListDetails(detail.HeaderId);

                // assert
                results.First(d => d.Id == detailId)
                       .CustomInventoryItemId
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void NullEach_ReturnsNull() {
                // arrange
                var detail = new CustomListDetail() {
                    Active = true,
                    CatalogId = "FDF",
                    CustomInventoryItemId = 15,
                    Each = null,
                    HeaderId = 1,
                    ItemNumber = "100123",
                    Label = "Fake Label",
                    LineNumber = 300,
                    Par = 17
                };
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveCustomListDetail(detail);
                var results = repo.GetCustomListDetails(detail.HeaderId);

                // assert
                results.First(d => d.Id == detailId)
                       .Each
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void NullItemNumber_ReturnsNull() {
                // arrange
                var detail = new CustomListDetail() {
                    Active = true,
                    CatalogId = "FDF",
                    CustomInventoryItemId = 15,
                    Each = true,
                    HeaderId = 1,
                    ItemNumber = null,
                    Label = "Fake Label",
                    LineNumber = 300,
                    Par = 17
                };
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveCustomListDetail(detail);
                var results = repo.GetCustomListDetails(detail.HeaderId);

                // assert
                results.First(d => d.Id == detailId)
                       .ItemNumber
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void NullLabel_ReturnsNull() {
                // arrange
                var detail = new CustomListDetail() {
                    Active = true,
                    CatalogId = "FDF",
                    CustomInventoryItemId = 15,
                    Each = true,
                    HeaderId = 1,
                    ItemNumber = "100123",
                    Label = null,
                    LineNumber = 300,
                    Par = 17
                };
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveCustomListDetail(detail);
                var results = repo.GetCustomListDetails(detail.HeaderId);

                // assert
                results.First(d => d.Id == detailId)
                       .Label
                       .Should()
                       .BeNull();
            }

        }
    }
}
