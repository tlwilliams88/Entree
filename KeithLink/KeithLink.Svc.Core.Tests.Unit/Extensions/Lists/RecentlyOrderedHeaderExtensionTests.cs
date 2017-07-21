using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.RecentlyOrdered;
using Xunit;

namespace KeithLink.Svc.Core.Tests.Unit.Extensions.Lists {
    public class RecentlyOrderedHeaderExtensionTests {
        private static RecentlyOrderedListHeader MakeHeader() {
            return new RecentlyOrderedListHeader() {
                                                       BranchId = "FUT",
                                                       CustomerNumber = "123456",
                                                       CreatedUtc = new DateTime(2017, 7, 6, 14, 37, 0, DateTimeKind.Utc),
                                                       Id = 15,
                                                       ModifiedUtc = new DateTime(2017, 7, 6, 14, 38, 0, DateTimeKind.Utc)
                                                   };
        }

        private static List<RecentlyOrderedListDetail> MakeListOfDetails() {
            return new List<RecentlyOrderedListDetail>() {
                                                             new RecentlyOrderedListDetail() {
                                                                                                 Id = 1,
                                                                                                 CatalogId = "FUT",
                                                                                                 Each = false,
                                                                                                 HeaderId = 15,
                                                                                                 ItemNumber = "123456",
                                                                                                 LineNumber = 1,
                                                                                                 CreatedUtc = new DateTime(2017, 7, 6, 15, 9, 0, DateTimeKind.Utc),
                                                                                                 ModifiedUtc = new DateTime(2017, 7, 6, 15, 10, 0, DateTimeKind.Utc)
                                                                                             },
                                                             new RecentlyOrderedListDetail() {
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
                                                                         Position = 1,
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

        public class ToListModel {
            [Fact]
            public void GoodHeader_ReturnsExpectedBranchId() {
                // arrange
                var expected = "FUT";
                var header = MakeHeader();

                // act
                var results = header.ToListModel();

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

                // act
                var results = header.ToListModel();

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

                // act
                var results = header.ToListModel();

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

                // act
                var results = header.ToListModel();

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

                // act
                var results = header.ToListModel();

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

                // act
                var results = header.ToListModel();

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

                // act
                var results = header.ToListModel();

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

                // act
                var results = header.ToListModel();

                // assert
                results.IsCustomInventory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedType() {
                // arrange
                var expected = ListType.RecentlyOrdered;
                var header = MakeHeader();

                // act
                var results = header.ToListModel();

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

                // act
                var results = header.ToListModel();

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedName() {
                // arrange
                var expected = "Recently Ordered";
                var header = MakeHeader();

                // act
                var results = header.ToListModel();

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedReadonly() {
                // arrange
                var expected = true;
                var header = MakeHeader();

                // act
                var results = header.ToListModel();

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ItemsIsNotNull() {
                // arrange
                var header = MakeHeader();

                // act
                var results = header.ToListModel();

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

                // act
                var results = header.ToListModel();

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

                // act
                var results = header.ToListModel();

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

                // act
                var results = header.ToListModel();

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

                // act
                var results = header.ToListModel();

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

                // act
                var results = header.ToListModel();

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

                // act
                var results = header.ToListModel();

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

                // act
                var results = header.ToListModel();

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

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

                // assert
                results.IsCustomInventory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedType() {
                // arrange
                var details = MakeListOfDetails();
                var expected = ListType.RecentlyOrdered;
                var header = MakeHeader();

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedName() {
                // arrange
                var details = MakeListOfDetails();
                var expected = "Recently Ordered";
                var header = MakeHeader();

                // act
                var results = header.ToListModel(details);

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedReadonly() {
                // arrange
                var details = MakeListOfDetails();
                var expected = true;
                var header = MakeHeader();

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

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
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(details);

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
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(details);

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
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(details);

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
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

                // assert
                results.IsCustomInventory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedType() {
                // arrange
                var details = MakeListOfModels();
                var expected = ListType.RecentlyOrdered;
                var header = MakeHeader();

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedName() {
                // arrange
                var details = MakeListOfModels();
                var expected = "Recently Ordered";
                var header = MakeHeader();

                // act
                var results = header.ToListModel(details);

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedReadonly() {
                // arrange
                var details = MakeListOfModels();
                var expected = true;
                var header = MakeHeader();

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

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

                // act
                var results = header.ToListModel(details);

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
                var details = MakeListOfModels();

                // act
                var results = header.ToListModel(details);

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
                var details = MakeListOfModels();

                // act
                var results = header.ToListModel(details);

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
                var details = MakeListOfModels();

                // act
                var results = header.ToListModel(details);

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
                var details = MakeListOfModels();

                // act
                var results = header.ToListModel(details);

                // assert
                results.IsSharing
                       .Should()
                       .Be(expected);
            }
        }
    }
}
