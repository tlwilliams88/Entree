using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Logic;
using KeithLink.Svc.Impl.Repository.SiteCatalog;
using KeithLink.Svc.Test.MockRepository;

namespace KeithLink.Svc.Test
{
    [TestClass]
    public class Impl_ListLogic
    {
		private readonly IListLogic listLogic = new ListLogicImpl(new BasketRepositoryMock(), new StubCatalogRepositoryImpl());

        [TestMethod]
        public void ReadAllList()
        {
			var lists = listLogic.ReadAllLists(Guid.NewGuid(), "fdf",true);

			Assert.IsNotNull(lists);
        }

		[TestMethod]
		public void ReadList()
		{
			var list = listLogic.ReadList(Guid.NewGuid(), Guid.NewGuid());

			Assert.IsNotNull(list);
		}
    }
}
