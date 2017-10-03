using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.InventoryValuationList {
    public class InventoryValidationListHeaderTests {
        private static InventoryValuationListHeader MakeHeader() {
            return new InventoryValuationListHeader {
                Name = "Fake Name",
                Active = true
            };
        }

        public class Get_Name {
            [Fact]
            public void GoodHeader_returnsExpectedValue() {
                // arrange
                InventoryValuationListHeader fakeHeader = MakeHeader();
                string expected = "Fake Name";

                // act

                // assert
                fakeHeader.Name
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitalizedHeader_HasDefaultValue() {
                // arrange
                InventoryValuationListHeader test = new InventoryValuationListHeader();

                // act

                // assert
                test.Name
                    .Should()
                    .BeNull();
            }
        }

        public class Get_Active {
            [Fact]
            public void GoodHeader_returnsExpectedValue() {
                // arrange
                InventoryValuationListHeader fakeHeader = MakeHeader();
                bool expected = true;

                // act

                // assert
                fakeHeader.Active
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitalizedHeader_HasDefaultValue() {
                // arrange
                InventoryValuationListHeader test = new InventoryValuationListHeader();
                bool expected = false;

                // act

                // assert
                test.Active
                    .Should()
                    .Be(expected);
            }
        }
    }
}