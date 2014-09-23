using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Logic;
using KeithLink.Svc.Impl.Repository.SiteCatalog;
using KeithLink.Svc.Test.MockRepository;
using KeithLink.Svc.Core.Models.Profile;

namespace KeithLink.Svc.Test.Logic
{
    [TestClass]
    public class ListLogicTests
    {
		private readonly IListLogic listLogic = new ListLogicImpl(new BasketRepositoryMock(), new StubCatalogRepositoryImpl(), new PriceRepositoryImpl(), new ItemNoteLogicImpl(new MockRepository.BasketRepositoryMock()));

        [TestMethod]
        public void ReadAllList()
        {
			//var lists = listLogic.ReadAllLists(new UserProfile() { UserId = Guid.NewGuid() }, "fdf", true);

			//Assert.IsNotNull(lists);
        }

		[TestMethod]
		public void ReadList()
		{
			//var list = listLogic.ReadList(new UserProfile() { UserId = Guid.NewGuid() }, Guid.NewGuid());

			//Assert.IsNotNull(list);
		}
    }
}
