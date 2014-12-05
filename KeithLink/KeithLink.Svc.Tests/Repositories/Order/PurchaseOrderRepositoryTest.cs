using KeithLink.Svc.Impl.Repository.Orders;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeithLink.Svc.Test.Repositories.Order {
    [TestClass]
    public class PurchaseOrderRepositoryTest {
        #region attributes
        private PurchaseOrderRepositoryImpl _repo;
        #endregion

        #region ctor
        public PurchaseOrderRepositoryTest() {
            _repo = new PurchaseOrderRepositoryImpl();
        }
        #endregion

        #region methods
        [TestMethod]
        public void SuccessfulGetOrderByInvoiceNumber() {
            _repo.ReadPurchaseOrderByInvoice("FAM", "01203045");
        }
        #endregion
    }
}
