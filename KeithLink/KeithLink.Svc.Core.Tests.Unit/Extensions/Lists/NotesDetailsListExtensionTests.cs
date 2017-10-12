using System;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Notes;

namespace KeithLink.Svc.Core.Tests.Unit.Extensions.Lists {
    public class NotesDetailsListExtensionTests {
        private static NotesListDetail MakeDetail() {
            return new NotesListDetail {
                Id = 15,
                ItemNumber = "123456",
                HeaderId = 23,
                LineNumber = 7,
                ModifiedUtc = new DateTime(2017, 7, 20, 10, 45, 0, DateTimeKind.Utc),
                CreatedUtc = new DateTime(2017, 7, 20, 10, 44, 0, DateTimeKind.Utc),
                Each = true,
                CatalogId = "FUT",
                Note = "Fake Note"
            };
        }

        private static ListItemModel MakeModel() {
            return new ListItemModel {
                Active = true,
                CatalogId = "FUT",
                Each = true,
                ListItemId = 19,
                Position = 23,
                ItemNumber = "123456",
                Notes = "Fake Note"
            };
        }

        public class ToWebModel {
            [Fact]
            public void GoodDetail_ReturnsExpectedActive() {
                // arrange
                NotesListDetail detail = MakeDetail();
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
                NotesListDetail detail = MakeDetail();
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
                NotesListDetail detail = MakeDetail();
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
                NotesListDetail detail = MakeDetail();
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
                NotesListDetail detail = MakeDetail();
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
                NotesListDetail detail = MakeDetail();
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
                NotesListDetail detail = MakeDetail();
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
                NotesListDetail detail = MakeDetail();
                DateTime expected = new DateTime(2017, 7, 20, 10, 45, 0, DateTimeKind.Utc);

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.ModifiedUtc
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedNotes() {
                // arrange
                NotesListDetail detail = MakeDetail();
                string expected = "Fake Note";

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Notes
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodDetail_ReturnsExpectedPosition() {
                // arrange
                NotesListDetail detail = MakeDetail();
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
                NotesListDetail detail = MakeDetail();
                ListType expected = ListType.Notes;

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
                NotesListDetail detail = MakeDetail();
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
                NotesListDetail detail = MakeDetail();

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
                NotesListDetail detail = MakeDetail();

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
                NotesListDetail detail = MakeDetail();

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
                NotesListDetail detail = MakeDetail();

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
                NotesListDetail detail = MakeDetail();

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
                NotesListDetail detail = MakeDetail();

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
                NotesListDetail detail = MakeDetail();

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
                NotesListDetail detail = MakeDetail();

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
                NotesListDetail detail = MakeDetail();

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
                NotesListDetail detail = MakeDetail();

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
                NotesListDetail detail = MakeDetail();
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
                NotesListDetail detail = MakeDetail();
                int expected = 0;

                // act
                ListItemModel results = detail.ToWebModel();

                // assert
                results.Quantity
                       .Should()
                       .Be(expected);
            }
        }

        public class ToListModel {
            [Fact]
            public void GoodModel_ReturnsExpectedCatalogId() {
                // arrange
                string expected = "FUT";
                int headerId = 1;
                ListItemModel model = MakeModel();

                // act
                NotesListDetail results = model.ToListModel();

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
                NotesListDetail results = model.ToListModel();

                // assert
                results.Each
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
                NotesListDetail results = model.ToListModel();

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
                NotesListDetail results = model.ToListModel();

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
                NotesListDetail results = model.ToListModel();

                // assert
                results.LineNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModel_ReturnsExpectedNote() {
                // arrange
                string expected = "Fake Note";
                int headerId = 1;
                ListItemModel model = MakeModel();

                // act
                NotesListDetail results = model.ToListModel();

                // assert
                results.Note
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModelWithHeaderId_ReturnsExpectedHeaderId() {
                // arrange
                int expected = 1;
                int headerId = 1;
                ListItemModel model = MakeModel();

                // act
                NotesListDetail results = model.ToListModel(headerId);

                // assert
                results.HeaderId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodModelWithoutHeaderId_ReturnsDefaultedHeaderId() {
                // arrange
                int expected = 0;
                int headerId = 1;
                ListItemModel model = MakeModel();

                // act
                NotesListDetail results = model.ToListModel();

                // assert
                results.HeaderId
                       .Should()
                       .Be(expected);
            }
        }
    }
}