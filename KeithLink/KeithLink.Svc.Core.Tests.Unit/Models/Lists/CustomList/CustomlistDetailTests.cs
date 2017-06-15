using System;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Lists.CustomList;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.CustomList {
    public class CustomListDetailTests {
        private static CustomListDetail MakeItem() {
            return new CustomListDetail() {
                Active = true,
                CatalogId = "Fake CatalogId",
                CreatedUtc = new DateTime(2017, 6, 14, 14, 43, 0, DateTimeKind.Utc),
                CustomInventoryItemId = 1592,
                Each = true,
                Id = 932,
                ItemNumber = "Fake ItemNumber",
                Label = "Fake Label",
                ModifiedUtc = new DateTime(2017, 6, 14, 14, 44, 00, DateTimeKind.Utc),
                Par = 19.5m,
                ParentCustomListHeaderId = 632
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
                var test = new CustomListDetail();
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
                var test = new CustomListDetail();

                // act

                // assert
                test.CatalogId
                    .Should()
                    .BeNull();
            }
        }

        public class Get_CustomInventoryItemId {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                var fakeItem = MakeItem();
                var expected = 1592;

                // act

                // assert
                fakeItem.CustomInventoryItemId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                var test = new CustomListDetail();

                // act

                // assert
                test.CustomInventoryItemId
                    .Should()
                    .BeNull();
            }
        }

        public class Get_CreatedUtc {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                var fakeItem = MakeItem();
                var expected = new DateTime(2017, 6, 14, 14, 43, 0, DateTimeKind.Utc);

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
                var test = new CustomListDetail();
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
                var test = new CustomListDetail();
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
                var test = new CustomListDetail();

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
                var expected = 932;

                // act

                // assert
                fakeItem.Id
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                var test = new CustomListDetail();
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
                var test = new CustomListDetail();

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
                var test = new CustomListDetail();

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
                var expected = new DateTime(2017, 6, 14, 14, 44, 00, DateTimeKind.Utc);

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
                var test = new CustomListDetail();
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
                var test = new CustomListDetail();
                var expected = DateTimeKind.Unspecified;

                // act

                // assert
                test.ModifiedUtc
                    .Kind
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_Par {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                var fakeItem = MakeItem();
                var expected = 19.5m;

                // act

                // assert
                fakeItem.Par
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                var test = new CustomListDetail();
                var expected = 0;

                // act

                // assert
                test.Par
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_ParentCustomListHeaderId {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                var fakeItem = MakeItem();
                var expected = 632;

                // act

                // assert
                fakeItem.ParentCustomListHeaderId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                var test = new CustomListDetail();
                var expected = 0;

                // act

                // assert
                test.ParentCustomListHeaderId
                    .Should()
                    .Be(expected);
            }
        }
    }
}
