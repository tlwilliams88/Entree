using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using EF = KeithLink.Svc.Core.Models.OnlinePayments.Invoice.EF;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace KeithLink.Svc.Impl.Repository.OnlinePayments.Invoice {
    public class KPayInvoiceRepositoryImpl : IKPayInvoiceRepository {
        #region attributes
        private readonly IKPayDBContext _dbContext;
        #endregion

        #region ctor
        public KPayInvoiceRepositoryImpl(IKPayDBContext kpayDBContext) {
            _dbContext = kpayDBContext;
        }
        #endregion

        #region methods
        public void DeleteInvoice(string division, string customerNumber, string invoiceNumber) {
            SqlParameter branchParm = new SqlParameter("@Division", division);
            SqlParameter custParm = new SqlParameter("@CustNum", customerNumber);
            SqlParameter invParm = new SqlParameter("@Invoice", invoiceNumber.PadRight(30));

            _dbContext.Context.Database.ExecuteSqlCommand(
                    "procDeleteInvoice @Division, @CustNum, @Invoice",
                    branchParm,
                    custParm,
                    invParm
                );
        }

        public List<Core.Models.OnlinePayments.Invoice.EF.Invoice> GetInvoiceTransactoin(string division, string customerNumber, string invoiceNumber) {
            return _dbContext.Invoices.Where(i => i.Division.Equals(division) && i.CustomerNumber.Equals(customerNumber) && i.InvoiceNumber.StartsWith(invoiceNumber) && i.ItemSequence > 0).ToList();
        }

        public List<Core.Models.OnlinePayments.Invoice.EF.Invoice> GetMainInvoices(string division, string customerNumber) {
            return _dbContext.Invoices.Where(i => i.Division.Equals(division) && i.CustomerNumber.Equals(customerNumber) && i.ItemSequence == 0).ToList();
        }
        #endregion
    }
}
