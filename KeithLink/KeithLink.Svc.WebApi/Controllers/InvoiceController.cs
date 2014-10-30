using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
    public class InvoiceController : BaseController
    {

		private readonly IInvoiceServiceRepository invoiceServiceRepository;

		public InvoiceController(IUserProfileLogic profileLogic, IInvoiceServiceRepository invoiceServiceRepository): base(profileLogic)
		{
			this.invoiceServiceRepository = invoiceServiceRepository;
		}

		[HttpGet]
		[ApiKeyedRoute("invoice/")]
		public List<InvoiceModel> invoice()
		{
			return invoiceServiceRepository.ReadInvoices(this.SelectedUserContext);
		}

		[HttpGet]
		[ApiKeyedRoute("invoice/{id}")]
		public InvoiceModel invoice(long id)
		{
			return invoiceServiceRepository.ReadInvoice(id);
		}

    }
}
