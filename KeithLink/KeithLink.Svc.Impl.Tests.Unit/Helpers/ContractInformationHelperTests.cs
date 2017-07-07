using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Helpers;

using Moq;
using FluentAssertions;
using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Helpers
{
    public class ContractInformationHelperTests {

        private static IListService TestListSvc() {
            var dict = new Dictionary<string, string>();
            dict.Add("111111", "Category 1");
            var lsvc = Mock.Of<IListService>(s => s.GetContractInformation(It.IsAny<UserSelectedContext>() ) ==
              dict );
            return lsvc;
        }

        private static IListService TestListSvcNoContract()
        {
            var nocontract = Mock.Of<IListService>(s => s.GetContractInformation(It.IsAny<UserSelectedContext>()) ==
              new Dictionary<string, string>());
            return nocontract;
        }

        private static Product TestProd = new Product() { ItemNumber = "111111" };

        private static Product TestOtherProd = new Product() { ItemNumber = "999999" };

        public class GetContractCategoriesFromLists_PassedInProduct
        {

            [Fact]
            public void GoodProduct_ExpectCategory()
            {
                // arrange
                Product prod = TestProd;

                // act
                ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prod, TestListSvc());

                // assert
                prod.Category
                    .Should()
                    .Be("Category 1");
            }

            [Fact]
            public void BadProduct_ExpectNullCategory()
            {
                // arrange
                Product prod = TestOtherProd;

                // act
                ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prod, TestListSvc());

                // assert
                prod.Category
                    .Should()
                    .BeNullOrEmpty();
            }

            [Fact]
            public void AnyProductCustomerHasNoContract_Completes()
            {
                // arrange
                Product prod = TestOtherProd;

                // act
                ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prod, TestListSvcNoContract());

                // assert
                prod.Category
                    .Should()
                    .BeNullOrEmpty();
            }
        }

        public class GetContractCategoriesFromLists_PassedInListOfProduct
        {

            private static List<Product> TestProducts = new List<Product> { TestProd, TestOtherProd };

            [Fact]
            public void GoodProductFromList_ExpectCategory()
            {
                // arrange
                List<Product> prods = TestProducts;

                // act
                ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prods, TestListSvc());

                // assert
                prods.Where(p => p.ItemNumber == "111111")
                     .First()
                     .Category
                     .Should()
                     .Be("Category 1");
            }

            [Fact]
            public void BadProductFromList_ExpectNullCategory()
            {
                // arrange
                List<Product> prods = TestProducts;

                // act
                ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prods, TestListSvc());

                // assert
                prods.Where(p => p.ItemNumber == "999999")
                     .First()
                     .Category
                     .Should()
                     .BeNullOrEmpty();
            }

            [Fact]
            public void AnyProductCustomerHasNoContract_Completes()
            {
                // arrange
                List<Product> prods = TestProducts;

                // act
                ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prods, TestListSvcNoContract());

                // assert
                prods.Should()
                     .NotBeNullOrEmpty();
            }
        }
    }
}
