using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Lists.MandatoryItem;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.MandatoryItems {
    public class MandatoryItemsListDetailTests {
        private static MandatoryItemsListDetail MakeDetail() {
            return new MandatoryItemsListDetail {
                Active = true
            };
        }

        public class Get_Active {
            [Fact]
            public void GoodItem_HasDefaultValue() {
                // arrange
                MandatoryItemsListDetail test = new MandatoryItemsListDetail();
                bool expected = false;

                // act

                // assert
                test.Active
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodItem_ReturnsValue() {
                // arrange
                MandatoryItemsListDetail test = MakeDetail();
                bool expected = true;
                // act

                // assert
                test.Active
                    .Should()
                    .Be(expected);
            }
        }
    }
}