using System;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.MandatoryItem;

namespace KeithLink.Svc.Core.Tests.Unit.Extensions.Lists {
    public class MandatoryItemDetailExtensionTests {
        private static MandatoryItemsListDetail MakeDetail() {
            return new MandatoryItemsListDetail {
                Id = 15,
                ItemNumber = "123456",
                HeaderId = 23,
                LineNumber = 7,
                ModifiedUtc = new DateTime(2017, 7, 20, 10, 45, 0, DateTimeKind.Utc),
                CreatedUtc = new DateTime(2017, 7, 20, 10, 44, 0, DateTimeKind.Utc),
                Each = true,
                CatalogId = "FUT",
                Active = true
            };
        }

        private static ListItemModel MakeModel() {
            return new ListItemModel {
                Active = true,
                CatalogId = "FUT",
                Each = true,
                ListItemId = 19,
                ItemNumber = "123456",
                Position = 23,
                Label = "Fake Label",
                Quantity = 17
            };
        }

        public class ToWebModel {
            [Fact]
            public void GoodDetail_ReturnsExpectedActive() {
                // arrange
                MandatoryItemsListDetail detail = MakeDetail();
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
                MandatoryItemsListDetail detail = MakeDetail();
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
                MandatoryItemsListDetail detail = MakeDetail();
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
                MandatoryItemsListDetail detail = MakeDetail();
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
                MandatoryItemsListDetail detail = MakeDetail();
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
                MandatoryItemsListDetail detail = MakeDetail();
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
                MandatoryItemsListDetail detail = MakeDetail();
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
                MandatoryItemsListDetail detail = MakeDetail();
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
                MandatoryItemsListDetail detail = MakeDetail();
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
                MandatoryItemsListDetail detail = MakeDetail();
                ListType expected = ListType.Mandatory;

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
                MandatoryItemsListDetail detail = MakeDetail();
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
                MandatoryItemsListDetail detail = MakeDetail();

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
                MandatoryItemsListDetail detail = MakeDetail();

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
                MandatoryItemsListDetail detail = MakeDetail();

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
                MandatoryItemsListDetail detail = MakeDetail();

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
                MandatoryItemsListDetail detail = MakeDetail();

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
                MandatoryItemsListDetail detail = MakeDetail();

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
                MandatoryItemsListDetail detail = MakeDetail();

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
                MandatoryItemsListDetail detail = MakeDetail();

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
                MandatoryItemsListDetail detail = MakeDetail();

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
                MandatoryItemsListDetail detail = MakeDetail();

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
                MandatoryItemsListDetail detail = MakeDetail();
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
                MandatoryItemsListDetail detail = MakeDetail();
                int expected = 0;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Quantity
                       .Should()
                       .Be(expected);
            }
        }

        public class ToMandatoryItemsListDetail {
            [Fact]
            public void GoodModel_ReturnsExpectedActive() {
                // arrange
                bool expected = true;
                int headerId = 1;
                ListItemModel model = MakeModel();

                // act
                MandatoryItemsListDetail results = model.ToMandatoryItemsListDetail(headerId);

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
                MandatoryItemsListDetail results = model.ToMandatoryItemsListDetail(headerId);

                // assert
                results.CatalogId
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
                MandatoryItemsListDetail results = model.ToMandatoryItemsListDetail(headerId);

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
                MandatoryItemsListDetail results = model.ToMandatoryItemsListDetail(headerId);

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
                MandatoryItemsListDetail results = model.ToMandatoryItemsListDetail(headerId);

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
                MandatoryItemsListDetail results = model.ToMandatoryItemsListDetail(headerId);

                // assert
                results.ItemNumber
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
                MandatoryItemsListDetail results = model.ToMandatoryItemsListDetail(headerId);

                // assert
                results.LineNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpecteQuantity() {
                // arrange
                int expected = 17;
                int headerId = 1;
                ListItemModel model = MakeModel();

                // act
                MandatoryItemsListDetail results = model.ToMandatoryItemsListDetail(headerId);

                // assert
                results.Quantity
                       .Should()
                       .Be(expected);
            }
        }
    }
}