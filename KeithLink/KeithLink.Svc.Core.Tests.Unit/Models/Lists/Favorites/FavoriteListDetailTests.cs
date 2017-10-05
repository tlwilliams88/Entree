using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Lists.Favorites;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.Favorites {
    public class FavoriteListDetailTests {
        private static FavoritesListDetail MakeItem() {
            return new FavoritesListDetail {
                Active = true,
                Label = "Fake Label"
            };
        }

        public class Get_Active {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                FavoritesListDetail fakeItem = MakeItem();
                bool expected = true;

                // act

                // assert
                fakeItem.Active
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                FavoritesListDetail test = new FavoritesListDetail();
                bool expected = false;

                // act

                // assert
                test.Active
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_Label {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                FavoritesListDetail fakeItem = MakeItem();
                string expected = "Fake Label";

                // act

                // assert
                fakeItem.Label
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                FavoritesListDetail test = new FavoritesListDetail();

                // act

                // assert
                test.Label
                    .Should()
                    .BeNull();
            }
        }
    }
}