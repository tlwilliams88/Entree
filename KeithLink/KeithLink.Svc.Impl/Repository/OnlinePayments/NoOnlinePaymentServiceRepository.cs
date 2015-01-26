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
        public void DeleteInvoice(UserSelectedContext userContext, string invoiceNumber) {
            throw new NotImplementedException();
        }

        public List<CustomerBank> GetAllCustomerBanks(UserSelectedContext userContext) {
            throw new NotImplementedException();
        }

        public Core.Models.Invoices.InvoiceHeaderReturnModel GetAllOpenInvoices(UserSelectedContext userContext, Core.Models.Paging.PagingModel paging) {
            throw new NotImplementedException();
        }

        public Core.Models.Invoices.InvoiceHeaderReturnModel GetAllPaidInvoices(UserSelectedContext userContext, Core.Models.Paging.PagingModel paging) {
            throw new NotImplementedException();
        }

        public Core.Models.Invoices.InvoiceHeaderReturnModel GetAllPastDueInvoices(UserSelectedContext userContext, Core.Models.Paging.PagingModel paging) {
            throw new NotImplementedException();
        }

        public Core.Models.Invoices.InvoiceHeaderReturnModel GetAllPayableInvoices(UserSelectedContext userContext, Core.Models.Paging.PagingModel paging) {
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
       
		public void MakeInvoicePayment(UserSelectedContext userContext, string emailAddress, List<Core.Models.OnlinePayments.Payment.PaymentTransactionModel> payments)
		{
			throw new NotImplementedException();
		}


		public Core.Models.Invoices.InvoiceModel GetInvoiceDetails(UserSelectedContext userContext, string invoiceNumber)
		{
			throw new NotImplementedException();
		}


		public Core.Models.Paging.PagedResults<Core.Models.Invoices.InvoiceModel> GetInvoiceHeaders(UserSelectedContext userContext, Core.Models.Paging.PagingModel paging)
		{
			throw new NotImplementedException();
		}


		


		public Core.Models.Invoices.InvoiceHeaderReturnModel GetInvoiceHeaders(Core.Models.Profile.UserProfile user, UserSelectedContext userContext, Core.Models.Paging.PagingModel paging, bool forAllCustomers)
		{
			throw new NotImplementedException();
		}


		public Core.Models.Paging.PagedResults<Core.Models.OnlinePayments.Payment.PaymentTransactionModel> PendingTransactionsAllCustomers(Core.Models.Profile.UserProfile user, Core.Models.Paging.PagingModel paging)
		{
			throw new NotImplementedException();
		}
	}
}
