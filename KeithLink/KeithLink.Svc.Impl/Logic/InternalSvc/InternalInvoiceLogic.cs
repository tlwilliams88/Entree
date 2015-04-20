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
using KeithLink.Svc.Core.Enumerations;
using KeithLink.Svc.Core.Helpers;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Enumerations.List;

namespace KeithLink.Svc.Impl.Logic.InternalSvc
{
	public class InternalInvoiceLogic : IInternalInvoiceLogic {
        #region attributes
        private readonly ICatalogLogic catalogLogic;
        private readonly IInvoiceItemRepository invoiceItemRepository;
        private readonly IInvoiceRepository invoiceRepository;
        private readonly IListRepository listRepository;
        private readonly ITermRepository termRepository;
        private readonly IUnitOfWork unitOfWork;
        #endregion

        #region ctor
        public InternalInvoiceLogic(IUnitOfWork unitOfWork, IInvoiceRepository invoiceRepository, IInvoiceItemRepository invoiceItemRepository,
            ICatalogLogic catalogLogic, IListRepository listRepository, ITermRepository termRepository) {
            this.invoiceRepository = invoiceRepository;
            this.invoiceItemRepository = invoiceItemRepository;
            this.unitOfWork = unitOfWork;
            this.catalogLogic = catalogLogic;
            this.listRepository = listRepository;
            this.termRepository = termRepository;
        }
        #endregion

        #region methods
        public long CreateInvoice(InvoiceModel invoice, InvoiceType type)
		{
			var newInvoice = invoice.ToEFInvoice();
			invoiceRepository.CreateOrUpdate(newInvoice);
			unitOfWork.SaveChanges();
			return newInvoice.Id;
		}

        public void DeleteInvoice(long Id) {
            var invoice = invoiceRepository.Read(x => x.Id.Equals(Id), i => i.Items).FirstOrDefault();

            invoice.Items.ToList().ForEach(delegate(InvoiceItem item) {
                invoiceItemRepository.Delete(item);
            });

            invoiceRepository.Delete(invoice);
            unitOfWork.SaveChanges();
        }

        private void DetermineCorrectStatus(InvoiceModel invoice) {
            if (invoice.Status == InvoiceStatus.Open && invoice.DueDate < DateTime.UtcNow)
                invoice.Status = InvoiceStatus.PastDue;

            invoice.StatusDescription = EnumUtils<InvoiceStatus>.GetDescription(invoice.Status, "");

        }

        private void LookupProductDetails(UserProfile user, InvoiceModel invoice, UserSelectedContext catalogInfo) {
            if (invoice.Items == null || invoice.Items.Count == 0)
                return;

            var products = catalogLogic.GetProductsByIds(catalogInfo.BranchId, invoice.Items.Select(i => i.ItemNumber).Distinct().ToList());
            var notes = listRepository.ReadListForCustomer(catalogInfo, false).Where(l => l.Type == ListType.Notes).FirstOrDefault();
			

            Parallel.ForEach(invoice.Items, invoiceItem => {
                var prod = products.Products.Where(p => p.ItemNumber.Equals(invoiceItem.ItemNumber)).FirstOrDefault();
                if (prod != null) {
                    invoiceItem.Name = prod.Name;
                    invoiceItem.PackSize = string.Format("{0} / {1}", prod.Pack, prod.Size);
                    invoiceItem.Brand = prod.BrandExtendedDescription;
                    invoiceItem.TempZone = prod.TempZone;

                }
                if (notes != null && notes.Items != null)
                    invoiceItem.Notes = notes.Items.Where(n => n.ItemNumber.Equals(invoiceItem.ItemNumber)).Select(i => i.Note).FirstOrDefault();
            });

        }
        
        public InvoiceModel ReadInvoice(UserProfile user, UserSelectedContext catalogInfo, long Id)
		{
			var invoice = invoiceRepository.Read(x => x.Id.Equals(Id), i => i.Items).FirstOrDefault();

			if (invoice == null)
				return null;

			var returnInvoice = invoice.ToInvoiceModel();
			DetermineCorrectStatus(returnInvoice);
			LookupProductDetails(user, returnInvoice, catalogInfo);
			return returnInvoice;
		}

		public List<InvoiceModel> ReadInvoices(UserProfile user, UserSelectedContext catalogInfo)
		{
			var invoices = invoiceRepository.Read(x => x.CustomerNumber.Equals(catalogInfo.CustomerId)).ToList();
			var returnModels = invoices.Select(i => i.ToInvoiceModel(true)).ToList();

			returnModels.ForEach(delegate(InvoiceModel invoice)
			{
				DetermineCorrectStatus(invoice);
			});
			return returnModels;
		}

        public TermModel ReadTermInformation(string branchId, string termCode) {
            var intTerm = termCode.ToInt();

            if (!intTerm.HasValue)
                return null;

            var term = termRepository.Read(t => t.BranchId.Equals(branchId) && t.TermCode.Equals(intTerm.Value)).FirstOrDefault();

            if (term == null)
                return null;

            return new TermModel() { BranchId = term.BranchId, TermCode = term.TermCode, Description = term.Description, Age1 = term.Age1, Age2 = term.Age2, Age3 = term.Age3, Age4 = term.Age4 };
        }

        public void UpdateInvoice(InvoiceModel invoice)
		{

		}
        #endregion
    }
}
