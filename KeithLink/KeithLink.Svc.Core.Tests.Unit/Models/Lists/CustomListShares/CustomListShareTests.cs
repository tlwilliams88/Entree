using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Lists.CustomListShares;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.CustomListShares {
    public class CustomListShareTests {
        private  static CustomListShare MakeShare() {
            return new CustomListShare() {
                Active = true,
                BranchId = "Fake BranchId",
                CreatedUtc = new DateTime(2017, 6, 15, 8, 54, 0, DateTimeKind.Utc),
                CustomerNumber = "Fake CustomerNumber",
                Id = 12,
                ModifiedUtc = new DateTime(2017, 6, 15, 8, 55, 0, DateTimeKind.Utc),
                ParentCustomListHeaderId = 899
            };
        }

        public class Get_Active {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                var fakeItem = MakeShare();
                var expected = true;

                // act

                // assert
                fakeItem.Active
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                var test = new CustomListShare();
                var expected = false;

                // act

                // assert
                test.Active
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_BranchId {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                var fakeItem = MakeShare();
                var expected = "Fake BranchId";

                // act

                // assert
                fakeItem.BranchId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                var test = new CustomListShare();

                // act

                // assert
                test.BranchId
                    .Should()
                    .BeNull();
            }
        }

        public class Get_CreatedUtc {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                var fakeItem = MakeShare();
                var expected = new DateTime(2017, 6, 15, 8, 54, 0, DateTimeKind.Utc);

                // act

                // assert
                fakeItem.CreatedUtc
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedDateTimeKind() {
                // arrange
                var fakeItem = MakeShare();
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
                var test = new CustomListShare();
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
                var test = new CustomListShare();
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
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                var fakeItem = MakeShare();
                var expected = "Fake CustomerNumber";

                // act

                // assert
                fakeItem.CustomerNumber
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                var test = new CustomListShare();

                // act

                // assert
                test.CustomerNumber
                    .Should()
                    .BeNull();
            }
        }

        public class Get_Id {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                var fakeItem = MakeShare();
                var expected = 12;

                // act

                // assert
                fakeItem.Id
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                var test = new CustomListShare();
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
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                var fakeItem = MakeShare();
                var expected = new DateTime(2017, 6, 15, 8, 55, 0, DateTimeKind.Utc);

                // act

                // assert
                fakeItem.ModifiedUtc
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedDateTimeKind() {
                // arrange
                var fakeItem = MakeShare();
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
                var test = new CustomListShare();
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
                var test = new CustomListShare();
                var expected = DateTimeKind.Unspecified;

                // act

                // assert
                test.ModifiedUtc
                    .Kind
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_ParentCustomListHeaderId {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                var fakeItem = MakeShare();
                var expected = 899;

                // act

                // assert
                fakeItem.ParentCustomListHeaderId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                var test = new CustomListShare();
                var expected = 0;

                // act

                // assert
                test.ParentCustomListHeaderId
                    .Should()
                    .Be(expected);
            }
        }
    }
}
