using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Repository.Lists;
using KeithLink.Svc.Impl.Logic;
using KeithLink.Svc.Impl.Repository.SiteCatalog;

namespace KeithLink.Svc.Test
{
    [TestClass]
    public class Impl_ListLogic
    {
        private readonly IListLogic listLogic = new ListLogicImpl(new MockListRepositoryImpl(), new StubCatalogRepositoryImpl());

        [TestMethod]
        public void ReadList()
        {
            var list = listLogic.ReadAllLists(Guid.NewGuid(), "fdf",true);

            Assert.IsNotNull(list);
        }
    }
}
