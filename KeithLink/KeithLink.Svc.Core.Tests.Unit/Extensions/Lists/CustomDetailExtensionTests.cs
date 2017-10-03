using System;

using FluentAssertions;
using Xunit;

using KeithLink.Common.Core.Seams;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Contract;
using KeithLink.Svc.Core.Models.Lists.CustomList;

namespace KeithLink.Svc.Core.Tests.Unit.Extensions.Lists {
    public class CustomDetailExtensionTests {
        private static CustomListDetail MakeDetail() {
            return new CustomListDetail {
                Id = 21,
                CatalogId = "FUT",
                Each = true,
                HeaderId = 10,
                ItemNumber = "123456",
                LineNumber = 17,
                CreatedUtc = new DateTime(2017, 7, 5, 16, 41, 0, DateTimeKind.Utc),
                ModifiedUtc = new DateTime(2017, 7, 5, 16, 42, 0, DateTimeKind.Utc),
                Active = true,
                CustomInventoryItemId = 101,
                Par = 2.3M,
                Label = "Fake Label"
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
                Label = "Fake Label",
                ParLevel = 9.76m
            };
        }

        public class ToWebModel {
            [Fact]
            public void GoodDetail_ReturnsExpectedActive() {
                // arrange
                CustomListDetail detail = MakeDetail();
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
                CustomListDetail detail = MakeDetail();
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
                CustomListDetail detail = MakeDetail();
                DateTime expected = new DateTime(2017, 7, 5, 16, 41, 0, DateTimeKind.Utc);

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
                CustomListDetail detail = MakeDetail();
                int expected = 101;

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
                CustomListDetail detail = MakeDetail();
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
                CustomListDetail detail = MakeDetail();
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
                CustomListDetail detail = MakeDetail();
                int expected = 21;

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
                CustomListDetail detail = MakeDetail();
                DateTime expected = new DateTime(2017, 7, 5, 16, 42, 0, DateTimeKind.Utc);

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
                CustomListDetail detail = MakeDetail();
                int expected = 17;

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
                CustomListDetail detail = MakeDetail();
                ListType expected = ListType.Custom;

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
                CustomListDetail detail = MakeDetail();
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
                CustomListDetail detail = MakeDetail();

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
                CustomListDetail detail = MakeDetail();

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
                CustomListDetail detail = MakeDetail();

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
                CustomListDetail detail = MakeDetail();

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
                CustomListDetail detail = MakeDetail();

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
                CustomListDetail detail = MakeDetail();

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
                CustomListDetail detail = MakeDetail();

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
                CustomListDetail detail = MakeDetail();

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
                CustomListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.ToDate
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsQuantityOfZero() {
                // arrange
                CustomListDetail detail = MakeDetail();
                int expected = 0;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Quantity
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void NullCustomInventoryItemId_ReturnsZero() {
                // arrange
                CustomListDetail detail = new CustomListDetail {
                    Id = 21,
                    CatalogId = "FUT",
                    Each = true,
                    HeaderId = 10,
                    ItemNumber = "123456",
                    LineNumber = 17,
                    CreatedUtc = new DateTime(2017, 7, 5, 16, 41, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 7, 5, 16, 42, 0, DateTimeKind.Utc)
                };
                int expected = 0;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.CustomInventoryItemId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void NullEach_ReturnsFalse() {
                // arrange
                ContractListDetail detail = new ContractListDetail {
                    Id = 21,
                    CatalogId = "FUT",
                    Category = "Fake Category",
                    FromDate = new DateTime(2017, 7, 1),
                    ToDate = new DateTime(2017, 7, 30),
                    HeaderId = 10,
                    ItemNumber = "123456",
                    LineNumber = 17,
                    CreatedUtc = new DateTime(2017, 7, 5, 16, 41, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 7, 5, 16, 42, 0, DateTimeKind.Utc)
                };
                bool expected = false;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Each
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void NullToDateWithNowAfterTwoMonths_ReturnsDeltaOfActive() {
                // arrange
                ContractListDetail detail = new ContractListDetail {
                    Id = 21,
                    CatalogId = "FUT",
                    Category = "Fake Category",
                    Each = true,
                    FromDate = new DateTime(2017, 7, 1),
                    HeaderId = 10,
                    ItemNumber = "123456",
                    LineNumber = 17,
                    CreatedUtc = new DateTime(2017, 7, 5, 16, 41, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 7, 5, 16, 42, 0, DateTimeKind.Utc)
                };
                string expected = "active";

                // act
                SystemTime.Set(new DateTime(2017, 9, 5));
                ListItemModel results = detail.ToWebModel();
                SystemTime.Reset();

                // assert
                results.Delta
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void NullToDateWithNowAfterTwoWeeks_ReturnsDeltaOfActive() {
                // arrange
                ContractListDetail detail = new ContractListDetail {
                    Id = 21,
                    CatalogId = "FUT",
                    Category = "Fake Category",
                    Each = true,
                    FromDate = new DateTime(2017, 7, 1),
                    HeaderId = 10,
                    ItemNumber = "123456",
                    LineNumber = 17,
                    CreatedUtc = new DateTime(2017, 7, 5, 16, 41, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 7, 5, 16, 42, 0, DateTimeKind.Utc)
                };
                string expected = "active";

                // act
                SystemTime.Set(new DateTime(2017, 7, 20));
                ListItemModel results = detail.ToWebModel();
                SystemTime.Reset();

                // assert
                results.Delta
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
                CustomListDetail results = model.ToCustomListDetail(headerId);

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
                CustomListDetail results = model.ToCustomListDetail(headerId);

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
                CustomListDetail results = model.ToCustomListDetail(headerId);

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
                CustomListDetail results = model.ToCustomListDetail(headerId);

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
                CustomListDetail results = model.ToCustomListDetail(headerId);

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
                CustomListDetail results = model.ToCustomListDetail(headerId);

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
                CustomListDetail results = model.ToCustomListDetail(headerId);

                // assert
                results.ItemNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpectedLabel() {
                // arrange
                string expected = "Fake Label";
                int headerId = 1;
                ListItemModel model = MakeModel();

                // act
                CustomListDetail results = model.ToCustomListDetail(headerId);

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
                CustomListDetail results = model.ToCustomListDetail(headerId);

                // assert
                results.LineNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpectedPar() {
                // arrange
                decimal expected = 9.76m;
                int headerId = 1;
                ListItemModel model = MakeModel();

                // act
                CustomListDetail results = model.ToCustomListDetail(headerId);

                // assert
                results.Par
                       .Should()
                       .Be(expected);
            }
        }
    }
}