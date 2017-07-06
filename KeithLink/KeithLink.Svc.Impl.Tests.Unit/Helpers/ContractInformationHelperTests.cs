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

using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Helpers
{
    public class ContractInformationHelperTests
    {
        public IListService TestListSvc
        {
            get
            {
                Dictionary<string,string> contractDictionary = new Dictionary<string, string>();
                contractDictionary.Add("111111", "1 Category");
                contractDictionary.Add("222222", "2 Category");
                return Mock.Of<IListService>(s => s.GetContractInformation(It.IsAny<UserSelectedContext>()) == contractDictionary);
            }
        }

        public Product TestProd
        {
            get
            {
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
        public void Class_Exists_And_Is_Here()
        {
            // arrange
            var IsObject = new ContractInformationHelper();

            // act

            // assert
            Assert.NotNull(IsObject);
        }

        [Fact]
        public void Assigns_Category_When_Single_Prod_Is_In_Contract()
        {
            // arrange
            Product prod = TestProd;

            // act
            ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prod, TestListSvc);

            // assert
            Assert.Same("1 Category", prod.Category);
        }

        [Fact]
        public void Does_Not_Assign_Category_When_Single_Prod_Is_Not_In_Contract()
        {
            // arrange
            Product prod = TestOtherProd;

            // act
            ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prod, TestListSvc);

            // assert
            Assert.Null(prod.Category);
        }
    }
}
