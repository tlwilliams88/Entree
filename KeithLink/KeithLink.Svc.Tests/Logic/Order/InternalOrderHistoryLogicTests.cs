// KeithLink
using KeithLink.Svc.Core.Interface.Orders;

// Core
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Logic.Order {
    [TestClass]
    public class InternalOrderHistoryLogicTests {

        #region attributes
        private IOrderLogic _logic;
        private IContainer _container;
        #endregion

        #region constructor

        public InternalOrderHistoryLogicTests() {
            _container = DependencyMap.Build();

            _logic = _container.Resolve<IOrderLogic>();
        }
        #endregion

        #region test methods

        [TestMethod]
        public void InternalOrderGetOrderTest() {
            string branchId = "FDF";
            string invoiceNumber = "0060043";

            var order = _logic.GetOrder( branchId, invoiceNumber );

            Assert.IsNotNull( order );
        }

        #endregion

    }
}
