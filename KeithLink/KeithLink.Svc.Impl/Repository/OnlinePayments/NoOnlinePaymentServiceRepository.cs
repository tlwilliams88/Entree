using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.OnlinePayments {
    public class NoOnlinePaymentServiceRepository : IOnlinePaymentServiceRepository {
        #region methods
        public void DeleteInvoice(UserSelectedContext userContext, string invoiceNumber) {
            throw new NotImplementedException();
        }

        public List<CustomerBank> GetAllCustomerBanks(UserSelectedContext userContext) {
            throw new NotImplementedException();
        }

        public CustomerBank GetBankAccount(UserSelectedContext userContext, string accountNumber) {
            throw new NotImplementedException();
        }

        public List<Core.Models.Invoices.InvoiceModel> GetInvoiceTransactions(UserSelectedContext userContext, string invoiceNumber) {
            throw new NotImplementedException();
        }

        public List<Core.Models.Invoices.InvoiceModel> GetOpenInvoiceHeaders(UserSelectedContext userContext) {
            throw new NotImplementedException();
        }
        #endregion
    }
}
