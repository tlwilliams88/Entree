using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.WebApi.Repository.OnlinePayments {
    public class OnlinePaymentServiceRepositoryImpl : IOnlinePaymentServiceRepository {
        #region attributes
        private com.benekeith.OnlinePaymentService.IOnlinePaymentService _client;
        #endregion

        #region ctor
        public OnlinePaymentServiceRepositoryImpl(com.benekeith.OnlinePaymentService.IOnlinePaymentService serviceClient) {
            _client = serviceClient;
        }
        #endregion

        #region methods
        public void DeleteInvoice(UserSelectedContext userContext, string invoiceNumber) {
            _client.DeleteInvoice(userContext, invoiceNumber);
        }

        public List<CustomerBank> GetAllCustomerBanks(UserSelectedContext userContext) {
            return _client.GetAllBankAccounts(userContext).ToList<CustomerBank>();
        }

		public Core.Models.OnlinePayments.Customer.CustomerBank GetBankAccount(UserSelectedContext userContext, string accountNumber) {
            return _client.GetBankAccount(userContext, accountNumber);
        }

		public CustomerAccountBalanceModel GetCustomerAccountBalance(string customerId, string branchId)
		{
			return _client.GetCustomerAccountBalance(customerId, branchId);
		}
        
        public InvoiceModel GetInvoiceDetails(UserSelectedContext userContext, string invoiceNumber) {
            return _client.GetInvoiceDetails(userContext, invoiceNumber);
        }

		public InvoiceHeaderReturnModel GetInvoiceHeaders(UserProfile user, UserSelectedContext userContext, PagingModel paging, bool forAllCustomers)
		{
            return _client.GetInvoiceHeaders(user, userContext, paging, forAllCustomers);
        }
		        
		public void MakeInvoicePayment(UserSelectedContext userContext, string emailAddress, List<Core.Models.OnlinePayments.Payment.PaymentTransactionModel> payments)
		{
			_client.MakeInvoicePayment(userContext, emailAddress, payments.ToArray());
		}

        public PagedResults<PaymentTransactionModel> PendingTransactions(UserSelectedContext customer, PagingModel paging) {
            return _client.PendingTransactions(customer, paging);
        }

		public PagedResults<PaymentTransactionModel> PendingTransactionsAllCustomers(UserProfile user, PagingModel paging)
		{
			return _client.PendingTransactionsAllCustomers(user, paging);
		}

        public List<Core.Models.OnlinePayments.Payment.PaymentTransactionModel> ValidatePayment( UserSelectedContext userContext, List<PaymentTransactionModel> payments ) {
            return _client.ValidatePayment(userContext, payments.ToArray()).ToList<Core.Models.OnlinePayments.Payment.PaymentTransactionModel>();
        }
        #endregion
	}
}