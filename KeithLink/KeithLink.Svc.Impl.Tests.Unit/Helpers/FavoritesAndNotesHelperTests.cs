using System;
using System.Collections.Generic;
using System.Linq;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using Xunit;

using KeithLink.Svc.Impl.Helpers;
using KeithLink.Svc.Impl.Service.List;

using FluentAssertions;
using Moq;

namespace KeithLink.Svc.Impl.Tests.Unit.Helpers {
    public class FavoritesAndNotesHelperTests {
        private static List<Product> items = new List<Product>() { new Product() { ItemNumber = "111111", Favorite = true, Notes = "test note", InHistory = true } };

        private static IListService TestListSvc = Mock.Of<IListService>
                (s => s.MarkFavoritesAndAddNotes(It.IsAny<UserProfile>(), It.IsAny<List<Product>>(), It.IsAny<UserSelectedContext>()) ==
                      items );

        private static List<ListItemModel> noitems = new List<ListItemModel>() { new ListItemModel() };

        private static IListService TestListSvcNoFavoritesOrNotes = Mock.Of<IListService>
            (s => s.MarkFavoritesAndAddNotes(It.IsAny<UserProfile>(), It.IsAny<List<Product>>(), It.IsAny<UserSelectedContext>()) ==
                      new List<Product>());

        private static Product TestProd = new Product() { ItemNumber = "111111" };

        private static Product TestOtherProd = new Product() { ItemNumber = "999999" };

        public class GetFavoritesAndNotesFromLists_PassedInProduct {

            [Fact]
            public void GoodFavoriteProduct_ReturnsFavoriteAsTrue() {
                // arrange
                Product prod = TestProd;

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(), prod, TestListSvc);

                // assert
                prod.Favorite
                    .Should()
                    .BeTrue();
            }

            [Fact]
            public void BadFavoriteProduct_ReturnsFavoriteAsFalse() {
                // arrange
                Product prod = TestOtherProd;

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(), prod, TestListSvc);

                // assert
                prod.Favorite
                    .Should()
                    .BeFalse();
            }

            [Fact]
            public void BadNotesProduct_ReturnsNullNotes() {
                // arrange
                Product prod = TestProd;

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(), prod, TestListSvc);

                // assert
                prod.Notes
                    .Should()
                    .Be("test note");
            }

            [Fact]
            public void GoodNotesProduct_ReturnsExpectedNotes() {
                // arrange
                Product prod = TestOtherProd;

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(), TestProd, TestListSvc);

                // assert
                prod.Notes
                    .Should()
                    .BeNullOrEmpty();
            }


            [Fact]
            public void GoodInHistoryProduct_ReturnsInHistoryAsTrue()
            {
                // arrange
                Product prod = TestProd;

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(), prod, TestListSvc);

                // assert
                prod.InHistory
                    .Should()
                    .BeTrue();
            }

            [Fact]
            public void BadInHistoryProduct_ReturnsInHistoryAsFalse()
            {
                // arrange
                Product prod = TestOtherProd;

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(), prod, TestListSvc);

                // assert
                prod.InHistory
                    .Should()
                    .BeFalse();
            }
            [Fact]
            public void NoFavoritesOrNotes_ReturnsNullNotes()
            {
                // arrange
                Product prod = TestProd;

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(), prod, TestListSvcNoFavoritesOrNotes);

                // assert
                prod.Notes
                    .Should()
                    .BeNullOrEmpty();
            }
        }

        public class GetFavoritesAndNotesFromLists_PassedInListOfProduct
        {

            private static List<Product> TestProducts = new List<Product> { TestProd, TestOtherProd };

            [Fact]
            public void GoodFavoriteProductInList_ReturnsProductInListWithFavoriteAsTrue()
            {
                // arrange
                List<Product> prods = TestProducts;

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(), prods, TestListSvc);

                // assert
                prods.Where(p => p.ItemNumber == "111111")
                     .First()
                     .Favorite
                     .Should()
                     .BeTrue();
            }

            [Fact]
            public void BadFavoriteProductInList_ReturnsProductInListWithFavoriteAsFalse()
            {
                // arrange
                List<Product> prods = TestProducts;

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(), prods, TestListSvc);

                // assert
                prods.Where(p => p.ItemNumber == "999999")
                     .First()
                     .Favorite
                     .Should()
                     .BeFalse();
            }

            [Fact]
            public void GoodNotesProductInList_ReturnsProductInListWithNotesAsExpected()
            {
                // arrange
                List<Product> prods = TestProducts;

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(), prods, TestListSvc);

                // assert
                prods.Where(p => p.ItemNumber == "111111")
                     .First()
                     .Notes
                     .Should()
                     .Be("test note");
            }

            [Fact]
            public void BadNotesProductInList_ReturnsProductInListWithNotesAsNull()
            {
                // arrange
                List<Product> prods = TestProducts;

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(), prods, TestListSvc);

                // assert
                prods.Where(p => p.ItemNumber == "999999")
                     .First()
                     .Notes
                     .Should()
                     .BeNullOrEmpty();
            }


            [Fact]
            public void GoodInHistoryProductInList_ReturnsProductInListWithInHistoryAsTrue()
            {
                // arrange
                List<Product> prods = TestProducts;

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(), prods, TestListSvc);

                // assert
                prods.Where(p => p.ItemNumber == "111111")
                     .First()
                     .InHistory
                     .Should()
                     .BeTrue();
            }

            [Fact]
            public void BadInHistoryProductInList_ReturnsProductInListWithInHistoryAsFalse()
            {
                // arrange
                List<Product> prods = TestProducts;

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(), prods, TestListSvc);

                // assert
                prods.Where(p => p.ItemNumber == "999999")
                     .First()
                     .InHistory
                     .Should()
                     .BeFalse();
            }

            [Fact]
            public void NoFavoritesOrNotes_ReturnsFavoritesAsFalse()
            {
                // arrange
                List<Product> prods = TestProducts;

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(), prods, TestListSvcNoFavoritesOrNotes);

                // assert
                prods.Any(p => p.Favorite)
                     .Should()
                     .BeFalse();
            }
        }
    }
}
