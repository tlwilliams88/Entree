using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using FluentAssertions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists.Favorites;
using KeithLink.Svc.Impl.Repository.SmartResolver;
using Xunit;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Lists {
    public class FavoriteListDetailsRepositoryTests {
        private static IFavoriteListDetailsRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return diMap.Resolve<IFavoriteListDetailsRepository>();
        }

        public class DeleteFavoriteListDetail : MigratedDatabaseTest { }

        public class GetFavorieDetail : MigratedDatabaseTest {
            [Fact]
            public void GoodDetail_ReturnsExpectedActive() {
                // arrange
                var detailId = 1;
                var expected = true;
                var repo = MakeRepo();

                // act
                var results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                    .Active
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedHeaderId() {
                // arrange
                var detailId = 1;
                var expected = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                    .HeaderId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedItemNumber() {
                // arrange
                var detailId = 1;
                var expected = "123456";
                var repo = MakeRepo();

                // act
                var results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                    .ItemNumber
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedLineNumber() {
                // arrange
                var detailId = 1;
                var expected = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                    .LineNumber
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedEach() {
                // arrange
                var detailId = 1;
                var expected = true;
                var repo = MakeRepo();

                // act
                var results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                    .Each
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedLabel() {
                // arrange
                var detailId = 1;
                var expected = "Fake Label";
                var repo = MakeRepo();

                // act
                var results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                    .Label
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCatalogId() {
                // arrange
                var detailId = 1;
                var expected = "FDF";
                var repo = MakeRepo();

                // act
                var results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                    .CatalogId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCreatedUtc() {
                // arrange
                var detailId = 1;
                var expected = new DateTime(2017, 6, 29, 16, 29, 0, DateTimeKind.Utc);
                var repo = MakeRepo();

                // act
                var results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                    .CreatedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedModifiedUtc() {
                // arrange
                var detailId = 1;
                var expected = new DateTime(2017, 6, 29, 16, 30, 0, DateTimeKind.Utc);
                var repo = MakeRepo();

                // act
                var results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                    .ModifiedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void NullEach_ReturnsExpectedValue() {
                // arrange
                var detailId = 4;
                var repo = MakeRepo();

                // act
                var results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                    .Each
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void NullLabel_ReturnsExpectedValue() {
                // arrange
                var detailId = 4;
                var repo = MakeRepo();

                // act
                var results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                    .Label
                    .Should()
                    .BeNull();
            }

            [Fact]
            public void NullCatalogId_ReturnsExpectedValue() {
                // arrange
                var detailId = 4;
                var repo = MakeRepo();

                // act
                var results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                    .CatalogId
                    .Should()
                    .BeNull();
            }
        }

        public class GetFavoritesListDetails : MigratedDatabaseTest {
            [Fact]
            public void GoodHeaderId_ReturnsExpectedCount() {
                // arrange
                var expected = 3;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetFavoritesListDetails(headerId);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedActiveCount() {
                // arrange
                var expected = 3;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetFavoritesListDetails(headerId);

                // assert
                results.Count(f => f.Active)
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedInactiveCount() {
                // arrange
                var expected = 0;
                var headerId = 1;
                var repo = MakeRepo();

                // act
                var results = repo.GetFavoritesListDetails(headerId);

                // assert
                results.Count(f => f.Active == false)
                       .Should()
                       .Be(expected);
            }
        }

        public class SaveFavoriteListDetail : MigratedDatabaseTest {
            private static FavoritesListDetail MakeDetail() {
                return new FavoritesListDetail() {
                    Active = true,
                    CatalogId = "FAT",
                    CreatedUtc = new DateTime(2017, 6, 30, 8, 47, 0, DateTimeKind.Utc),
                    Each = true,
                    HeaderId = 1,
                    ItemNumber = "123456",
                    Label = "Fake Label",
                    LineNumber = 17,
                    ModifiedUtc = new DateTime(2017, 6, 30, 8, 48, 0, DateTimeKind.Utc)
                };
            }

            [Fact]
            public void GoodDetail_SavesCorrectActive() {
                // arrange
                var detail = MakeDetail();
                var expected = true;
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveFavoriteListDetail(detail);
                var result = repo.GetFavoriteDetail(detailId);

                // assert
                result.Active
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesCorrectCatalogId() {
                // arrange
                var detail = MakeDetail();
                var expected = "FAT";
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveFavoriteListDetail(detail);
                var result = repo.GetFavoriteDetail(detailId);

                // assert
                result.CatalogId
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodDetail_DoesNotUseSetCreatedUtc() {
                // arrange
                var detail = MakeDetail();
                var expected = new DateTime(2017, 6, 30, 8, 47, 0, DateTimeKind.Utc);
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveFavoriteListDetail(detail);
                var result = repo.GetFavoriteDetail(detailId);

                // assert
                result.CreatedUtc
                      .Should()
                      .NotBe(expected);
            }

            [Fact]
            public void GoodDetail_SavesCorrectEach() {
                // arrange
                var detail = MakeDetail();
                var expected = true;
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveFavoriteListDetail(detail);
                var result = repo.GetFavoriteDetail(detailId);

                // assert
                result.Each
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesCorrectHeaderId() {
                // arrange
                var detail = MakeDetail();
                var expected = 1;
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveFavoriteListDetail(detail);
                var result = repo.GetFavoriteDetail(detailId);

                // assert
                result.HeaderId
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesCorrectItemNumber() {
                // arrange
                var detail = MakeDetail();
                var expected = "123456";
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveFavoriteListDetail(detail);
                var result = repo.GetFavoriteDetail(detailId);

                // assert
                result.ItemNumber
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesCorrectLabel() {
                // arrange
                var detail = MakeDetail();
                var expected = "Fake Label";
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveFavoriteListDetail(detail);
                var result = repo.GetFavoriteDetail(detailId);

                // assert
                result.Label
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesCorrectLineNumber() {
                // arrange
                var detail = MakeDetail();
                var expected = 17;
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveFavoriteListDetail(detail);
                var result = repo.GetFavoriteDetail(detailId);

                // assert
                result.LineNumber
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodDetail_DoesNotUseSetModifiedUtc() {
                // arrange
                var detail = MakeDetail();
                var expected = new DateTime(2017, 6, 30, 8, 48, 0, DateTimeKind.Utc);
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveFavoriteListDetail(detail);
                var result = repo.GetFavoriteDetail(detailId);

                // assert
                result.ModifiedUtc
                      .Should()
                      .NotBe(expected);
            }

            [Fact]
            public void NullCatalogId_SavesExpectedValue() {
                // arrange
                var detail = new FavoritesListDetail() {
                    Active = true,
                    CreatedUtc = new DateTime(2017, 6, 30, 8, 47, 0, DateTimeKind.Utc),
                    Each = true,
                    HeaderId = 1,
                    ItemNumber = "123456",
                    Label = "Fake Label",
                    LineNumber = 17,
                    ModifiedUtc = new DateTime(2017, 6, 30, 8, 48, 0, DateTimeKind.Utc)
                };
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveFavoriteListDetail(detail);
                var results = repo.GetFavoriteDetail(detailId);

                // assert
                results.CatalogId
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void NullEach_SavesExpectedValue() {
                // arrange
                var detail = new FavoritesListDetail() {
                    Active = true,
                    CatalogId = "FAT",
                    CreatedUtc = new DateTime(2017, 6, 30, 8, 47, 0, DateTimeKind.Utc),
                    HeaderId = 1,
                    ItemNumber = "123456",
                    Label = "Fake Label",
                    LineNumber = 17,
                    ModifiedUtc = new DateTime(2017, 6, 30, 8, 48, 0, DateTimeKind.Utc)
                };
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveFavoriteListDetail(detail);
                var results = repo.GetFavoriteDetail(detailId);

                // assert
                results.Each
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void NullLabel_SavesExpectedValue() {
                // arrange
                var detail = new FavoritesListDetail() {
                    Active = true,
                    CatalogId = "FAT",
                    CreatedUtc = new DateTime(2017, 6, 30, 8, 47, 0, DateTimeKind.Utc),
                    Each = true,
                    HeaderId = 1,
                    ItemNumber = "123456",
                    LineNumber = 17,
                    ModifiedUtc = new DateTime(2017, 6, 30, 8, 48, 0, DateTimeKind.Utc)
                };
                var repo = MakeRepo();

                // act
                var detailId = repo.SaveFavoriteListDetail(detail);
                var results = repo.GetFavoriteDetail(detailId);

                // assert
                results.Label
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void NullItemNumber_ShouldThrowSqlException() {
                // arrange
                var detail = new FavoritesListDetail() {
                    Active = true,
                    CatalogId = "FAT",
                    CreatedUtc = new DateTime(2017, 6, 30, 8, 47, 0, DateTimeKind.Utc),
                    Each = true,
                    HeaderId = 1,
                    Label = "Fake Label",
                    LineNumber = 17,
                    ModifiedUtc = new DateTime(2017, 6, 30, 8, 48, 0, DateTimeKind.Utc)
                };
                var repo = MakeRepo();

                // act
                Action act = () => { repo.SaveFavoriteListDetail(detail); };

                // assert
                act.ShouldThrow<SqlException>();
            }

        }
    }
}
