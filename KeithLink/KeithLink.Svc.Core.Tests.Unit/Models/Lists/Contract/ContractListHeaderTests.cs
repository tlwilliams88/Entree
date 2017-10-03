using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Lists.Contract;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.Contract {
    public class ContractListHeaderTests {
        private static ContractListHeader MakeHeader() {
            return new ContractListHeader {
                ContractId = "Fake ContractId"
            };
        }

        public class Get_ContractId {
            [Fact]
            public void GoodHeader_ReturnsExpectedValue() {
                // arrange
                ContractListHeader fakeHeader = MakeHeader();
                string expected = "Fake ContractId";

                // act

                // assert
                fakeHeader.ContractId
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultValue() {
                // arrange
                ContractListHeader test = new ContractListHeader();

                // act

                // assert
                test.ContractId
                    .Should()
                    .BeNull();
            }
        }
    }
}