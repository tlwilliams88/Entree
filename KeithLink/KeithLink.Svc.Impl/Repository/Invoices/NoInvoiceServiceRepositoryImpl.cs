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

		public List<InvoiceModel> ReadInvoices(Core.Models.Profile.UserProfile user, UserSelectedContext catalogInfo)
		{
			return new List<InvoiceModel>();
		}

		public InvoiceModel ReadInvoice(Core.Models.Profile.UserProfile user, UserSelectedContext catalogInfo, long Id)
		{
			return null;
		}


		public TermModel ReadTermInformation(string branchId, string termCode)
		{
			return new TermModel();
		}
	}
}
