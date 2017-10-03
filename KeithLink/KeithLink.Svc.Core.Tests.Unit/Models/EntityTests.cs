using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models;

namespace KeithLink.Svc.Core.Tests.Unit.Models {
    public class EntityTests {
        private static StubEntity MakeEntity() {
            return new StubEntity {
                Id = 53
            };
        }

        private class StubEntity : Entity {}

        public class Get_Id {
            [Fact]
            public void GoodEntity_ReturnsExpectedValue() {
                // arrange
                StubEntity fakeEntity = MakeEntity();
                int expected = 53;

                // act

                // assert
                fakeEntity.Id
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitializedEntity_HasDefaultValue() {
                // arrange
                StubEntity test = new StubEntity();
                int expected = 0;

                // act

                // assert
                test.Id
                    .Should()
                    .Be(expected);
            }
        }
    }
}