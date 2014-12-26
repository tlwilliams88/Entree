using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment.EF;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeithLink.Svc.Core.Interface.OnlinePayments {
    public interface IOnlinePaymentServiceRepository {
        void DeleteInvoice(UserSelectedContext userContext, string invoiceNumber);

        List<CustomerBank> GetAllCustomerBanks(UserSelectedContext userContext);

        InvoiceHeaderReturnModel GetAllOpenInvoices(UserSelectedContext userContext, PagingModel paging);

        InvoiceHeaderReturnModel GetAllPaidInvoices(UserSelectedContext userContext, PagingModel paging);

        InvoiceHeaderReturnModel GetAllPastDueInvoices(UserSelectedContext userContext, PagingModel paging);

        InvoiceHeaderReturnModel GetAllPayableInvoices(UserSelectedContext userContext, PagingModel paging);

        CustomerBank GetBankAccount(UserSelectedContext userContext, string accountNumber);

        List<InvoiceModel> GetInvoiceTransactions(UserSelectedContext userContext, string invoiceNumber);

		InvoiceHeaderReturnModel GetInvoiceHeaders(UserSelectedContext userContext, PagingModel paging);

		void MakeInvoicePayment(UserSelectedContext userContext, string emailAddress, List<PaymentTransactionModel> payments);

		InvoiceModel GetInvoiceDetails(UserSelectedContext userContext, string invoiceNumber);
    }
}
