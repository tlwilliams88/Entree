using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Lists;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists {
    public class BaseListHeaderTests {
        private static StubHeader MakeHeader() {
            return new StubHeader {
                BranchId = "Fake BranchId",
                CustomerNumber = "Fake CustomerNumber"
            };
        }

        private class StubHeader : BaseListHeader {}

        public class Get_BranchId {
            [Fact]
            public void GoodHeader_ReturnsExpectedValue() {
                // arrange
                StubHeader fakeHeader = MakeHeader();
                string expected = "Fake BranchId";

                // act

                // assert
                fakeHeader.BranchId
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultValue() {
                // arrange
                StubHeader test = new StubHeader();

                // act

                // assert
                test.BranchId
                    .Should()
                    .BeNull();
            }
        }

        public class Get_CustomerNumber {
            [Fact]
            public void GoodHeader_ReturnsExpectedValue() {
                // arrange
                StubHeader fakeHeader = MakeHeader();
                string expected = "Fake CustomerNumber";

                // act

                // assert
                fakeHeader.CustomerNumber
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultValue() {
                // arrange
                StubHeader test = new StubHeader();

                // act

                // assert
                test.CustomerNumber
                    .Should()
                    .BeNull();
            }
        }
    }
}