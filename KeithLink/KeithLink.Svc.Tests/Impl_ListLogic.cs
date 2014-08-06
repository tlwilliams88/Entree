using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeithLink.Svc.Core;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Repository;
using KeithLink.Svc.Impl.Logic;

namespace KeithLink.Svc.Test
{
    [TestClass]
    public class Impl_ListLogic
    {
        private readonly IListLogic listLogic = new ListLogicImpl(new MockListRepositoryImpl());

        [TestMethod]
        public void ReadList()
        {
            var list = listLogic.ReadAllLists(true);

            Assert.IsNotNull(list);
        }
    }
}
