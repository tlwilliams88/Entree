using System;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Lists.Contract;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.Contract {
    public class ContractListDetailTests {
        private static ContractListDetail MakeContractListDetail() {
            return new ContractListDetail() {
                CatalogId = "Fake CatalogId",
                Category = "Fake Category",
                CreatedUtc = new DateTime(2017, 6, 13, 16, 38, 0, DateTimeKind.Utc),
                Each = true,
                FromDate = new DateTime(2017, 6, 13, 16, 50, 00),
                Id = 127,
                ItemNumber = "Fake ItemNumber",
                LineNumber = 1023,
                ModifiedUtc = new DateTime(2017, 6, 13, 16, 39, 00, DateTimeKind.Utc),
                ParentContractHeaderId = 19,
                ToDate = new DateTime(2017, 6, 13, 16, 52, 0)
            };
        }

        public class Get_CatalogId {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                var fakeItem = MakeContractListDetail();
                var expected = "Fake CatalogId";

                // act

                // assert
                fakeItem.CatalogId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue() {
                // arrange
                var test = new ContractListDetail();

                // act

                // assert
                test.CatalogId
                    .Should()
                    .BeNull();
            }
        }

        public class Get_Category {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                var fakeItem = MakeContractListDetail();
                var expected = "Fake Category";

                // act

                // assert
                fakeItem.Category
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue() {
                // arrange
                var test = new ContractListDetail();

                // act

                // assert
                test.Category
                    .Should()
                    .BeNull();
            }
        }

        public class Get_CreatedUtc {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                var fakeItem = MakeContractListDetail();
                var expected = new DateTime(2017, 6, 13, 16, 38, 0, DateTimeKind.Utc);

                // act

                // assert
                fakeItem.CreatedUtc
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedDateTimeKind() {
                // arrange
                var fakeItem = MakeContractListDetail();
                var expected = DateTimeKind.Utc;

                // act

                // assert
                fakeItem.CreatedUtc
                        .Kind
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue() {
                // arrange
                var test = new ContractListDetail();
                var expected = DateTime.MinValue;

                // act

                // assert
                test.CreatedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void InitializedDetail_HeadDefaultDateTimeKind() {
                // arrange
                var test = new ContractListDetail();
                var expected = DateTimeKind.Unspecified;

                // act

                // assert
                test.CreatedUtc
                    .Kind
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_Each {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                var fakeItem = MakeContractListDetail();
                var expected = true;

                // act

                // assert
                fakeItem.Each
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue() {
                // arrange
                var test = new ContractListDetail();

                // act

                // assert
                test.Each
                    .Should()
                    .BeNull();
            }
        }

        public class Get_FromDate {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                var fakeItem = MakeContractListDetail();
                var expected = new DateTime(2017, 6, 13, 16, 50, 00);

                // act

                // assert
                fakeItem.FromDate
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedDateTimeKind() {
                // arrange
                var fakeItem = MakeContractListDetail();
                var expected = DateTimeKind.Unspecified;

                // act

                // assert
                fakeItem.FromDate
                        .Value
                        .Kind
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue() {
                // arrange
                var test = new ContractListDetail();

                // act

                // assert
                test.FromDate
                    .Should()
                    .BeNull();
            }
        }

        public class Get_Id {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                var fakeItem = MakeContractListDetail();
                var expected = 127;

                // act

                // assert
                fakeItem.Id
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue() {
                // arrange
                var test = new ContractListDetail();
                var expected = 0;

                // act

                // assert
                test.Id
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_ItemNumber {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                var fakeItem = MakeContractListDetail();
                var expected = "Fake ItemNumber";

                // act

                // assert
                fakeItem.ItemNumber
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue() {
                // arrange
                var test = new ContractListDetail();

                // act

                // assert
                test.ItemNumber
                    .Should()
                    .BeNull();
            }
        }

        public class Get_LineNumber {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                var fakeItem = MakeContractListDetail();
                var expected = 1023;

                // act

                // assert
                fakeItem.LineNumber
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue() {
                // arrange
                var test = new ContractListDetail();
                var expected = 0;

                // act

                // assert
                test.LineNumber
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_ModifiedUtc {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                var fakeItem = MakeContractListDetail();
                var expected = new DateTime(2017, 6, 13, 16, 39, 00, DateTimeKind.Utc);

                // act

                // assert
                fakeItem.ModifiedUtc
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedDateTimeKind() {
                // arrange
                var fakeItem = MakeContractListDetail();
                var expected = DateTimeKind.Utc;

                // act

                // assert
                fakeItem.ModifiedUtc
                        .Kind
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue() {
                // arrange
                var test = new ContractListDetail();
                var expected = DateTime.MinValue;

                // act

                // assert
                test.ModifiedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void InitializedDetail_HeadDefaultDateTimeKind() {
                // arrange
                var test = new ContractListDetail();
                var expected = DateTimeKind.Unspecified;

                // act

                // assert
                test.ModifiedUtc
                    .Kind
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_ParentContractHeaderId {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                var fakeItem = MakeContractListDetail();
                var expected = 19;

                // act

                // assert
                fakeItem.ParentContractHeaderId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue() {
                // arrange
                var test = new ContractListDetail();
                var expected = 0;

                // act

                // assert
                test.ParentContractHeaderId
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_ToDate {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                var fakeItem = MakeContractListDetail();
                var expected = new DateTime(2017, 6, 13, 16, 52, 0);

                // act

                // assert
                fakeItem.ToDate
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedDateTimeKind() {
                // arrange
                var fakeItem = MakeContractListDetail();
                var expected = DateTimeKind.Unspecified;

                // act

                // assert
                fakeItem.ToDate
                        .Value
                        .Kind
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue() {
                // arrange
                var test = new ContractListDetail();

                // act

                // assert
                test.ToDate
                    .Should()
                    .BeNull();
            }
        }
    }
}
