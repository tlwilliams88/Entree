using KeithLink.Svc.Core.Models.Invoices;
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
		
        List<CustomerBank> GetAllBankAccounts(UserSelectedContext userContext);

        CustomerBank GetBankAccount(UserSelectedContext userContext, string accountNumber);

		InvoiceModel GetInvoiceDetails(UserSelectedContext userContext, string invoiceNumber);

		InvoiceHeaderReturnModel GetInvoiceHeaders(UserProfile user, UserSelectedContext userContext, PagingModel paging, bool forAllCustomers);
        		
        void MakeInvoicePayment(UserSelectedContext userContext, string emailAddress, List<PaymentTransactionModel> payments);
	}
}
