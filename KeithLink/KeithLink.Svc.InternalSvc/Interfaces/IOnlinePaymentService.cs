﻿using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
//using EFCustomer = KeithLink.Svc.Core.Models.OnlinePayments.Customer.EF;
//using EFInvoice = KeithLink.Svc.Core.Models.OnlinePayments.Invoice.EF;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace KeithLink.Svc.InternalSvc.Interfaces {
    [ServiceContract]
    public interface IOnlinePaymentService {
        [OperationContract]
        void DeleteInvoice(UserSelectedContext userContext, string invoiceNumber);

        [OperationContract]
        List<CustomerBank> GetAllBankAccounts(UserSelectedContext userContext);

        [OperationContract]
        InvoiceHeaderReturnModel GetAllOpenInvoices(UserSelectedContext userContext, PagingModel paging);

        [OperationContract]
        InvoiceHeaderReturnModel GetAllPaidInvoices(UserSelectedContext userContext, PagingModel paging);

        [OperationContract]
        InvoiceHeaderReturnModel GetAllPastDueInvoices(UserSelectedContext userContext, PagingModel paging);

        [OperationContract]
        InvoiceHeaderReturnModel GetAllPayableInvoices(UserSelectedContext userContext, PagingModel paging);

        [OperationContract]
        CustomerBank GetBankAccount(UserSelectedContext userContext, string accountNumber);

        [OperationContract]
        List<InvoiceModel> GetInvoiceTransactions(UserSelectedContext userContext, string invoiceNumber);

		[OperationContract]
		InvoiceHeaderReturnModel GetInvoiceHeaders(UserSelectedContext userContext, PagingModel paging);

		[OperationContract]
		void MakeInvoicePayment(UserSelectedContext userContext, string emailAddress, List<PaymentTransactionModel> payments);

		[OperationContract]
		InvoiceModel GetInvoiceDetails(UserSelectedContext userContext, string invoiceNumber);
    }
}
