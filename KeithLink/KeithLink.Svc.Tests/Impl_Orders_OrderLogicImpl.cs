using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Impl.Repository.Orders;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test
{
    [TestClass]
    public class Impl_Orders_OrderLogicImpl
    {
        [TestMethod]
        public void SendMockOrder()
        {
            StubOrderLogicImpl order = new StubOrderLogicImpl(new OrderSocketConnectionRepositoryImpl());

            order.ParseFile("somefilename");
            order.SendToHost();
        }
    }
}
