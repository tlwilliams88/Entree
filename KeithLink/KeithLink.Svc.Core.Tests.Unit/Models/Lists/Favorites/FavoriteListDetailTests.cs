using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Lists.Favorites;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.Favorites {
    public class FavoriteListDetailTests {
        private static FavoritesListDetail MakeItem() {
            return new FavoritesListDetail() {
                Active = true,
                CatalogId = "Fake CatalogId",
                CreatedUtc = new DateTime(2017, 6, 15, 10, 43, 0, DateTimeKind.Utc),
                Each = true,
                Id = 75,
                ItemNumber = "Fake ItemNumber",
                Label = "Fake Label",
                ModifiedUtc = new DateTime(2017, 6, 15, 10, 44, 0, DateTimeKind.Utc),
                HeaderId = 6
            };
        }

        public class Get_Active {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
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
            public void IniatlizedItem_HasDefaultValue() {
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

        public class Get_CatalogId {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                var fakeItem = MakeItem();
                var expected = "Fake CatalogId";

                // act

                // assert
                fakeItem.CatalogId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                var test = new FavoritesListDetail();

                // act

                // assert
                test.CatalogId
                    .Should()
                    .BeNull();
            }
        }

        public class Get_CreatedUtc {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                var fakeItem = MakeItem();
                var expected = new DateTime(2017, 6, 15, 10, 43, 0, DateTimeKind.Utc);

                // act

                // assert
                fakeItem.CreatedUtc
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedDateTimeKind() {
                // arrange
                var fakeItem = MakeItem();
                var expected = DateTimeKind.Utc;

                // act

                // assert
                fakeItem.CreatedUtc
                        .Kind
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue() {
                // arrange
                var test = new FavoritesListDetail();
                var expected = DateTime.MinValue;

                // act

                // assert
                test.CreatedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void InitializedDetail_HeadDefaultDateTimeKind() {
                // arrange
                var test = new FavoritesListDetail();
                var expected = DateTimeKind.Unspecified;

                // act

                // assert
                test.CreatedUtc
                    .Kind
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_Each {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                var fakeItem = MakeItem();
                var expected = true;

                // act

                // assert
                fakeItem.Each
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                var test = new FavoritesListDetail();

                // act

                // assert
                test.Each
                    .Should()
                    .BeNull();
            }
        }

        public class Get_Id {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                var fakeItem = MakeItem();
                var expected = 75;

                // act

                // assert
                fakeItem.Id
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                var test = new FavoritesListDetail();
                var expected = 0;

                // act

                // assert
                test.Id
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_ItemNumber {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                var fakeItem = MakeItem();
                var expected = "Fake ItemNumber";

                // act

                // assert
                fakeItem.ItemNumber
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                var test = new FavoritesListDetail();

                // act

                // assert
                test.ItemNumber
                    .Should()
                    .BeNull();
            }
        }

        public class Get_Label {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
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
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                var test = new FavoritesListDetail();

                // act

                // assert
                test.Label
                    .Should()
                    .BeNull();
            }
        }

        public class Get_ModifiedUtc {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                var fakeItem = MakeItem();
                var expected = new DateTime(2017, 6, 15, 10, 44, 0, DateTimeKind.Utc);

                // act

                // assert
                fakeItem.ModifiedUtc
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedDateTimeKind() {
                // arrange
                var fakeItem = MakeItem();
                var expected = DateTimeKind.Utc;

                // act

                // assert
                fakeItem.ModifiedUtc
                        .Kind
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue() {
                // arrange
                var test = new FavoritesListDetail();
                var expected = DateTime.MinValue;

                // act

                // assert
                test.ModifiedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void InitializedDetail_HeadDefaultDateTimeKind() {
                // arrange
                var test = new FavoritesListDetail();
                var expected = DateTimeKind.Unspecified;

                // act

                // assert
                test.ModifiedUtc
                    .Kind
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_HeaderId {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                var fakeItem = MakeItem();
                var expected = 6;

                // act

                // assert
                fakeItem.HeaderId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                var test = new FavoritesListDetail();
                var expected = 0;

                // act

                // assert
                test.HeaderId
                    .Should()
                    .Be(expected);
            }
        }
    }
}
