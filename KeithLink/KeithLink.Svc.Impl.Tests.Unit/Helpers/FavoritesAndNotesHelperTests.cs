using System;
using System.Collections.Generic;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using Xunit;

using KeithLink.Svc.Impl.Helpers;
using KeithLink.Svc.Impl.Service.List;

using Moq;

namespace KeithLink.Svc.Impl.Tests.Unit.Helpers {
    public class FavoritesAndNotesHelperTests {
        public IListService TestListSvc {
            get {
                ListModel returned = new ListModel();
                returned.Items = new List<ListItemModel>();
                returned.Items.Add(new ListItemModel() { ItemNumber = "111111", Favorite = true, Notes = "test note" });
                return Mock.Of<IListService>(s => s.MarkFavoritesAndAddNotes(It.IsAny<UserProfile>(), It.IsAny<ListModel>(), It.IsAny<UserSelectedContext>()) == returned);
            }
        }

        public Product TestProd {
            get {
                return new Product() { ItemNumber = "111111" }; 
                
            }
        }

        public Product TestOtherProd
        {
            get
            {
                return new Product() { ItemNumber = "999999" };

            }
        }

        [Fact]
        public void Class_Exists_And_Is_Here() {
            // arrange
            var IsObject = new FavoritesAndNotesHelper();

            // act

            // assert
            Assert.NotNull(IsObject);
        }

        [Fact]
        public void Assigns_Favorite_When_Single_Prod_Is_Favorite() {
            // arrange
            Product prod = TestProd;

            // act
            FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(), prod, TestListSvc);

            // assert
            Assert.True(prod.Favorite);
        }

        [Fact]
        public void Assigns_Favorite_As_False_When_Single_Prod_Is_Not_Favorite()
        {
            // arrange
            Product prod = TestOtherProd;

            // act
            FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(), prod, TestListSvc);

            // assert
            Assert.False(prod.Favorite);
        }

        [Fact]
        public void Assigns_Notes_When_Single_Prod_Has_Notes()
        {
            // arrange
            Product prod = TestProd;

            // act
            FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(), prod, TestListSvc);

            // assert
            Assert.Same("test note", prod.Notes);
        }

        [Fact]
        public void Assigns_Notes_As_Null_When_Single_Prod_Doesnt_Have_Notes()
        {
            // arrange
            Product prod = TestOtherProd;

            // act
            FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(new UserProfile(), new UserSelectedContext(), TestProd, TestListSvc);

            // assert
            Assert.Null(prod.Notes);
        }
    }
}
