using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeithLink.Svc.Core;
using KeithLink.Svc.Impl.Repository.SiteCatalog;

namespace KeithLink.Svc.Test.Repositories.Catalog
{
    [TestClass]
    public class CategoryImageRepostiroyTests
    {
        [TestMethod]
        public void CategoryShouldReturnImageInformation()
        {
            string categoryId = "ap000";

            CategoryImageRepository repo = new CategoryImageRepository(new KeithLink.Common.Impl.Logging.EventLogRepositoryImpl("KeithLink Tests"));

            Assert.IsTrue(repo.GetImageByCategory(categoryId).CategoryImage.FileName.Contains(categoryId));
        }
    }
}
