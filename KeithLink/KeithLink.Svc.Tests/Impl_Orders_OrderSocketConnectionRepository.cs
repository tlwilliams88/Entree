using KeithLink.Svc.Impl.Repository.Orders;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test
{
    [TestClass]
    public class Impl_Orders_OrderSocketConnectionRepository
    {
        [TestMethod]
        public void Connect()
        {
            OrderSocketConnectionRepositoryImpl mf = new OrderSocketConnectionRepositoryImpl();
            mf.Connect();
            mf.Close()
        }
    }
}
