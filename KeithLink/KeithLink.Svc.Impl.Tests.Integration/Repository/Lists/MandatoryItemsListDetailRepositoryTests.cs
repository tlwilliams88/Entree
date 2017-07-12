using System;
using System.Linq;

using Autofac;
using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.MandatoryItem;
using KeithLink.Svc.Impl.Repository.SmartResolver;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Lists {
    public class MandatoryItemsListDetailRepositoryTests {
        public static IMandatoryItemsListDetailsRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return diMap.Resolve<IMandatoryItemsListDetailsRepository>();
        }

        public class GetMandatoryItemsDetails : MigratedDatabaseTest {
            [Fact]
            public void BadHeaderId_ReturnsNoResults() {
                // arrange
                var expected = 0;
                var headerId = 0;
                var repo = MakeRepo();

                // act
                var results = repo.GetAllByHeader(headerId);
                
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
                var results = repo.GetAllByHeader(headerId);

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
                var results = repo.GetAllByHeader(headerId);

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
                var results = repo.GetAllByHeader(headerId);

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
                var results = repo.GetAllByHeader(headerId);

                // assert
                results[0]
                    .HeaderId
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
                var results = repo.GetAllByHeader(headerId);

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
                var results = repo.GetAllByHeader(headerId);

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
                var results = repo.GetAllByHeader(headerId);

                // assert
                results[0]
                    .Each
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
                var results = repo.GetAllByHeader(headerId);

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
                var results = repo.GetAllByHeader(headerId);

                // assert
                results[0]
                    .Active
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
                var results = repo.GetAllByHeader(headerId);

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
                var results = repo.GetAllByHeader(headerId);

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
                var results = repo.GetAllByHeader(headerId);

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
                var results = repo.GetAllByHeader(headerId);

                // assert
                results[2]
                    .CatalogId
                    .Should()
                    .BeNull();
            }

        public class SaveInventoryValudationDetail : MigratedDatabaseTest { 
            private static MandatoryItemsListDetail MakeDetail() {
                return new MandatoryItemsListDetail() {
                    Active = true,
                    CatalogId = "FRT",
                    CreatedUtc = new DateTime(2017, 7, 3, 14, 46, 0, DateTimeKind.Utc),
                    Each = true,
                    HeaderId = 1,
                    ItemNumber = "123456",
                    LineNumber = 2,
                    ModifiedUtc = new DateTime(2017, 7, 3, 14, 47, 0, DateTimeKind.Utc),
                };
            }

            [Fact]
            public void GoodDetail_SavesExpectedActive() {
                // arrange
                var detail = MakeDetail();
                var expected = true;
                var repo = MakeRepo();

                // act
                var detailId = repo.Save(detail);
                var results = repo.GetAllByHeader(detail.HeaderId);
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
                var detailId = repo.Save(detail);
                var results = repo.GetAllByHeader(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId).First();

                // assert
                result
                    .HeaderId
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
                var detailId = repo.Save(detail);
                var results = repo.GetAllByHeader(detail.HeaderId);
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
                var detailId = repo.Save(detail);
                var results = repo.GetAllByHeader(detail.HeaderId);
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
                var detailId = repo.Save(detail);
                var results = repo.GetAllByHeader(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId).First();

                // assert
                result
                    .ItemNumber
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
                var detailId = repo.Save(detail);
                var results = repo.GetAllByHeader(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId).First();

                // assert
                result
                    .LineNumber
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
                var detailId = repo.Save(detail);
                var results = repo.GetAllByHeader(detail.HeaderId);
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
                var detailId = repo.Save(detail);
                var results = repo.GetAllByHeader(detail.HeaderId);
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
                var detail = new MandatoryItemsListDetail() {
                    Active = true,
                    CreatedUtc = new DateTime(2017, 7, 3, 14, 46, 0, DateTimeKind.Utc),
                    Each = true,
                    HeaderId = 1,
                    ItemNumber = "123456",
                    LineNumber = 2,
                    ModifiedUtc = new DateTime(2017, 7, 3, 14, 47, 0, DateTimeKind.Utc),
                };
                var repo = MakeRepo();

                // act
                var detailId = repo.Save(detail);
                var results = repo.GetAllByHeader(detail.HeaderId);
                var result = results.Where(r => r.Id == detailId).First();

                // assert
                result
                    .CatalogId
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void NullEach_SavesNull() {
                // arrange
                var detail = new MandatoryItemsListDetail() {
                    Active = true,
                    CatalogId = "FRT",
                    CreatedUtc = new DateTime(2017, 7, 3, 14, 46, 0, DateTimeKind.Utc),
                    HeaderId = 1,
                    ItemNumber = "123456",
                    LineNumber = 2,
                    ModifiedUtc = new DateTime(2017, 7, 3, 14, 47, 0, DateTimeKind.Utc),
                };
                var repo = MakeRepo();

                // act
                var detailId = repo.Save(detail);
                var results = repo.GetAllByHeader(detail.HeaderId);
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
