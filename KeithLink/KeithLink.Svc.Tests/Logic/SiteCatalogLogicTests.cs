using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeithLink.Svc.Impl.Logic;
using KeithLink.Svc.Core.Interface;
using KeithLink.Svc.Impl.Repository.SiteCatalog;

namespace KeithLink.Svc.Test.Logic
{
    [TestClass]
    public class SiteCatalogLogicTests
    {
        private SiteCatalogLogicImpl _logic;

        #region constructor
        public SiteCatalogLogicTests()
        {
            _logic = new SiteCatalogLogicImpl(
                new ElasticSearchCatalogRepositoryImpl(),
                new PriceLogicImpl(new PriceRepositoryImpl(), new NoCachePriceCacheRepositoryImpl()),
                new ProductImageRepositoryImpl(),
                new ListLogicImpl(new Impl.Repository.Orders.BasketRepositoryImpl(), new ElasticSearchCatalogRepositoryImpl(), new PriceRepositoryImpl()),
                new DivisionRepositoryImpl(),
                new CategoryImageRepository()
                );
        }
        #endregion

        [TestMethod]
        public void CategoryImageShouldBeAddedToCategories()
        {
            Assert.IsTrue(_logic.GetCategories(0, 100).Categories[0].CategoryImage.FileName.Length > 0);
        }
    }
}
