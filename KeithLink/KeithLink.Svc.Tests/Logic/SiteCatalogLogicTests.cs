using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeithLink.Svc.Impl.Logic;
using KeithLink.Svc.Core.Interface;
using KeithLink.Svc.Impl.Repository.SiteCatalog;
using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Impl.Repository.Profile.Cache;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Test.MockRepository;
using KeithLink.Svc.Impl.Repository.Lists;

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
				new ListServiceRepositoryImpl(new KeithLink.Svc.Impl.com.benekeith.ListService.ListServcieClient()),
                new DivisionRepositoryImpl(),
                new CategoryImageRepository(new KeithLink.Common.Impl.Logging.EventLogRepositoryImpl("KeithLink Tests")),
                new NoCacheCatalogCacheRepositoryImpl()
                );
        }
        #endregion

        [TestMethod]
        public void CategoryImageShouldBeAddedToCategories()
        {
			//TODO: Add service endpoint to config
           // Assert.IsTrue(_logic.GetCategories(0, 100).Categories[0].CategoryImage.FileName.Length > 0);
        }
    }
}
