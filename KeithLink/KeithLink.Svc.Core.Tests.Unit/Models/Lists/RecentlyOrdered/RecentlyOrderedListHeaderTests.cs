using System;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Lists.RecentlyOrdered;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.RecentlyOrdered {
    public class RecentlyOrderedListHeaderTests {
        public static RecentlyOrderedListHeader MakeHeader() {
            return new RecentlyOrderedListHeader {
                UserId = Guid.Parse("12345678-1234-1234-1234-123456789012")
            };
        }

        public class Get_UserId {
            [Fact]
            public void GoodItem_ReturnsValue() {
                // arrange
                RecentlyOrderedListHeader test = MakeHeader();
                Guid expected = Guid.Parse("12345678-1234-1234-1234-123456789012");
                // act

                // assert
                test.UserId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void NewItem_UserIdHasDefaultValue() {
                // arrange
                RecentlyOrderedListHeader test = new RecentlyOrderedListHeader();
                Guid expected = Guid.Parse("00000000-0000-0000-0000-000000000000");

                // act

                // assert
                test.UserId
                    .Should()
                    .Be(expected);
            }
        }
    }
}