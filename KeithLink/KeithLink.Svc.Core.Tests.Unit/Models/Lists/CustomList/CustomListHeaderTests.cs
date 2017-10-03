using System;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Lists.CustomList;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.CustomList {
    public class CustomListHeaderTests {
        private static CustomListHeader MakeHeader() {
            return new CustomListHeader {
                Active = true,
                Name = "Fake Name",
                UserId = new Guid("a08bb907-ab8e-4e56-9f22-b94b3d6a08e3")
            };
        }

        public class Get_Active {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                CustomListHeader fakeItem = MakeHeader();
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
                CustomListHeader test = new CustomListHeader();
                bool expected = false;

                // act

                // assert
                test.Active
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_Name {
            [Fact]
            public void GoodItem_ReturnsDefaultValue() {
                // arrange
                CustomListHeader fakeItem = MakeHeader();
                string expected = "Fake Name";

                // act

                // assert
                fakeItem.Name
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                CustomListHeader test = new CustomListHeader();

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
                CustomListHeader fakeItem = MakeHeader();
                Guid expected = new Guid("a08bb907-ab8e-4e56-9f22-b94b3d6a08e3");

                // act

                // assert
                fakeItem.UserId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void IniatlizedItem_HasDefaultValue() {
                // arrange
                CustomListHeader test = new CustomListHeader();

                // act

                // assert
                test.UserId
                    .Should()
                    .BeNull();
            }
        }
    }
}