using KeithLink.Svc.Core.Models.Lists.Favorites;

using FluentAssertions;
using Xunit;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.Favorites
{
    public class FavoriteListDetailTests
    {
        private static FavoritesListDetail MakeItem()
        {
            return new FavoritesListDetail()
            {
                Active = true,
                Label = "Fake Label"
            };
        }

        public class Get_Active
        {
            [Fact]
            public void GoodItem_ReturnsDefaultValue()
            {
                // arrange
                var fakeItem = MakeItem();
                var expected = true;

                // act

                // assert
                fakeItem.Active
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue()
            {
                // arrange
                var test = new FavoritesListDetail();
                var expected = false;

                // act

                // assert
                test.Active
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_Label
        {
            [Fact]
            public void GoodItem_ReturnsDefaultValue()
            {
                // arrange
                var fakeItem = MakeItem();
                var expected = "Fake Label";

                // act

                // assert
                fakeItem.Label
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue()
            {
                // arrange
                var test = new FavoritesListDetail();

                // act

                // assert
                test.Label
                    .Should()
                    .BeNull();
            }
        }
    }
}