using System;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Lists.Contract;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.Contract {
    public class ContractListDetailTests {
        private static ContractListDetail MakeContractListDetail() {
            return new ContractListDetail {
                Category = "Fake Category",
                FromDate = new DateTime(2017, 6, 13, 16, 50, 00),
                LineNumber = 1023,
                ToDate = new DateTime(2017, 6, 13, 16, 52, 0)
            };
        }

        public class Get_Category {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                ContractListDetail fakeItem = MakeContractListDetail();
                string expected = "Fake Category";

                // act

                // assert
                fakeItem.Category
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue() {
                // arrange
                ContractListDetail test = new ContractListDetail();

                // act

                // assert
                test.Category
                    .Should()
                    .BeNull();
            }
        }

        public class Get_FromDate {
            [Fact]
            public void GoodDetail_ReturnsExpectedDateTimeKind() {
                // arrange
                ContractListDetail fakeItem = MakeContractListDetail();
                DateTimeKind expected = DateTimeKind.Unspecified;

                // act

                // assert
                fakeItem.FromDate
                        .Value
                        .Kind
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                ContractListDetail fakeItem = MakeContractListDetail();
                DateTime expected = new DateTime(2017, 6, 13, 16, 50, 00);

                // act

                // assert
                fakeItem.FromDate
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue() {
                // arrange
                ContractListDetail test = new ContractListDetail();

                // act

                // assert
                test.FromDate
                    .Should()
                    .BeNull();
            }
        }

        public class Get_LineNumber {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                ContractListDetail fakeItem = MakeContractListDetail();
                int expected = 1023;

                // act

                // assert
                fakeItem.LineNumber
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue() {
                // arrange
                ContractListDetail test = new ContractListDetail();
                int expected = 0;

                // act

                // assert
                test.LineNumber
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_ToDate {
            [Fact]
            public void GoodDetail_ReturnsExpectedDateTimeKind() {
                // arrange
                ContractListDetail fakeItem = MakeContractListDetail();
                DateTimeKind expected = DateTimeKind.Unspecified;

                // act

                // assert
                fakeItem.ToDate
                        .Value
                        .Kind
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                ContractListDetail fakeItem = MakeContractListDetail();
                DateTime expected = new DateTime(2017, 6, 13, 16, 52, 0);

                // act

                // assert
                fakeItem.ToDate
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue() {
                // arrange
                ContractListDetail test = new ContractListDetail();

                // act

                // assert
                test.ToDate
                    .Should()
                    .BeNull();
            }
        }
    }
}