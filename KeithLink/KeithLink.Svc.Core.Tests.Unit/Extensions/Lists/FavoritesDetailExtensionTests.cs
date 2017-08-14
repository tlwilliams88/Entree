using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;
using Moq;
using Xunit;

using KeithLink.Common.Core.Seams;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Favorites;


namespace KeithLink.Svc.Core.Tests.Unit.Extensions.Lists
{
    public class FavoritesDetailExtensionTests
    {
        private static FavoritesListDetail MakeDetail()
        {
            return new FavoritesListDetail()
            {
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

        private static ListItemModel MakeListItem()
        {
            return new ListItemModel()
            {
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
            public void GoodDetail_ReturnsExpectedListItemId()
            {
                // arrange
                var detail = MakeDetail();
                var expected = 21;

                // act
                var results = detail.ToWebModel();

                // assert
                results.ListItemId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedType()
            {
                // arrange
                var detail = MakeDetail();
                var expected = ListType.Favorite;

                // act
                var results = detail.ToWebModel();

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedItemNumber()
            {
                // arrange
                var detail = MakeDetail();
                var expected = "123456";

                // act
                var results = detail.ToWebModel();

                // assert
                results.ItemNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedPosition()
            {
                // arrange
                var detail = MakeDetail();
                var expected = 17;

                // act
                var results = detail.ToWebModel();

                // assert
                results.Position
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedEach()
            {
                // arrange
                var detail = MakeDetail();
                var expected = true;

                // act
                var results = detail.ToWebModel();

                // assert
                results.Each
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCatalogId()
            {
                // arrange
                var detail = MakeDetail();
                var expected = "FUT";

                // act
                var results = detail.ToWebModel();

                // assert
                results.CatalogId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCreatedUtc()
            {
                // arrange
                var detail = MakeDetail();
                var expected = new DateTime(2017, 7, 5, 16, 41, 0, DateTimeKind.Utc);

                // act
                var results = detail.ToWebModel();

                // assert
                results.CreatedUtc
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedModifiedUtc()
            {
                // arrange
                var detail = MakeDetail();
                var expected = new DateTime(2017, 7, 5, 16, 42, 0, DateTimeKind.Utc);

                // act
                var results = detail.ToWebModel();

                // assert
                results.ModifiedUtc
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsZeroCustomInventoryId()
            {
                // arrange
                var detail = MakeDetail();
                var expected = 0;

                // act
                var results = detail.ToWebModel();

                // assert
                results.CustomInventoryItemId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsNullDelta()
            {
                // arrange
                var detail = MakeDetail();

                // act
                var results = detail.ToWebModel();

                // assert
                results.Delta
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedActive()
            {
                // arrange
                var detail = MakeDetail();
                var expected = true;

                // act
                var results = detail.ToWebModel();

                // assert
                results.Active
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsNullCategory()
            {
                // arrange
                var detail = MakeDetail();

                // act
                var results = detail.ToWebModel();

                // assert
                results.Category
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedIsDelete()
            {
                // arrange
                var detail = MakeDetail();
                var expected = false;

                // act
                var results = detail.ToWebModel();

                // assert
                results.IsDelete
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsNullFromDate()
            {
                // arrange
                var detail = MakeDetail();

                // act
                var results = detail.ToWebModel();

                // assert
                results.FromDate
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullItemStatistics()
            {
                // arrange
                var detail = MakeDetail();

                // act
                var results = detail.ToWebModel();

                // assert
                results.ItemStatistics
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullLabel()
            {
                // arrange
                var detail = MakeDetail();

                // act
                var results = detail.ToWebModel();

                // assert
                results.Label
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void NullEach_ReturnsFalse()
            {
                // arrange
                var detail = new FavoritesListDetail()
                {
                    Id = 21,
                    CatalogId = "FUT",
                    HeaderId = 10,
                    ItemNumber = "123456",
                    LineNumber = 17,
                    CreatedUtc = new DateTime(2017, 7, 5, 16, 41, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 7, 5, 16, 42, 0, DateTimeKind.Utc)
                };
                var expected = false;

                // act
                var results = detail.ToWebModel();

                // assert
                results.Each
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsNullPackSize()
            {
                // arrange
                var detail = MakeDetail();

                // act
                var results = detail.ToWebModel();

                // assert
                results.PackSize
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullStorageTemp()
            {
                // arrange
                var detail = MakeDetail();

                // act
                var results = detail.ToWebModel();

                // assert
                results.StorageTemp
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsQuantityOfZero()
            {
                // arrange
                var detail = MakeDetail();
                var expected = 0;

                // act
                var results = detail.ToWebModel();

                // assert
                results.Quantity
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsIsDeleteAsFalse()
            {
                // arrange
                var detail = MakeDetail();
                var expected = false;

                // act
                var results = detail.ToWebModel();

                // assert
                results.IsDelete
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsNullProprietaryCustomers()
            {
                // arrange
                var detail = MakeDetail();

                // act
                var results = detail.ToWebModel();

                // assert
                results.ProprietaryCustomers
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullSupplier()
            {
                // arrange
                var detail = MakeDetail();

                // act
                var results = detail.ToWebModel();

                // assert
                results.Supplier
                       .Should()
                       .BeNull();
            }

        }

        public class ToFavoritesListDetail
        {

            [Fact]
            public void GoodListItem_ReturnsExpectedId()
            {
                // arrange
                var listitem = MakeListItem();
                var expected = 21;

                // act
                var results = listitem.ToFavoritesListDetail();

                // assert
                results.Id
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodListItem_ReturnsExpectedItemNumber()
            {
                // arrange
                var listitem = MakeListItem();
                var expected = "123456";

                // act
                var results = listitem.ToFavoritesListDetail();

                // assert
                results.ItemNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodListItem_ReturnsExpectedLineNumber()
            {
                // arrange
                var listitem = MakeListItem();
                var expected = 17;

                // act
                var results = listitem.ToFavoritesListDetail();

                // assert
                results.LineNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodListItem_ReturnsExpectedEach()
            {
                // arrange
                var listitem = MakeListItem();
                var expected = true;

                // act
                var results = listitem.ToFavoritesListDetail();

                // assert
                results.Each
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodListItem_ReturnsExpectedCatalogId()
            {
                // arrange
                var listitem = MakeListItem();
                var expected = "FUT";

                // act
                var results = listitem.ToFavoritesListDetail();

                // assert
                results.CatalogId
                       .Should()
                       .Be(expected);
            }

        }
    }
}
