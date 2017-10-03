using KeithLink.Svc.Core.Models;

using FluentAssertions;
using Xunit;

namespace KeithLink.Svc.Core.Tests.Unit.Models
{
    public class EntityTests
    {
        private class StubEntity : Entity { }

        private static StubEntity MakeEntity()
        {
            return new StubEntity()
            {
                Id = 53
            };
        }

        public class Get_Id
        {
            [Fact]
            public void GoodEntity_ReturnsExpectedValue()
            {
                // arrange
                var fakeEntity = MakeEntity();
                var expected = 53;

                // act

                // assert
                fakeEntity.Id
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitializedEntity_HasDefaultValue()
            {
                // arrange
                var test = new StubEntity();
                var expected = 0;

                // act

                // assert
                test.Id
                    .Should()
                    .Be(expected);
            }
        }
    }
}