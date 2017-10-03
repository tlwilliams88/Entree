using System;

using KeithLink.Svc.Core.Models.Lists.Favorites;

using FluentAssertions;
using Xunit;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.Favorites
{
    public class FavoritesListHeaderTests
    {
        private static FavoritesListHeader MakeHeader()
        {
            return new FavoritesListHeader()
            {
                UserId = new Guid("a08bb907-ab8e-4e56-9f22-b94b3d6a08e3")
            };
        }

        public class Get_UserId
        {
            [Fact]
            public void GoodHeader_ReturnsExpectedValue()
            {
                // arrange
                var fakeHeader = MakeHeader();
                var expected = new Guid("a08bb907-ab8e-4e56-9f22-b94b3d6a08e3");

                // act

                // assert
                fakeHeader.UserId
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultValue()
            {
                // arrange
                var test = new FavoritesListHeader();

                // act

                // assert
                test.UserId
                    .Should()
                    .BeNull();
            }
        }
    }
}