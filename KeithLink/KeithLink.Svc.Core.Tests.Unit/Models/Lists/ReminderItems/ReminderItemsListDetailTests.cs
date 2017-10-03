using KeithLink.Svc.Core.Models.Lists.ReminderItems;

using FluentAssertions;
using Xunit;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.ReminderItems
{
    public class ReminderItemsListDetailTests
    {
        private static ReminderItemsListDetail MakeItem()
        {
            return new ReminderItemsListDetail()
            {
                Active = true
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
                var test = new ReminderItemsListDetail();
                var expected = false;

                // act

                // assert
                test.Active
                    .Should()
                    .Be(expected);
            }
        }
    }
}