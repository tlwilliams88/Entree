using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentAssertions;

using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Helpers;

using Moq;
using Xunit;

namespace KeithLink.Svc.Impl.Tests.Unit.Helpers
{
    public class ItemOrderHistoryHelperTests
    {
        private static ICatalogLogic TestCatalogLogic = Mock.Of<ICatalogLogic>
                (s => s.GetLastFiveOrderHistory(It.IsAny<UserSelectedContext>(), "111111") ==
                      new List<OrderHistoryFile>() { new OrderHistoryFile() {
                                                                                Header = new OrderHistoryHeader() { DeliveryDate="1/1/1970" },
                                                                                Details = new List<OrderHistoryDetail>() {
                                                                                                                             new OrderHistoryDetail() {
                                                                                                                                                          ShippedQuantity = 2
                                                                                                                                                      }
                                                                                                                         }
                                                                            } });

        #region GetItemOrderHistories_PassedInListOfItemUsageReportItemModel
        public class GetItemOrderHistories_PassedInListOfItemUsageReportItemModel
        {

            [Fact]
            public void GoodItemUsageReportItemModel_ReturnsOrderHistoriesAsExpected()
            {
                // arrange
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testprods = new List<ItemUsageReportItemModel>() {
                                                                         new ItemUsageReportItemModel() {
                                                                                                            ItemNumber = "111111"
                                                                                                        }
                                                                     };
                var expected = "1/1/1970-2, ";

                // act
                ItemOrderHistoryHelper.GetItemOrderHistories(TestCatalogLogic, testcontext, testprods);

                // assert
                testprods.First().OrderHistoryString
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void BadItemUsageReportItemModel_ReturnsOrderHistoriesAsEmptyString()
            {
                // arrange
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testprods = new List<ItemUsageReportItemModel>() {
                                                                         new ItemUsageReportItemModel() {
                                                                                                            ItemNumber = "222222"
                                                                                                        }
                                                                     };
                var expected = string.Empty;

                // act
                ItemOrderHistoryHelper.GetItemOrderHistories(TestCatalogLogic, testcontext, testprods);

                // assert
                testprods.First().OrderHistoryString
                    .Should()
                    .Be(expected);
            }
        }
        #endregion

        #region GetItemOrderHistories_PassedInListOfOrderLine
        public class GetItemOrderHistories_PassedInListOfOrderLine
        {

            [Fact]
            public void GoodItemUsageReportItemModel_ReturnsOrderHistoriesAsExpected()
            {
                // arrange
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testprods = new List<OrderLine>() {
                                                          new OrderLine() {
                                                                              ItemNumber = "111111"
                                                                          }
                                                      };
                var expected = "1/1/1970-2, ";

                // act
                ItemOrderHistoryHelper.GetItemOrderHistories(TestCatalogLogic, testcontext, testprods);

                // assert
                testprods.First().OrderHistoryString
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void BadItemUsageReportItemModel_ReturnsOrderHistoriesAsEmptyString()
            {
                // arrange
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testprods = new List<OrderLine>() {
                                                          new OrderLine() {
                                                                              ItemNumber = "222222"
                                                                          }
                                                      };
                var expected = string.Empty;

                // act
                ItemOrderHistoryHelper.GetItemOrderHistories(TestCatalogLogic, testcontext, testprods);

                // assert
                testprods.First().OrderHistoryString
                    .Should()
                    .Be(expected);
            }
        }
        #endregion

        #region GetItemOrderHistories_PassedInListOfShoppingCartItem
        public class GetItemOrderHistories_PassedInListOfShoppingCartItem
        {

            [Fact]
            public void GoodItemUsageReportItemModel_ReturnsOrderHistoriesAsExpected()
            {
                // arrange
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testprods = new List<ShoppingCartItem>() {
                                                                 new ShoppingCartItem() {
                                                                                            ItemNumber = "111111"
                                                                                        }
                                                             };
                var expected = "1/1/1970-2, ";

                // act
                ItemOrderHistoryHelper.GetItemOrderHistories(TestCatalogLogic, testcontext, testprods);

                // assert
                testprods.First().OrderHistoryString
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void BadItemUsageReportItemModel_ReturnsOrderHistoriesAsEmptyString()
            {
                // arrange
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testprods = new List<ShoppingCartItem>() {
                                                                 new ShoppingCartItem() {
                                                                                            ItemNumber = "222222"
                                                                                        }
                                                             };
                var expected = string.Empty;

                // act
                ItemOrderHistoryHelper.GetItemOrderHistories(TestCatalogLogic, testcontext, testprods);

                // assert
                testprods.First().OrderHistoryString
                    .Should()
                    .Be(expected);
            }
        }
        #endregion

        #region GetItemOrderHistories_PassedInListOfProduct
        public class GetItemOrderHistories_PassedInListOfProduct
        {

            [Fact]
            public void GoodItemUsageReportItemModel_ReturnsOrderHistoriesAsExpected()
            {
                // arrange
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testprods = new List<Product>() {
                                                        new Product() {
                                                                          ItemNumber = "111111"
                                                                      }
                                                    };
                var expected = "1/1/1970-2, ";

                // act
                ItemOrderHistoryHelper.GetItemOrderHistories(TestCatalogLogic, testcontext, testprods);

                // assert
                testprods.First().OrderHistoryString
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void BadItemUsageReportItemModel_ReturnsOrderHistoriesAsEmptyString()
            {
                // arrange
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testprods = new List<Product>() {
                                                        new Product() {
                                                                          ItemNumber = "222222"
                                                                      }
                                                    };
                var expected = string.Empty;

                // act
                ItemOrderHistoryHelper.GetItemOrderHistories(TestCatalogLogic, testcontext, testprods);

                // assert
                testprods.First().OrderHistoryString
                    .Should()
                    .Be(expected);
            }
        }
        #endregion

        #region GetItemOrderHistories_PassedInListOfListItemModel
        public class GetItemOrderHistories_PassedInListOfListItemModel
        {

            [Fact]
            public void GoodItemUsageReportItemModel_ReturnsOrderHistoriesAsExpected()
            {
                // arrange
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testprods = new List<ListItemModel>() {
                                                              new ListItemModel() {
                                                                                      ItemNumber = "111111"
                                                                                  }
                                                          };
                var expected = "1/1/1970-2, ";

                // act
                ItemOrderHistoryHelper.GetItemOrderHistories(TestCatalogLogic, testcontext, testprods);

                // assert
                testprods.First().OrderHistoryString
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void BadItemUsageReportItemModel_ReturnsOrderHistoriesAsEmptyString()
            {
                // arrange
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testprods = new List<ListItemModel>() {
                                                              new ListItemModel() {
                                                                                      ItemNumber = "222222"
                                                                                  }
                                                          };
                var expected = string.Empty;

                // act
                ItemOrderHistoryHelper.GetItemOrderHistories(TestCatalogLogic, testcontext, testprods);

                // assert
                testprods.First().OrderHistoryString
                    .Should()
                    .Be(expected);
            }
        }
        #endregion

        #region GetItemOrderHistories_PassedInListOfInvoiceItemModel
        public class GetItemOrderHistories_PassedInListOfInvoiceItemModel
        {

            [Fact]
            public void GoodItemUsageReportItemModel_ReturnsOrderHistoriesAsExpected()
            {
                // arrange
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testprods = new List<InvoiceItemModel>() {
                                                                 new InvoiceItemModel() {
                                                                                            ItemNumber = "111111"
                                                                                        }
                                                             };
                var expected = "1/1/1970-2, ";

                // act
                ItemOrderHistoryHelper.GetItemOrderHistories(TestCatalogLogic, testcontext, testprods);

                // assert
                testprods.First().OrderHistoryString
                    .Should()
                    .Be(expected);
            }

            [Fact]
            public void BadItemUsageReportItemModel_ReturnsOrderHistoriesAsEmptyString()
            {
                // arrange
                var testcontext = new UserSelectedContext()
                {
                    BranchId = "XXX",
                    CustomerId = "123456"
                };
                var testprods = new List<InvoiceItemModel>() {
                                                                 new InvoiceItemModel() {
                                                                                            ItemNumber = "222222"
                                                                                        }
                                                             };
                var expected = string.Empty;

                // act
                ItemOrderHistoryHelper.GetItemOrderHistories(TestCatalogLogic, testcontext, testprods);

                // assert
                testprods.First().OrderHistoryString
                    .Should()
                    .Be(expected);
            }
        }
        #endregion

    }
}
