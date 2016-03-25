using KeithLink.Svc.Core.Extensions.Orders;

using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.Profile;

using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment;
using KeithLink.Svc.Core.Models.Orders;
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
    /// <summary>
    /// end points for online bill payment
    /// </summary>
	[Authorize]
    public class InvoiceController : BaseController {
        #region attributes
        private readonly IOnlinePaymentsLogic _invLogic;
		private readonly IExportSettingLogic _exportLogic;
        private readonly IOrderHistoryLogic _orderLogic;
        private readonly IImagingLogic _imgLogic;
		#endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="profileLogic"></param>
        /// <param name="invoiceLogic"></param>
        /// <param name="exportSettingsLogic"></param>
        /// <param name="invoiceImagingLogic"></param>
        /// <param name="orderHistoryLogic"></param>
		public InvoiceController(IUserProfileLogic profileLogic, IOnlinePaymentsLogic invoiceLogic, IExportSettingLogic exportSettingsLogic,
                                 IOrderHistoryLogic orderHistoryLogic, IImagingLogic invoiceImagingLogic) : base(profileLogic) {
            _invLogic = invoiceLogic;
            _orderLogic = orderHistoryLogic;
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
            return _invLogic.GetAllBankAccounts(SelectedUserContext);
        }

        /// <summary>
        /// Retrieve bank information for account
        /// </summary>
        /// <param name="accountNumber">Account number</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("banks/{accountNumber}")]
        public CustomerBank GetBackAccount(string accountNumber) {
            return _invLogic.GetBankAccount(this.SelectedUserContext, accountNumber);
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
            return _invLogic.GetInvoiceHeaders(this.AuthenticatedUser, SelectedUserContext, paging, forAllCustomers);
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
            var list = _invLogic.GetInvoiceHeaders(this.AuthenticatedUser, SelectedUserContext, request.paging, forAllCustomers);

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
            InvoiceModel inv = _invLogic.GetInvoiceDetails(this.SelectedUserContext, invoiceNumber);
            Order order = _orderLogic.GetOrder(SelectedUserContext.BranchId, invoiceNumber);

            inv.Items = order.Items.Select(i => i.ToInvoiceItem()).ToList();

            return inv;
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
            if (exportRequest.Fields != null)
                _exportLogic.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.InvoiceDetail, KeithLink.Svc.Core.Enumerations.List.ListType.Custom, exportRequest.Fields, exportRequest.SelectedType);

            Order order = _orderLogic.GetOrder(SelectedUserContext.BranchId, invoiceNumber);
            List<InvoiceItemModel> items = order.Items.Select(i => i.ToInvoiceItem()).ToList();

            return ExportModel<InvoiceItemModel>(items, exportRequest);
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
                _invLogic.MakeInvoicePayment(this.SelectedUserContext, this.AuthenticatedUser.EmailAddress, payments);

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

            List<PaymentTransactionModel> transactionErrors = _invLogic.ValidatePayment(this.SelectedUserContext, payments);

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
            return _invLogic.PendingTransactions(this.SelectedUserContext, paging);
        }

        /// <summary>
        /// Retrieve paged list of pending transactions
        /// </summary>
        /// <param name="paging">Paging options</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("invoice/transactions/pending")]
        public PagedResults<PaymentTransactionModel> PendingTransactionsForAllCustomers(PagingModel paging) {
            return _invLogic.PendingTransactionsAllCustomers(this.AuthenticatedUser, paging);
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

            var transactions = _invLogic.PendingTransactionsAllCustomers(this.AuthenticatedUser, request.paging);

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
