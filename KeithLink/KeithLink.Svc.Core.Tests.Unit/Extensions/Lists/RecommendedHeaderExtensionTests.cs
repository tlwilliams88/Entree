using System;
using System.Collections.Generic;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.RecommendedItems;

namespace KeithLink.Svc.Core.Tests.Unit.Extensions.Lists {
    public class RecommendedHeaderExtensionTests {
        private static RecommendedItemsListHeader MakeHeader() {
            return new RecommendedItemsListHeader {
                BranchId = "FUT",
                CustomerNumber = "123456",
                CreatedUtc = new DateTime(2017, 7, 6, 14, 37, 0, DateTimeKind.Utc),
                Id = 15,
                ModifiedUtc = new DateTime(2017, 7, 6, 14, 38, 0, DateTimeKind.Utc)
            };
        }

        private static List<RecommendedItemsListDetail> MakeListOfDetails() {
            return new List<RecommendedItemsListDetail> {
                new RecommendedItemsListDetail {
                    Id = 1,
                    CatalogId = "FUT",
                    Each = false,
                    HeaderId = 15,
                    ItemNumber = "123456",
                    LineNumber = 1,
                    CreatedUtc = new DateTime(2017, 7, 6, 15, 9, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 7, 6, 15, 10, 0, DateTimeKind.Utc)
                },
                new RecommendedItemsListDetail {
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
            public void GoodHeader_ItemsHasZeroCount() {
                // arrange
                int expected = 0;
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.Items
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ItemsIsNotNull() {
                // arrange
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.Items
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedBranchId() {
                // arrange
                string expected = "FUT";
                RecommendedItemsListHeader header = MakeHeader();

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
                string expected = "123456";
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedHasContractItems() {
                // arrange
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.HasContractItems
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsContractList() {
                // arrange
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();

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
                RecommendedItemsListHeader header = MakeHeader();

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
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();

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
                RecommendedItemsListHeader header = MakeHeader();

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
                bool expected = true;
                RecommendedItemsListHeader header = MakeHeader();

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
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.IsReminder
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsShared() {
                // arrange
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.IsShared
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsSharing() {
                // arrange
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsWorksheet() {
                // arrange
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();

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
                int expected = 15;
                RecommendedItemsListHeader header = MakeHeader();

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
                string expected = "Recommended";
                RecommendedItemsListHeader header = MakeHeader();

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
                bool expected = true;
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedSharedWithCount() {
                // arrange
                int expected = 0;
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.SharedWith
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedType() {
                // arrange
                ListType expected = ListType.RecommendedItems;
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_SharedWithIsNotNull() {
                // arrange
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel();

                // assert
                results.SharedWith
                       .Should()
                       .NotBeNull();
            }
        }

        public class ToListModel_WithListOfDetails {
            [Fact]
            public void GoodHeader_ItemsHasExpectedCount() {
                // arrange
                List<RecommendedItemsListDetail> details = MakeListOfDetails();
                int expected = 2;
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.Items
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ItemsIsNotNull() {
                // arrange
                List<RecommendedItemsListDetail> details = MakeListOfDetails();
                RecommendedItemsListHeader header = MakeHeader();

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
                List<RecommendedItemsListDetail> details = MakeListOfDetails();
                string expected = "FUT";
                RecommendedItemsListHeader header = MakeHeader();

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
                List<RecommendedItemsListDetail> details = MakeListOfDetails();
                string expected = "123456";
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedHasContractItems() {
                // arrange
                List<RecommendedItemsListDetail> details = MakeListOfDetails();
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.HasContractItems
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsContractList() {
                // arrange
                List<RecommendedItemsListDetail> details = MakeListOfDetails();
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();

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
                List<RecommendedItemsListDetail> details = MakeListOfDetails();
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();

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
                List<RecommendedItemsListDetail> details = MakeListOfDetails();
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();

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
                List<RecommendedItemsListDetail> details = MakeListOfDetails();
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();

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
                List<RecommendedItemsListDetail> details = MakeListOfDetails();
                bool expected = true;
                RecommendedItemsListHeader header = MakeHeader();

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
                List<RecommendedItemsListDetail> details = MakeListOfDetails();
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.IsReminder
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsShared() {
                // arrange
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();
                List<RecommendedItemsListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.IsShared
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsSharing() {
                // arrange
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();
                List<RecommendedItemsListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsWorksheet() {
                // arrange
                List<RecommendedItemsListDetail> details = MakeListOfDetails();
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();

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
                List<RecommendedItemsListDetail> details = MakeListOfDetails();
                int expected = 15;
                RecommendedItemsListHeader header = MakeHeader();

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
                List<RecommendedItemsListDetail> details = MakeListOfDetails();
                string expected = "Recommended";
                RecommendedItemsListHeader header = MakeHeader();

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
                List<RecommendedItemsListDetail> details = MakeListOfDetails();
                bool expected = true;
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedSharedWithCount() {
                // arrange
                int expected = 0;
                RecommendedItemsListHeader header = MakeHeader();
                List<RecommendedItemsListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.SharedWith
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedType() {
                // arrange
                List<RecommendedItemsListDetail> details = MakeListOfDetails();
                ListType expected = ListType.RecommendedItems;
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_SharedWithIsNotNull() {
                // arrange
                RecommendedItemsListHeader header = MakeHeader();
                List<RecommendedItemsListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.SharedWith
                       .Should()
                       .NotBeNull();
            }
        }

        public class ToListModel_WithListOfModels {
            [Fact]
            public void GoodHeader_ItemsHasExpectedCount() {
                // arrange
                List<ListItemModel> details = MakeListOfModels();
                int expected = 2;
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.Items
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ItemsIsNotNull() {
                // arrange
                List<ListItemModel> details = MakeListOfModels();
                RecommendedItemsListHeader header = MakeHeader();

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
                List<ListItemModel> details = MakeListOfModels();
                string expected = "FUT";
                RecommendedItemsListHeader header = MakeHeader();

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
                List<ListItemModel> details = MakeListOfModels();
                string expected = "123456";
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedHasContractItems() {
                // arrange
                List<ListItemModel> details = MakeListOfModels();
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.HasContractItems
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsContractList() {
                // arrange
                List<ListItemModel> details = MakeListOfModels();
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();

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
                List<ListItemModel> details = MakeListOfModels();
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();

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
                List<ListItemModel> details = MakeListOfModels();
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();

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
                List<ListItemModel> details = MakeListOfModels();
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();

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
                List<ListItemModel> details = MakeListOfModels();
                bool expected = true;
                RecommendedItemsListHeader header = MakeHeader();

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
                List<ListItemModel> details = MakeListOfModels();
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.IsReminder
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsShared() {
                // arrange
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();
                List<ListItemModel> details = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.IsShared
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsSharing() {
                // arrange
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();
                List<ListItemModel> details = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsWorksheet() {
                // arrange
                List<ListItemModel> details = MakeListOfModels();
                bool expected = false;
                RecommendedItemsListHeader header = MakeHeader();

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
                List<ListItemModel> details = MakeListOfModels();
                int expected = 15;
                RecommendedItemsListHeader header = MakeHeader();

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
                List<ListItemModel> details = MakeListOfModels();
                string expected = "Recommended";
                RecommendedItemsListHeader header = MakeHeader();

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
                List<ListItemModel> details = MakeListOfModels();
                bool expected = true;
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedSharedWithCount() {
                // arrange
                int expected = 0;
                RecommendedItemsListHeader header = MakeHeader();
                List<ListItemModel> details = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.SharedWith
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedType() {
                // arrange
                List<ListItemModel> details = MakeListOfModels();
                ListType expected = ListType.RecommendedItems;
                RecommendedItemsListHeader header = MakeHeader();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_SharedWithIsNotNull() {
                // arrange
                RecommendedItemsListHeader header = MakeHeader();
                List<ListItemModel> details = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(details);

                // assert
                results.SharedWith
                       .Should()
                       .NotBeNull();
            }
        }
    }
}