using System;

using FluentAssertions;
using Xunit;

using KeithLink.Common.Core.Seams;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Contract;

namespace KeithLink.Svc.Core.Tests.Unit.Extensions.Lists {
    public class ContractDetailExtensionTests {
        private static ContractListDetail MakeDetail() {
            return new ContractListDetail {
                Id = 21,
                CatalogId = "FUT",
                Category = "Fake Category",
                Each = true,
                FromDate = new DateTime(2017, 7, 1),
                ToDate = new DateTime(2017, 7, 30),
                HeaderId = 10,
                ItemNumber = "123456",
                LineNumber = 17,
                CreatedUtc = new DateTime(2017, 7, 5, 16, 41, 0, DateTimeKind.Utc),
                ModifiedUtc = new DateTime(2017, 7, 5, 16, 42, 0, DateTimeKind.Utc)
            };
        }

        public class ToWebModel {
            [Fact]
            public void GoodDetail_FromDate() {
                // arrange
                ContractListDetail detail = MakeDetail();
                DateTime expected = new DateTime(2017, 7, 1);

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.FromDate
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCatalogId() {
                // arrange
                ContractListDetail detail = MakeDetail();
                string expected = "FUT";

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.CatalogId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCategory() {
                // arrange
                ContractListDetail detail = MakeDetail();
                string expected = "Fake Category";

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Category
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCreatedUtc() {
                // arrange
                ContractListDetail detail = MakeDetail();
                DateTime expected = new DateTime(2017, 7, 5, 16, 41, 0, DateTimeKind.Utc);

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.CreatedUtc
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedEach() {
                // arrange
                ContractListDetail detail = MakeDetail();
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
                ContractListDetail detail = MakeDetail();
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
                ContractListDetail detail = MakeDetail();
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
                ContractListDetail detail = MakeDetail();
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
                ContractListDetail detail = MakeDetail();
                int expected = 17;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Position
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedToDate() {
                // arrange
                ContractListDetail detail = MakeDetail();
                DateTime expected = new DateTime(2017, 7, 30);

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.ToDate
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedType() {
                // arrange
                ContractListDetail detail = MakeDetail();
                ListType expected = ListType.Contract;

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
                ContractListDetail detail = MakeDetail();
                bool expected = false;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.IsDelete
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsNullItemStatistics() {
                // arrange
                ContractListDetail detail = MakeDetail();

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
                ContractListDetail detail = MakeDetail();

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
                ContractListDetail detail = MakeDetail();

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
                ContractListDetail detail = MakeDetail();

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
                ContractListDetail detail = MakeDetail();

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Supplier
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsQuantityOfZero() {
                // arrange
                ContractListDetail detail = MakeDetail();
                int expected = 0;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Quantity
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetailWithNowFifteenDaysout_ReturnsDeltaOfActive() {
                // arrange
                ContractListDetail detail = MakeDetail();
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

            [Fact]
            public void GoodDetailWithNowGreaterThanToDate_ReturnsDeltaOfDeleted() {
                // arrange
                ContractListDetail detail = MakeDetail();
                string expected = "newly deleted";

                // act
                SystemTime.Set(new DateTime(2017, 8, 1));
                ListItemModel results = detail.ToWebModel();
                SystemTime.Reset();

                // assert
                results.Delta
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetailWithNowLessThanTwoWeeks_ReturnsDeltaOfAddedAndActive() {
                // arrange
                ContractListDetail detail = MakeDetail();
                string expected = "newly added active";

                // act
                SystemTime.Set(new DateTime(2017, 7, 17));
                ListItemModel results = detail.ToWebModel();
                SystemTime.Reset();

                // assert
                results.Delta
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void NullCustomInventoryItemId_ReturnsZero() {
                // arrange
                ContractListDetail detail = new ContractListDetail {
                    Id = 21,
                    CatalogId = "FUT",
                    Category = "Fake Category",
                    Each = true,
                    FromDate = new DateTime(2017, 7, 1),
                    ToDate = new DateTime(2017, 7, 30),
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
    }
}