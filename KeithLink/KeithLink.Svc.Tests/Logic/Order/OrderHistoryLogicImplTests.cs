using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Logic.Order {
    [TestClass]
    public class OrderHistoryLogicImplTests {
        #region attributes
        private OrderHistoryLogicImpl _logic;

        private const string TEST_FILE = "Assets\\hourlyupdate.txt";
        #endregion

        #region ctor
        public OrderHistoryLogicImplTests() {
            _logic = new OrderHistoryLogicImpl();
        }
        #endregion

        #region methods
        [TestMethod]
        public void ParseOrderHistoryFile() {
            OrderHistoryFileReturn parsedFile = _logic.ParseMainframeFile(String.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, TEST_FILE));

            Assert.IsTrue(parsedFile.Files.Count > 0);
        }
        #endregion
    }
}
