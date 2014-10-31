using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Impl.Repository.Orders.History;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Logic.Order {
    [TestClass]
    public class OrderHistoryLogicImplTests {
        #region attributes
        private OrderHistoryLogicImpl _logic;
        private EventLogRepositoryImpl _log;
        private IOrderHistoryHeaderRepsitory _headerRepo;
        private IOrderHistoryDetailRepository _detailRepo;
        private OrderUpdateQueueRepositoryImpl _queue;
        private IUnitOfWork _unitOfWork;

        private const string TEST_FILE = "Assets\\hourlyupdate.txt";
        #endregion

        #region ctor
        public OrderHistoryLogicImplTests() {
            _logic = new OrderHistoryLogicImpl(_log, _headerRepo, _detailRepo, _queue,  _unitOfWork);
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
