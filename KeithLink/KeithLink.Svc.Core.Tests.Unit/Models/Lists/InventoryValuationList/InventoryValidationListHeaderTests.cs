using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.InventoryValuationList {
    public class InventoryValidationListHeaderTests {
        private static InventoryValuationListHeader MakeHeader() {
            return new InventoryValuationListHeader() {
                Name = "Fake Name"
            };
        }

        public class Get_Name {
            [Fact]
            public void GoodHeader_returnsExpectedValue() {
                // arrange
                var fakeHeader = MakeHeader();
                var expected = "Fake Name";

                // act

                // assert
                fakeHeader.Name
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitalizedHeader_HasDefaultValue() {
                // arrange
                var test = new InventoryValuationListHeader();

                // act

                // assert
                test.Name
                    .Should()
                    .BeNull();
            }
        }
    }
}
