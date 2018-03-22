using System.Collections.Generic;
using System.Linq;

using KeithLink.Svc.Core.Interface.Customers;
using KeithLink.Svc.Core.Models.Customers;
using KeithLink.Svc.Impl.Repository.SmartResolver;

using Autofac;
using FluentAssertions;
using Xunit;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Customer {
    public class RecommendedItemRepositoryTests {
        private static IRecommendedItemsRepository MakeRepo() {
            IContainer diMap = DependencyMapFactory.GetTestsContainer()
                                                   .Build();
            return diMap.Resolve<IRecommendedItemsRepository>();
        }

        public class GetRecommendedItemsForCustomer : MigratedDatabaseTest {
            [Fact]
            public void BadCustomerNoCart_ResultsNotNull() {
                // arrange
                string customerNumber = "654321";
                string branchId = "FUT";
                IRecommendedItemsRepository repo = MakeRepo();

                // act
                List<RecommendedItemsModel> results = repo.GetRecommendedItemsForCustomer(customerNumber, branchId);

                // assert
                results.Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodCustomerCartWithMatchingItemNumber_ResultsItemNumbersDontContainCartItemNumbers() {
                // arrange
                List<string> cartItemsList = new List<string> {"111111"};
                string customerNumber = "123456";
                string branchId = "FUT";
                IRecommendedItemsRepository repo = MakeRepo();

                // act
                List<RecommendedItemsModel> results = repo.GetRecommendedItemsForCustomer(customerNumber, branchId, cartItemsList);

                // assert
                results.Select(r => r.ItemNumber)
                       .ToList()
                       .Should()
                       .NotContain(cartItemsList);
            }

            [Fact]
            public void GoodCustomerCartWithMatchingItemNumber_ResultsNotNull() {
                // arrange
                List<string> cartItemsList = new List<string> {"111111"};
                string customerNumber = "123456";
                string branchId = "FUT";
                IRecommendedItemsRepository repo = MakeRepo();

                // act
                List<RecommendedItemsModel> results = repo.GetRecommendedItemsForCustomer(customerNumber, branchId, cartItemsList);

                // assert
                results.Should()
                       .NotBeNull();
            }

            [Fact]
            public void GoodCustomerCartWithMatchingItemNumber_ResultsRecommendedItemsDontContainCartItemNumbers() {
                // arrange
                List<string> cartItemsList = new List<string> {"111111"};
                string customerNumber = "123456";
                string branchId = "FUT";
                IRecommendedItemsRepository repo = MakeRepo();

                // act
                List<RecommendedItemsModel> results = repo.GetRecommendedItemsForCustomer(customerNumber, branchId, cartItemsList);

                // assert
                results.Select(r => r.RecommendedItem)
                       .ToList()
                       .Should()
                       .NotContain(cartItemsList);
            }

            [Fact]
            public void GoodCustomerCartWithMatchingItemNumberRequesting2_Resultsin2() {
                // arrange
                int expected = 2;
                List<string> cartItemsList = new List<string> {"111111"};
                string customerNumber = "123456";
                string branchId = "FUT";
                IRecommendedItemsRepository repo = MakeRepo();

                // act
                List<RecommendedItemsModel> results = repo.GetRecommendedItemsForCustomer(customerNumber, branchId, cartItemsList, expected);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomerCartWithMatchingItemNumberRequestingNoSetNumber_Resultsin4() {
                // arrange
                int expected = 4;
                List<string> cartItemsList = new List<string> {"111111"};
                string customerNumber = "123456";
                string branchId = "FUT";
                IRecommendedItemsRepository repo = MakeRepo();

                // act
                List<RecommendedItemsModel> results = repo.GetRecommendedItemsForCustomer(customerNumber, branchId, cartItemsList);

                // assert
                results.Count
                       .Should()
                       .Be(expected);
            }

            [Fact]
            public void GoodCustomerNoCart_ResultsNotNull() {
                // arrange
                string customerNumber = "123456";
                string branchId = "FUT";
                IRecommendedItemsRepository repo = MakeRepo();

                // act
                List<RecommendedItemsModel> results = repo.GetRecommendedItemsForCustomer(customerNumber, branchId);

                // assert
                results.Should()
                       .NotBeNull();
            }
        }
    }
}