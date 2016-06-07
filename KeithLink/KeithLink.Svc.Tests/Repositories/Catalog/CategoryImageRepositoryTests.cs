using KeithLink.Common.Impl.Repository.Logging;

using KeithLink.Svc.Impl.Repository.SiteCatalog;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Repositories.Catalog
{
    [TestClass]
    public class CategoryImageRepostiroyTests
    {
        [TestMethod]
        public void CategoryShouldReturnImageInformation()
        {
            string categoryId = "ap000";

            CategoryImageRepository repo = new CategoryImageRepository(new EventLogRepositoryImpl("KeithLink Tests"));

            Assert.IsTrue(repo.GetImageByCategory(categoryId).CategoryImage.FileName.Contains(categoryId));
        }
    }
}
