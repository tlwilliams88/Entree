using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Favorites;

namespace KeithLink.Svc.Core.Tests.Unit.Extensions.Lists {
    public class FavoritesHeaderExtensionsTests {
        private static FavoritesListHeader MakeHeader() {
            return new FavoritesListHeader {
                Id = 21,
                BranchId = "BYE",
                CustomerNumber = "SEWRLD",
                CreatedUtc = new DateTime(2017, 7, 5, 16, 41, 0, DateTimeKind.Utc),
                ModifiedUtc = new DateTime(2017, 7, 5, 16, 42, 0, DateTimeKind.Utc)
            };
        }

        private static List<FavoritesListDetail> MakeListOfDetails() {
            return new List<FavoritesListDetail> {
                new FavoritesListDetail {
                    Id = 1,
                    CatalogId = "FUT",
                    Each = false,
                    HeaderId = 15,
                    ItemNumber = "123456",
                    LineNumber = 1,
                    CreatedUtc = new DateTime(2017, 7, 6, 15, 9, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 7, 6, 15, 10, 0, DateTimeKind.Utc)
                },
                new FavoritesListDetail {
                    Id = 2,
                    CatalogId = "FUT",
                    Each = false,
                    HeaderId = 15,
                    ItemNumber = "234567",
                    LineNumber = 2,
                    CreatedUtc = new DateTime(2017, 7, 6, 15, 9, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 7, 6, 15, 10, 0, DateTimeKind.Utc)
                }
            };
        }

        private static List<ListItemModel> MakeListOfModels() {
            return new List<ListItemModel> {
                new ListItemModel {
                    ListItemId = 1,
                    CatalogId = "FUT",
                    Category = "Fake Category",
                    Each = false,
                    ItemNumber = "123456",
                    Position = 1,
                    CreatedUtc = new DateTime(2017, 7, 6, 15, 9, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 7, 6, 15, 10, 0, DateTimeKind.Utc)
                },
                new ListItemModel {
                    ListItemId = 2,
                    CatalogId = "FUT",
                    Category = "Fake Category",
                    Each = false,
                    ItemNumber = "234567",
                    Position = 2,
                    CreatedUtc = new DateTime(2017, 7, 6, 15, 9, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 7, 6, 15, 10, 0, DateTimeKind.Utc)
                }
            };
        }

        public class ToListModel {
            [Fact]
            public void GoodHeader_ItemsIsNull() {
                // arrange
                FavoritesListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.Items
                       .Should()
                       .BeNull();
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedBranchId() {
                // arrange
                string expected = "BYE";
                FavoritesListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedCustomerNumber() {
                // arrange
                FavoritesListHeader header = MakeHeader();
                string expected = "SEWRLD";

                // act
                ListModel results = header.ToListModel();

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsContractList() {
                // arrange
                bool expected = false;
                FavoritesListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.IsContractList
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsCustomInventory() {
                // arrange
                bool expected = false;
                FavoritesListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.IsCustomInventory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsFavoriteList() {
                // arrange
                bool expected = true;
                FavoritesListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.IsFavorite
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsMandatory() {
                // arrange
                bool expected = false;
                FavoritesListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.IsMandatory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsRecommended() {
                // arrange
                bool expected = false;
                FavoritesListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.IsRecommended
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsReminder() {
                // arrange
                bool expected = false;
                FavoritesListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.IsReminder
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsWorksheet() {
                // arrange
                bool expected = false;
                FavoritesListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.IsWorksheet
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedListId() {
                // arrange
                FavoritesListHeader header = MakeHeader();
                int expected = 21;

                // act
                ListModel results = header.ToListModel();

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedName() {
                // arrange
                string expected = "Favorites";
                FavoritesListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedReadonly() {
                // arrange
                bool expected = false;
                FavoritesListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedType() {
                // arrange
                FavoritesListHeader header = MakeHeader();
                ListType expected = ListType.Favorite;

                // act
                ListModel results = header.ToListModel();

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }
        }

        public class ToListModel_WithListOfDetails {
            [Fact]
            public void GoodHeader_ItemsIsNotNull() {
                // arrange
                FavoritesListHeader header = MakeHeader();
                List<FavoritesListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.Items
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedBranchId() {
                // arrange
                string expected = "BYE";
                FavoritesListHeader header = MakeHeader();
                List<FavoritesListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedCustomerNumber() {
                // arrange
                FavoritesListHeader header = MakeHeader();
                string expected = "SEWRLD";
                List<FavoritesListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsContractList() {
                // arrange
                bool expected = false;
                FavoritesListHeader header = MakeHeader();
                List<FavoritesListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.IsContractList
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsCustomInventory() {
                // arrange
                bool expected = false;
                FavoritesListHeader header = MakeHeader();
                List<FavoritesListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.IsCustomInventory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsFavoriteList() {
                // arrange
                bool expected = true;
                FavoritesListHeader header = MakeHeader();
                List<FavoritesListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.IsFavorite
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsMandatory() {
                // arrange
                bool expected = false;
                FavoritesListHeader header = MakeHeader();
                List<FavoritesListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.IsMandatory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsRecommended() {
                // arrange
                bool expected = false;
                FavoritesListHeader header = MakeHeader();
                List<FavoritesListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.IsRecommended
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsReminder() {
                // arrange
                bool expected = false;
                FavoritesListHeader header = MakeHeader();
                List<FavoritesListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.IsReminder
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsWorksheet() {
                // arrange
                bool expected = false;
                FavoritesListHeader header = MakeHeader();
                List<FavoritesListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.IsWorksheet
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedListId() {
                // arrange
                FavoritesListHeader header = MakeHeader();
                int expected = 21;
                List<FavoritesListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedName() {
                // arrange
                string expected = "Favorites";
                FavoritesListHeader header = MakeHeader();
                List<FavoritesListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedReadonly() {
                // arrange
                bool expected = false;
                FavoritesListHeader header = MakeHeader();
                List<FavoritesListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedType() {
                // arrange
                FavoritesListHeader header = MakeHeader();
                ListType expected = ListType.Favorite;
                List<FavoritesListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_TwoItems() {
                // arrange
                int expected = 2;
                FavoritesListHeader header = MakeHeader();
                List<FavoritesListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.Items
                       .Count()
                       .Should()
                       .Be(expected);
            }
        }

        public class ToListModel_WithListOfModels {
            [Fact]
            public void GoodHeader_ItemsIsNotNull() {
                // arrange
                FavoritesListHeader header = MakeHeader();
                List<ListItemModel> items = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(items);

                // assert
                results.Items
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedBranchId() {
                // arrange
                string expected = "BYE";
                FavoritesListHeader header = MakeHeader();
                List<ListItemModel> items = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(items);

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedCustomerNumber() {
                // arrange
                FavoritesListHeader header = MakeHeader();
                string expected = "SEWRLD";
                List<ListItemModel> items = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(items);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsContractList() {
                // arrange
                bool expected = false;
                FavoritesListHeader header = MakeHeader();
                List<ListItemModel> items = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(items);

                // assert
                results.IsContractList
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsCustomInventory() {
                // arrange
                bool expected = false;
                FavoritesListHeader header = MakeHeader();
                List<ListItemModel> items = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(items);

                // assert
                results.IsCustomInventory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsFavoriteList() {
                // arrange
                bool expected = true;
                FavoritesListHeader header = MakeHeader();
                List<ListItemModel> items = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(items);

                // assert
                results.IsFavorite
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsMandatory() {
                // arrange
                bool expected = false;
                FavoritesListHeader header = MakeHeader();
                List<ListItemModel> items = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(items);

                // assert
                results.IsMandatory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsRecommended() {
                // arrange
                bool expected = false;
                FavoritesListHeader header = MakeHeader();
                List<ListItemModel> items = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(items);

                // assert
                results.IsRecommended
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsReminder() {
                // arrange
                bool expected = false;
                FavoritesListHeader header = MakeHeader();
                List<ListItemModel> items = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(items);

                // assert
                results.IsReminder
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsWorksheet() {
                // arrange
                bool expected = false;
                FavoritesListHeader header = MakeHeader();
                List<ListItemModel> items = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(items);

                // assert
                results.IsWorksheet
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedListId() {
                // arrange
                FavoritesListHeader header = MakeHeader();
                int expected = 21;
                List<ListItemModel> items = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(items);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedName() {
                // arrange
                string expected = "Favorites";
                FavoritesListHeader header = MakeHeader();
                List<ListItemModel> items = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(items);

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedReadonly() {
                // arrange
                bool expected = false;
                FavoritesListHeader header = MakeHeader();
                List<ListItemModel> items = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(items);

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedType() {
                // arrange
                FavoritesListHeader header = MakeHeader();
                ListType expected = ListType.Favorite;
                List<ListItemModel> items = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(items);

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_TwoItems() {
                // arrange
                int expected = 2;
                FavoritesListHeader header = MakeHeader();
                List<ListItemModel> items = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(items);

                // assert
                results.Items
                       .Count()
                       .Should()
                       .Be(expected);
            }
        }
    }
}