using System;

using KeithLink.Common.Core.Seams;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Models.Lists.Contract;

using FluentAssertions;
using Xunit;

namespace KeithLink.Svc.Core.Tests.Unit.Extensions.Lists
{
    public class ContractDetailExtensionTests
    {
        private static ContractListDetail MakeDetail()
        {
            return new ContractListDetail()
            {
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

        public class ToWebModel
        {
            [Fact]
            public void GoodDetail_ReturnsExpectedListItemId()
            {
                // arrange
                var detail = MakeDetail();
                var expected = 21;

                // act
                var results = detail.ToWebModel();

                // assert
                results.ListItemId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedType()
            {
                // arrange
                var detail = MakeDetail();
                var expected = ListType.Contract;

                // act
                var results = detail.ToWebModel();

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCategory()
            {
                // arrange
                var detail = MakeDetail();
                var expected = "Fake Category";

                // act
                var results = detail.ToWebModel();

                // assert
                results.Category
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedItemNumber()
            {
                // arrange
                var detail = MakeDetail();
                var expected = "123456";

                // act
                var results = detail.ToWebModel();

                // assert
                results.ItemNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_FromDate()
            {
                // arrange
                var detail = MakeDetail();
                var expected = new DateTime(2017, 7, 1);

                // act
                var results = detail.ToWebModel();

                // assert
                results.FromDate
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedToDate()
            {
                // arrange
                var detail = MakeDetail();
                var expected = new DateTime(2017, 7, 30);

                // act
                var results = detail.ToWebModel();

                // assert
                results.ToDate
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedPosition()
            {
                // arrange
                var detail = MakeDetail();
                var expected = 17;

                // act
                var results = detail.ToWebModel();

                // assert
                results.Position
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCreatedUtc()
            {
                // arrange
                var detail = MakeDetail();
                var expected = new DateTime(2017, 7, 5, 16, 41, 0, DateTimeKind.Utc);

                // act
                var results = detail.ToWebModel();

                // assert
                results.CreatedUtc
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedModifiedUtc()
            {
                // arrange
                var detail = MakeDetail();
                var expected = new DateTime(2017, 7, 5, 16, 42, 0, DateTimeKind.Utc);

                // act
                var results = detail.ToWebModel();

                // assert
                results.ModifiedUtc
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedEach()
            {
                // arrange
                var detail = MakeDetail();
                var expected = true;

                // act
                var results = detail.ToWebModel();

                // assert
                results.Each
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCatalogId()
            {
                // arrange
                var detail = MakeDetail();
                var expected = "FUT";

                // act
                var results = detail.ToWebModel();

                // assert
                results.CatalogId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetailWithNowLessThanTwoWeeks_ReturnsDeltaOfAddedAndActive()
            {
                // arrange
                var detail = MakeDetail();
                var expected = "newly added active";

                // act
                SystemTime.Set(new DateTime(2017, 7, 17));
                var results = detail.ToWebModel();
                SystemTime.Reset();

                // assert
                results.Delta
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetailWithNowFifteenDaysout_ReturnsDeltaOfActive()
            {
                // arrange
                var detail = MakeDetail();
                var expected = "active";

                // act
                SystemTime.Set(new DateTime(2017, 7, 20));
                var results = detail.ToWebModel();
                SystemTime.Reset();

                // assert
                results.Delta
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetailWithNowGreaterThanToDate_ReturnsDeltaOfDeleted()
            {
                // arrange
                var detail = MakeDetail();
                var expected = "newly deleted";

                // act
                SystemTime.Set(new DateTime(2017, 8, 1));
                var results = detail.ToWebModel();
                SystemTime.Reset();

                // assert
                results.Delta
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void NullToDateWithNowAfterTwoWeeks_ReturnsDeltaOfActive()
            {
                // arrange
                var detail = new ContractListDetail()
                {
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
                var expected = "active";

                // act
                SystemTime.Set(new DateTime(2017, 7, 20));
                var results = detail.ToWebModel();
                SystemTime.Reset();

                // assert
                results.Delta
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void NullToDateWithNowAfterTwoMonths_ReturnsDeltaOfActive()
            {
                // arrange
                var detail = new ContractListDetail()
                {
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
                var expected = "active";

                // act
                SystemTime.Set(new DateTime(2017, 9, 5));
                var results = detail.ToWebModel();
                SystemTime.Reset();

                // assert
                results.Delta
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void NullEach_ReturnsFalse()
            {
                // arrange
                var detail = new ContractListDetail()
                {
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
                var expected = false;

                // act
                var results = detail.ToWebModel();

                // assert
                results.Each
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void NullCustomInventoryItemId_ReturnsZero()
            {
                // arrange
                var detail = new ContractListDetail()
                {
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
                var expected = 0;

                // act
                var results = detail.ToWebModel();

                // assert
                results.CustomInventoryItemId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsNullPackSize()
            {
                // arrange
                var detail = MakeDetail();

                // act
                var results = detail.ToWebModel();

                // assert
                results.PackSize
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullStorageTemp()
            {
                // arrange
                var detail = MakeDetail();

                // act
                var results = detail.ToWebModel();

                // assert
                results.StorageTemp
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsQuantityOfZero()
            {
                // arrange
                var detail = MakeDetail();
                var expected = 0;

                // act
                var results = detail.ToWebModel();

                // assert
                results.Quantity
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsIsDeleteAsFalse()
            {
                // arrange
                var detail = MakeDetail();
                var expected = false;

                // act
                var results = detail.ToWebModel();

                // assert
                results.IsDelete
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsNullItemStatistics()
            {
                // arrange
                var detail = MakeDetail();

                // act
                var results = detail.ToWebModel();

                // assert
                results.ItemStatistics
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullProprietaryCustomers()
            {
                // arrange
                var detail = MakeDetail();

                // act
                var results = detail.ToWebModel();

                // assert
                results.ProprietaryCustomers
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullSupplier()
            {
                // arrange
                var detail = MakeDetail();

                // act
                var results = detail.ToWebModel();

                // assert
                results.Supplier
                       .Should()
                       .BeNull();
            }
        }
    }
}