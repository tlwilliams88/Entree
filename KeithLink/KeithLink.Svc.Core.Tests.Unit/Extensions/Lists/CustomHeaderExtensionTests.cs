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
            return new UserSelectedContext() {
                BranchId = "FUT",
                CustomerId = "123456"
            };
        }

        private static CustomListHeader MakeHeader() {
            return new CustomListHeader() {
                BranchId = "FUT",
                CustomerNumber = "123456",
                Name = "Fake List Name",
                CreatedUtc = new DateTime(2017, 7, 6, 14, 37, 0, DateTimeKind.Utc),
                Id = 15,
                ModifiedUtc = new DateTime(2017, 7, 6, 14, 38, 0, DateTimeKind.Utc)
            };
        }

        private static List<CustomListDetail> MakeListOfDetails() {
            return new List<CustomListDetail>() {
                new CustomListDetail() {
                    Id = 1,
                    CatalogId = "FUT",
                    Each = false,
                    HeaderId = 15,
                    ItemNumber = "123456",
                    LineNumber = 1,
                    CreatedUtc = new DateTime(2017, 7, 6, 15, 9, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 7, 6, 15, 10, 0, DateTimeKind.Utc)
                },
                new CustomListDetail() {
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
            return new List<ListItemModel>() {
                new ListItemModel() {
                    ListItemId = 1,
                    CatalogId = "FUT",
                    Category = "Fake Category",
                    Each = false,
                    ItemNumber = "123456",
                    Position= 1,
                    CreatedUtc = new DateTime(2017, 7, 6, 15, 9, 0, DateTimeKind.Utc),
                    ModifiedUtc = new DateTime(2017, 7, 6, 15, 10, 0, DateTimeKind.Utc)
                },
                new ListItemModel() {
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
            return new List<CustomListShare>() {
                new CustomListShare() {
                    BranchId = "FDF",
                    CustomerNumber = "234567"
                },
                new CustomListShare() {
                    BranchId = "FDF",
                    CustomerNumber = "345678"
                }
            };
        }

        public class ToListModel {
            [Fact]
            public void GoodHeader_ReturnsExpectedBranchId() {
                // arrange
                var expected = "FUT";
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsContractList() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.IsContractList
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsFavoriteList() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.IsFavorite
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsWorksheet() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.IsWorksheet
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsReminder() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.IsReminder
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsMandatory() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.IsMandatory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsRecommended() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.IsRecommended
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsCustomInventory() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.IsCustomInventory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedType() {
                // arrange
                var expected = ListType.Custom;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedListId() {
                // arrange
                var expected = 15;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedName() {
                // arrange
                var expected = "Fake List Name";
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedReadonly() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ItemsIsNotNull() {
                // arrange
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.Items
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeader_ItemsHasZeroCount() {
                // arrange
                var expected = 0;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.Items
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedHasContractItems() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.HasContractItems
                       .Should()
                       .Be(expected);

            }

            [Fact]
            public void GoodHeader_ReturnsExpectedCustomerNumber() {
                // arrange
                var expected = "123456";
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);

            }

            [Fact]
            public void GoodHeader_ReturnsExpectedSharedWithCount() {
                // arrange
                var expected = 0;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.SharedWith
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_SharedWithIsNotNull() {
                // arrange
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.SharedWith
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsShared() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.IsShared
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsSharing() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithDifferentCustomer_ReturnsReadOnlyIsTrue() {
                // arrange
                var expected = true;
                var header = MakeHeader();
                var customer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithDifferentCustomer_ReturnsIsSharedIsTrue() {
                // arrange
                var expected = true;
                var header = MakeHeader();
                var customer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };

                // act
                var results = header.ToListModel(customer, null);

                // assert
                results.IsShared
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedList_ReturnsSharedWithHasExpectedCount() {
                // arrange
                var expected = 2;
                var header = MakeHeader();
                var customer = MakeCustomer();
                var shares = MakeShares();

                // act
                var results = header.ToListModel(customer, shares);

                // assert
                results.SharedWith
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedList_ReturnsSharedWithIsNotNull() {
                // arrange
                var header = MakeHeader();
                var customer = MakeCustomer();
                var shares = MakeShares();

                // act
                var results = header.ToListModel(customer, shares);

                // assert
                results.SharedWith
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeaderWithSharedList_SharedWithContainsExpectedCustomerNumber() {
                // arrange
                var expected = "234567";
                var header = MakeHeader();
                var customer = MakeCustomer();
                var shares = MakeShares();

                // act
                var results = header.ToListModel(customer, shares);

                // assert
                results.SharedWith
                       .First()
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndGoodCustomerAndBranch_IsSharingReturnsTrue() {
                // arrange
                var expected = true;
                var header = MakeHeader();
                var customer = MakeCustomer();
                var shares = MakeShares();

                // act
                var results = header.ToListModel(customer, shares);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndBadBranch_IsSharingReturnsFalse() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var shares = MakeShares();

                // act
                var results = header.ToListModel(customer, shares);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndLowerCaseBranch_IsSharingReturnsTrue() {
                // arrange
                var expected = true;
                var header = MakeHeader();
                var customer = new UserSelectedContext() {
                    BranchId = "fut",
                    CustomerId = "123456"
                };
                var shares = MakeShares();

                // act
                var results = header.ToListModel(customer, shares);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndBadCustomer_IsSharingReturnsFalse() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "999999"
                };
                var shares = MakeShares();

                // act
                var results = header.ToListModel(customer, shares);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithEmptySharedList_IsSharingReturnsFalse() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();
                var shares = new List<CustomListShare>();

                // act
                var results = header.ToListModel(customer, shares);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }
        }

        public class ToListModel_WithListOfDetails {
            [Fact]
            public void GoodHeader_ReturnsExpectedBranchId() {
                // arrange
                var details = MakeListOfDetails();
                var expected = "FUT";
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsContractList() {
                // arrange
                var details = MakeListOfDetails();
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.IsContractList
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsFavoriteList() {
                // arrange
                var details = MakeListOfDetails();
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.IsFavorite
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsWorksheet() {
                // arrange
                var details = MakeListOfDetails();
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.IsWorksheet
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsReminder() {
                // arrange
                var details = MakeListOfDetails();
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.IsReminder
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsMandatory() {
                // arrange
                var details = MakeListOfDetails();
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.IsMandatory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsRecommended() {
                // arrange
                var details = MakeListOfDetails();
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.IsRecommended
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsCustomInventory() {
                // arrange
                var details = MakeListOfDetails();
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.IsCustomInventory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedType() {
                // arrange
                var details = MakeListOfDetails();
                var expected = ListType.Custom;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedListId() {
                // arrange
                var details = MakeListOfDetails();
                var expected = 15;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedName() {
                // arrange
                var details = MakeListOfDetails();
                var expected = "Fake List Name";
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedReadonly() {
                // arrange
                var details = MakeListOfDetails();
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ItemsIsNotNull() {
                // arrange
                var details = MakeListOfDetails();
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.Items
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeader_ItemsHasExpectedCount() {
                // arrange
                var details = MakeListOfDetails();
                var expected = 2;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.Items
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedHasContractItems() {
                // arrange
                var details = MakeListOfDetails();
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.HasContractItems
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedCustomerNumber() {
                // arrange
                var details = MakeListOfDetails();
                var expected = "123456";
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedSharedWithCount() {
                // arrange
                var expected = 0;
                var header = MakeHeader();
                var customer = MakeCustomer();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.SharedWith
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_SharedWithIsNotNull() {
                // arrange
                var header = MakeHeader();
                var customer = MakeCustomer();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.SharedWith
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsShared() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.IsShared
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsSharing() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithDifferentCustomer_ReturnsReadOnlyIsTrue() {
                // arrange
                var expected = true;
                var header = MakeHeader();
                var customer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithDifferentCustomer_ReturnsIsSharedIsTrue() {
                // arrange
                var expected = true;
                var header = MakeHeader();
                var customer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.IsShared
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedList_ReturnsSharedWithHasExpectedCount() {
                // arrange
                var expected = 2;
                var header = MakeHeader();
                var customer = MakeCustomer();
                var shares = MakeShares();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(customer, shares, details);

                // assert
                results.SharedWith
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedList_ReturnsSharedWithIsNotNull() {
                // arrange
                var header = MakeHeader();
                var customer = MakeCustomer();
                var shares = MakeShares();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(customer, shares, details);

                // assert
                results.SharedWith
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeaderWithSharedList_SharedWithContainsExpectedCustomerNumber() {
                // arrange
                var expected = "234567";
                var header = MakeHeader();
                var customer = MakeCustomer();
                var shares = MakeShares();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(customer, shares, details);

                // assert
                results.SharedWith
                       .First()
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndGoodCustomerAndBranch_IsSharingReturnsTrue() {
                // arrange
                var expected = true;
                var header = MakeHeader();
                var customer = MakeCustomer();
                var shares = MakeShares();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(customer, shares, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndBadBranch_IsSharingReturnsFalse() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = new UserSelectedContext() {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var shares = MakeShares();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(customer, shares, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndLowerCaseBranch_IsSharingReturnsTrue() {
                // arrange
                var expected = true;
                var header = MakeHeader();
                var customer = new UserSelectedContext() {
                    BranchId = "fut",
                    CustomerId = "123456"
                };
                var shares = MakeShares();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(customer, shares, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndBadCustomer_IsSharingReturnsFalse() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = new UserSelectedContext() {
                    BranchId = "FUT",
                    CustomerId = "999999"
                };
                var shares = MakeShares();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(customer, shares, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithEmptySharedList_IsSharingReturnsFalse() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();
                var shares = new List<CustomListShare>();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(customer, shares, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }
        }

        public class ToListModel_WithListOfModels {
            [Fact]
            public void GoodHeader_ReturnsExpectedBranchId() {
                // arrange
                var details = MakeListOfModels();
                var expected = "FUT";
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsContractList() {
                // arrange
                var details = MakeListOfModels();
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.IsContractList
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsFavoriteList() {
                // arrange
                var details = MakeListOfModels();
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.IsFavorite
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsWorksheet() {
                // arrange
                var details = MakeListOfModels();
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.IsWorksheet
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsReminder() {
                // arrange
                var details = MakeListOfModels();
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.IsReminder
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsMandatory() {
                // arrange
                var details = MakeListOfModels();
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.IsMandatory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsRecommended() {
                // arrange
                var details = MakeListOfModels();
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.IsRecommended
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsCustomInventory() {
                // arrange
                var details = MakeListOfModels();
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.IsCustomInventory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedType() {
                // arrange
                var details = MakeListOfModels();
                var expected = ListType.Custom;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedListId() {
                // arrange
                var details = MakeListOfModels();
                var expected = 15;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedName() {
                // arrange
                var details = MakeListOfModels();
                var expected = "Fake List Name";
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedReadonly() {
                // arrange
                var details = MakeListOfModels();
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ItemsIsNotNull() {
                // arrange
                var details = MakeListOfModels();
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.Items
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeader_ItemsHasExpectedCount() {
                // arrange
                var details = MakeListOfModels();
                var expected = 2;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.Items
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedHasContractItems() {
                // arrange
                var details = MakeListOfModels();
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.HasContractItems
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedCustomerNumber() {
                // arrange
                var details = MakeListOfModels();
                var expected = "123456";
                var header = MakeHeader();
                var customer = MakeCustomer();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedSharedWithCount() {
                // arrange
                var expected = 0;
                var header = MakeHeader();
                var customer = MakeCustomer();
                var details = MakeListOfModels();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.SharedWith
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_SharedWithIsNotNull() {
                // arrange
                var header = MakeHeader();
                var customer = MakeCustomer();
                var details = MakeListOfModels();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.SharedWith
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsShared() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();
                var details = MakeListOfModels();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.IsShared
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsSharing() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();
                var details = MakeListOfModels();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithDifferentCustomer_ReturnsReadOnlyIsTrue() {
                // arrange
                var expected = true;
                var header = MakeHeader();
                var customer = new UserSelectedContext() {
                                                             BranchId = "FUT",
                                                             CustomerId = "234567"
                                                         };
                var details = MakeListOfModels();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithDifferentCustomer_ReturnsIsSharedIsTrue() {
                // arrange
                var expected = true;
                var header = MakeHeader();
                var customer = new UserSelectedContext() {
                                                             BranchId = "FUT",
                                                             CustomerId = "234567"
                                                         };
                var details = MakeListOfModels();

                // act
                var results = header.ToListModel(customer, null, details);

                // assert
                results.IsShared
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedList_ReturnsSharedWithHasExpectedCount() {
                // arrange
                var expected = 2;
                var header = MakeHeader();
                var customer = MakeCustomer();
                var shares = MakeShares();
                var details = MakeListOfModels();

                // act
                var results = header.ToListModel(customer, shares, details);

                // assert
                results.SharedWith
                       .Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedList_ReturnsSharedWithIsNotNull() {
                // arrange
                var header = MakeHeader();
                var customer = MakeCustomer();
                var shares = MakeShares();
                var details = MakeListOfModels();

                // act
                var results = header.ToListModel(customer, shares, details);

                // assert
                results.SharedWith
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeaderWithSharedList_SharedWithContainsExpectedCustomerNumber() {
                // arrange
                var expected = "234567";
                var header = MakeHeader();
                var customer = MakeCustomer();
                var shares = MakeShares();
                var details = MakeListOfModels();

                // act
                var results = header.ToListModel(customer, shares, details);

                // assert
                results.SharedWith
                       .First()
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndGoodCustomerAndBranch_IsSharingReturnsTrue() {
                // arrange
                var expected = true;
                var header = MakeHeader();
                var customer = MakeCustomer();
                var shares = MakeShares();
                var details = MakeListOfModels();

                // act
                var results = header.ToListModel(customer, shares, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndBadBranch_IsSharingReturnsFalse() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = new UserSelectedContext() {
                                                             BranchId = "XXX",
                                                             CustomerId = "123456"
                                                         };
                var shares = MakeShares();
                var details = MakeListOfModels();

                // act
                var results = header.ToListModel(customer, shares, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndLowerCaseBranch_IsSharingReturnsTrue() {
                // arrange
                var expected = true;
                var header = MakeHeader();
                var customer = new UserSelectedContext() {
                                                             BranchId = "fut",
                                                             CustomerId = "123456"
                                                         };
                var shares = MakeShares();
                var details = MakeListOfModels();

                // act
                var results = header.ToListModel(customer, shares, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithSharedListAndBadCustomer_IsSharingReturnsFalse() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = new UserSelectedContext() {
                                                             BranchId = "FUT",
                                                             CustomerId = "999999"
                                                         };
                var shares = MakeShares();
                var details = MakeListOfModels();

                // act
                var results = header.ToListModel(customer, shares, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeaderWithEmptySharedList_IsSharingReturnsFalse() {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var customer = MakeCustomer();
                var shares = new List<CustomListShare>();
                var details = MakeListOfModels();

                // act
                var results = header.ToListModel(customer, shares, details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }
        }
    }
}
