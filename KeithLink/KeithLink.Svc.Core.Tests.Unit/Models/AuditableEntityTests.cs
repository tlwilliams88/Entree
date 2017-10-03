using System;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models;

namespace KeithLink.Svc.Core.Tests.Unit.Models {
    public class AuditableEntityTests {
        private static StubAuditableEntity MakeEntity() {
            return new StubAuditableEntity {
                CreatedUtc = new DateTime(2017, 6, 15, 14, 15, 0, DateTimeKind.Utc),
                ModifiedUtc = new DateTime(2017, 6, 15, 14, 16, 0, DateTimeKind.Utc)
            };
        }

        private class StubAuditableEntity : AuditableEntity {}

        public class Get_CreatedUtc {
            [Fact]
            public void GoodDetail_ReturnsExpectedDateTimeKind() {
                // arrange
                StubAuditableEntity fakeEntity = MakeEntity();
                DateTimeKind expected = DateTimeKind.Utc;

                // act

                // assert
                fakeEntity.CreatedUtc
                          .Kind
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                StubAuditableEntity fakeEntity = MakeEntity();
                DateTime expected = new DateTime(2017, 6, 15, 14, 15, 0, DateTimeKind.Utc);

                // act

                // assert
                fakeEntity.CreatedUtc
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue() {
                // arrange
                StubAuditableEntity test = new StubAuditableEntity();
                DateTime expected = DateTime.MinValue;

                // act

                // assert
                test.CreatedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void InitializedDetail_HeadDefaultDateTimeKind() {
                // arrange
                StubAuditableEntity test = new StubAuditableEntity();
                DateTimeKind expected = DateTimeKind.Unspecified;

                // act

                // assert
                test.CreatedUtc
                    .Kind
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void LocalTime_ReturnsExpectedDateTimeKind() {
                // arrange
                StubAuditableEntity test = new StubAuditableEntity();
                test.CreatedUtc = new DateTime(2017, 7, 26, 10, 17, 0, DateTimeKind.Local);
                DateTimeKind expected = DateTimeKind.Utc;

                // act
                DateTimeKind results = test.CreatedUtc.Kind;

                // assert
                results.Should()
                       .Be(expected);
            }

            [Fact]
            public void LocalTime_ReturnsExpectedValue() {
                // arrange
                StubAuditableEntity test = new StubAuditableEntity();
                test.CreatedUtc = new DateTime(2017, 7, 26, 10, 17, 0, DateTimeKind.Local);
                DateTime expected = new DateTime(2017, 7, 26, 15, 17, 0, DateTimeKind.Utc);

                // act
                DateTime results = test.CreatedUtc;

                // assert
                results.Should()
                       .Be(expected);
            }
        }

        public class Get_ModifiedUtc {
            [Fact]
            public void GoodDetail_ReturnsExpectedDateTimeKind() {
                // arrange
                StubAuditableEntity fakeEntity = MakeEntity();
                DateTimeKind expected = DateTimeKind.Utc;

                // act

                // assert
                fakeEntity.ModifiedUtc
                          .Kind
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                StubAuditableEntity fakeEntity = MakeEntity();
                DateTime expected = new DateTime(2017, 6, 15, 14, 16, 0, DateTimeKind.Utc);

                // act

                // assert
                fakeEntity.ModifiedUtc
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue() {
                // arrange
                StubAuditableEntity test = new StubAuditableEntity();
                DateTime expected = DateTime.MinValue;

                // act

                // assert
                test.ModifiedUtc
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void InitializedDetail_HeadDefaultDateTimeKind() {
                // arrange
                StubAuditableEntity test = new StubAuditableEntity();
                DateTimeKind expected = DateTimeKind.Unspecified;

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