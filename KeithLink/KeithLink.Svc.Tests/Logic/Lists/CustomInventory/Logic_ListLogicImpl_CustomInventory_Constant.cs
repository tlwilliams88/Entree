using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeithLink.Svc.Core;

namespace KeithLink.Svc.Test.Logic.Lists.CustomInventory
{
    [TestClass]
    public class Logic_ListLogicImpl_CustomInventory_Constant
    {
        [TestMethod]
        public void ConstantExists()
        {
            Assert.AreEqual(Constants.CATALOG_CUSTOMINVENTORY,"CUSTOM");
        }
    }
}
