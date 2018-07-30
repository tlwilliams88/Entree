﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Helpers;
using Moq;
using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Helpers
{
    public class FavoritesAndNotesHelperTests
    {
        private static readonly IListService TestListSvc = Mock.Of<IListService>
        (s => s.MarkFavoritesAndAddNotes(It.IsAny<UserProfile>(), It.IsAny<List<Product>>(),
                  It.IsAny<UserSelectedContext>()) ==
              new List<Product>
              {
                  new Product {ItemNumber = "111111", Favorite = true, Notes = "test note", InHistory = true}
              });

        private static readonly IListService TestListSvcNoFavoritesOrNotes = Mock.Of<IListService>
            ();

        public class GetFavoritesAndNotesFromLists_PassedInProduct
        {
            [Fact]
            public void BadFavoriteProduct_ReturnsFavoriteAsFalse()
            {
                // arrange
                var prod = new Product {ItemNumber = "999999"};

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(),
                    prod, TestListSvc);

                // assert
                prod.Favorite
                    .Should()
                    .BeFalse();
            }

            [Fact]
            public void BadInHistoryProduct_ReturnsInHistoryAsFalse()
            {
                // arrange
                var prod = new Product {ItemNumber = "999999"};

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(),
                    prod, TestListSvc);

                // assert
                prod.InHistory
                    .Should()
                    .BeFalse();
            }

            [Fact]
            public void BadNotesProduct_ReturnsNullNotes()
            {
                // arrange
                var prod = new Product {ItemNumber = "111111"};

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(),
                    prod, TestListSvc);

                // assert
                prod.Notes
                    .Should()
                    .Be("test note");
            }

            [Fact]
            public void GoodFavoriteProduct_ReturnsFavoriteAsTrue()
            {
                // arrange
                var prod = new Product {ItemNumber = "111111"};

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(),
                    prod, TestListSvc);

                // assert
                prod.Favorite
                    .Should()
                    .BeTrue();
            }

            [Fact]
            public void GoodInHistoryProduct_ReturnsInHistoryAsTrue()
            {
                // arrange
                var prod = new Product {ItemNumber = "111111"};

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(),
                    prod, TestListSvc);

                // assert
                prod.InHistory
                    .Should()
                    .BeTrue();
            }

            [Fact]
            public void GoodNotesProduct_ReturnsExpectedNotes()
            {
                // arrange
                var prod = new Product {ItemNumber = "111111"};
                var expected = "test note";

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(),
                    prod, TestListSvc);

                // assert
                prod.Notes
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void NoFavoritesOrNotes_ReturnsNullNotes()
            {
                // arrange
                var prod = new Product {ItemNumber = "111111"};
                var testcontext = new UserSelectedContext
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), testcontext, prod,
                    TestListSvcNoFavoritesOrNotes);

                // assert
                prod.Notes
                    .Should()
                    .BeNullOrEmpty();
            }
        }

        public class GetFavoritesAndNotesFromLists_PassedInListOfProduct
        {
            [Fact]
            public void BadFavoriteProductInList_ReturnsProductInListWithFavoriteAsFalse()
            {
                // arrange
                var prods = new List<Product>
                {
                    new Product {ItemNumber = "111111"},
                    new Product {ItemNumber = "999999"}
                };

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(),
                    prods, TestListSvc);

                // assert
                prods.Where(p => p.ItemNumber == "999999")
                    .First()
                    .Favorite
                    .Should()
                    .BeFalse();
            }

            [Fact]
            public void BadInHistoryProductInList_ReturnsProductInListWithInHistoryAsFalse()
            {
                // arrange
                var prods = new List<Product>
                {
                    new Product {ItemNumber = "111111"},
                    new Product {ItemNumber = "999999"}
                };

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(),
                    prods, TestListSvc);

                // assert
                prods.Where(p => p.ItemNumber == "999999")
                    .First()
                    .InHistory
                    .Should()
                    .BeFalse();
            }

            [Fact]
            public void BadNotesProductInList_ReturnsProductInListWithNotesAsNull()
            {
                // arrange
                var prods = new List<Product>
                {
                    new Product {ItemNumber = "111111"},
                    new Product {ItemNumber = "999999"}
                };

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(),
                    prods, TestListSvc);

                // assert
                prods.Where(p => p.ItemNumber == "999999")
                    .First()
                    .Notes
                    .Should()
                    .BeNullOrEmpty();
            }

            [Fact]
            public void GoodFavoriteProductInList_ReturnsProductInListWithFavoriteAsTrue()
            {
                // arrange
                var prods = new List<Product>
                {
                    new Product {ItemNumber = "111111"},
                    new Product {ItemNumber = "999999"}
                };

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(),
                    prods, TestListSvc);

                // assert
                prods.Where(p => p.ItemNumber == "111111")
                    .First()
                    .Favorite
                    .Should()
                    .BeTrue();
            }

            [Fact]
            public void GoodInHistoryProductInList_ReturnsProductInListWithInHistoryAsTrue()
            {
                // arrange
                var prods = new List<Product>
                {
                    new Product {ItemNumber = "111111"},
                    new Product {ItemNumber = "999999"}
                };

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(),
                    prods, TestListSvc);

                // assert
                prods.Where(p => p.ItemNumber == "111111")
                    .First()
                    .InHistory
                    .Should()
                    .BeTrue();
            }

            [Fact]
            public void GoodNotesProductInList_ReturnsProductInListWithNotesAsExpected()
            {
                // arrange
                var prods = new List<Product>
                {
                    new Product {ItemNumber = "111111"},
                    new Product {ItemNumber = "999999"}
                };

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(),
                    prods, TestListSvc);

                // assert
                prods.Where(p => p.ItemNumber == "111111")
                    .First()
                    .Notes
                    .Should()
                    .Be("test note");
            }

            [Fact]
            public void NoFavoritesOrNotes_ReturnsExpectedItemFavoriteAsFalse()
            {
                // arrange
                var prods = new List<Product>
                {
                    new Product {ItemNumber = "111111"},
                    new Product {ItemNumber = "999999"}
                };
                var expectedItemNumber = "111111";
                var testcontext = new UserSelectedContext
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), testcontext, prods,
                    TestListSvcNoFavoritesOrNotes);

                // assert
                prods.Where(p => p.ItemNumber == expectedItemNumber)
                    .First()
                    .Favorite
                    .Should()
                    .BeFalse();
            }
        }
    }
}