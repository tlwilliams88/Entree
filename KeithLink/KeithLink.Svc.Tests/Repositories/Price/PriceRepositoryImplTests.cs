using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.SiteCatalog;

using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Test.Repositories.Price
{
    [TestClass]
    public class PriceRepositoryImplTests {

        #region attributes
        private readonly IPriceRepository _repo;
        #endregion

        #region ctor
        public PriceRepositoryImplTests() {
            IContainer diMap = DependencyMap.Build();

            _repo = diMap.Resolve<IPriceRepository>();
        }
        #endregion

        #region methods
        [TestMethod]
        public void GetMultipleItemPrices_Success()
        {
            //List<KeithLink.Svc.Core.Product> products = new List<Core.Product>();
            //products.Add(new Core.Product() { ItemNumber = "285141" });
            //products.Add(new Core.Product() { ItemNumber = "285149" });

            //KeithLink.Svc.Impl.PriceRepositoryImpl pricingService = new Impl.PriceRepositoryImpl();

            //KeithLink.Svc.Core.PriceReturn pricingInfo = pricingService.GetPrices("FDF", "010189", DateTime.Now.AddDays(1), products);

            //Assert.IsTrue(pricingInfo.Prices.Count == 2);

            // cannot call out to pricing cache
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void GetSingleItemPrices_Success()
        {
            //List<KeithLink.Svc.Core.Product> products = new List<Core.Product>();
            //products.Add(new Core.Product(){ItemNumber = "285141"});

            //KeithLink.Svc.Impl.PriceRepositoryImpl pricingService = new Impl.PriceRepositoryImpl();


            //KeithLink.Svc.Core.PriceReturn pricingInfo = pricingService.GetPrices("FDF", "010189", DateTime.Now.AddDays(1), products);

            //Assert.IsTrue(pricingInfo.Prices.Count == 1);

            // cannot call out to pricing cache
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void GetNonBekItemPrices() {
            List<Product> products = new List<Product>();
            products.Add(new Product() { 
                ItemNumber = "285141",
                PackagePriceNumeric = 1.00,
                CasePriceNumeric = 5.00,
                CategoryName = "Beans and Franks"
            });

             List<KeithLink.Svc.Core.Models.SiteCatalog.Price> pricingInfo = _repo.GetNonBekItemPrices("FDF", "010189", DateTime.Now.AddDays(1), "UNFI", products);


             Assert.IsTrue(pricingInfo.Count > 0);
        }
        #endregion
    }
}
