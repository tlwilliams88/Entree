using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Lists.CustomListShares;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.CustomListShares {
    public class CustomListShareTests {
        private static CustomListShare MakeShare() {
            return new CustomListShare {
                Active = true,
                HeaderId = 899
            };
        }

        public class Get_Active {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                CustomListShare fakeItem = MakeShare();
                bool expected = true;

                // act

                // assert
                fakeItem.Active
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                CustomListShare test = new CustomListShare();
                bool expected = false;

                // act

                // assert
                test.Active
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_HeaderId {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                CustomListShare fakeItem = MakeShare();
                int expected = 899;

                // act

                // assert
                fakeItem.HeaderId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                CustomListShare test = new CustomListShare();
                int expected = 0;

                // act

                // assert
                test.HeaderId
                    .Should()
                    .Be(expected);
            }
        }
    }
}