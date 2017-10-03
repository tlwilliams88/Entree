using System;
using System.Collections.Generic;
using System.Linq;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Favorites;

using FluentAssertions;
using Xunit;

namespace KeithLink.Svc.Core.Tests.Unit.Extensions.Lists
{
    public class FavoritesHeaderExtensionsTests
    {
        private static FavoritesListHeader MakeHeader()
        {
            return new FavoritesListHeader()
            {
                Id = 21,
                BranchId = "BYE",
                CustomerNumber = "SEWRLD",
                CreatedUtc = new DateTime(2017, 7, 5, 16, 41, 0, DateTimeKind.Utc),
                ModifiedUtc = new DateTime(2017, 7, 5, 16, 42, 0, DateTimeKind.Utc)
            };
        }

        private static List<FavoritesListDetail> MakeListOfDetails()
        {
            return new List<FavoritesListDetail>() {
                                                       new FavoritesListDetail() {
                                                                                     Id = 1,
                                                                                     CatalogId = "FUT",
                                                                                     Each = false,
                                                                                     HeaderId = 15,
                                                                                     ItemNumber = "123456",
                                                                                     LineNumber = 1,
                                                                                     CreatedUtc = new DateTime(2017, 7, 6, 15, 9, 0, DateTimeKind.Utc),
                                                                                     ModifiedUtc = new DateTime(2017, 7, 6, 15, 10, 0, DateTimeKind.Utc)
                                                                                 },
                                                       new FavoritesListDetail() {
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

        private static List<ListItemModel> MakeListOfModels()
        {
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

        public class ToListModel
        {
            [Fact]
            public void GoodHeader_ReturnsExpectedListId()
            {
                // arrange
                var header = MakeHeader();
                var expected = 21;

                // act
                var results = header.ToListModel();

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedType()
            {
                // arrange
                var header = MakeHeader();
                var expected = ListType.Favorite;

                // act
                var results = header.ToListModel();

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedBranchId()
            {
                // arrange
                var expected = "BYE";
                var header = MakeHeader();

                // act
                var results = header.ToListModel();

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedCustomerNumber()
            {
                // arrange
                var header = MakeHeader();
                var expected = "SEWRLD";

                // act
                var results = header.ToListModel();

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsContractList()
            {
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
            public void GoodHeader_ReturnsExpectedIsFavoriteList()
            {
                // arrange
                var expected = true;
                var header = MakeHeader();

                // act
                var results = header.ToListModel();

                // assert
                results.IsFavorite
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsWorksheet()
            {
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
            public void GoodHeader_ReturnsExpectedIsReminder()
            {
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
            public void GoodHeader_ReturnsExpectedIsMandatory()
            {
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
            public void GoodHeader_ReturnsExpectedIsRecommended()
            {
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
            public void GoodHeader_ReturnsExpectedIsCustomInventory()
            {
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
            public void GoodHeader_ReturnsExpectedName()
            {
                // arrange
                var expected = "Favorites";
                var header = MakeHeader();

                // act
                var results = header.ToListModel();

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedReadonly()
            {
                // arrange
                var expected = false;
                var header = MakeHeader();

                // act
                var results = header.ToListModel();

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ItemsIsNull()
            {
                // arrange
                var header = MakeHeader();

                // act
                var results = header.ToListModel();

                // assert
                results.Items
                       .Should()
                       .BeNull();
            }
        }

        public class ToListModel_WithListOfDetails
        {
            [Fact]
            public void GoodHeader_ReturnsExpectedListId()
            {
                // arrange
                var header = MakeHeader();
                var expected = 21;
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(details);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedType()
            {
                // arrange
                var header = MakeHeader();
                var expected = ListType.Favorite;
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(details);

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedBranchId()
            {
                // arrange
                var expected = "BYE";
                var header = MakeHeader();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(details);

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedCustomerNumber()
            {
                // arrange
                var header = MakeHeader();
                var expected = "SEWRLD";
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(details);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsContractList()
            {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(details);

                // assert
                results.IsContractList
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsFavoriteList()
            {
                // arrange
                var expected = true;
                var header = MakeHeader();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(details);

                // assert
                results.IsFavorite
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsWorksheet()
            {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(details);

                // assert
                results.IsWorksheet
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsReminder()
            {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(details);

                // assert
                results.IsReminder
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsMandatory()
            {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(details);

                // assert
                results.IsMandatory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsRecommended()
            {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(details);

                // assert
                results.IsRecommended
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsCustomInventory()
            {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(details);

                // assert
                results.IsCustomInventory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedName()
            {
                // arrange
                var expected = "Favorites";
                var header = MakeHeader();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(details);

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedReadonly()
            {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(details);

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ItemsIsNotNull()
            {
                // arrange
                var header = MakeHeader();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(details);

                // assert
                results.Items
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeader_TwoItems()
            {
                // arrange
                var expected = 2;
                var header = MakeHeader();
                var details = MakeListOfDetails();

                // act
                var results = header.ToListModel(details);

                // assert
                results.Items
                       .Count()
                       .Should()
                       .Be(expected);
            }
        }

        public class ToListModel_WithListOfModels
        {
            [Fact]
            public void GoodHeader_ReturnsExpectedListId()
            {
                // arrange
                var header = MakeHeader();
                var expected = 21;
                var items = MakeListOfModels();

                // act
                var results = header.ToListModel(items);

                // assert
                results.ListId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedType()
            {
                // arrange
                var header = MakeHeader();
                var expected = ListType.Favorite;
                var items = MakeListOfModels();

                // act
                var results = header.ToListModel(items);

                // assert
                results.Type
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedBranchId()
            {
                // arrange
                var expected = "BYE";
                var header = MakeHeader();
                var items = MakeListOfModels();

                // act
                var results = header.ToListModel(items);

                // assert
                results.BranchId
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedCustomerNumber()
            {
                // arrange
                var header = MakeHeader();
                var expected = "SEWRLD";
                var items = MakeListOfModels();

                // act
                var results = header.ToListModel(items);

                // assert
                results.CustomerNumber
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsContractList()
            {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var items = MakeListOfModels();

                // act
                var results = header.ToListModel(items);

                // assert
                results.IsContractList
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsFavoriteList()
            {
                // arrange
                var expected = true;
                var header = MakeHeader();
                var items = MakeListOfModels();

                // act
                var results = header.ToListModel(items);

                // assert
                results.IsFavorite
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsWorksheet()
            {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var items = MakeListOfModels();

                // act
                var results = header.ToListModel(items);

                // assert
                results.IsWorksheet
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsReminder()
            {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var items = MakeListOfModels();

                // act
                var results = header.ToListModel(items);

                // assert
                results.IsReminder
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsMandatory()
            {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var items = MakeListOfModels();

                // act
                var results = header.ToListModel(items);

                // assert
                results.IsMandatory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsRecommended()
            {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var items = MakeListOfModels();

                // act
                var results = header.ToListModel(items);

                // assert
                results.IsRecommended
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedIsCustomInventory()
            {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var items = MakeListOfModels();

                // act
                var results = header.ToListModel(items);

                // assert
                results.IsCustomInventory
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedName()
            {
                // arrange
                var expected = "Favorites";
                var header = MakeHeader();
                var items = MakeListOfModels();

                // act
                var results = header.ToListModel(items);

                // assert
                results.Name
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ReturnsExpectedReadonly()
            {
                // arrange
                var expected = false;
                var header = MakeHeader();
                var items = MakeListOfModels();

                // act
                var results = header.ToListModel(items);

                // assert
                results.ReadOnly
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodHeader_ItemsIsNotNull()
            {
                // arrange
                var header = MakeHeader();
                var items = MakeListOfModels();

                // act
                var results = header.ToListModel(items);

                // assert
                results.Items
                       .Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodHeader_TwoItems()
            {
                // arrange
                var expected = 2;
                var header = MakeHeader();
                var items = MakeListOfModels();

                // act
                var results = header.ToListModel(items);

                // assert
                results.Items
                       .Count()
                       .Should()
                       .Be(expected);
            }
        }
    }
}