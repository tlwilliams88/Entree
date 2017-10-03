using System;

using KeithLink.Svc.Core.Models.Lists.RecentlyViewed;

using FluentAssertions;
using Xunit;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.RecentlyViewed
{
    public class RecentlyViewedListHeaderTests
    {
        public static RecentlyViewedListHeader MakeHeader()
        {
            return new RecentlyViewedListHeader()
            {
                UserId = Guid.Parse("12345678-1234-1234-1234-123456789012")
            };
        }

        public class Get_UserId
        {
            [Fact]
            public void GoodItem_ReturnsValue()
            {
                // arrange
                RecentlyViewedListHeader test = MakeHeader();
                var expected = Guid.Parse("12345678-1234-1234-1234-123456789012");
                // act

                // assert
                test.UserId
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void NewItem_UserIdHasDefaultValue()
            {
                // arrange
                RecentlyViewedListHeader test = new RecentlyViewedListHeader();
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