using System;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Lists.CustomList;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.CustomList {
    public class CustomListHeaderTests {
        private static CustomListHeader MakeHeader() {
            return new CustomListHeader() {
                Active = true,
                BranchId = "Fake BranchId",
                CreatedUtc = new DateTime(2017, 6, 14, 16, 34, 0, DateTimeKind.Utc),
                CustomerNumber = "Fake CustomerNumber",
                Id = 1500,
                ModifiedUtc = new DateTime(2017, 6,14,16,35,0, DateTimeKind.Utc),
                Name = "Fake Name",
                UserId = new Guid("a08bb907-ab8e-4e56-9f22-b94b3d6a08e3")
            };
        }

        public class Get_Active {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                var fakeItem = MakeHeader();
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
                var test = new CustomListHeader();
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
                var fakeItem = MakeHeader();
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
                var test = new CustomListHeader();

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
                var fakeItem = MakeHeader();
                var expected = new DateTime(2017, 6, 14, 16, 34, 0, DateTimeKind.Utc);

                // act

                // assert
                fakeItem.CreatedUtc
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedDateTimeKind() {
                // arrange
                var fakeItem = MakeHeader();
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
                var test = new CustomListHeader();
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
                var test = new CustomListHeader();
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
                var fakeItem = MakeHeader();
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
                var test = new CustomListHeader();

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
                var fakeItem = MakeHeader();
                var expected = 1500;

                // act

                // assert
                fakeItem.Id
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                var test = new CustomListHeader();
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
                var fakeItem = MakeHeader();
                var expected = new DateTime(2017, 6, 14, 16, 35, 0, DateTimeKind.Utc);

                // act

                // assert
                fakeItem.ModifiedUtc
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedDateTimeKind() {
                // arrange
                var fakeItem = MakeHeader();
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
                var test = new CustomListHeader();
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
                var test = new CustomListHeader();
                var expected = DateTimeKind.Unspecified;

                // act

                // assert
                test.ModifiedUtc
                    .Kind
                    .Should()
                    .Be(expected);
            }
        }


        public class Get_Name {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                var fakeItem = MakeHeader();
                var expected = "Fake Name";

                // act

                // assert
                fakeItem.Name
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                var test = new CustomListHeader();

                // act

                // assert
                test.Name
                    .Should()
                    .BeNull();
            }
        }

        public class Get_UserId {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                var fakeItem = MakeHeader();
                var expected = new Guid("a08bb907-ab8e-4e56-9f22-b94b3d6a08e3");

                // act

                // assert
                fakeItem.UserId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                var test = new CustomListHeader();

                // act

                // assert
                test.UserId
                    .Should()
                    .BeNull();
            }
        }
    }
}
