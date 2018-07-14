using Entree.Core.Models.Invoices;
using Entree.Core.Models.OnlinePayments.Customer;
using Entree.Core.Models.OnlinePayments.Payment;
using Entree.Core.Models.Paging;
using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.OnlinePayments{
	public interface IOnlinePaymentsLogic{
		void DeleteInvoice(UserSelectedContext userContext, string invoiceNumber);

        InvoiceItemModel AssignContractCategory
            (Dictionary<string, string> contractdictionary, InvoiceItemModel item);


        List<CustomerBank> GetAllBankAccounts(UserSelectedContext userContext);

        CustomerBank GetBankAccount(UserSelectedContext userContext, string accountNumber);

		CustomerAccountBalanceModel GetCustomerAccountBalance(string customerId, string branchId);

		InvoiceModel GetInvoiceDetails(UserSelectedContext userContext, string invoiceNumber);

        List<InvoiceTransactionModel> GetInvoiceTransactions(UserSelectedContext userContext, string invoiceNumber);

		InvoiceHeaderReturnModel GetInvoiceHeaders(UserProfile user, UserSelectedContext userContext, PagingModel paging, bool forAllCustomers);

        InvoiceCustomers GetInvoiceCustomers(UserProfile user, UserSelectedContext userContext, PagingModel paging, bool forAllCustomers);

        void MakeInvoicePayment(UserSelectedContext userContext, string emailAddress, List<PaymentTransactionModel> payments);

        PagedResults<PaymentTransactionModel> PendingTransactions(UserSelectedContext customer, PagingModel paging);

		PagedResults<PaymentTransactionModel> PendingTransactionsAllCustomers(UserProfile user, PagingModel paging);

        List<PaymentTransactionModel> ValidatePayment( UserSelectedContext userContext, List<PaymentTransactionModel> payments );
	}
}
