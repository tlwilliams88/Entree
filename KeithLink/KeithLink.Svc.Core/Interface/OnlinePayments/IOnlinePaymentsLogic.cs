﻿using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.OnlinePayments{
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
        		
        void MakeInvoicePayment(UserSelectedContext userContext, string emailAddress, List<PaymentTransactionModel> payments);

        PagedResults<PaymentTransactionModel> PendingTransactions(UserSelectedContext customer, PagingModel paging);

		PagedResults<PaymentTransactionModel> PendingTransactionsAllCustomers(UserProfile user, PagingModel paging);

        List<PaymentTransactionModel> ValidatePayment( UserSelectedContext userContext, List<PaymentTransactionModel> payments );
	}
}
