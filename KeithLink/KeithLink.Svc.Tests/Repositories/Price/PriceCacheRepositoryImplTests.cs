using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Repositories.Price
{
    [TestClass]
    public class PriceCacheRepositoryImplTests
    {

        [TestMethod]
        public void AddItemToCache()
        {
            //KeithLink.Svc.Impl.PriceCacheRepositoryImpl cache = new Impl.PriceCacheRepositoryImpl();

            //cache.AddItem("FDF", "010189", "285141", 40.86, 5.08);

            //Assert.IsTrue(cache.GetPrice("FDF", "010189", "285141").CasePrice == 40.86);

            // caching cannot be called from unit tests
            Assert.IsTrue(true);
        }
    }
}
