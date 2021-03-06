﻿using System;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;

namespace KeithLink.Svc.Core.Tests.Unit.Extensions.Lists {
    public class InventoryValuationDetailExtensionTests {
        private static InventoryValuationListDetail MakeDetail() {
            return new InventoryValuationListDetail {
                Id = 15,
                ItemNumber = "123456",
                LineNumber = 7,
                ModifiedUtc = new DateTime(2017, 7, 20, 10, 45, 0, DateTimeKind.Utc),
                CreatedUtc = new DateTime(2017, 7, 20, 10, 44, 0, DateTimeKind.Utc),
                Each = true,
                CatalogId = "FUT",
                CustomInventoryItemId = 1900,
                Quantity = 9.76m,
                Label = "Test Label"
            };
        }

        private static ListItemModel MakeModel() {
            return new ListItemModel {
                Active = true,
                CatalogId = "FUT",
                CustomInventoryItemId = 1900,
                Each = true,
                ListItemId = 19,
                ItemNumber = "123456",
                Position = 23,
                Quantity = 9.76m,
                Label = "Test Label"
            };
        }

        public class ToWebModel {
            [Fact]
            public void GoodDetail_ReturnsExpectedActive() {
                // arrange
                InventoryValuationListDetail detail = MakeDetail();
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
                InventoryValuationListDetail detail = MakeDetail();
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
                InventoryValuationListDetail detail = MakeDetail();
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
                InventoryValuationListDetail detail = MakeDetail();
                int expected = 1900;

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
                InventoryValuationListDetail detail = MakeDetail();
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
                InventoryValuationListDetail detail = MakeDetail();
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
                InventoryValuationListDetail detail = MakeDetail();
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
                InventoryValuationListDetail detail = MakeDetail();
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
                InventoryValuationListDetail detail = MakeDetail();
                int expected = 7;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Position
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedQuantity() {
                // arrange
                InventoryValuationListDetail detail = MakeDetail();
                decimal expected = 9.76m;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Quantity
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedType() {
                // arrange
                InventoryValuationListDetail detail = MakeDetail();
                ListType expected = ListType.InventoryValuation;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsGoodLabel() {
                // arrange
                InventoryValuationListDetail detail = MakeDetail();
                string expected = "Test Label";

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Label
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsIsDeleteAsFalse() {
                // arrange
                InventoryValuationListDetail detail = MakeDetail();
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
                InventoryValuationListDetail detail = MakeDetail();

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
                InventoryValuationListDetail detail = MakeDetail();

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
                InventoryValuationListDetail detail = MakeDetail();

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
                InventoryValuationListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.ItemStatistics
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullPackSize() {
                // arrange
                InventoryValuationListDetail detail = MakeDetail();

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
                InventoryValuationListDetail detail = MakeDetail();

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
                InventoryValuationListDetail detail = MakeDetail();

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
                InventoryValuationListDetail detail = MakeDetail();

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
                InventoryValuationListDetail detail = MakeDetail();

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
                InventoryValuationListDetail detail = MakeDetail();
                int expected = 0;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.ParLevel
                       .Should()
                       .Be(expected);
            }
        }

        public class ToCustomListDetail {
            [Fact]
            public void GoodModel_ReturnsExpectedActive() {
                // arrange
                bool expected = true;
                int headerId = 1;
                ListItemModel model = MakeModel();

                // act
                InventoryValuationListDetail results = model.ToInventoryValuationListDetail(headerId);

                // assert
                results.Active
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpectedCatalogId() {
                // arrange
                string expected = "FUT";
                int headerId = 1;
                ListItemModel model = MakeModel();

                // act
                InventoryValuationListDetail results = model.ToInventoryValuationListDetail(headerId);

                // assert
                results.CatalogId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpectedCustomInventoryItemId() {
                // arrange
                int expected = 1900;
                int headerId = 1;
                ListItemModel model = MakeModel();

                // act
                InventoryValuationListDetail results = model.ToInventoryValuationListDetail(headerId);

                // assert
                results.CustomInventoryItemId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpectedEach() {
                // arrange
                bool expected = true;
                int headerId = 1;
                ListItemModel model = MakeModel();

                // act
                InventoryValuationListDetail results = model.ToInventoryValuationListDetail(headerId);

                // assert
                results.Each
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpectedHeaderId() {
                // arrange
                int expected = 1;
                int headerId = 1;
                ListItemModel model = MakeModel();

                // act
                InventoryValuationListDetail results = model.ToInventoryValuationListDetail(headerId);

                // assert
                results.HeaderId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpectedId() {
                // arrange
                int expected = 19;
                int headerId = 1;
                ListItemModel model = MakeModel();

                // act
                InventoryValuationListDetail results = model.ToInventoryValuationListDetail(headerId);

                // assert
                results.Id
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpectedItemNumber() {
                // arrange
                string expected = "123456";
                int headerId = 1;
                ListItemModel model = MakeModel();

                // act
                InventoryValuationListDetail results = model.ToInventoryValuationListDetail(headerId);

                // assert
                results.ItemNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpectedLabel() {
                // arrange
                string expected = "Test Label";
                int headerId = 1;
                ListItemModel model = MakeModel();

                // act
                InventoryValuationListDetail results = model.ToInventoryValuationListDetail(headerId);

                // assert
                results.Label
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpectedLineNumber() {
                // arrange
                int expected = 23;
                int headerId = 1;
                ListItemModel model = MakeModel();

                // act
                InventoryValuationListDetail results = model.ToInventoryValuationListDetail(headerId);

                // assert
                results.LineNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpectedQuantity() {
                // arrange
                decimal expected = 9.76m;
                int headerId = 1;
                ListItemModel model = MakeModel();

                // act
                InventoryValuationListDetail results = model.ToInventoryValuationListDetail(headerId);

                // assert
                results.Quantity
                       .Should()
                       .Be(expected);
            }
        }
    }
}