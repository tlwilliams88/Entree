using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Models.Lists.Favorites;
using KeithLink.Svc.Core.Models.Lists;

using FluentAssertions;
using Xunit;

namespace KeithLink.Svc.Core.Tests.Unit.Extensions.Lists
{
    public class FavoritesHeaderExtensionsTests
    {
        private static FavoritesListHeader MakeHeader()
        {
            return new FavoritesListHeader()
            {
                Id = 21,
                BranchId = "BYE",
                CustomerNumber = "SEWRLD",
                CreatedUtc = new DateTime(2017, 7, 5, 16, 41, 0, DateTimeKind.Utc),
                ModifiedUtc = new DateTime(2017, 7, 5, 16, 42, 0, DateTimeKind.Utc)
            };
        }

        public class ToListModel
        {

            [Fact]
            public void GoodHeader_ReturnsExpectedListId()
            {
                // arrange
                var header = MakeHeader();
                var expected = 21;

                // act
                var results = header.ToListModel();

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedType()
            {
                // arrange
                var header = MakeHeader();
                var expected = ListType.Favorite;

                // act
                var results = header.ToListModel();

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedCustomerNumber()
            {
                // arrange
                var header = MakeHeader();
                var expected = "SEWRLD";

                // act
                var results = header.ToListModel();

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedPosition()
            {
                // arrange
                var header = MakeHeader();
                var expected = "BYE";

                // act
                var results = header.ToListModel();

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

        }

    }
}
