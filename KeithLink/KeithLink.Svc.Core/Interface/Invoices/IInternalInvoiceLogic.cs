using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Invoices
{
	public interface IInternalInvoiceLogic
	{
		long CreateInvoice(InvoiceModel invoice, InvoiceType type);
		//long? AddItem(long invoiceId, InvoiceItemModel newItem);
		InvoiceModel ReadInvoice(long Id);
		void DeleteInvoice(long Id);
		void UpdateInvoice(InvoiceModel invoice);
	}
}
