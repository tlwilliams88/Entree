using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Logic.Order {
    [TestClass]
    public class OrderHistoryLogicImplTests {
        #region attributes
        private OrderHistoryLogicImpl _logic;
        #endregion

        #region ctor
        public OrderHistoryLogicImplTests() {
            _logic = new OrderHistoryLogicImpl();
        }
        #endregion

        #region methods
        [TestMethod]
        public void ParseOrderHistoryFile() {
             OrderHistoryFileReturn parsedFile = _logic.ParseMainframeFile(@"c:\test\hourlyupdate.txt");

             Assert.IsTrue(parsedFile.Files.Count > 0);
        }
        #endregion
    }
}
