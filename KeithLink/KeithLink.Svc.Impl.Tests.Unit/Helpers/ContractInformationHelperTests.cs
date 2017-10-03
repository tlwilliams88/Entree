using System.Collections.Generic;
using System.Linq;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Helpers;

using FluentAssertions;
using Moq;
using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Helpers
{
    public class ContractInformationHelperTests
    {
        #region setup

        private static IListService TestListSvc()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("111111", "Category 1");
            var lsvc = Mock.Of<IListService>(s => s.GetContractInformation(It.IsAny<UserSelectedContext>()) ==
              dict);
            return lsvc;
        }

        private static IListService TestListSvcNoContract()
        {
            var nocontract = Mock.Of<IListService>(s => s.GetContractInformation(It.IsAny<UserSelectedContext>()) ==
              new Dictionary<string, string>());
            return nocontract;
        }

        private static Product TestProd = new Product()
        {
            ItemNumber = "111111",
            Name = "Fake Name",
            BrandExtendedDescription = "Fake Brand",
            Size = "Fake Size",
            Pack = "Fake Pack"
        };

        private static Product TestOtherProd = new Product()
        {
            ItemNumber = "999999",
            Name = "Fake Name",
            BrandExtendedDescription = "Fake Brand",
            Size = "Fake Size",
            Pack = "Fake Pack"
        };

        #endregion setup

        #region GetContractCategoriesFromLists_PassedInProduct

        public class GetContractCategoriesFromLists_PassedInProduct
        {
            [Fact]
            public void GoodProduct_ExpectCategory()
            {
                // arrange
                Product prod = TestProd;
                var expected = "Category 1";

                // act
                ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prod, TestListSvc());

                // assert
                prod.Category
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void GoodProduct_ExpectDetail()
            {
                // arrange
                Product prod = TestProd;
                var expected = "Fake Name / 111111 / Fake Brand / Category 1 / Fake Pack / Fake Size";

                // act
                ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prod, TestListSvc());

                // assert
                prod.Detail
                    .Should()
                    .Be(expected);
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
            public void BadProduct_ExpectNullDetail()
            {
                // arrange
                Product prod = TestOtherProd;

                // act
                ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prod, TestListSvc());

                // assert
                prod.Detail
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

        #endregion GetContractCategoriesFromLists_PassedInProduct

        #region GetContractCategoriesFromLists_PassedInListOfProduct

        public class GetContractCategoriesFromLists_PassedInListOfProduct
        {
            private static List<Product> TestProducts = new List<Product> { TestProd, TestOtherProd };

            [Fact]
            public void GoodProductFromList_ExpectCategory()
            {
                // arrange
                List<Product> prods = TestProducts;
                var expected = "Category 1";

                // act
                ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prods, TestListSvc());

                // assert
                prods.Where(p => p.ItemNumber == "111111")
                     .First()
                     .Category
                     .Should()
                     .Be(expected);
            }

            [Fact]
            public void GoodProductFromList_ExpectDetail()
            {
                // arrange
                List<Product> prods = TestProducts;
                var expected = "Fake Name / 111111 / Fake Brand / Category 1 / Fake Pack / Fake Size";

                // act
                ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prods, TestListSvc());

                // assert
                prods.Where(p => p.ItemNumber == "111111")
                     .First()
                     .Detail
                     .Should()
                     .Be(expected);
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
            public void BadProductFromList_ExpectNullDetail()
            {
                // arrange
                List<Product> prods = TestProducts;

                // act
                ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prods, TestListSvc());

                // assert
                prods.Where(p => p.ItemNumber == "999999")
                     .First()
                     .Detail
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

        #endregion GetContractCategoriesFromLists_PassedInListOfProduct

        #region GetContractCategoriesFromLists_PassedInListOfShoppingCartItem

        public class GetContractCategoriesFromLists_PassedInListOfShoppingCartItem
        {
            private static List<ShoppingCartItem> TestShoppingCartItems = new List<ShoppingCartItem> {
                                                                                                new ShoppingCartItem() {
                                                                                                                           ItemNumber = "111111",
                                                                                                                           Name = "Fake Name",
                                                                                                                           BrandExtendedDescription = "Fake Brand",
                                                                                                                           PackSize = "Fake Pack / Fake Size"
                                                                                                                       },
                                                                                                new ShoppingCartItem() {
                                                                                                                           ItemNumber = "999999",
                                                                                                                           Name = "Fake Name",
                                                                                                                           BrandExtendedDescription = "Fake Brand",
                                                                                                                           PackSize = "Fake Pack / Fake Size"
                                                                                                                       }
                                                                                            };

            [Fact]
            public void GoodProductFromList_ExpectDetail()
            {
                // arrange
                List<ShoppingCartItem> prods = TestShoppingCartItems;
                var expected = "Fake Name / 111111 / Fake Brand / Category 1 / Fake Pack / Fake Size";

                // act
                ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prods, TestListSvc());

                // assert
                prods.Where(p => p.ItemNumber == "111111")
                     .First()
                     .Detail
                     .Should()
                     .Be(expected);
            }

            [Fact]
            public void BadProductFromList_ExpectNullDetail()
            {
                // arrange
                List<ShoppingCartItem> prods = TestShoppingCartItems;

                // act
                ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prods, TestListSvc());

                // assert
                prods.Where(p => p.ItemNumber == "999999")
                     .First()
                     .Detail
                     .Should()
                     .BeNullOrEmpty();
            }

            [Fact]
            public void AnyProductCustomerHasNoContract_Completes()
            {
                // arrange
                List<ShoppingCartItem> prods = TestShoppingCartItems;

                // act
                ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prods, TestListSvcNoContract());

                // assert
                prods.Should()
                     .NotBeNullOrEmpty();
            }
        }

        #endregion GetContractCategoriesFromLists_PassedInListOfShoppingCartItem

        #region GetContractCategoriesFromLists_PassedInListOfOrderLine

        public class GetContractCategoriesFromLists_PassedInListOfOrderLine
        {
            private static List<OrderLine> TestOrderLines = new List<OrderLine> {
                                                                                    new OrderLine() {
                                                                                                        ItemNumber = "111111",
                                                                                                        Name = "Fake Name",
                                                                                                        BrandExtendedDescription = "Fake Brand",
                                                                                                        PackSize = "Fake Pack / Fake Size",
                                                                                                        Pack = "Fake Pack",
                                                                                                        Size = "Fake Size"
                                                                                                   },
                                                                                    new OrderLine() {
                                                                                                        ItemNumber = "999999",
                                                                                                        Name = "Fake Name",
                                                                                                        BrandExtendedDescription = "Fake Brand",
                                                                                                        PackSize = "Fake Pack / Fake Size",
                                                                                                        Pack = "Fake Pack",
                                                                                                        Size = "Fake Size"
                                                                                                    }
                                                                                };

            [Fact]
            public void GoodProductFromList_ExpectDetail()
            {
                // arrange
                List<OrderLine> prods = TestOrderLines;
                var expected = "Fake Name / 111111 / Fake Brand / Category 1 / Fake Pack / Fake Size";

                // act
                ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prods, TestListSvc());

                // assert
                prods.Where(p => p.ItemNumber == "111111")
                     .First()
                     .Detail
                     .Should()
                     .Be(expected);
            }

            [Fact]
            public void BadProductFromList_ExpectNullDetail()
            {
                // arrange
                List<OrderLine> prods = TestOrderLines;

                // act
                ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prods, TestListSvc());

                // assert
                prods.Where(p => p.ItemNumber == "999999")
                     .First()
                     .Detail
                     .Should()
                     .BeNullOrEmpty();
            }

            [Fact]
            public void AnyProductCustomerHasNoContract_Completes()
            {
                // arrange
                List<OrderLine> prods = TestOrderLines;

                // act
                ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prods, TestListSvcNoContract());

                // assert
                prods.Should()
                     .NotBeNullOrEmpty();
            }
        }

        #endregion GetContractCategoriesFromLists_PassedInListOfOrderLine

        #region GetContractCategoriesFromLists_PassedInListOfItemUsageReportItemModel

        public class GetContractCategoriesFromLists_PassedInListOfItemUsageReportItemModel
        {
            private static List<ItemUsageReportItemModel> TestItemUsageReportItemModels = new List<ItemUsageReportItemModel> {
                                                                                                                                 new ItemUsageReportItemModel() {
                                                                                                                                                                    ItemNumber = "111111",
                                                                                                                                                                    Name = "Fake Name",
                                                                                                                                                                    Brand = "Fake Brand",
                                                                                                                                                                    PackSize = "Fake Pack / Fake Size",
                                                                                                                                                                    Pack = "Fake Pack",
                                                                                                                                                                    Size = "Fake Size"
                                                                                                                                                                },
                                                                                                                                 new ItemUsageReportItemModel() {
                                                                                                                                                                    ItemNumber = "999999",
                                                                                                                                                                    Name = "Fake Name",
                                                                                                                                                                    Brand = "Fake Brand",
                                                                                                                                                                    PackSize = "Fake Pack / Fake Size",
                                                                                                                                                                    Pack = "Fake Pack",
                                                                                                                                                                    Size = "Fake Size"
                                                                                                                                                                }
                                                                                                                             };

            [Fact]
            public void GoodProductFromList_ExpectDetail()
            {
                // arrange
                List<ItemUsageReportItemModel> prods = TestItemUsageReportItemModels;
                var expected = "Fake Name / 111111 / Fake Brand / Category 1 / Fake Pack / Fake Size";

                // act
                ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prods, TestListSvc());

                // assert
                prods.Where(p => p.ItemNumber == "111111")
                     .First()
                     .Detail
                     .Should()
                     .Be(expected);
            }

            [Fact]
            public void BadProductFromList_ExpectNullDetail()
            {
                // arrange
                List<ItemUsageReportItemModel> prods = TestItemUsageReportItemModels;

                // act
                ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prods, TestListSvc());

                // assert
                prods.Where(p => p.ItemNumber == "999999")
                     .First()
                     .Detail
                     .Should()
                     .BeNullOrEmpty();
            }

            [Fact]
            public void AnyProductCustomerHasNoContract_Completes()
            {
                // arrange
                List<ItemUsageReportItemModel> prods = TestItemUsageReportItemModels;

                // act
                ContractInformationHelper.GetContractCategoriesFromLists(new UserSelectedContext(), prods, TestListSvcNoContract());

                // assert
                prods.Should()
                     .NotBeNullOrEmpty();
            }
        }

        #endregion GetContractCategoriesFromLists_PassedInListOfItemUsageReportItemModel
    }
}