using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.OnlinePayments
{
	public interface IOnlinePaymentsLogic
	{
		List<CustomerBank> GetAllBankAccounts(UserSelectedContext userContext);
		CustomerBank GetBankAccount(UserSelectedContext userContext, string accountNumber);

		void DeleteInvoice(UserSelectedContext userContext, string invoiceNumber);

		List<InvoiceModel> GetOpenInvoiceHeaders(UserSelectedContext userContext);
		void MakeInvoicePayment(UserSelectedContext userContext, string emailAddress, List<PaymentTransactionModel> payments);
		InvoiceModel GetInvoiceDetails(UserSelectedContext userContext, string invoiceNumber);
	}
}
