using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.WebApi.Repository.Invoices
{
	public class InvoiceServiceRepositoryImpl: IInvoiceServiceRepository
	{		
		private com.benekeith.InvoiceService.IInvoiceService serviceClient;

		public InvoiceServiceRepositoryImpl(com.benekeith.InvoiceService.IInvoiceService serviceClient)
		{
			this.serviceClient = serviceClient;
		}

		public List<InvoiceModel> ReadInvoices(UserProfile user, UserSelectedContext catalogInfo)
		{
			return serviceClient.ReadInvoices(user, catalogInfo).ToList();
		}

		public InvoiceModel ReadInvoice(UserProfile user, UserSelectedContext catalogInfo, long Id)
		{
			return serviceClient.ReadInvoice(user, catalogInfo, Id);
		}


		public TermModel ReadTermInformation(string branchId, string termCode)
		{
			return serviceClient.ReadTermInformation(branchId, termCode);
		}
	}
}
