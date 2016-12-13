using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeithLink.Svc.Impl.Logic.Lists;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using Moq;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.EF;
using System.Collections.Generic;

namespace KeithLink.Svc.Test.Logic.Lists.CustomInventory
{
    [TestClass]
    public class Logic_ListLogicImpl_CustomInventory_AddItem
    {
        private Core.Models.EF.List retList = null;
        [TestInitialize]
        public void Init()
        {
            //uow = new Mock<IUnitOfWork>();

            //listrepo = new Mock<IListRepository>();
            //retList = new Core.Models.EF.List() { Type = Core.Enumerations.List.ListType.CustomInventory };
            //listrepo.Setup(t => t.ReadById(0)).Returns(() => retList);

            //cache = new Mock<ICacheRepository>();

            //li = new ListItemModel()
            //{
            //    ItemNumber = "TEST",
            //    Name = "TestName",
            //    Brand = "TestBrand",
            //    Vendor1 = "TestVendor",
            //    Each = true,
            //    Quantity = 1,
            //    CasePrice = "2",
            //    Pack = "3",
            //    Size = "4",
            //    PackagePrice = "5",
            //    ParLevel = 6,
            //    Label = "TestLabel"
            //};

            //ListLogicImpl listlogic = new ListLogicImpl(uow.Object, listrepo.Object, null, null, cache.Object, null,
            //                                            null, null, null,
            //                                            null, null, null, null, null);
            //listlogic.AddItem(null,
            //                  new Core.Models.SiteCatalog.UserSelectedContext() { BranchId = "FDF", CustomerId = "726971" },
            //                  0,
            //                  li);

        }

        [TestMethod]
        public void TestCompletes()
        {
            Assert.IsNotNull(retList.Items);
        }

        [TestMethod]
        public void ListItem_Is_Added()
        {
            Assert.AreEqual(retList.Items.ToList().Count, 1);
        }
    }
}
