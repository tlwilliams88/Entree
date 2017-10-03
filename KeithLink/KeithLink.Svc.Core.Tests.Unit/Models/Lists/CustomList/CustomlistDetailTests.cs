using KeithLink.Svc.Core.Models.Lists.CustomList;

using FluentAssertions;
using Xunit;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.CustomList
{
    public class CustomListDetailTests
    {
        private static CustomListDetail MakeItem()
        {
            return new CustomListDetail()
            {
                Active = true,
                CustomInventoryItemId = 1592,
                Label = "Fake Label",
                Par = 19.5m
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
                var test = new CustomListDetail();
                var expected = false;

                // act

                // assert
                test.Active
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_CustomInventoryItemId
        {
            [Fact]
            public void GoodItem_ReturnsDefaultValue()
            {
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
            public void IniatlizedItem_HasDefaultValue()
            {
                // arrange
                var test = new CustomListDetail();

                // act

                // assert
                test.CustomInventoryItemId
                    .Should()
                    .BeNull();
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
                var test = new CustomListDetail();

                // act

                // assert
                test.Label
                    .Should()
                    .BeNull();
            }
        }

        public class Get_Par
        {
            [Fact]
            public void GoodItem_ReturnsDefaultValue()
            {
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
            public void IniatlizedItem_HasDefaultValue()
            {
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
    }
}