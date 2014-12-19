using KeithLink.Svc.Core.Interface.Configuration;
//using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment;
using KeithLink.Svc.Core.Models.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
	[Authorize]
    public class InvoiceController : BaseController {
        #region attributes
        private readonly IOnlinePaymentServiceRepository _repo;
		private readonly IExportSettingServiceRepository _exportSettingRepository;
		#endregion

        #region ctor
        #endregion
		public InvoiceController(IUserProfileLogic profileLogic, IOnlinePaymentServiceRepository invoiceRepository, IExportSettingServiceRepository exportSettingRepository)
			: base(profileLogic)
		{
            _repo = invoiceRepository;
			_exportSettingRepository = exportSettingRepository;
		}

        #region methods
        [HttpPost]
        [ApiKeyedRoute("invoice/")]
		public InvoiceHeaderReturnModel Invoice(PagingModel paging)
		{
            return _repo.GetInvoiceHeaders(SelectedUserContext, paging);
        }

		[HttpPost]
		[ApiKeyedRoute("invoice/export/")]
		public HttpResponseMessage ExportInvoices(ExportRequestModel exportRequest)
		{
			var list = _repo.GetInvoiceHeaders(SelectedUserContext, new PagingModel() { Size = 500, From = 0});

			if (exportRequest.Fields != null)
				_exportSettingRepository.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.Invoice, 0, exportRequest.Fields, exportRequest.SelectedType);
			return ExportModel<InvoiceModel>(list.PagedResults.Results, exportRequest);
		}

		[HttpGet]
		[ApiKeyedRoute("invoice/export/")]
		public ExportOptionsModel ExportInvoices()
		{
			return _exportSettingRepository.ReadCustomExportOptions(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.Invoice, 0);
		}

        [HttpGet]
        [ApiKeyedRoute("invoice/{invoiceNumber}")]
        public InvoiceModel InvoiceTransactions(string invoiceNumber) {
            return _repo.GetInvoiceDetails(this.SelectedUserContext, invoiceNumber);
        }

		[HttpPost]
		[ApiKeyedRoute("invoice/export/{invoiceNumber}")]
		public HttpResponseMessage ExportInvoiceDetail(string invoiceNumber, ExportRequestModel exportRequest)
		{
			var invoice = _repo.GetInvoiceDetails(this.SelectedUserContext, invoiceNumber);
			if (exportRequest.Fields != null)
				_exportSettingRepository.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.InvoiceDetail, KeithLink.Svc.Core.Enumerations.List.ListType.Custom, exportRequest.Fields, exportRequest.SelectedType);

			return ExportModel<InvoiceItemModel>(invoice.Items, exportRequest);
		}

		[HttpGet]
		[ApiKeyedRoute("invoice/export/{invoiceNumber}")]
		public ExportOptionsModel ExportInvoiceDetail(string invoiceNumber)
		{
			return _exportSettingRepository.ReadCustomExportOptions(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.InvoiceDetail, 0);
		}


		[HttpPost]
		[ApiKeyedRoute("invoice/payment")]
		public void Payment(List<PaymentTransactionModel> payments)
		{
			_repo.MakeInvoicePayment(this.SelectedUserContext, this.AuthenticatedUser.EmailAddress, payments);
		}

		[HttpGet]
		[ApiKeyedRoute("invoice/test")]        
		public List<PaymentTransactionModel> Test()
		{
			return new List<PaymentTransactionModel>() { new PaymentTransactionModel() { InvoiceNumber = "1234", AccountNumber = "1234", PaymentAmount = 123.43m, PaymentDate = DateTime.Now } };
		}


        #endregion
    }
}
