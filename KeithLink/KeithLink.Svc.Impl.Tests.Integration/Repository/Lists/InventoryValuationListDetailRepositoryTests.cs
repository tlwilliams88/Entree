using System;
using System.Linq;

using Autofac;
using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;
using KeithLink.Svc.Impl.Repository.SmartResolver;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Lists {
    public class InventoryValuationListDetailRepositoryTests {
        public static IInventoryValuationListDetailsRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return diMap.Resolve<IInventoryValuationListDetailsRepository>();
        }

        public class GetInventoryValuationDetails : MigratedDatabaseTest {
            [Fact]
            public void BadHeaderId_ReturnsNoResults() {
                // arrange
                var expected = 0;
                var headerId = 0;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);
                
                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodeaderId_ReturnsExpectedCount() {
                // arrange
                var expected = 3;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodeaderId_ReturnsExpectedCountOfActive() {
                // arrange
                var expected = 3;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results.Count(r => r.Active)
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodeaderId_ReturnsExpectedCountOfInactive() {
                // arrange
                var expected = 0;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results.Count(r => r.Active == false)
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedHeaderId() {
                // arrange
                var expected = 1;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[0]
                    .HeaderId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCustomInventoryItemId() {
                // arrange
                var expected = 100;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[0]
                    .CustomInventoryItemId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedItemNumber() {
                // arrange
                var expected = "123456";
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[0]
                    .ItemNumber
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedLineNumber() {
                // arrange
                var expected = 1;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[0]
                    .LineNumber
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedEach() {
                // arrange
                var expected = true;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[0]
                    .Each
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedQuantity() {
                // arrange
                var expected = 2;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[0]
                    .Quantity
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCatalogId() {
                // arrange
                var expected = "FDF";
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[0]
                    .CatalogId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedActive() {
                // arrange
                var expected = true;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[0]
                    .Active
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedLabel() {
                // arrange
                var expected = "Test Label #1";
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results.FirstOrDefault()
                       .Label
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCreatedUtc() {
                // arrange
                var expected = new DateTime(2017, 7, 3, 11, 33, 0, DateTimeKind.Utc);
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[0]
                    .CreatedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedModifiedUtc() {
                // arrange
                var expected = new DateTime(2017, 7, 3, 11, 34, 0, DateTimeKind.Utc);
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[0]
                    .ModifiedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void DetailWithNullEach_ReturnsNull() {
                // arrange
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[2]
                    .Each
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void DetailWithNullCatalogId_ReturnsNull() {
                // arrange
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[2]
                    .CatalogId
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void DetailWithNullCustomInventoryItemId_ReturnsNull() {
                // arrange
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetInventoryValuationDetails(headerId);

                // assert
                results[2]
                    .CustomInventoryItemId
                    .Should()
                    .BeNull();
            }
        }

        public class SaveInventoryValuationDetail : MigratedDatabaseTest { 
            private static InventoryValuationListDetail MakeDetail() {
                return new InventoryValuationListDetail() {
                    Active = true,
                    CatalogId = "FRT",
                    CreatedUtc = new DateTime(2017, 7, 3, 14, 46, 0, DateTimeKind.Utc),
                    CustomInventoryItemId = 1000,
                    Each = true,
                    HeaderId = 1,
                    ItemNumber = "123456",
                    LineNumber = 2,
                    ModifiedUtc = new DateTime(2017, 7, 3, 14, 47, 0, DateTimeKind.Utc),
                    Quantity = 15,
                    Label = "Test Save Label"
                };
            }

            [Fact]
            public void GoodDetail_SavesExpectedActive() {
                // arrange
                var detail = MakeDetail();
                var expected = true;
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveInventoryValuationDetail(detail);
                var results = repo.GetInventoryValuationDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId).First();

                // assert
                result
                    .Active
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
                var detailId = repo.SaveInventoryValuationDetail(detail);
                var results = repo.GetInventoryValuationDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId).First();

                // assert
                result
                    .HeaderId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesExpectedCustomInventoryItemId() {
                // arrange
                var detail = MakeDetail();
                var expected = 1000;
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveInventoryValuationDetail(detail);
                var results = repo.GetInventoryValuationDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId).First();

                // assert
                result
                    .CustomInventoryItemId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesExpectedCatalogId() {
                // arrange
                var detail = MakeDetail();
                var expected = "FRT";
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveInventoryValuationDetail(detail);
                var results = repo.GetInventoryValuationDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId).First();

                // assert
                result
                    .CatalogId
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
                var detailId = repo.SaveInventoryValuationDetail(detail);
                var results = repo.GetInventoryValuationDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId).First();

                // assert
                result
                    .Each
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesExpectedItemNumber() {
                // arrange
                var detail = MakeDetail();
                var expected = "123456";
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveInventoryValuationDetail(detail);
                var results = repo.GetInventoryValuationDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId).First();

                // assert
                result
                    .ItemNumber
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesExpectedQuantity() {
                // arrange
                var detail = MakeDetail();
                var expected = 15;
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveInventoryValuationDetail(detail);
                var results = repo.GetInventoryValuationDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId).First();

                // assert
                result
                    .Quantity
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesExpectedLineNumber() {
                // arrange
                var detail = MakeDetail();
                var expected = 2;
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveInventoryValuationDetail(detail);
                var results = repo.GetInventoryValuationDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId).First();

                // assert
                result
                    .LineNumber
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesExpectedLabel() {
                // arrange
                var detail = MakeDetail();
                var expected = "Test Save Label";
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveInventoryValuationDetail(detail);
                var results = repo.GetInventoryValuationDetails(detail.HeaderId);
                
                // assert
                results.FirstOrDefault(d => d.Id.Equals(detailId))
                       .Label
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_DoesNotSaveSetCreatedUtc() {
                // arrange
                var detail = MakeDetail();
                var expected = new DateTime(2017, 7, 3, 14, 46, 0, DateTimeKind.Utc);
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveInventoryValuationDetail(detail);
                var results = repo.GetInventoryValuationDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId).First();

                // assert
                result
                    .CreatedUtc
                    .Should()
                    .NotBe(expected);
            }

            [Fact]
            public void GoodDetail_DoesNotSaveSetModifiedUtc() {
                // arrange
                var detail = MakeDetail();
                var expected = new DateTime(2017, 7, 3, 14, 47, 0, DateTimeKind.Utc);
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveInventoryValuationDetail(detail);
                var results = repo.GetInventoryValuationDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId).First();

                // assert
                result
                    .ModifiedUtc
                    .Should()
                    .NotBe(expected);
            }

            [Fact]
            public void NullCatalogId_SavesNull() {
                // arrange
                var detail = new InventoryValuationListDetail() {
                    Active = true,
                    CreatedUtc = new DateTime(2017, 7, 3, 14, 46, 0, DateTimeKind.Utc),
                    CustomInventoryItemId = 1000,
                    Each = true,
                    HeaderId = 1,
                    ItemNumber = "123456",
                    LineNumber = 2,
                    ModifiedUtc = new DateTime(2017, 7, 3, 14, 47, 0, DateTimeKind.Utc),
                    Quantity = 15
                };
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveInventoryValuationDetail(detail);
                var results = repo.GetInventoryValuationDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId).First();

                // assert
                result
                    .CatalogId
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void NullCustomInventoryItemId_SavesNull() {
                // arrange
                var detail = new InventoryValuationListDetail() {
                    Active = true,
                    CatalogId = "FRT",
                    CreatedUtc = new DateTime(2017, 7, 3, 14, 46, 0, DateTimeKind.Utc),
                    Each = true,
                    HeaderId = 1,
                    ItemNumber = "123456",
                    LineNumber = 2,
                    ModifiedUtc = new DateTime(2017, 7, 3, 14, 47, 0, DateTimeKind.Utc),
                    Quantity = 15
                };
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveInventoryValuationDetail(detail);
                var results = repo.GetInventoryValuationDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId).First();

                // assert
                result
                    .CustomInventoryItemId
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void NullEach_SavesNull() {
                // arrange
                var detail = new InventoryValuationListDetail() {
                    Active = true,
                    CatalogId = "FRT",
                    CreatedUtc = new DateTime(2017, 7, 3, 14, 46, 0, DateTimeKind.Utc),
                    CustomInventoryItemId = 1000,
                    HeaderId = 1,
                    ItemNumber = "123456",
                    LineNumber = 2,
                    ModifiedUtc = new DateTime(2017, 7, 3, 14, 47, 0, DateTimeKind.Utc),
                    Quantity = 15
                };
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveInventoryValuationDetail(detail);
                var results = repo.GetInventoryValuationDetails(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId).First();

                // assert
                result
                    .Each
                    .Should()
                    .BeNull();
            }

        }
    }
}
