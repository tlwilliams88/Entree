using System;

using KeithLink.Svc.Core.Models;

using FluentAssertions;
using Xunit;

namespace KeithLink.Svc.Core.Tests.Unit.Models
{
    public class AuditableEntityTests
    {
        private class StubAuditableEntity : AuditableEntity { }

        private static StubAuditableEntity MakeEntity()
        {
            return new StubAuditableEntity()
            {
                CreatedUtc = new DateTime(2017, 6, 15, 14, 15, 0, DateTimeKind.Utc),
                ModifiedUtc = new DateTime(2017, 6, 15, 14, 16, 0, DateTimeKind.Utc)
            };
        }

        public class Get_CreatedUtc
        {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue()
            {
                // arrange
                var fakeEntity = MakeEntity();
                var expected = new DateTime(2017, 6, 15, 14, 15, 0, DateTimeKind.Utc);

                // act

                // assert
                fakeEntity.CreatedUtc
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedDateTimeKind()
            {
                // arrange
                var fakeEntity = MakeEntity();
                var expected = DateTimeKind.Utc;

                // act

                // assert
                fakeEntity.CreatedUtc
                          .Kind
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue()
            {
                // arrange
                var test = new StubAuditableEntity();
                var expected = DateTime.MinValue;

                // act

                // assert
                test.CreatedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void InitializedDetail_HeadDefaultDateTimeKind()
            {
                // arrange
                var test = new StubAuditableEntity();
                var expected = DateTimeKind.Unspecified;

                // act

                // assert
                test.CreatedUtc
                    .Kind
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void LocalTime_ReturnsExpectedValue()
            {
                // arrange
                var test = new StubAuditableEntity();
                test.CreatedUtc = new DateTime(2017, 7, 26, 10, 17, 0, DateTimeKind.Local);
                var expected = new DateTime(2017, 7, 26, 15, 17, 0, DateTimeKind.Utc);

                // act
                var results = test.CreatedUtc;

                // assert
                results.Should()
                       .Be(expected);
            }

            [Fact]
            public void LocalTime_ReturnsExpectedDateTimeKind()
            {
                // arrange
                var test = new StubAuditableEntity();
                test.CreatedUtc = new DateTime(2017, 7, 26, 10, 17, 0, DateTimeKind.Local);
                var expected = DateTimeKind.Utc;

                // act
                var results = test.CreatedUtc.Kind;

                // assert
                results.Should()
                       .Be(expected);
            }
        }

        public class Get_ModifiedUtc
        {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue()
            {
                // arrange
                var fakeEntity = MakeEntity();
                var expected = new DateTime(2017, 6, 15, 14, 16, 0, DateTimeKind.Utc);

                // act

                // assert
                fakeEntity.ModifiedUtc
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedDateTimeKind()
            {
                // arrange
                var fakeEntity = MakeEntity();
                var expected = DateTimeKind.Utc;

                // act

                // assert
                fakeEntity.ModifiedUtc
                          .Kind
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue()
            {
                // arrange
                var test = new StubAuditableEntity();
                var expected = DateTime.MinValue;

                // act

                // assert
                test.ModifiedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void InitializedDetail_HeadDefaultDateTimeKind()
            {
                // arrange
                var test = new StubAuditableEntity();
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