﻿using KeithLink.Svc.Core.Models.Lists;

using FluentAssertions;
using Xunit;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists
{
    public class BaseListDetailTests
    {
        private class StubDetail : BaseListDetail { }

        private static StubDetail MakeDetail()
        {
            return new StubDetail()
            {
                CatalogId = "Fake CatalogId",
                Each = true,
                HeaderId = 15,
                ItemNumber = "Fake ItemNumber",
                LineNumber = 100
            };
        }

        public class Get_CatalogId
        {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeDetail();
                var expected = "Fake CatalogId";

                // act

                // assert
                fakeItem.CatalogId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue()
            {
                // arrange
                var test = new StubDetail();

                // act

                // assert
                test.CatalogId
                    .Should()
                    .BeNull();
            }
        }

        public class Get_Each
        {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeDetail();
                var expected = true;

                // act

                // assert
                fakeItem.Each
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue()
            {
                // arrange
                var test = new StubDetail();

                // act

                // assert
                test.Each
                    .Should()
                    .BeNull();
            }
        }

        public class Get_HeaderId
        {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeDetail();
                var expected = 15;

                // act

                // assert
                fakeItem.HeaderId
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue()
            {
                // arrange
                var test = new StubDetail();
                var expected = 0;

                // act

                // assert
                test.HeaderId
                    .Should()
                    .Be(expected);
            }
        }

        public class Get_ItemNumber
        {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeDetail();
                var expected = "Fake ItemNumber";

                // act

                // assert
                fakeItem.ItemNumber
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitalizedDetail_HasDefaultValue()
            {
                // arrange
                var test = new StubDetail();

                // act

                // assert
                test.ItemNumber
                    .Should()
                    .BeNull();
            }
        }

        public class Get_LineNumber
        {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue()
            {
                // arrange
                var fakeItem = MakeDetail();
                var expected = 100;

                // act

                // assert
                fakeItem.LineNumber
                        .Should()
                        .Be(expected);
            }

            [Fact]
            public void InitializedDetail_HasDefaultValue()
            {
                // arrange
                var test = new StubDetail();

                // act

                // assert
                test.LineNumber
                    .Should()
                    .Be(0);
            }
        }
    }
}