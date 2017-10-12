using System;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.History;

namespace KeithLink.Svc.Core.Tests.Unit.Extensions.Lists {
    public class HistoryDetailExtensionTests {
        private static HistoryListDetail MakeDetail() {
            return new HistoryListDetail {
                Id = 15,
                ItemNumber = "123456",
                LineNumber = 7,
                ModifiedUtc = new DateTime(2017, 7, 20, 10, 45, 0, DateTimeKind.Utc),
                CreatedUtc = new DateTime(2017, 7, 20, 10, 44, 0, DateTimeKind.Utc),
                Each = true,
                CatalogId = "FUT"
            };
        }

        public class ToWebModel {
            [Fact]
            public void GoodDetail_ReturnsExpectedActive() {
                // arrange
                HistoryListDetail detail = MakeDetail();
                bool expected = true;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Active
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCatalogId() {
                // arrange
                HistoryListDetail detail = MakeDetail();
                string expected = "FUT";

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.CatalogId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCreatedUtc() {
                // arrange
                HistoryListDetail detail = MakeDetail();
                DateTime expected = new DateTime(2017, 7, 20, 10, 44, 0, DateTimeKind.Utc);

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.CreatedUtc
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCustomInveotryItemId() {
                // arrange
                HistoryListDetail detail = MakeDetail();
                int expected = 0;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.CustomInventoryItemId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedEach() {
                // arrange
                HistoryListDetail detail = MakeDetail();
                bool expected = true;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Each
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedItemNumber() {
                // arrange
                HistoryListDetail detail = MakeDetail();
                string expected = "123456";

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.ItemNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedListItemId() {
                // arrange
                HistoryListDetail detail = MakeDetail();
                int expected = 15;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.ListItemId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedModifiedUtc() {
                // arrange
                HistoryListDetail detail = MakeDetail();
                DateTime expected = new DateTime(2017, 7, 20, 10, 45, 0, DateTimeKind.Utc);

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.ModifiedUtc
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedPosition() {
                // arrange
                HistoryListDetail detail = MakeDetail();
                int expected = 7;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Position
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedType() {
                // arrange
                HistoryListDetail detail = MakeDetail();
                ListType expected = ListType.Worksheet;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsIsDeleteAsFalse() {
                // arrange
                HistoryListDetail detail = MakeDetail();
                bool expected = false;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.IsDelete
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsNullCategory() {
                // arrange
                HistoryListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Category
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullDelta() {
                // arrange
                HistoryListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Delta
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullFromDate() {
                // arrange
                HistoryListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.FromDate
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullItemStatistics() {
                // arrange
                HistoryListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.ItemStatistics
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullLabel() {
                // arrange
                HistoryListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Label
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullPackSize() {
                // arrange
                HistoryListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.PackSize
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullProprietaryCustomers() {
                // arrange
                HistoryListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.ProprietaryCustomers
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullStorageTemp() {
                // arrange
                HistoryListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.StorageTemp
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullSupplier() {
                // arrange
                HistoryListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Supplier
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullToDate() {
                // arrange
                HistoryListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.ToDate
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsParLevelOfZero() {
                // arrange
                HistoryListDetail detail = MakeDetail();
                int expected = 0;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.ParLevel
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsQuantityOfZero() {
                // arrange
                HistoryListDetail detail = MakeDetail();
                int expected = 0;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Quantity
                       .Should()
                       .Be(expected);
            }
        }
    }
}