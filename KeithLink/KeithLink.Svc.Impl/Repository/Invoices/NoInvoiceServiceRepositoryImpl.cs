using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Invoices
{
	public class NoInvoiceServiceRepositoryImpl: IInvoiceServiceRepository
	{
		public NoInvoiceServiceRepositoryImpl()
		{
		}

		public List<InvoiceModel> ReadInvoices(UserSelectedContext catalogInfo)
		{
            throw new NotImplementedException();
		}

		public InvoiceModel ReadInvoice(long Id)
        {
            throw new NotImplementedException();
		}
	}
}
