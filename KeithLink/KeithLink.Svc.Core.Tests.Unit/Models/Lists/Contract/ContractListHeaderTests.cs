using KeithLink.Svc.Core.Models.Lists.Contract;

using FluentAssertions;
using Xunit;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.Contract
{
    public class ContractListHeaderTests
    {
        private static ContractListHeader MakeHeader()
        {
            return new ContractListHeader()
            {
                ContractId = "Fake ContractId"
            };
        }

        public class Get_ContractId
        {
            [Fact]
            public void GoodHeader_ReturnsExpectedValue()
            {
                // arrange
                var fakeHeader = MakeHeader();
                var expected = "Fake ContractId";

                // act

                // assert
                fakeHeader.ContractId
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultValue()
            {
                // arrange
                var test = new ContractListHeader();

                // act

                // assert
                test.ContractId
                    .Should()
                    .BeNull();
            }
        }
    }
}