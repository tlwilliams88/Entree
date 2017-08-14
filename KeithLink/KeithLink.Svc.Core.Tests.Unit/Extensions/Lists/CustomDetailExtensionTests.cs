using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return new CustomListDetail() {
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
            return new ListItemModel() {
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
            public void GoodDetail_ReturnsExpectedListItemId() {
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
            public void GoodDetail_ReturnsExpectedType() {
                // arrange
                var detail = MakeDetail();
                var expected = ListType.Custom;

                // act
                var results = detail.ToWebModel();

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedItemNumber() {
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
            public void GoodDetail_ReturnsExpectedPosition() {
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
            public void GoodDetail_ReturnsExpectedCreatedUtc() {
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
            public void GoodDetail_ReturnsExpectedModifiedUtc() {
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
            public void GoodDetail_ReturnsExpectedEach() {
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
            public void GoodDetail_ReturnsExpectedCatalogId() {
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
            public void GoodDetail_ReturnsExpectedActive() {
                // arrange
                var detail = MakeDetail();
                var expected = true;

                // act
                var results = detail.ToWebModel();

                // assert
                results.Active
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedCustomInveotryItemId() {
                // arrange
                var detail = MakeDetail();
                var expected =  101;

                // act
                var results = detail.ToWebModel();

                // assert
                results.CustomInventoryItemId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void NullToDateWithNowAfterTwoWeeks_ReturnsDeltaOfActive() {
                // arrange
                var detail = new ContractListDetail() {
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
            public void NullToDateWithNowAfterTwoMonths_ReturnsDeltaOfActive() {
                // arrange
                var detail = new ContractListDetail() {
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
            public void NullEach_ReturnsFalse() {
                // arrange
                var detail = new ContractListDetail() {
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
            public void NullCustomInventoryItemId_ReturnsZero() {
                // arrange
                var detail = new CustomListDetail() {
                    Id = 21,
                    CatalogId = "FUT",
                    Each = true,
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
            public void GoodDetail_ReturnsNullPackSize() {
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
            public void GoodDetail_ReturnsNullStorageTemp() {
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
            public void GoodDetail_ReturnsNullCategory() {
                // arrange
                var detail = MakeDetail();

                // act
                var results = detail.ToWebModel();

                // assert
                results.Category
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullFromDate() {
                // arrange
                var detail = MakeDetail();

                // act
                var results = detail.ToWebModel();

                // assert
                results.FromDate
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullToDate() {
                // arrange
                var detail = MakeDetail();

                // act
                var results = detail.ToWebModel();

                // assert
                results.ToDate
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsNullDelta() {
                // arrange
                var detail = MakeDetail();

                // act
                var results = detail.ToWebModel();

                // assert
                results.Delta
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodDetail_ReturnsQuantityOfZero() {
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
            public void GoodDetail_ReturnsIsDeleteAsFalse() {
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
            public void GoodDetail_ReturnsNullItemStatistics() {
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
            public void GoodDetail_ReturnsNullProprietaryCustomers() {
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
            public void GoodDetail_ReturnsNullSupplier() {
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

        public class ToCustomListDetail {
            [Fact]
            public void GoodModel_ReturnsExpectedActive() {
                // arrange
                var expected = true;
                var headerId = 1;
                var model = MakeModel();

                // act
                var results = model.ToCustomListDetail(headerId);

                // assert
                results.Active
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpectedCatalogId() {
                // arrange
                var expected = "FUT";
                var headerId = 1;
                var model = MakeModel();

                // act
                var results = model.ToCustomListDetail(headerId);

                // assert
                results.CatalogId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpectedCustomInventoryItemId() {
                // arrange
                var expected = 1900;
                var headerId = 1;
                var model = MakeModel();

                // act
                var results = model.ToCustomListDetail(headerId);

                // assert
                results.CustomInventoryItemId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpectedEach() {
                // arrange
                var expected = true;
                var headerId = 1;
                var model = MakeModel();

                // act
                var results = model.ToCustomListDetail(headerId);

                // assert
                results.Each
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpectedId() {
                // arrange
                var expected = 19;
                var headerId = 1;
                var model = MakeModel();

                // act
                var results = model.ToCustomListDetail(headerId);

                // assert
                results.Id
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpectedItemNumber() {
                // arrange
                var expected = "123456";
                var headerId = 1;
                var model = MakeModel();

                // act
                var results = model.ToCustomListDetail(headerId);

                // assert
                results.ItemNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpectedLineNumber() {
                // arrange
                var expected = 23;
                var headerId = 1;
                var model = MakeModel();

                // act
                var results = model.ToCustomListDetail(headerId);

                // assert
                results.LineNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpectedLabel() {
                // arrange
                var expected = "Fake Label";
                var headerId = 1;
                var model = MakeModel();

                // act
                var results = model.ToCustomListDetail(headerId);

                // assert
                results.Label
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpectedPar() {
                // arrange
                var expected = 9.76m;
                var headerId = 1;
                var model = MakeModel();

                // act
                var results = model.ToCustomListDetail(headerId);

                // assert
                results.Par
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpectedHeaderId() {
                // arrange
                var expected = 1;
                var headerId = 1;
                var model = MakeModel();

                // act
                var results = model.ToCustomListDetail(headerId);

                // assert
                results.HeaderId
                       .Should()
                       .Be(expected);
            }
        }
    }
}
