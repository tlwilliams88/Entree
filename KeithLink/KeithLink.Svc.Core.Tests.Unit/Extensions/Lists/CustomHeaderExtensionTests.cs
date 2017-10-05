using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.CustomList;
using KeithLink.Svc.Core.Models.Lists.CustomListShares;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Tests.Unit.Extensions.Lists {
    public class CustomHeaderExtensionTests {
        private static UserSelectedContext MakeCustomer() {
            return new UserSelectedContext {
                BranchId = "FUT",
                CustomerId = "123456"
            };
        }

        private static CustomListHeader MakeHeader() {
            return new CustomListHeader {
                BranchId = "FUT",
                CustomerNumber = "123456",
                Name = "Fake List Name",
                CreatedUtc = new DateTime(2017, 7, 6, 14, 37, 0, DateTimeKind.Utc),
                Id = 15,
                ModifiedUtc = new DateTime(2017, 7, 6, 14, 38, 0, DateTimeKind.Utc)
            };
        }

        private static List<CustomListDetail> MakeListOfDetails() {
            return new List<CustomListDetail> {
                new CustomListDetail {
                    Id = 1,
                    CatalogId = "FUT",
                    Each = false,
                    HeaderId = 15,
                    ItemNumber = "123456",
                    LineNumber = 1,
                    CreatedUtc = new DateTime(2017, 7, 6, 15, 9, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 7, 6, 15, 10, 0, DateTimeKind.Utc)
                },
                new CustomListDetail {
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

        private static List<CustomListShare> MakeShares() {
            return new List<CustomListShare> {
                new CustomListShare {
                    BranchId = "FDF",
                    CustomerNumber = "234567"
                },
                new CustomListShare {
                    BranchId = "FDF",
                    CustomerNumber = "345678"
                }
            };
        }

        public class ToListModel {
            [Fact]
            public void GoodHeader_ItemsHasZeroCount() {
                // arrange
                int expected = 0;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.Items
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ItemsIsNotNull() {
                // arrange
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.Items
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedBranchId() {
                // arrange
                string expected = "FUT";
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedCustomerNumber() {
                // arrange
                string expected = "123456";
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedHasContractItems() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.HasContractItems
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsContractList() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.IsContractList
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsCustomInventory() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.IsCustomInventory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsFavoriteList() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.IsFavorite
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsMandatory() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.IsMandatory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsRecommended() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.IsRecommended
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsReminder() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.IsReminder
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsShared() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.IsShared
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsSharing() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsWorksheet() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.IsWorksheet
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedListId() {
                // arrange
                int expected = 15;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedName() {
                // arrange
                string expected = "Fake List Name";
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedReadonly() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedSharedWithCount() {
                // arrange
                int expected = 0;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.SharedWith
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedType() {
                // arrange
                ListType expected = ListType.Custom;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_SharedWithIsNotNull() {
                // arrange
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.SharedWith
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeaderWithDifferentCustomer_ReturnsIsSharedIsTrue() {
                // arrange
                bool expected = true;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.IsShared
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithDifferentCustomer_ReturnsReadOnlyIsTrue() {
                // arrange
                bool expected = true;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };

                // act
                ListModel results = header.ToListModel(customer, null);

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithEmptySharedList_IsSharingReturnsFalse() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<CustomListShare> shares = new List<CustomListShare>();

                // act
                ListModel results = header.ToListModel(customer, shares);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedList_ReturnsSharedWithHasExpectedCount() {
                // arrange
                int expected = 2;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<CustomListShare> shares = MakeShares();

                // act
                ListModel results = header.ToListModel(customer, shares);

                // assert
                results.SharedWith
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedList_ReturnsSharedWithIsNotNull() {
                // arrange
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<CustomListShare> shares = MakeShares();

                // act
                ListModel results = header.ToListModel(customer, shares);

                // assert
                results.SharedWith
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeaderWithSharedList_SharedWithContainsExpectedCustomerNumber() {
                // arrange
                string expected = "234567";
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<CustomListShare> shares = MakeShares();

                // act
                ListModel results = header.ToListModel(customer, shares);

                // assert
                results.SharedWith
                       .First()
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndBadBranch_IsSharingReturnsFalse() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                List<CustomListShare> shares = MakeShares();

                // act
                ListModel results = header.ToListModel(customer, shares);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndBadCustomer_IsSharingReturnsFalse() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "999999"
                };
                List<CustomListShare> shares = MakeShares();

                // act
                ListModel results = header.ToListModel(customer, shares);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndGoodCustomerAndBranch_IsSharingReturnsTrue() {
                // arrange
                bool expected = true;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<CustomListShare> shares = MakeShares();

                // act
                ListModel results = header.ToListModel(customer, shares);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndLowerCaseBranch_IsSharingReturnsTrue() {
                // arrange
                bool expected = true;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "fut",
                    CustomerId = "123456"
                };
                List<CustomListShare> shares = MakeShares();

                // act
                ListModel results = header.ToListModel(customer, shares);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }
        }

        public class ToListModel_WithListOfDetails {
            [Fact]
            public void GoodHeader_ItemsHasExpectedCount() {
                // arrange
                List<CustomListDetail> details = MakeListOfDetails();
                int expected = 2;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.Items
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ItemsIsNotNull() {
                // arrange
                List<CustomListDetail> details = MakeListOfDetails();
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.Items
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedBranchId() {
                // arrange
                List<CustomListDetail> details = MakeListOfDetails();
                string expected = "FUT";
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedCustomerNumber() {
                // arrange
                List<CustomListDetail> details = MakeListOfDetails();
                string expected = "123456";
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedHasContractItems() {
                // arrange
                List<CustomListDetail> details = MakeListOfDetails();
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.HasContractItems
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsContractList() {
                // arrange
                List<CustomListDetail> details = MakeListOfDetails();
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.IsContractList
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsCustomInventory() {
                // arrange
                List<CustomListDetail> details = MakeListOfDetails();
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.IsCustomInventory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsFavoriteList() {
                // arrange
                List<CustomListDetail> details = MakeListOfDetails();
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.IsFavorite
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsMandatory() {
                // arrange
                List<CustomListDetail> details = MakeListOfDetails();
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.IsMandatory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsRecommended() {
                // arrange
                List<CustomListDetail> details = MakeListOfDetails();
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.IsRecommended
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsReminder() {
                // arrange
                List<CustomListDetail> details = MakeListOfDetails();
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.IsReminder
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsShared() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<CustomListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.IsShared
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsSharing() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<CustomListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsWorksheet() {
                // arrange
                List<CustomListDetail> details = MakeListOfDetails();
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.IsWorksheet
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedListId() {
                // arrange
                List<CustomListDetail> details = MakeListOfDetails();
                int expected = 15;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedName() {
                // arrange
                List<CustomListDetail> details = MakeListOfDetails();
                string expected = "Fake List Name";
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedReadonly() {
                // arrange
                List<CustomListDetail> details = MakeListOfDetails();
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedSharedWithCount() {
                // arrange
                int expected = 0;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<CustomListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.SharedWith
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedType() {
                // arrange
                List<CustomListDetail> details = MakeListOfDetails();
                ListType expected = ListType.Custom;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_SharedWithIsNotNull() {
                // arrange
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<CustomListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.SharedWith
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeaderWithDifferentCustomer_ReturnsIsSharedIsTrue() {
                // arrange
                bool expected = true;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                List<CustomListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.IsShared
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithDifferentCustomer_ReturnsReadOnlyIsTrue() {
                // arrange
                bool expected = true;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                List<CustomListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithEmptySharedList_IsSharingReturnsFalse() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<CustomListShare> shares = new List<CustomListShare>();
                List<CustomListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(customer, shares, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedList_ReturnsSharedWithHasExpectedCount() {
                // arrange
                int expected = 2;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<CustomListShare> shares = MakeShares();
                List<CustomListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(customer, shares, details);

                // assert
                results.SharedWith
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedList_ReturnsSharedWithIsNotNull() {
                // arrange
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<CustomListShare> shares = MakeShares();
                List<CustomListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(customer, shares, details);

                // assert
                results.SharedWith
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeaderWithSharedList_SharedWithContainsExpectedCustomerNumber() {
                // arrange
                string expected = "234567";
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<CustomListShare> shares = MakeShares();
                List<CustomListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(customer, shares, details);

                // assert
                results.SharedWith
                       .First()
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndBadBranch_IsSharingReturnsFalse() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                List<CustomListShare> shares = MakeShares();
                List<CustomListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(customer, shares, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndBadCustomer_IsSharingReturnsFalse() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "999999"
                };
                List<CustomListShare> shares = MakeShares();
                List<CustomListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(customer, shares, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndGoodCustomerAndBranch_IsSharingReturnsTrue() {
                // arrange
                bool expected = true;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<CustomListShare> shares = MakeShares();
                List<CustomListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(customer, shares, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndLowerCaseBranch_IsSharingReturnsTrue() {
                // arrange
                bool expected = true;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "fut",
                    CustomerId = "123456"
                };
                List<CustomListShare> shares = MakeShares();
                List<CustomListDetail> details = MakeListOfDetails();

                // act
                ListModel results = header.ToListModel(customer, shares, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }
        }

        public class ToListModel_WithListOfModels {
            [Fact]
            public void GoodHeader_ItemsHasExpectedCount() {
                // arrange
                List<ListItemModel> details = MakeListOfModels();
                int expected = 2;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

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
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

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
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

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
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

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
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

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
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

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
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

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
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

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
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.IsMandatory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsRecommended() {
                // arrange
                List<ListItemModel> details = MakeListOfModels();
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

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
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.IsReminder
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsShared() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<ListItemModel> details = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.IsShared
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsSharing() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<ListItemModel> details = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(customer, null, details);

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
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

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
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedName() {
                // arrange
                List<ListItemModel> details = MakeListOfModels();
                string expected = "Fake List Name";
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedReadonly() {
                // arrange
                List<ListItemModel> details = MakeListOfModels();
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedSharedWithCount() {
                // arrange
                int expected = 0;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<ListItemModel> details = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(customer, null, details);

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
                ListType expected = ListType.Custom;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_SharedWithIsNotNull() {
                // arrange
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<ListItemModel> details = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.SharedWith
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeaderWithDifferentCustomer_ReturnsIsSharedIsTrue() {
                // arrange
                bool expected = true;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                List<ListItemModel> details = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.IsShared
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithDifferentCustomer_ReturnsReadOnlyIsTrue() {
                // arrange
                bool expected = true;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                List<ListItemModel> details = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(customer, null, details);

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithEmptySharedList_IsSharingReturnsFalse() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<CustomListShare> shares = new List<CustomListShare>();
                List<ListItemModel> details = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(customer, shares, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedList_ReturnsSharedWithHasExpectedCount() {
                // arrange
                int expected = 2;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<CustomListShare> shares = MakeShares();
                List<ListItemModel> details = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(customer, shares, details);

                // assert
                results.SharedWith
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedList_ReturnsSharedWithIsNotNull() {
                // arrange
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<CustomListShare> shares = MakeShares();
                List<ListItemModel> details = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(customer, shares, details);

                // assert
                results.SharedWith
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeaderWithSharedList_SharedWithContainsExpectedCustomerNumber() {
                // arrange
                string expected = "234567";
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<CustomListShare> shares = MakeShares();
                List<ListItemModel> details = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(customer, shares, details);

                // assert
                results.SharedWith
                       .First()
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndBadBranch_IsSharingReturnsFalse() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                List<CustomListShare> shares = MakeShares();
                List<ListItemModel> details = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(customer, shares, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndBadCustomer_IsSharingReturnsFalse() {
                // arrange
                bool expected = false;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "999999"
                };
                List<CustomListShare> shares = MakeShares();
                List<ListItemModel> details = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(customer, shares, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndGoodCustomerAndBranch_IsSharingReturnsTrue() {
                // arrange
                bool expected = true;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = MakeCustomer();
                List<CustomListShare> shares = MakeShares();
                List<ListItemModel> details = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(customer, shares, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndLowerCaseBranch_IsSharingReturnsTrue() {
                // arrange
                bool expected = true;
                CustomListHeader header = MakeHeader();
                UserSelectedContext customer = new UserSelectedContext {
                    BranchId = "fut",
                    CustomerId = "123456"
                };
                List<CustomListShare> shares = MakeShares();
                List<ListItemModel> details = MakeListOfModels();

                // act
                ListModel results = header.ToListModel(customer, shares, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }
        }
    }
}