using System;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Favorites;

namespace KeithLink.Svc.Core.Tests.Unit.Extensions.Lists {
    public class FavoritesDetailExtensionTests {
        private static FavoritesListDetail MakeDetail() {
            return new FavoritesListDetail {
                Id = 21,
                CatalogId = "FUT",
                Each = true,
                HeaderId = 10,
                ItemNumber = "123456",
                LineNumber = 17,
                Active = true,
                CreatedUtc = new DateTime(2017, 7, 5, 16, 41, 0, DateTimeKind.Utc),
                ModifiedUtc = new DateTime(2017, 7, 5, 16, 42, 0, DateTimeKind.Utc)
            };
        }

        private static ListItemModel MakeListItem() {
            return new ListItemModel {
                ListItemId = 21,
                CatalogId = "FUT",
                Each = true,
                ItemNumber = "123456",
                Position = 17,
                CreatedUtc = new DateTime(2017, 7, 5, 16, 41, 0, DateTimeKind.Utc),
                ModifiedUtc = new DateTime(2017, 7, 5, 16, 42, 0, DateTimeKind.Utc)
            };
        }

        public class ToWebModel {
            [Fact]
            public void GoodDetail_ReturnsExpectedActive() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                bool expected = true;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Active
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCatalogId() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                string expected = "FUT";

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.CatalogId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCreatedUtc() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                DateTime expected = new DateTime(2017, 7, 5, 16, 41, 0, DateTimeKind.Utc);

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.CreatedUtc
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedEach() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                bool expected = true;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Each
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedIsDelete() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                bool expected = false;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.IsDelete
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedItemNumber() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                string expected = "123456";

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.ItemNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedListItemId() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                int expected = 21;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.ListItemId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedModifiedUtc() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                DateTime expected = new DateTime(2017, 7, 5, 16, 42, 0, DateTimeKind.Utc);

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.ModifiedUtc
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedPosition() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                int expected = 17;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Position
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedType() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                ListType expected = ListType.Favorite;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsIsDeleteAsFalse() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                bool expected = false;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.IsDelete
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsNullCategory() {
                // arrange
                FavoritesListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Category
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullDelta() {
                // arrange
                FavoritesListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Delta
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullFromDate() {
                // arrange
                FavoritesListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.FromDate
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullItemStatistics() {
                // arrange
                FavoritesListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.ItemStatistics
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullLabel() {
                // arrange
                FavoritesListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Label
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullPackSize() {
                // arrange
                FavoritesListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.PackSize
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullProprietaryCustomers() {
                // arrange
                FavoritesListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.ProprietaryCustomers
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullStorageTemp() {
                // arrange
                FavoritesListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.StorageTemp
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullSupplier() {
                // arrange
                FavoritesListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Supplier
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsQuantityOfZero() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                int expected = 0;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Quantity
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsZeroCustomInventoryId() {
                // arrange
                FavoritesListDetail detail = MakeDetail();
                int expected = 0;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.CustomInventoryItemId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void NullEach_ReturnsFalse() {
                // arrange
                FavoritesListDetail detail = new FavoritesListDetail {
                    Id = 21,
                    CatalogId = "FUT",
                    HeaderId = 10,
                    ItemNumber = "123456",
                    LineNumber = 17,
                    CreatedUtc = new DateTime(2017, 7, 5, 16, 41, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 7, 5, 16, 42, 0, DateTimeKind.Utc)
                };
                bool expected = false;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Each
                       .Should()
                       .Be(expected);
            }
        }

        public class ToFavoritesListDetail {
            [Fact]
            public void GoodListItem_ReturnsExpectedCatalogId() {
                // arrange
                ListItemModel listitem = MakeListItem();
                string expected = "FUT";

                // act
                FavoritesListDetail results = listitem.ToFavoritesListDetail();

                // assert
                results.CatalogId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodListItem_ReturnsExpectedEach() {
                // arrange
                ListItemModel listitem = MakeListItem();
                bool expected = true;

                // act
                FavoritesListDetail results = listitem.ToFavoritesListDetail();

                // assert
                results.Each
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodListItem_ReturnsExpectedId() {
                // arrange
                ListItemModel listitem = MakeListItem();
                int expected = 21;

                // act
                FavoritesListDetail results = listitem.ToFavoritesListDetail();

                // assert
                results.Id
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodListItem_ReturnsExpectedItemNumber() {
                // arrange
                ListItemModel listitem = MakeListItem();
                string expected = "123456";

                // act
                FavoritesListDetail results = listitem.ToFavoritesListDetail();

                // assert
                results.ItemNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodListItem_ReturnsExpectedLineNumber() {
                // arrange
                ListItemModel listitem = MakeListItem();
                int expected = 17;

                // act
                FavoritesListDetail results = listitem.ToFavoritesListDetail();

                // assert
                results.LineNumber
                       .Should()
                       .Be(expected);
            }
        }
    }
}