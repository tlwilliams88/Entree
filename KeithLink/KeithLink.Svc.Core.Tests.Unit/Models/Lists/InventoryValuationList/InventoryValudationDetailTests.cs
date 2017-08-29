using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.InventoryValuationList {
    public class InventoryValudationDetailTests {
        private static InventoryValuationListDetail MakeItem() {
            return new InventoryValuationListDetail() {
                Active = true,
                CustomInventoryItemId = 15,
                Quantity = 7.37m,
                Label = "Test Label"
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
                var test = new InventoryValuationListDetail();
                var expected = false;

                // act

                // assert
                test.Active
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_CustomInventoryItemId {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                var fakeItem = MakeItem();
                var expected = 15;

                // act

                // assert
                fakeItem.CustomInventoryItemId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                var test = new InventoryValuationListDetail();

                // act

                // assert
                test.CustomInventoryItemId
                    .Should()
                    .BeNull();
            }
        }

        public class Get_Quantity {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                var fakeItem = MakeItem();
                var expected = 7.37m;

                // act

                // assert
                fakeItem.Quantity
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                var test = new InventoryValuationListDetail();
                var expected = 0;

                // act

                // assert
                test.Quantity
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_Label {
            [Fact]
            public void GoodItem_ReturnsGoodValue() {
                // arrange
                var fakeItem = MakeItem();
                var expected = "Test Label";

                // act

                // assert
                fakeItem.Label
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitializedItem_HasDefaultValue() {
                // arrange
                var test = new InventoryValuationListDetail();

                // act

                // assert
                test.Label
                    .Should()
                    .BeNull();

            }
        }
    }
}
