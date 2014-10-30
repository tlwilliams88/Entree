using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Invoices
{
	public interface IInvoiceServiceRepository
	{
		List<InvoiceModel> ReadInvoices(UserSelectedContext catalogInfo);
		InvoiceModel ReadInvoice(long Id);
	}
}
