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
      
        private static List<ListItemModel> items = new List<ListItemModel>() { new ListItemModel() { ItemNumber = "111111", Favorite = true, Notes = "test note" } };

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
            public void Assigns_Favorite_When_Single_Prod_Is_Favorite() {
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
            public void Assigns_Favorite_As_False_When_Single_Prod_Is_Not_Favorite() {
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
            public void Assigns_Notes_When_Single_Prod_Has_Notes() {
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
            public void Assigns_Notes_As_Null_When_Single_Prod_Doesnt_Have_Notes() {
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
            public void Assigns_InHistory_When_Single_Prod_Is_InHistory()
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
            public void Assigns_InHistory_As_False_When_Single_Prod_Is_Not_InHistory()
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
            public void No_Favorites_Or_Notes()
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
            public void Assigns_Favorite_When_ListOf_Prod_Contains_Favorite()
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
            public void Assigns_Favorite_As_False_When_ListOf_Prod_Contains_Prod_Is_Not_Favorite()
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
            public void Assigns_Notes_When_ListOf_Prod_Contains_Notes()
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
            public void Null_Notes_When_ListOf_Prod_Contains_Prod_Does_Not_Have_Notes()
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
            public void Assigns_InHistory_When_ListOf_Prod_Contains_InHistory()
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
            public void Assigns_InHistory_As_False_When_ListOf_Prod_Contains_Prod_Is_Not_InHistory()
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
            public void No_Favorites()
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
