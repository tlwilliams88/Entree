using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Models.Orders.History;

using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace KeithLink.Svc.Test.Logic.Order {
    [TestClass]
    public class OrderHistoryLogicImplTests {
        #region attributes
        private IOrderHistoryLogic _logic;

        private const string TEST_FILE = "Assets\\hourlyupdate.txt";
        #endregion

        #region ctor
        public OrderHistoryLogicImplTests() {
            IContainer diMap = DependencyMap.Build();

            _logic = diMap.Resolve<IOrderHistoryLogic>();
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
