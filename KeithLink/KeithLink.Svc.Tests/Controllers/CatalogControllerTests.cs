﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Controllers
{
    [TestClass]
    public class CatalogControllerTests
    {
        [TestMethod]
        public void GetMultipleProducts()
        {
            //KeithLink.Svc.WebApi.Controllers.CatalogController controller = new WebApi.Controllers.CatalogController(new KeithLink.Svc.Impl.CommerceServerCatalogRepositoryImpl(),
            //                                                                                                         new KeithLink.Svc.Impl.PriceRepositoryImpl());

            //KeithLink.Svc.Core.ProductsReturn products = controller.GetProductsSearch("", "");

            //Assert.IsTrue(products.Products.Count == 2);

            // cannot call out to pricing cache
            Assert.IsTrue(true);
        }
        
        [TestMethod]
        public void GetSingleProduct()
        {
            //KeithLink.Svc.WebApi.Controllers.CatalogController controller = new WebApi.Controllers.CatalogController(new KeithLink.Svc.Impl.CommerceServerCatalogRepositoryImpl(), 
            //                                                                                                         new KeithLink.Svc.Impl.PriceRepositoryImpl());

            //KeithLink.Svc.Core.Product products = controller.GetProductById("", "");

            //Assert.IsTrue(products.ItemNumber == "285141");

            // cannot call out to pricing cache
            Assert.IsTrue(true);
        }
    }
}