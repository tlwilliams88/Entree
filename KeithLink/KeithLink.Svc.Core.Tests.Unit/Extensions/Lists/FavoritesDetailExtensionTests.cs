using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Models.Lists.Favorites;

using FluentAssertions;

using KeithLink.Svc.Core.Models.Lists;

using Xunit;


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
