using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test
{
    [TestClass]
    public class Impl_SiteCatalog_ProductImageRepository
    {
        [TestMethod]
        public void GetListOfImages()
        {
            KeithLink.Svc.Impl.Repository.SiteCatalog.ProductImageRepositoryImpl imgRepo = new Impl.Repository.SiteCatalog.ProductImageRepositoryImpl();

            KeithLink.Svc.Core.Models.SiteCatalog.ProductImageReturn imgReturn = imgRepo.GetImageList("370003");

            Assert.IsTrue(imgReturn.ProductImages.Count > 0);
        }

        [TestMethod]
        public void GetListOfImagesWithBadItemNumber()
        {
            KeithLink.Svc.Impl.Repository.SiteCatalog.ProductImageRepositoryImpl imgRepo = new Impl.Repository.SiteCatalog.ProductImageRepositoryImpl();

            KeithLink.Svc.Core.Models.SiteCatalog.ProductImageReturn imgReturn = imgRepo.GetImageList("999");

            Assert.IsTrue(imgReturn.ProductImages.Count == 0);
        }

        [TestMethod]
        public void GetListOfImagesWithNoItemNumber()
        {
            KeithLink.Svc.Impl.Repository.SiteCatalog.ProductImageRepositoryImpl imgRepo = new Impl.Repository.SiteCatalog.ProductImageRepositoryImpl();

            KeithLink.Svc.Core.Models.SiteCatalog.ProductImageReturn imgReturn = imgRepo.GetImageList(string.Empty);

            Assert.IsTrue(imgReturn.ProductImages.Count == 0);
        }
    }
}
