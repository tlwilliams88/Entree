using KeithLink.Svc.Impl.Repository.Orders;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Repositories.Order
{
    [TestClass]
    public class OrderSocketConnectionRepositoryTests
    {
        [TestMethod]
        public void Connect()
        {
            OrderSocketConnectionRepositoryImpl mf = new OrderSocketConnectionRepositoryImpl();
            mf.Connect();
            mf.Close();
        }
    }
}
