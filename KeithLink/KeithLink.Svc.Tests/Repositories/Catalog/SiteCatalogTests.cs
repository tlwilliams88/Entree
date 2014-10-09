using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeithLink.Svc.Impl.Repository.SiteCatalog;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Logic;

namespace KeithLink.Svc.Test.Repositories.Catalog
{
    [TestClass]
    public class SiteCatalogTests
    {
        // A request for products by house brand should be served from ElasticSearch by the repository
        // it should always return a count of more than 0
        [TestMethod]
        public void CatalogShouldReturnProductsForHouseBrandByControlLabel()
        {
            // A branch and control label are used for getting the information needed
            string branchId = "fdf";
            string controlLabel = "kp";
            SearchInputModel searchModel = new SearchInputModel() { From = 0, Size = 500 };
            ElasticSearchCatalogRepositoryImpl siteCatalog = new ElasticSearchCatalogRepositoryImpl();

			Assert.IsTrue(siteCatalog.GetHouseProductsByBranch(new UserSelectedContext() { BranchId = branchId }, controlLabel, searchModel).Count > 0);
        }

    }
}
