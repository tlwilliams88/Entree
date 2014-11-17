using KeithLink.Svc.Core.Models.OnlinePayments.Invoice.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Impl.Repository.OnlinePayments.Invoice;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Test.Repositories.OnlinePayments {
    [TestClass]
    public class KPayInvoiceRepositoryTests {
        #region attributes
        private KPayDBContext _dbContext;
        private KPayInvoiceRepositoryImpl _invRepo;
        #endregion

        #region ctor
        public KPayInvoiceRepositoryTests() {
            _dbContext = new KPayDBContext();
            _invRepo = new KPayInvoiceRepositoryImpl(_dbContext);
        }
        #endregion

        #region methods
        [TestMethod]
        public void SuccessfullyDeleteInvoice() {
            _invRepo.DeleteInvoice("FAM04", "700353", "41577160");
        }

        [TestMethod]
        public void SuccessfullyGetInvoiceTransactions() {
            List<Invoice> details = _invRepo.GetInvoiceTransactoin("FAM04", "700353", "41563444");

            Assert.IsTrue(details.Count > 0);
        }

        [TestMethod]
        public void SuccessfullyGetMainInvoicesForCustomer() {
            List<Invoice> invoices = _invRepo.GetMainInvoices("FAM04", "700353");

            Assert.IsTrue(invoices.Count > 0);
        }
        #endregion
    }
}
