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
using KeithLink.Svc.Core.Models.Customers;
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
                var parms = new RecommendedItemsParametersModel() {
                    CustomerNumber = "654321",
                    BranchId = "FUT",
                    CartItemsList = null
                };
                var repo = MakeRepo();

                // act
                var results = repo.GetRecommendedItemsForCustomer(parms);

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
                var parms = new RecommendedItemsParametersModel()
                {
                    CustomerNumber = "654321",
                    BranchId = "FUT",
                    CartItemsList = null
                };
                var repo = MakeRepo();

                // act
                var results = repo.GetRecommendedItemsForCustomer(parms);

                // assert
                results.Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodCustomerCartWithMatchingItemNumber_ResultsNotNull()
            {
                // arrange
                var parms = new RecommendedItemsParametersModel()
                {
                    CustomerNumber = "123456",
                    BranchId = "FUT",
                    CartItemsList = new List<string>() { "111111" }
                };
                var repo = MakeRepo();

                // act
                var results = repo.GetRecommendedItemsForCustomer(parms);

                // assert
                results.Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodCustomerCartWithMatchingItemNumber_ResultsItemNumbersDontContainCartItemNumbers()
            {
                // arrange
                var parms = new RecommendedItemsParametersModel()
                {
                    CustomerNumber = "123456",
                    BranchId = "FUT",
                    CartItemsList = new List<string>() { "111111" }
                };
                var repo = MakeRepo();

                // act
                var results = repo.GetRecommendedItemsForCustomer(parms);

                // assert
                results.Select(r => r.ItemNumber)
                       .ToList()
                       .Should()
                       .NotContain(parms.CartItemsList);
            }

            [Fact]
            public void GoodCustomerCartWithMatchingItemNumber_ResultsRecommendedItemsDontContainCartItemNumbers()
            {
                // arrange
                var parms = new RecommendedItemsParametersModel()
                {
                    CustomerNumber = "123456",
                    BranchId = "FUT",
                    CartItemsList = new List<string>() { "111111" }
                };
                var repo = MakeRepo();

                // act
                var results = repo.GetRecommendedItemsForCustomer(parms);

                // assert
                results.Select(r => r.RecommendedItem)
                       .ToList()
                       .Should()
                       .NotContain(parms.CartItemsList);
            }

            [Fact]
            public void GoodCustomerCartWithMatchingItemNumberRequesting2_Resultsin2()
            {
                // arrange
                var expected = 2;
                var parms = new RecommendedItemsParametersModel()
                {
                    CustomerNumber = "123456",
                    BranchId = "FUT",
                    CartItemsList = new List<string>() { "111111" }
                };
                var repo = MakeRepo();

                // act
                var results = repo.GetRecommendedItemsForCustomer(parms);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomerCartWithMatchingItemNumberRequestingNoSetNumber_Resultsin4()
            {
                // arrange
                var expected = 4;
                var parms = new RecommendedItemsParametersModel()
                {
                    CustomerNumber = "123456",
                    BranchId = "FUT",
                    CartItemsList = new List<string>() { "111111" }
                };
                var repo = MakeRepo();

                // act
                var results = repo.GetRecommendedItemsForCustomer(parms);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }
        }
    }
}
