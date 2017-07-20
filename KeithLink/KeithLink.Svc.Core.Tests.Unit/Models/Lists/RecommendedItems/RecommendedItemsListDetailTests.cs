using FluentAssertions;

using KeithLink.Svc.Core.Models.Lists.RecommendedItems;

using Xunit;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.RecommendedItems {
    public class RecommendedItemsListDetailTests {
        public static RecommendedItemsListDetail MakeDetail() {
            return new RecommendedItemsListDetail {
                Note = "Scooby Dooby Doo"
            };
        }

        public class Get_Note {
            [Fact]
            public void GoodItem_HasDefaultValue() {
                // arrange
                RecommendedItemsListDetail detail = new RecommendedItemsListDetail();
                string expected = null;

                // act

                // assert
                detail.Note
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodItem_ReturnsValue() {
                // arrange
                RecommendedItemsListDetail detail = MakeDetail();
                string expected = "Scooby Dooby Doo";

                // act

                // assert
                detail.Note
                      .Should()
                      .Be(expected);
            }
        }
    }
}