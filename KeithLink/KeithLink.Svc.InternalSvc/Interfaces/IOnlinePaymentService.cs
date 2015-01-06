using KeithLink.Svc.Core.Models.Invoices;
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
        CustomerBank GetBankAccount(UserSelectedContext userContext, string accountNumber);

		[OperationContract]
		InvoiceHeaderReturnModel GetInvoiceHeaders(UserProfile user, UserSelectedContext userContext, PagingModel paging, bool forAllCustomers);

		[OperationContract]
		void MakeInvoicePayment(UserSelectedContext userContext, string emailAddress, List<PaymentTransactionModel> payments);

		[OperationContract]
		InvoiceModel GetInvoiceDetails(UserSelectedContext userContext, string invoiceNumber);
    }
}
