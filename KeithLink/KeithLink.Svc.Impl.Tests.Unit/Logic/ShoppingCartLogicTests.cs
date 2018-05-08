using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic;

using Autofac;
using FluentAssertions;
using Moq;
using Xunit;

using Basket = KeithLink.Svc.Core.Models.Generated.Basket;
using LineItem = KeithLink.Svc.Core.Models.Generated.LineItem;

namespace KeithLink.Svc.Impl.Tests.Unit.Logic {
    public class ShoppingCartLogicTests : BaseDITests {

        public class CartReport {
            [Fact]
            public void AnyCall_CallsBasketLogicRetrieveSharedCustomerBasket() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                Guid testCart = new Guid("dddddddddddddddddddddddddddddddd");
                PagedListModel testPagedListModel = new PagedListModel {
                    Name = "Fake Name"
                };
                PrintListModel testPrintModel = new PrintListModel();

                // act
                MemoryStream results = testunit.CartReport(fakeUser, testContext, testCart, testPagedListModel, testPrintModel);

                // assert
                mockDependents.BasketLogic.Verify(m => m.RetrieveSharedCustomerBasket(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<Guid>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyCall_CallsUserActiveCartLogicGetUserActiveCart() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                Guid testCart = new Guid("dddddddddddddddddddddddddddddddd");
                PagedListModel testPagedListModel = new PagedListModel {
                    Name = "Fake Name"
                };
                PrintListModel testPrintModel = new PrintListModel();

                // act
                MemoryStream results = testunit.CartReport(fakeUser, testContext, testCart, testPagedListModel, testPrintModel);

                // assert
                mockDependents.UserActiveCartLogic.Verify(m => m.GetUserActiveCart(It.IsAny<UserSelectedContext>(), It.IsAny<Guid>()), Times.Once, "not called");
            }

            [Fact]
            public void AnyCall_ProducesStream() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(true, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                Guid testCart = new Guid("dddddddddddddddddddddddddddddddd");
                PagedListModel testPagedListModel = new PagedListModel {
                    Name = "Fake Name"
                };
                PrintListModel testPrintModel = new PrintListModel();

                // act
                MemoryStream results = testunit.CartReport(fakeUser, testContext, testCart, testPagedListModel, testPrintModel);

                // assert
                results.Should()
                       .NotBeNull();
            }
        }

        public class LookupProductDetails {
            [Fact]
            public void CartWithGoodItem_DetailIsExpected() {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(true, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                ShoppingCart testCart = new ShoppingCart {
                    Active = true,
                    BranchId = "FUT",
                    CartId = new Guid("dddddddddddddddddddddddddddddddd"),
                    Name = "Fake Cart Name",
                    Items = new List<ShoppingCartItem> {
                        new ShoppingCartItem {
                            ItemNumber = "123456",
                            CatalogId = "FUT"
                        }
                    }
                };
                string expected = "Fake Name / 123456 / Fake Brand / Fake Class / Fake Pack / Fake Size";

                // act
                testunit.LookupProductDetails(fakeUser, testContext, testCart);

                // assert
                testCart.Items.First()
                        .Detail.Should()
                        .Be(expected);
            }
        }

        public class CreateCart
        {
            [Fact]
            public void EveryCall_CallsRepositoryToCreateOrUpdateCartOnce()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                ShoppingCart testCart = new ShoppingCart
                {
                    Active = true,
                    BranchId = "FUT",
                    CartId = new Guid("dddddddddddddddddddddddddddddddd"),
                    Name = "Fake Cart Name",
                    Items = new List<ShoppingCartItem> {
                        new ShoppingCartItem {
                            ItemNumber = "123456",
                            CatalogId = "FUT"
                        }
                    }
                };

                // act
                var result = testunit.CreateCart(fakeUser, testContext, testCart);

                // assert - verify that createorupdatebasket is called once where the lineposition of the first item is "1"
                mockDependents.BasketRepository.Verify(m => m.CreateOrUpdateBasket(It.IsAny<Guid>(),
                                                       It.IsAny<string>(),
                                                       It.IsAny<Basket>(),
                                                       It.IsAny<List<LineItem>>(),
                                                       It.IsAny<bool>()), Times.Once);
            }

            [Fact]
            public void EveryCall_CallsRepositoryToCreateOrUpdateCartOnceWithBasketHavingExpectedBranchId()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                ShoppingCart testCart = new ShoppingCart
                {
                    Active = true,
                    BranchId = "FUT",
                    CartId = new Guid("dddddddddddddddddddddddddddddddd"),
                    Name = "Fake Cart Name",
                    Items = new List<ShoppingCartItem> {
                        new ShoppingCartItem {
                            ItemNumber = "123456",
                            CatalogId = "FUT"
                        }
                    }
                };
                var expected = "fut";

                // act
                var result = testunit.CreateCart(fakeUser, testContext, testCart);

                // assert - verify that createorupdatebasket is called once where the lineposition of the first item is "1"
                mockDependents.BasketRepository.Verify(m => m.CreateOrUpdateBasket(It.IsAny<Guid>(),
                                                       It.IsAny<string>(),
                                                       It.Is<Basket>(r => r.BranchId == expected),
                                                       It.IsAny<List<LineItem>>(),
                                                       It.IsAny<bool>()), Times.Once);
            }

            [Fact]
            public void EveryCall_CallsRepositoryToCreateOrUpdateCartOnceWithBasketHavingExpectedListType()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                ShoppingCart testCart = new ShoppingCart
                {
                    Active = true,
                    BranchId = "FUT",
                    CartId = new Guid("dddddddddddddddddddddddddddddddd"),
                    Name = "Fake Cart Name",
                    Items = new List<ShoppingCartItem> {
                        new ShoppingCartItem {
                            ItemNumber = "123456",
                            CatalogId = "FUT"
                        }
                    }
                };
                var expected = (int)BasketType.Cart;

                // act
                var result = testunit.CreateCart(fakeUser, testContext, testCart);

                // assert - verify that createorupdatebasket is called once where the lineposition of the first item is "1"
                mockDependents.BasketRepository.Verify(m => m.CreateOrUpdateBasket(It.IsAny<Guid>(),
                                                       It.IsAny<string>(),
                                                       It.Is<Basket>(r => r.ListType.Value == expected),
                                                       It.IsAny<List<LineItem>>(),
                                                       It.IsAny<bool>()), Times.Once);
            }

            [Fact]
            public void EveryCall_CallsRepositoryToCreateOrUpdateCartOnceWithBasketHavingExpectedName()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                ShoppingCart testCart = new ShoppingCart
                {
                    Active = true,
                    BranchId = "FUT",
                    CartId = new Guid("dddddddddddddddddddddddddddddddd"),
                    Name = "Fake Cart Name",
                    Items = new List<ShoppingCartItem> {
                        new ShoppingCartItem {
                            ItemNumber = "123456",
                            CatalogId = "FUT"
                        }
                    }
                };
                var expected = string.Format("s{0}_{1}_{2}", 
                                             "fut", 
                                             "234567", 
                                             Regex.Replace("Fake Cart Name", @"\s+", ""));

                // act
                var result = testunit.CreateCart(fakeUser, testContext, testCart);

                // assert - verify that createorupdatebasket is called once where the lineposition of the first item is "1"
                mockDependents.BasketRepository.Verify(m => m.CreateOrUpdateBasket(It.IsAny<Guid>(),
                                                       It.IsAny<string>(),
                                                       It.Is<Basket>(r => r.Name == expected),
                                                       It.IsAny<List<LineItem>>(),
                                                       It.IsAny<bool>()), Times.Once);
            }

            [Fact]
            public void EveryCall_CallsRepositoryToCreateOrUpdateCartOnceWithBasketHavingExpectedSharedValue()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                ShoppingCart testCart = new ShoppingCart
                {
                    Active = true,
                    BranchId = "FUT",
                    CartId = new Guid("dddddddddddddddddddddddddddddddd"),
                    Name = "Fake Cart Name",
                    Items = new List<ShoppingCartItem> {
                        new ShoppingCartItem {
                            ItemNumber = "123456",
                            CatalogId = "FUT"
                        }
                    }
                };
                var expected = true;

                // act
                var result = testunit.CreateCart(fakeUser, testContext, testCart);

                // assert - verify that createorupdatebasket is called once where the lineposition of the first item is "1"
                mockDependents.BasketRepository.Verify(m => m.CreateOrUpdateBasket(It.IsAny<Guid>(),
                                                       It.IsAny<string>(),
                                                       It.Is<Basket>(r => r.Shared.Value == expected),
                                                       It.IsAny<List<LineItem>>(),
                                                       It.IsAny<bool>()), Times.Once);
            }

            [Fact]
            public void EveryCall_CallsRepositoryToCreateOrUpdateCartOnceWithBasketHavingExpectedTempSubtotal()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                ShoppingCart testCart = new ShoppingCart
                {
                    Active = true,
                    BranchId = "FUT",
                    CartId = new Guid("dddddddddddddddddddddddddddddddd"),
                    Name = "Fake Cart Name",
                    SubTotal = 1,
                    Items = new List<ShoppingCartItem> {
                        new ShoppingCartItem {
                            ItemNumber = "123456",
                            CatalogId = "FUT"
                        }
                    }
                };
                var expected = (decimal)1.00;

                // act
                var result = testunit.CreateCart(fakeUser, testContext, testCart);

                // assert - verify that createorupdatebasket is called once where the lineposition of the first item is "1"
                mockDependents.BasketRepository.Verify(m => m.CreateOrUpdateBasket(It.IsAny<Guid>(),
                                                       It.IsAny<string>(),
                                                       It.Is<Basket>(r => r.TempSubTotal.Value == expected),
                                                       It.IsAny<List<LineItem>>(),
                                                       It.IsAny<bool>()), Times.Once);
            }

            [Fact]
            public void EveryCall_CallsRepositoryToCreateOrUpdateCartOnceWithBasketHavingExpectedPONumber()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                ShoppingCart testCart = new ShoppingCart
                {
                    Active = true,
                    BranchId = "FUT",
                    CartId = new Guid("dddddddddddddddddddddddddddddddd"),
                    Name = "Fake Cart Name",
                    SubTotal = 1,
                    PONumber = "Test PO",
                    Items = new List<ShoppingCartItem> {
                        new ShoppingCartItem {
                            ItemNumber = "123456",
                            CatalogId = "FUT"
                        }
                    }
                };
                var expected = "Test PO";

                // act
                var result = testunit.CreateCart(fakeUser, testContext, testCart);

                // assert - verify that createorupdatebasket is called once where the lineposition of the first item is "1"
                mockDependents.BasketRepository.Verify(m => m.CreateOrUpdateBasket(It.IsAny<Guid>(),
                                                       It.IsAny<string>(),
                                                       It.Is<Basket>(r => r.PONumber == expected),
                                                       It.IsAny<List<LineItem>>(),
                                                       It.IsAny<bool>()), Times.Once);
            }

            [Fact]
            public void EveryCall_CallsRepositoryToCreateOrUpdateCartOnceWithBasketHavingExpectedRequestedShipDate()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                ShoppingCart testCart = new ShoppingCart
                {
                    Active = true,
                    BranchId = "FUT",
                    CartId = new Guid("dddddddddddddddddddddddddddddddd"),
                    Name = "Fake Cart Name",
                    SubTotal = 1,
                    PONumber = "Test PO",
                    RequestedShipDate = "1/1/1970".ToFormattedDateString(),
                    Items = new List<ShoppingCartItem> {
                        new ShoppingCartItem {
                            ItemNumber = "123456",
                            CatalogId = "FUT"
                        }
                    }
                };
                var expected = "1/1/1970".ToFormattedDateString();

                // act
                var result = testunit.CreateCart(fakeUser, testContext, testCart);

                // assert - verify that createorupdatebasket is called once where the lineposition of the first item is "1"
                mockDependents.BasketRepository.Verify(m => m.CreateOrUpdateBasket(It.IsAny<Guid>(),
                                                       It.IsAny<string>(),
                                                       It.Is<Basket>(r => r.RequestedShipDate == expected),
                                                       It.IsAny<List<LineItem>>(),
                                                       It.IsAny<bool>()), Times.Once);
            }

            [Fact]
            public void CartWithGoodItemAtPosition5_CartCreatedWithItemAtLinePosition1()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                ShoppingCart testCart = new ShoppingCart
                {
                    Active = true,
                    BranchId = "FUT",
                    CartId = new Guid("dddddddddddddddddddddddddddddddd"),
                    Name = "Fake Cart Name",
                    Items = new List<ShoppingCartItem> {
                        new ShoppingCartItem {
                            ItemNumber = "123456",
                            CatalogId = "FUT",
                            Position = 5 // passing in a position of 5. This test fails if this is not renumbered to 1
                        }
                    }
                };

                // act
                var result = testunit.CreateCart(fakeUser, testContext, testCart);

                // assert - verify that createorupdatebasket is called once where the lineposition of the first item is "1"
                mockDependents.BasketRepository.Verify(m => m.CreateOrUpdateBasket(It.IsAny<Guid>(),
                                                       It.IsAny<string>(),
                                                       It.IsAny<Basket>(),
                                                       It.Is<List<LineItem>>(r => r.First().LinePosition == "1"),
                                                       It.IsAny<bool>()), Times.Once);
            }

            [Fact]
            public void EveryCallWithBasketThatHasListId_CallsOrderedFromListRepositoryOnce()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                ShoppingCart testCart = new ShoppingCart
                {
                    Active = true,
                    BranchId = "FUT",
                    CartId = new Guid("dddddddddddddddddddddddddddddddd"),
                    Name = "Fake Cart Name",
                    ListType = ListType.Contract,
                    ListId = 1,
                    Items = new List<ShoppingCartItem> {
                        new ShoppingCartItem {
                            ItemNumber = "123456",
                            CatalogId = "FUT",
                            OrderedFromSource = "Test List"
                        }
                    }
                };
                var expected = "fut";

                // act
                var result = testunit.CreateCart(fakeUser, testContext, testCart);

                // assert - verify that createorupdatebasket is called once where the lineposition of the first item is "1"
                mockDependents.OrderedFromListRepository.Verify(m => m.Write(It.IsAny<OrderedFromList>()), Times.Once);
            }

            [Fact]
            public void EveryCallWithBasketThatDoesntHaveListId_DoesNotCallsOrderedFromListRepository()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                ShoppingCart testCart = new ShoppingCart
                {
                    Active = true,
                    BranchId = "FUT",
                    CartId = new Guid("dddddddddddddddddddddddddddddddd"),
                    Name = "Fake Cart Name",
                    Items = new List<ShoppingCartItem> {
                        new ShoppingCartItem {
                            ItemNumber = "123456",
                            CatalogId = "FUT",
                            OrderedFromSource = "Test List"
                        }
                    }
                };
                var expected = "fut";

                // act
                var result = testunit.CreateCart(fakeUser, testContext, testCart);

                // assert - verify that createorupdatebasket is called once where the lineposition of the first item is "1"
                mockDependents.OrderedFromListRepository.Verify(m => m.Write(It.IsAny<OrderedFromList>()), Times.Never);
            }

            [Fact]
            public void CallWithCartWithOneItemHavingSourceList_CallsOrderedItemsFromListRepositoryOnce()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                ShoppingCart testCart = new ShoppingCart
                {
                    Active = true,
                    BranchId = "FUT",
                    CartId = new Guid("dddddddddddddddddddddddddddddddd"),
                    Name = "Fake Cart Name",
                    Items = new List<ShoppingCartItem> {
                        new ShoppingCartItem {
                            ItemNumber = "123456",
                            CatalogId = "FUT",
                            OrderedFromSource = "Test List"
                        }
                    }
                };
                var expected = "fut";

                // act
                var result = testunit.CreateCart(fakeUser, testContext, testCart);

                // assert - verify that createorupdatebasket is called once where the lineposition of the first item is "1"
                mockDependents.OrderedItemsFromListRepository.Verify(m => m.Write(It.IsAny<OrderedItemFromList>()), Times.Once);
            }

            [Fact]
            public void CallWithCartWithOneItemNotHavingSourceList_CallsOrderedItemsFromListRepositoryZeroTimes()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(false, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                ShoppingCart testCart = new ShoppingCart
                {
                    Active = true,
                    BranchId = "FUT",
                    CartId = new Guid("dddddddddddddddddddddddddddddddd"),
                    Name = "Fake Cart Name",
                    Items = new List<ShoppingCartItem> {
                        new ShoppingCartItem {
                            ItemNumber = "123456",
                            CatalogId = "FUT"
                        }
                    }
                };
                var expected = "fut";

                // act
                var result = testunit.CreateCart(fakeUser, testContext, testCart);

                // assert - verify that createorupdatebasket is called once where the lineposition of the first item is "1"
                mockDependents.OrderedItemsFromListRepository.Verify(m => m.Write(It.IsAny<OrderedItemFromList>()), Times.Never);
            }

        }

        public class ReadCart
        {
            [Fact]
            public void CallForCartWithGoodId_CartWithNameIsExpected()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(true, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                Guid testCartId = new Guid(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                string expected = "Fake Name";

                // act
                var result = testunit.ReadCart(fakeUser, testContext, testCartId);

                // assert
                result.Name
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void CallForCartWithGoodId_ActiveCartIsExpected()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(true, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                Guid testCartId = new Guid(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                bool expected = true;

                // act
                var result = testunit.ReadCart(fakeUser, testContext, testCartId);

                // assert
                result.Active
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void CallForCartWithGoodId_CartHavingIdIsExpected()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(true, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                Guid testCartId = new Guid(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                bool expected = true;

                // act
                var result = testunit.ReadCart(fakeUser, testContext, testCartId);

                // assert
                result.Active
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void CallForCartWithGoodId_CartHavingNameIsExpected()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(true, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                Guid testCartId = new Guid(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                string expected = "Fake Name";

                // act
                var result = testunit.ReadCart(fakeUser, testContext, testCartId);

                // assert
                result.Name
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void CallForCartWithGoodId_CartHavingShippingDateIsExpected()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(true, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                Guid testCartId = new Guid(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                string expected = "1/1/2017";

                // act
                var result = testunit.ReadCart(fakeUser, testContext, testCartId);

                // assert
                result.RequestedShipDate
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void CallForCartWithGoodId_CartHavingBranchIsExpected()
            {
                // arrange
                MockDependents mockDependents = new MockDependents();
                IShoppingCartLogic testunit = MakeTestsLogic(true, ref mockDependents);
                UserProfile fakeUser = new UserProfile();
                UserSelectedContext testContext = new UserSelectedContext
                {
                    BranchId = "FUT",
                    CustomerId = "234567"
                };
                Guid testCartId = new Guid(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                string expected = "FUT";

                // act
                var result = testunit.ReadCart(fakeUser, testContext, testCartId);

                // assert
                result.BranchId
                      .Should()
                      .Be(expected);
            }

        }

        #region Setup
        public class MockDependents {
            public Mock<ICacheRepository> CacheRepository { get; set; }

            public Mock<ICustomerRepository> CustomerRepository { get; set; }

            public Mock<IBasketRepository> BasketRepository { get; set; }

            public Mock<ICatalogLogic> CatalogLogic { get; set; }

            public Mock<IPriceLogic> PriceLogic { get; set; }

            public Mock<IPurchaseOrderRepository> PurchaseOrderRepository { get; set; }

            public Mock<IGenericQueueRepository> GenericQueueRepository { get; set; }

            public Mock<IBasketLogic> BasketLogic { get; set; }

            public Mock<INotesListLogic> NotesListLogic { get; set; }

            public Mock<IOrderQueueLogic> OrderQueueLogic { get; set; }

            public Mock<IOrderHistoryLogic> OrderHistoryLogic { get; set; }

            public Mock<IAuditLogRepository> AuditLogRepository { get; set; }

            public Mock<IEventLogRepository> EventLogRepository { get; set; }

            public Mock<IUserActiveCartLogic> UserActiveCartLogic { get; set; }

            public Mock<IExternalCatalogRepository> ExternalCatalogRepository { get; set; }

            public Mock<IOrderedFromListRepository> OrderedFromListRepository { get; set; }

            public Mock<IOrderedItemsFromListRepository> OrderedItemsFromListRepository { get; set; }

            public static void RegisterInContainer(ref ContainerBuilder cb) {
                cb.RegisterInstance(MakeICacheRepository().Object)
                  .As<ICacheRepository>();
                cb.RegisterInstance(MakeICustomerRepository().Object)
                  .As<ICustomerRepository>();
                cb.RegisterInstance(MakeIBasketRepository().Object)
                  .As<IBasketRepository>();
                cb.RegisterInstance(MakeICatalogLogic().Object)
                  .As<ICatalogLogic>();
                cb.RegisterInstance(MakeIPriceLogic().Object)
                  .As<IPriceLogic>();
                cb.RegisterInstance(MakeIPurchaseOrderRepository().Object)
                  .As<IPurchaseOrderRepository>();
                cb.RegisterInstance(MakeIGenericQueueRepository().Object)
                  .As<IGenericQueueRepository>();
                cb.RegisterInstance(MakeIBasketLogic().Object)
                  .As<IBasketLogic>();
                cb.RegisterInstance(MakeINotesListLogic().Object)
                  .As<INotesListLogic>();
                cb.RegisterInstance(MakeIOrderQueueLogic().Object)
                  .As<IOrderQueueLogic>();
                cb.RegisterInstance(MakeIOrderHistoryLogic().Object)
                  .As<IOrderHistoryLogic>();
                cb.RegisterInstance(MakeIAuditLogRepository().Object)
                  .As<IAuditLogRepository>();
                cb.RegisterInstance(MakeIEventLogRepository().Object)
                  .As<IEventLogRepository>();
                cb.RegisterInstance(MakeIUserActiveCartLogic().Object)
                  .As<IUserActiveCartLogic>();
                cb.RegisterInstance(MakeIExternalCatalogRepository().Object)
                  .As<IExternalCatalogRepository>();
                cb.RegisterInstance(MakeIOrderedFromListRepository().Object)
                  .As<IOrderedFromListRepository>();
                cb.RegisterInstance(MakeIOrderedItemsFromListRepository().Object)
                  .As<IOrderedItemsFromListRepository>();
            }

            public static Mock<ICacheRepository> MakeICacheRepository() {
                Mock<ICacheRepository> mock = new Mock<ICacheRepository>();

                return mock;
            }

            public static Mock<ICustomerRepository> MakeICustomerRepository() {
                Mock<ICustomerRepository> mock = new Mock<ICustomerRepository>();

                mock.Setup(f => f.GetCustomerByCustomerNumber(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(new Customer());

                return mock;
            }

            public static Mock<IBasketRepository> MakeIBasketRepository() {
                Mock<IBasketRepository> mock = new Mock<IBasketRepository>();

                mock.Setup(m => m.CreateOrUpdateBasket(It.IsAny<Guid>(),
                                                       It.IsAny<string>(),
                                                       It.IsAny<Basket>(),
                                                       It.IsAny<List<LineItem>>(), 
                                                       It.IsAny<bool>()))
                    .Returns(new Guid(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1));

                return mock;
            }

            public static Mock<ICatalogLogic> MakeICatalogLogic() {
                Mock<ICatalogLogic> mock = new Mock<ICatalogLogic>();

                mock.Setup(f => f.GetProductsByIds("fut", It.IsAny<List<string>>()))
                    .Returns(new ProductsReturn {
                        Products = new List<Product> {
                            new Product {
                                ItemNumber = "123456",
                                Name = "Fake Name",
                                BrandExtendedDescription = "Fake Brand",
                                ItemClass = "Fake Class",
                                Size = "Fake Size",
                                Pack = "Fake Pack"
                            }
                        }
                    });

                return mock;
            }

            public static Mock<IPriceLogic> MakeIPriceLogic() {
                Mock<IPriceLogic> mock = new Mock<IPriceLogic>();

                return mock;
            }

            public static Mock<IPurchaseOrderRepository> MakeIPurchaseOrderRepository() {
                Mock<IPurchaseOrderRepository> mock = new Mock<IPurchaseOrderRepository>();

                return mock;
            }

            public static Mock<IGenericQueueRepository> MakeIGenericQueueRepository() {
                Mock<IGenericQueueRepository> mock = new Mock<IGenericQueueRepository>();

                return mock;
            }

            public static Mock<IBasketLogic> MakeIBasketLogic() {
                Mock<IBasketLogic> mock = new Mock<IBasketLogic>();

                Basket returnedBasket = new Basket {
                    Active = true,
                    Id = new Guid(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1).ToString(),
                    DisplayName = "Fake Name",
                    BranchId = "FUT",
                    RequestedShipDate = "1/1/2017",
                    ListType = (int)BasketType.Cart,
                    TempSubTotal = 0,
                    ReadOnly = false
                };
                mock.Setup(f => f.RetrieveSharedCustomerBasket(It.IsAny<UserProfile>(), It.IsAny<UserSelectedContext>(), It.IsAny<Guid>()))
                    .Returns(returnedBasket);

                return mock;
            }

            public static Mock<INotesListLogic> MakeINotesListLogic() {
                Mock<INotesListLogic> mock = new Mock<INotesListLogic>();

                return mock;
            }

            public static Mock<IOrderQueueLogic> MakeIOrderQueueLogic() {
                Mock<IOrderQueueLogic> mock = new Mock<IOrderQueueLogic>();

                return mock;
            }

            public static Mock<IOrderHistoryLogic> MakeIOrderHistoryLogic() {
                Mock<IOrderHistoryLogic> mock = new Mock<IOrderHistoryLogic>();

                return mock;
            }

            public static Mock<IAuditLogRepository> MakeIAuditLogRepository() {
                Mock<IAuditLogRepository> mock = new Mock<IAuditLogRepository>();

                return mock;
            }

            public static Mock<IEventLogRepository> MakeIEventLogRepository() {
                Mock<IEventLogRepository> mock = new Mock<IEventLogRepository>();

                return mock;
            }

            public static Mock<IUserActiveCartLogic> MakeIUserActiveCartLogic() {
                Mock<IUserActiveCartLogic> mock = new Mock<IUserActiveCartLogic>();

                mock.Setup(f => f.GetUserActiveCart(It.IsAny<UserSelectedContext>(), It.IsAny<Guid>()))
                    .Returns(new UserActiveCartModel() {
                                                         CartId = new Guid(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1),
                                                         UserId = new Guid(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1) });

                return mock;
            }

            public static Mock<IExternalCatalogRepository> MakeIExternalCatalogRepository() {
                Mock<IExternalCatalogRepository> mock = new Mock<IExternalCatalogRepository>();

                return mock;
            }

            public static Mock<IOrderedFromListRepository> MakeIOrderedFromListRepository() {
                Mock<IOrderedFromListRepository> mock = new Mock<IOrderedFromListRepository>();

                return mock;
            }

            public static Mock<IOrderedItemsFromListRepository> MakeIOrderedItemsFromListRepository()
            {
                Mock<IOrderedItemsFromListRepository> mock = new Mock<IOrderedItemsFromListRepository>();

                mock.Setup(f => f.Write(It.IsAny<OrderedItemFromList>()));

                return mock;
            }
        }

        private static IShoppingCartLogic MakeTestsLogic(bool useAutoFac, ref MockDependents mockDependents) {
            if (useAutoFac) {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                MockDependents.RegisterInContainer(ref cb);

                IContainer testcontainer = cb.Build();

                return testcontainer.Resolve<IShoppingCartLogic>();
            }
            mockDependents = new MockDependents();
            mockDependents.AuditLogRepository = MockDependents.MakeIAuditLogRepository();
            mockDependents.BasketLogic = MockDependents.MakeIBasketLogic();
            mockDependents.BasketRepository = MockDependents.MakeIBasketRepository();
            mockDependents.CacheRepository = MockDependents.MakeICacheRepository();
            mockDependents.CatalogLogic = MockDependents.MakeICatalogLogic();
            mockDependents.CustomerRepository = MockDependents.MakeICustomerRepository();
            mockDependents.EventLogRepository = MockDependents.MakeIEventLogRepository();
            mockDependents.ExternalCatalogRepository = MockDependents.MakeIExternalCatalogRepository();
            mockDependents.GenericQueueRepository = MockDependents.MakeIGenericQueueRepository();
            mockDependents.NotesListLogic = MockDependents.MakeINotesListLogic();
            mockDependents.OrderHistoryLogic = MockDependents.MakeIOrderHistoryLogic();
            mockDependents.OrderQueueLogic = MockDependents.MakeIOrderQueueLogic();
            mockDependents.OrderedFromListRepository = MockDependents.MakeIOrderedFromListRepository();
            mockDependents.OrderedItemsFromListRepository = MockDependents.MakeIOrderedItemsFromListRepository();
            mockDependents.PriceLogic = MockDependents.MakeIPriceLogic();
            mockDependents.PurchaseOrderRepository = MockDependents.MakeIPurchaseOrderRepository();
            mockDependents.UserActiveCartLogic = MockDependents.MakeIUserActiveCartLogic();

            ShoppingCartLogicImpl testunit = new ShoppingCartLogicImpl(mockDependents.BasketRepository.Object, mockDependents.CatalogLogic.Object, mockDependents.PriceLogic.Object,
                                                                       mockDependents.OrderQueueLogic.Object, mockDependents.PurchaseOrderRepository.Object, mockDependents.GenericQueueRepository.Object,
                                                                       mockDependents.BasketLogic.Object, mockDependents.OrderHistoryLogic.Object, mockDependents.OrderedItemsFromListRepository.Object, 
                                                                       mockDependents.CustomerRepository.Object,
                                                                       mockDependents.AuditLogRepository.Object, mockDependents.NotesListLogic.Object, mockDependents.UserActiveCartLogic.Object,
                                                                       mockDependents.ExternalCatalogRepository.Object, mockDependents.CacheRepository.Object, mockDependents.EventLogRepository.Object,
                                                                       mockDependents.OrderedFromListRepository.Object, null);
            return testunit;
        }
        #endregion Setup
    }
}