using KeithLink.Svc.Core.Extensions.Orders.History;
using KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.SiteCatalog;
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
            //_repo.ReadPurchaseOrderByInvoice("FAM", "01203045");
        }

        [TestMethod]
        public void ReadPurchaseOrder() {
            //PurchaseOrder po = _repo.ReadPurchaseOrder(new Guid("{2D8FD728-0050-45CA-91C5-9FA4C429A8CB}"), "0066709");

            //po.ToOrderHistoryFile(new UserSelectedContext() { BranchId = "FDF", CustomerId = "024418" });
        }
        #endregion
    }
}
