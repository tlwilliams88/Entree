using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using FluentAssertions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Impl.Repository.SmartResolver;
using Xunit;
using KeithLink.Svc.Core.Interface.Customers;
using KeithLink.Svc.Core.Models.ShoppingCart;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Customer
{
    public class RecommendedItemRepositoryTests
    {
        private static IRecommendedItemsRepository MakeRepo()
        {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return diMap.Resolve<IRecommendedItemsRepository>();
        }

        public class GetRecommendedItemsForCustomer : MigratedDatabaseTest
        {
            [Fact]
            public void BadCustomerNoCart_ResultsNotNull()
            {
                // arrange
                var customerNumber = "654321";
                var branch = "FUT";
                var repo = MakeRepo();

                // act
                var results = repo.GetRecommendedItemsForCustomer(customerNumber, branch, null);

                // assert
                results.Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodCustomerNoCart_ResultsNotNull()
            {
                // arrange
                var customerNumber = "123456";
                var branch = "FUT";
                var repo = MakeRepo();

                // act
                var results = repo.GetRecommendedItemsForCustomer(customerNumber, branch, null);

                // assert
                results.Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodCustomerCartWithMatchingItemNumber_ResultsNotNull()
            {
                // arrange
                var customerNumber = "123456";
                var branch = "FUT";
                var cartitems = new List<ShoppingCartItem>() {
                    new ShoppingCartItem() {
                        ItemNumber = "111111"
                    }
                };
                var repo = MakeRepo();

                // act
                var results = repo.GetRecommendedItemsForCustomer(customerNumber, branch, cartitems);

                // assert
                results.Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodCustomerCartWithMatchingItemNumber_ResultsItemNumbersDontContainCartItemNumbers()
            {
                // arrange
                var customerNumber = "123456";
                var branch = "FUT";
                var cartitems = new List<ShoppingCartItem>() {
                    new ShoppingCartItem() {
                        ItemNumber = "111111"
                    }
                };
                var repo = MakeRepo();

                // act
                var results = repo.GetRecommendedItemsForCustomer(customerNumber, branch, cartitems);

                // assert
                results.Select(r => r.ItemNumber)
                       .ToList()
                       .Should()
                       .NotContain(cartitems.Select(i => i.ItemNumber).ToList());
            }

            [Fact]
            public void GoodCustomerCartWithMatchingItemNumber_ResultsRecommendedItemsDontContainCartItemNumbers()
            {
                // arrange
                var customerNumber = "123456";
                var branch = "FUT";
                var cartitems = new List<ShoppingCartItem>() {
                    new ShoppingCartItem() {
                        ItemNumber = "111111"
                    }
                };
                var repo = MakeRepo();

                // act
                var results = repo.GetRecommendedItemsForCustomer(customerNumber, branch, cartitems);

                // assert
                results.Select(r => r.RecommendedItem)
                       .ToList()
                       .Should()
                       .NotContain(cartitems.Select(i => i.ItemNumber).ToList());
            }

            [Fact]
            public void GoodCustomerCartWithMatchingItemNumberRequesting2_Resultsin2()
            {
                // arrange
                var customerNumber = "123456";
                var branch = "FUT";
                var cartitems = new List<ShoppingCartItem>() {
                    new ShoppingCartItem() {
                        ItemNumber = "111111"
                    }
                };
                var repo = MakeRepo();
                var expected = 2;

                // act
                var results = repo.GetRecommendedItemsForCustomer(customerNumber, branch, cartitems, expected);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }
        }
    }
}
