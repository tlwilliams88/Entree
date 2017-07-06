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
        [Fact]
        public void Class_Exists_And_Is_Here()
        {
            // arrange
            var IsObject = new FavoritesAndNotesHelper();

            // act

            // assert
            IsObject.Should()
                    .NotBeNull();
        }

        public class GetFavoritesAndNotesFromLists_PassedInProduct {

            public IListService TestListSvc
            {
                get
                {
                    ListModel returned = new ListModel();
                    returned.Items = new List<ListItemModel>();
                    returned.Items.Add(new ListItemModel() { ItemNumber = "111111", Favorite = true, Notes = "test note" });
                    return Mock.Of<IListService>(s => s.MarkFavoritesAndAddNotes(It.IsAny<UserProfile>(), It.IsAny<ListModel>(), It.IsAny<UserSelectedContext>()) == returned);
                }
            }

            public Product TestProd {
                get { return new Product() {ItemNumber = "111111"}; }
            }

            public Product TestOtherProd {
                get { return new Product() {ItemNumber = "999999"}; }
            }

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
        }

        public class GetFavoritesAndNotesFromLists_PassedInListOfProduct
        {

            public IListService TestListSvc
            {
                get
                {
                    ListModel returned = new ListModel();
                    returned.Items = new List<ListItemModel>();
                    returned.Items.Add(new ListItemModel() { ItemNumber = "111111", Favorite = true, Notes = "test note" });
                    return Mock.Of<IListService>(s => s.MarkFavoritesAndAddNotes(It.IsAny<UserProfile>(), It.IsAny<ListModel>(), It.IsAny<UserSelectedContext>()) == returned);
                }
            }

            public IListService TestListSvcNoFavoritesOrNotes
            {
                get
                {
                    ListModel returned = new ListModel();
                    returned.Items = new List<ListItemModel>();
                    return Mock.Of<IListService>(s => s.MarkFavoritesAndAddNotes(It.IsAny<UserProfile>(), It.IsAny<ListModel>(), It.IsAny<UserSelectedContext>()) == returned);
                }
            }

            public Product TestProd
            {
                get { return new Product() { ItemNumber = "111111" }; }
            }

            public Product TestOtherProd
            {
                get { return new Product() { ItemNumber = "999999" }; }
            }

            public List<Product> TestProducts {
                get {
                    List<Product> list = new List<Product>();
                    list.Add(TestProd);
                    list.Add(TestOtherProd);
                    return list;
                }
            }

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

            [Fact]
            public void No_Notes()
            {
                // arrange
                List<Product> prods = TestProducts;

                // act
                FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(), prods, TestListSvcNoFavoritesOrNotes);

                // assert
                prods.Any(p => p.Notes != null)
                     .Should()
                     .BeFalse();
            }
        }
    }
}
