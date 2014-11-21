using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment.EF;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeithLink.Svc.Core.Interface.OnlinePayments {
    public interface IOnlinePaymentServiceRepository {
        void DeleteInvoice(UserSelectedContext userContext, string invoiceNumber);

        List<CustomerBank> GetAllCustomerBanks(UserSelectedContext userContext);

        CustomerBank GetBankAccount(UserSelectedContext userContext, string accountNumber);

        List<InvoiceModel> GetInvoiceTransactions(UserSelectedContext userContext, string invoiceNumber);

        List<InvoiceModel> GetOpenInvoiceHeaders(UserSelectedContext userContext);

		void MakeInvoicePayment(UserSelectedContext userContext, UserProfile user, List<PaymentTransactionModel> payments);
    }
}
