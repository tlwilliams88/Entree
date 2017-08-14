using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

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
                int detailId = 1;
                bool expected = true;
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                FavoritesListDetail results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                        .Active
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCatalogId() {
                // arrange
                int detailId = 1;
                string expected = "FDF";
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                FavoritesListDetail results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                        .CatalogId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCreatedUtc() {
                // arrange
                int detailId = 1;
                DateTime expected = new DateTime(2017, 6, 29, 16, 29, 0, DateTimeKind.Utc);
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                FavoritesListDetail results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                        .CreatedUtc
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedEach() {
                // arrange
                int detailId = 1;
                bool expected = true;
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                FavoritesListDetail results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                        .Each
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedHeaderId() {
                // arrange
                int detailId = 1;
                int expected = 1;
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                FavoritesListDetail results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                        .HeaderId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedItemNumber() {
                // arrange
                int detailId = 1;
                string expected = "123456";
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                FavoritesListDetail results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                        .ItemNumber
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedLabel() {
                // arrange
                int detailId = 1;
                string expected = "Fake Label";
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                FavoritesListDetail results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                        .Label
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedLineNumber() {
                // arrange
                int detailId = 1;
                int expected = 1;
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                FavoritesListDetail results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                        .LineNumber
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedModifiedUtc() {
                // arrange
                int detailId = 1;
                DateTime expected = new DateTime(2017, 6, 29, 16, 30, 0, DateTimeKind.Utc);
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                FavoritesListDetail results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                        .ModifiedUtc
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void NullCatalogId_ReturnsExpectedValue() {
                // arrange
                int detailId = 4;
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                FavoritesListDetail results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                        .CatalogId
                        .Should()
                        .BeNull();
            }

            [Fact]
            public void NullEach_ReturnsExpectedValue() {
                // arrange
                int detailId = 4;
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                FavoritesListDetail results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                        .Each
                        .Should()
                        .BeNull();
            }

            [Fact]
            public void NullLabel_ReturnsExpectedValue() {
                // arrange
                int detailId = 4;
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                FavoritesListDetail results = repo.GetFavoriteDetail(detailId);

                // assert
                results
                        .Label
                        .Should()
                        .BeNull();
            }
        }

        public class GetFavoritesListDetails : MigratedDatabaseTest {
            [Fact]
            public void GoodHeaderId_ReturnsExpectedActiveCount() {
                // arrange
                int expected = 3;
                int headerId = 1;
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                List<FavoritesListDetail> results = repo.GetFavoritesListDetails(headerId);

                // assert
                results.Count(f => f.Active)
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedCount() {
                // arrange
                int expected = 3;
                int headerId = 1;
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                List<FavoritesListDetail> results = repo.GetFavoritesListDetails(headerId);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderId_ReturnsExpectedInactiveCount() {
                // arrange
                int expected = 0;
                int headerId = 1;
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                List<FavoritesListDetail> results = repo.GetFavoritesListDetails(headerId);

                // assert
                results.Count(f => f.Active == false)
                       .Should()
                       .Be(expected);
            }
        }

        public class SaveFavoriteListDetail : MigratedDatabaseTest {
            private static FavoritesListDetail MakeDetail() {
                return new FavoritesListDetail {
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
            public void GoodDetail_DeletesCorrectDetail() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                long detailId = repo.SaveFavoriteListDetail(detail);
                repo.DeleteFavoriteListDetail(detailId);
                FavoritesListDetail results = repo.GetFavoriteDetail(detailId);

                // assert
                results.Active
                       .Should()
                       .BeFalse();
            }

            [Fact]
            public void GoodDetail_DoesNotUseSetCreatedUtc() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                DateTime expected = new DateTime(2017, 6, 30, 8, 47, 0, DateTimeKind.Utc);
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                long detailId = repo.SaveFavoriteListDetail(detail);
                FavoritesListDetail result = repo.GetFavoriteDetail(detailId);

                // assert
                result.CreatedUtc
                      .Should()
                      .NotBe(expected);
            }

            [Fact]
            public void GoodDetail_DoesNotUseSetModifiedUtc() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                DateTime expected = new DateTime(2017, 6, 30, 8, 48, 0, DateTimeKind.Utc);
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                long detailId = repo.SaveFavoriteListDetail(detail);
                FavoritesListDetail result = repo.GetFavoriteDetail(detailId);

                // assert
                result.ModifiedUtc
                      .Should()
                      .NotBe(expected);
            }

            [Fact]
            public void GoodDetail_SavesCorrectActive() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                bool expected = true;
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                long detailId = repo.SaveFavoriteListDetail(detail);
                FavoritesListDetail result = repo.GetFavoriteDetail(detailId);

                // assert
                result.Active
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesCorrectCatalogId() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                string expected = "FAT";
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                long detailId = repo.SaveFavoriteListDetail(detail);
                FavoritesListDetail result = repo.GetFavoriteDetail(detailId);

                // assert
                result.CatalogId
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesCorrectEach() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                bool expected = true;
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                long detailId = repo.SaveFavoriteListDetail(detail);
                FavoritesListDetail result = repo.GetFavoriteDetail(detailId);

                // assert
                result.Each
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesCorrectHeaderId() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                int expected = 1;
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                long detailId = repo.SaveFavoriteListDetail(detail);
                FavoritesListDetail result = repo.GetFavoriteDetail(detailId);

                // assert
                result.HeaderId
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesCorrectItemNumber() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                string expected = "123456";
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                long detailId = repo.SaveFavoriteListDetail(detail);
                FavoritesListDetail result = repo.GetFavoriteDetail(detailId);

                // assert
                result.ItemNumber
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesCorrectLabel() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                string expected = "Fake Label";
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                long detailId = repo.SaveFavoriteListDetail(detail);
                FavoritesListDetail result = repo.GetFavoriteDetail(detailId);

                // assert
                result.Label
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodDetail_SavesCorrectLineNumber() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                int expected = 17;
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                long detailId = repo.SaveFavoriteListDetail(detail);
                FavoritesListDetail result = repo.GetFavoriteDetail(detailId);

                // assert
                result.LineNumber
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void NullCatalogId_SavesExpectedValue() {
                // arrange
                FavoritesListDetail detail = new FavoritesListDetail {
                    Active = true,
                    CreatedUtc = new DateTime(2017, 6, 30, 8, 47, 0, DateTimeKind.Utc),
                    Each = true,
                    HeaderId = 1,
                    ItemNumber = "123456",
                    Label = "Fake Label",
                    LineNumber = 17,
                    ModifiedUtc = new DateTime(2017, 6, 30, 8, 48, 0, DateTimeKind.Utc)
                };
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                long detailId = repo.SaveFavoriteListDetail(detail);
                FavoritesListDetail results = repo.GetFavoriteDetail(detailId);

                // assert
                results.CatalogId
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void NullEach_SavesExpectedValue() {
                // arrange
                FavoritesListDetail detail = new FavoritesListDetail {
                    Active = true,
                    CatalogId = "FAT",
                    CreatedUtc = new DateTime(2017, 6, 30, 8, 47, 0, DateTimeKind.Utc),
                    HeaderId = 1,
                    ItemNumber = "123456",
                    Label = "Fake Label",
                    LineNumber = 17,
                    ModifiedUtc = new DateTime(2017, 6, 30, 8, 48, 0, DateTimeKind.Utc)
                };
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                long detailId = repo.SaveFavoriteListDetail(detail);
                FavoritesListDetail results = repo.GetFavoriteDetail(detailId);

                // assert
                results.Each
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void NullItemNumber_ShouldThrowSqlException() {
                // arrange
                FavoritesListDetail detail = new FavoritesListDetail {
                    Active = true,
                    CatalogId = "FAT",
                    CreatedUtc = new DateTime(2017, 6, 30, 8, 47, 0, DateTimeKind.Utc),
                    Each = true,
                    HeaderId = 1,
                    Label = "Fake Label",
                    LineNumber = 17,
                    ModifiedUtc = new DateTime(2017, 6, 30, 8, 48, 0, DateTimeKind.Utc)
                };
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                Action act = () => { repo.SaveFavoriteListDetail(detail); };

                // assert
                act.ShouldThrow<SqlException>();
            }

            [Fact]
            public void NullLabel_SavesExpectedValue() {
                // arrange
                FavoritesListDetail detail = new FavoritesListDetail {
                    Active = true,
                    CatalogId = "FAT",
                    CreatedUtc = new DateTime(2017, 6, 30, 8, 47, 0, DateTimeKind.Utc),
                    Each = true,
                    HeaderId = 1,
                    ItemNumber = "123456",
                    LineNumber = 17,
                    ModifiedUtc = new DateTime(2017, 6, 30, 8, 48, 0, DateTimeKind.Utc)
                };
                IFavoriteListDetailsRepository repo = MakeRepo();

                // act
                long detailId = repo.SaveFavoriteListDetail(detail);
                FavoritesListDetail results = repo.GetFavoriteDetail(detailId);

                // assert
                results.Label
                       .Should()
                       .BeNull();
            }
        }
    }
}