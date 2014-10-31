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
using KeithLink.Common.Core.Extensions;

using EntityFramework.BulkInsert.Extensions;
using System.Transactions;

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

		public List<InvoiceModel> ReadInvoices(UserSelectedContext catalogInfo)
		{

			var invoices = invoiceRepository.Read(x => x.CustomerNumber.Equals(catalogInfo.CustomerId)).ToList();

			return invoices.Select(i => i.ToInvoiceModel(true)).ToList();
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

		public void DeleteAll()
		{
			unitOfWork.Context.DeleteTable("[Invoice].[InvoiceItems]");
			unitOfWork.Context.DeleteTable("[Invoice].[Invoices]");
		}



		public void BulkImport(List<InvoiceModel> invoices, List<InvoiceItemModel> invoiceItems)
		{
			var invoiceEntities = invoices.Select(i => i.ToEFInvoice()).ToList();
			var itemEntities = invoiceItems.Select(i => i.ToEFInvoiceItem()).ToList();

			Parallel.ForEach(invoiceEntities, invoice =>
				{
					invoice.CreatedUtc = DateTime.UtcNow;
					invoice.ModifiedUtc = DateTime.UtcNow;
				});

			Parallel.ForEach(itemEntities, item =>
			{
				item.CreatedUtc = DateTime.UtcNow;
				item.ModifiedUtc = DateTime.UtcNow;
			});

			using (var transactionScope = new TransactionScope())
			{
				unitOfWork.Context.BulkInsert<Invoice>(invoiceEntities, 5000);

				var insertedInvoices = unitOfWork.Context.Invoices.ToList();
								
				Parallel.ForEach(itemEntities, item =>
				{
					item.InvoiceId = insertedInvoices.Where(i => i.InvoiceNumber.Equals(item.InvoiceNumber)).FirstOrDefault().Id;
				});

				unitOfWork.Context.BulkInsert<InvoiceItem>(itemEntities);

				transactionScope.Complete();
			}


		}
				
	}
}
