using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Models.Invoices;

namespace KeithLink.Svc.Impl.Logic.InternalSvc
{
	public class InternalInvoiceLogic : IInternalInvoiceLogic
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly IInvoiceRepository invoiceRepository;
		private readonly IInvoiceItemRepository invoiceItemRepository;

		public InternalInvoiceLogic(IUnitOfWork unitOfWork, IInvoiceRepository invoiceRepository, IInvoiceItemRepository invoiceItemRepository)
		{
			this.invoiceRepository = invoiceRepository;
			this.invoiceItemRepository = invoiceItemRepository;
			this.unitOfWork = unitOfWork;
		}

		public long CreateInvoice(InvoiceModel invoice, InvoiceType type)
		{
			var newInvoice = invoice.ToEFInvoice();
			invoiceRepository.CreateOrUpdate(newInvoice);
			unitOfWork.SaveChanges();
			return newInvoice.Id;
		}

		public InvoiceModel ReadInvoice(long Id)
		{
			var invoice = invoiceRepository.Read(x => x.Id.Equals(Id), i => i.Items).FirstOrDefault();

			if (invoice == null)
				return null;

			var returnInvoice = invoice.ToInvoiceModel();

			return returnInvoice;
		}

		public void DeleteInvoice(long Id)
		{
			var invoice = invoiceRepository.Read(x => x.Id.Equals(Id), i => i.Items).FirstOrDefault();

			invoice.Items.ToList().ForEach(delegate(InvoiceItem item)
			{
				invoiceItemRepository.Delete(item);
			});
			
			invoiceRepository.Delete(invoice);
			unitOfWork.SaveChanges();
		}


		public void UpdateInvoice(InvoiceModel invoice)
		{

		}


		
	}
}
