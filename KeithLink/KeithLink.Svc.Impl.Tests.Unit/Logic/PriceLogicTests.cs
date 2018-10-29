
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

namespace KeithLink.Svc.Impl.Tests.Unit.Logic
{
    public class PriceLogicTests : BaseDITests
    {

        public class GetPrices
        {
            [Fact]
            public void ReturnsPrices()
            {
                // arrange
                string branchId = "FUT";
                string customerId = "234567";
                DateTime shipDate = DateTime.Now;

                var products = new List<Product>
                {
                    new Product { ItemNumber = "111111", },
                    new Product { ItemNumber = "222222", },
                    new Product { ItemNumber = "333333", },
                };

                MockDependents mockDependents = new MockDependents(branchId, customerId);

                // act
                IPriceLogic testunit = MakeUnitToBeTested(true, ref mockDependents);
                var priceReturn = testunit.GetPrices(branchId, customerId, shipDate, products);

                // assert
                priceReturn.Prices.Count.Should().Be(2);
            }
        }

        public class GetNonBekItemPrices
        {
            [Fact]
            public void ReturnsPrices()
            {
                // arrange
                string source = "";
                string branchId = "FUT";
                string customerId = "234567";
                DateTime shipDate = DateTime.Now;

                var products = new List<Product>
                {
                    new Product { ItemNumber = "111111", },
                    new Product { ItemNumber = "222222", },
                    new Product { ItemNumber = "333333", },
                };

                MockDependents mockDependents = new MockDependents(branchId, customerId);

                // act
                IPriceLogic testunit = MakeUnitToBeTested(true, ref mockDependents);
                var priceReturn = testunit.GetNonBekItemPrices(branchId, customerId, source, shipDate, products);

                // assert
                priceReturn.Prices.Count.Should().Be(2);
            }
        }

        public class GetPrice
        {
            [Fact]
            public void ReturnsPrice()
            {
                // arrange
                string branchId = "FUT";
                string customerId = "234567";
                DateTime shipDate = DateTime.Now;

                var product = new Product { ItemNumber = "333333", };

                MockDependents mockDependents = new MockDependents(branchId, customerId);

                // act
                IPriceLogic testunit = MakeUnitToBeTested(true, ref mockDependents);
                var price = testunit.GetPrice(branchId, customerId, shipDate, product);

                // assert
                price.CasePrice.Should().Be(3.33d);
            }
        }

        #region Setup
        public class MockDependents
        {
            public Mock<IPriceRepository> MockPriceRepository { get; set; }
            public Mock<ICacheRepository> MockCacheRepository { get; set; }
            public Mock<IEventLogRepository> MockEventLogRepository { get; set; }

            public MockDependents(string branchId, string customerId)
            {
                MockPriceRepository = new Mock<IPriceRepository>();
                MockCacheRepository = new Mock<ICacheRepository>();
                MockEventLogRepository = new Mock<IEventLogRepository>();

                MockPriceRepository
                    .Setup(m => m.GetPrices(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<List<Product>>()))
                    .Returns(BuildMockPrices(branchId, customerId));

                string cacheKey = GetCacheKey(branchId, customerId, "333333");
                MockCacheRepository
                    .Setup(m => m.GetItem<Price>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.Is<string>(parm => parm == cacheKey)))
                    .Returns(BuildMockPrice(branchId, customerId));

                MockCacheRepository
                    .Setup(m => m.AddItem(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<Price>()));

            }

            private Price BuildMockPrice(string branchId, string customerId)
            {
                var price = new Price { BranchId = branchId, CustomerNumber = customerId, ItemNumber = "333333", CasePrice = 3.33d, PackagePrice = 1.0d };
                return price;
            }

            private List<Price> BuildMockPrices(string branchId, string customerId)
            {
                var prices = new List<Price>
                {
                    new Price { BranchId = branchId, CustomerNumber = customerId, ItemNumber = "111111", CasePrice = 1.11d, PackagePrice = 1.0d },
                    new Price { BranchId = branchId, CustomerNumber = customerId, ItemNumber = "222222", CasePrice = 2.22d, PackagePrice = 1.0d },
                };
                return prices;
            }

            private string GetCacheKey(string branchId, string customerNumber, string itemNumber)
            {
                return string.Format("{0}-{1}-{2}", branchId, customerNumber, itemNumber);
            }
        }

        private static IPriceLogic MakeUnitToBeTested(bool useAutoFac, ref MockDependents mockDependents)
        {
            if (useAutoFac)
            {
                ContainerBuilder cb = GetTestsContainer();

                // Register mocks
                RegisterInContainer(ref cb, mockDependents);

                IContainer testcontainer = cb.Build();

                return testcontainer.Resolve<IPriceLogic>();
            }

            PriceLogicImpl testunit = new PriceLogicImpl(
                mockDependents.MockPriceRepository.Object, 
                mockDependents.MockCacheRepository.Object 
            );

            return testunit;
        }

        public static void RegisterInContainer(ref ContainerBuilder cb, MockDependents mockDependents)
        {
            cb.RegisterInstance(mockDependents.MockPriceRepository.Object)
              .As<IPriceRepository>();

            cb.RegisterInstance(mockDependents.MockCacheRepository.Object)
              .As<ICacheRepository>();
        }

        #endregion Setup
    }
}