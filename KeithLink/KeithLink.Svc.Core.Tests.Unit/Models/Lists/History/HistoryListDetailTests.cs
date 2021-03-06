﻿using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Lists.History;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.History {
    public class HistoryListDetailTests {
        private static HistoryListDetail MakeDetail() {
            return new HistoryListDetail {
                LineNumber = 72
            };
        }

        public class Get_LineNumber {
            [Fact]
            public void GoodDetail_ReturnsExpectedValue() {
                // arrange
                HistoryListDetail fakeDetail = MakeDetail();
                int expected = 72;

                // act

                // assert
                fakeDetail.LineNumber
                          .Should()
                          .Be(expected);
            }

            [Fact]
            public void InitializedDetail_HasDefaultValue() {
                // arrange
                HistoryListDetail test = new HistoryListDetail();
                int expected = 0;

                // act

                // assert
                test.LineNumber
                    .Should()
                    .Be(expected);
            }
        }
    }
}