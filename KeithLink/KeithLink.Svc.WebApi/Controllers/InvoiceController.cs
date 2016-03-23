﻿using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.Profile;

using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment;
using KeithLink.Svc.Core.Models.Paging;

using KeithLink.Svc.WebApi.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers {
	[Authorize]
    public class InvoiceController : BaseController {
        #region attributes
        private readonly IOnlinePaymentServiceRepository _repo;
		private readonly IExportSettingLogic _exportLogic;
        private readonly IImagingLogic _imgLogic;
		#endregion

        #region ctor
		public InvoiceController(IUserProfileLogic profileLogic, IOnlinePaymentServiceRepository invoiceRepository, IExportSettingLogic exportSettingsLogic,
                                 IImagingLogic invoiceImagingLogic) : base(profileLogic) {
            _repo = invoiceRepository;
			_exportLogic = exportSettingsLogic;
            _imgLogic = invoiceImagingLogic;
		}
        #endregion

        #region methods
        /// <summary>
        /// Retrieve customer banks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("banks")]
        public List<CustomerBank> Get() {
            return _repo.GetAllCustomerBanks(SelectedUserContext);
        }

        /// <summary>
        /// Retrieve bank information for account
        /// </summary>
        /// <param name="accountNumber">Account number</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("banks/{accountNumber}")]
        public CustomerBank GetBackAccount(string accountNumber) {
            return _repo.GetBankAccount(this.SelectedUserContext, accountNumber);
        }
  
        /// <summary>
        /// Read a paged list of invoices
        /// </summary>
        /// <param name="paging">Paging options</param>
        /// <param name="forAllCustomers">Invoices for all customers?</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("invoice/")]
        public InvoiceHeaderReturnModel Invoice(PagingModel paging, bool forAllCustomers = false) {
            return _repo.GetInvoiceHeaders(this.AuthenticatedUser, SelectedUserContext, paging, forAllCustomers);
        }

        /// <summary>
        /// Export invoice
        /// </summary>
        /// <param name="request">Export options</param>
        /// <param name="forAllCustomers">Invoices for all customers?</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("invoice/export/")]
        public HttpResponseMessage ExportInvoices(InvoiceExportRequestModel request, bool forAllCustomers = false) {
            var list = _repo.GetInvoiceHeaders(this.AuthenticatedUser, SelectedUserContext, request.paging, forAllCustomers);

            if (request.export.Fields != null)
                _exportLogic.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.Invoice, 0, request.export.Fields, request.export.SelectedType);
            return ExportModel<InvoiceModel>(list.PagedResults.Results, request.export);
        }

        /// <summary>
        /// Retrieve export options for invoices
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("invoice/export/")]
        public ExportOptionsModel ExportInvoices() {
            return _exportLogic.ReadCustomExportOptions(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.Invoice, 0);
        }

        /// <summary>
        /// Retrieve invoice image
        /// </summary>
        /// <param name="invoiceNumber">Invoice number</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("invoice/image/{invoiceNumber}")]
        public OperationReturnModel<List<string>> GetInvoiceImages(string invoiceNumber) {
            OperationReturnModel<List<string>> retVal = new OperationReturnModel<List<string>>();

            try {
                retVal.SuccessResponse = _imgLogic.GetInvoiceImages(this.SelectedUserContext, invoiceNumber);
            } catch (Exception ex) {
                retVal.ErrorMessage = string.Format("Could not retrieve invoice images at this time.");
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve specific invoice
        /// </summary>
        /// <param name="invoiceNumber">Invoice number</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("invoice/{invoiceNumber}")]
        public InvoiceModel InvoiceTransactions(string invoiceNumber) {
            return _repo.GetInvoiceDetails(this.SelectedUserContext, invoiceNumber);
        }

        /// <summary>
        /// Export invoice details
        /// </summary>
        /// <param name="invoiceNumber">Invoice to export</param>
        /// <param name="exportRequest">Export options</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("invoice/export/{invoiceNumber}")]
        public HttpResponseMessage ExportInvoiceDetail(string invoiceNumber, ExportRequestModel exportRequest) {
            var invoice = _repo.GetInvoiceDetails(this.SelectedUserContext, invoiceNumber);
            if (exportRequest.Fields != null)
                _exportLogic.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.InvoiceDetail, KeithLink.Svc.Core.Enumerations.List.ListType.Custom, exportRequest.Fields, exportRequest.SelectedType);

            return ExportModel<InvoiceItemModel>(invoice.Items, exportRequest);
        }

        /// <summary>
        /// Retrieve export options for specific invoice
        /// </summary>
        /// <param name="invoiceNumber">Invoice number</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("invoice/export/{invoiceNumber}")]
        public ExportOptionsModel ExportInvoiceDetail(string invoiceNumber) {
            return _exportLogic.ReadCustomExportOptions(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.InvoiceDetail, 0);
        }

        /// <summary>
        /// Submit invoice payment
        /// </summary>
        /// <param name="payments">Payment</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("invoice/payment")]
        public OperationReturnModel<bool> Payment(List<PaymentTransactionModel> payments) {
            OperationReturnModel<bool> retVal = new OperationReturnModel<bool>();

            try {
                _repo.MakeInvoicePayment(this.SelectedUserContext, this.AuthenticatedUser.EmailAddress, payments);

                retVal.SuccessResponse = true;
            } catch (Exception ex) {
                retVal.SuccessResponse = false;
                retVal.ErrorMessage = ex.Message;
            }

            return retVal;
        }

        /// <summary>
        /// Validate the transactions
        /// </summary>
        /// <param name="payments"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("invoice/payment/validate")]
        public OperationReturnModel<PaymentValidationResponseModel> ValidatePayment(List<PaymentTransactionModel> payments) {
            OperationReturnModel<PaymentValidationResponseModel> returnValue = new OperationReturnModel<PaymentValidationResponseModel>();

            List<PaymentTransactionModel> transactionErrors = _repo.ValidatePayment(this.SelectedUserContext, payments);

            // If the payment validation list comes back with a count > 0 then there were errors
            // validating a transaction. It will return the transactions that did not validate correctly.
            if (transactionErrors.Count > 0) {
                returnValue.ErrorMessage = string.Format("The total for Bank Account {0} on {1} must be positive.", transactionErrors.First().AccountNumber, transactionErrors.First().PaymentDate.Value.ToShortDateString());
                returnValue.SuccessResponse = new PaymentValidationResponseModel() {
                    IsValid = false,
                    PaymentTransactions = transactionErrors,
                };
            } else {
                returnValue.SuccessResponse = new PaymentValidationResponseModel() {
                    IsValid = true,
                    PaymentTransactions = null,
                };
            }

            return returnValue;
        }

        /// <summary>
        /// Retrieve paged list of pending transactions for a single customer
        /// </summary>
        /// <param name="paging">Paging options</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("invoice/transactions/pending/single")]
        public PagedResults<PaymentTransactionModel> PendingTransactions(PagingModel paging) {
            return _repo.PendingTransactions(this.SelectedUserContext, paging);
        }

        /// <summary>
        /// Retrieve paged list of pending transactions
        /// </summary>
        /// <param name="paging">Paging options</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("invoice/transactions/pending")]
        public PagedResults<PaymentTransactionModel> PendingTransactionsForAllCustomers(PagingModel paging) {
            return _repo.PendingTransactionsAllCustomers(this.AuthenticatedUser, paging);
        }

        /// <summary>
        /// Export pending transactions
        /// </summary>
        /// <param name="request">Export options</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("invoice/transactions/pending/export/")]
        public HttpResponseMessage ExportOrders(InvoiceExportRequestModel request) {
            request.paging.Size = int.MaxValue;
            request.paging.From = 0;

            var transactions = _repo.PendingTransactionsAllCustomers(this.AuthenticatedUser, request.paging);

            if (request.export.Fields != null)
                _exportLogic.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.PendingTransactions, KeithLink.Svc.Core.Enumerations.List.ListType.Custom, request.export.Fields, request.export.SelectedType);

            return ExportModel<PaymentTransactionModel>(transactions.Results, request.export);
        }

        /// <summary>
        /// Retrieve export options for pending transactions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("invoice/transactions/pending/export")]
        public ExportOptionsModel ExportOrders() {
            return _exportLogic.ReadCustomExportOptions(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.PendingTransactions, 0);
        }
        #endregion
    }
}
