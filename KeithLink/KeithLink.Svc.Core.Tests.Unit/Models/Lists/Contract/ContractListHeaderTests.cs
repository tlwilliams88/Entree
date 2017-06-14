using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Lists.Contract;


namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.Contract {
    public class ContractListHeaderTests {
        private static ContractListHeader MakeHeader() {
            return new ContractListHeader() {
                BranchId = "Fake BranchId",
                ContractId = "Fake ContractId",
                CreatedUtc = new DateTime(2017, 6, 14, 11, 35, 00, DateTimeKind.Utc),
                CustomerNumber = "Fake CustomerNumber",
                Id = 17,
                ModifiedUtc = new DateTime(2017, 6, 14, 11, 36, 0, DateTimeKind.Utc)
            };
        }

        public class Get_BranchId {
            [Fact]
            public void GoodHeader_ReturnsExpectedValue() {
                // arrange
                var fakeHeader = MakeHeader();
                var expected = "Fake BranchId";

                // act

                // assert
                fakeHeader.BranchId
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultValue() {
                // arrange
                var test = new ContractListHeader();

                // act

                // assert
                test.BranchId
                    .Should()
                    .BeNull();
            }
        }

        public class Get_ContractId {
            [Fact]
            public void GoodHeader_ReturnsExpectedValue() {
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
            public void InitializedHeader_HasDefaultValue() {
                // arrange
                var test = new ContractListHeader();

                // act

                // assert
                test.ContractId
                    .Should()
                    .BeNull();
            }
        }

        public class Get_CreatedUtc {
            [Fact]
            public void GoodHeader_ReturnsExpectedValue() {
                // arrange
                var fakeHeader = MakeHeader();
                var expected = new DateTime(2017, 6, 14, 11, 35, 00, DateTimeKind.Utc);

                // act

                // assert
                fakeHeader.CreatedUtc
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedDateTimeKind() {
                // arrange
                var fakeHeader = MakeHeader();
                var expected = DateTimeKind.Utc;

                // act

                // assert
                fakeHeader.CreatedUtc
                          .Kind
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultValue() {
                // arrange
                var test = new ContractListHeader();
                var expected = DateTime.MinValue;

                // act

                // assert
                test.CreatedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultDateTimeKind() {
                // arrange
                var test = new ContractListHeader();
                var expected = DateTimeKind.Unspecified;

                // act

                // assert
                test.CreatedUtc
                    .Kind
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_CustomerNumber {
            [Fact]
            public void GoodHeader_ReturnsExpectedValue() {
                // arrange
                var fakeHeader = MakeHeader();
                var expected = "Fake CustomerNumber";

                // act

                // assert
                fakeHeader.CustomerNumber
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultValue() {
                // arrange
                var test = new ContractListHeader();

                // act

                // assert
                test.CustomerNumber
                    .Should()
                    .BeNull();
            }
        }

        public class Get_Id {
            [Fact]
            public void GoodHeader_ReturnsExpectedValue() {
                // arrange
                var fakeHeader = MakeHeader();
                var expected = 17;

                // act

                // assert
                fakeHeader.Id
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultValue() {
                // arrange
                var test = new ContractListHeader();
                var expected = 0;

                // act

                // assert
                test.Id
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_ModifiedUtc {
            [Fact]
            public void GoodHeader_ReturnsExpectedValue() {
                // arrange
                var fakeHeader = MakeHeader();
                var expected = new DateTime(2017, 6, 14, 11, 36, 0, DateTimeKind.Utc);

                // act

                // assert
                fakeHeader.ModifiedUtc
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedDateTimeKind() {
                // arrange
                var fakeHeader = MakeHeader();
                var expected = DateTimeKind.Utc;

                // act

                // assert
                fakeHeader.ModifiedUtc
                          .Kind
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultValue() {
                // arrange
                var test = new ContractListHeader();
                var expected = DateTime.MinValue;

                // act

                // assert
                test.ModifiedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void InitializedHeader_HasDefaultDateTimeKind() {
                // arrange
                var test = new ContractListHeader();
                var expected = DateTimeKind.Unspecified;

                // act

                // assert
                test.ModifiedUtc
                    .Kind
                    .Should()
                    .Be(expected);
            }
        }
    }
}
