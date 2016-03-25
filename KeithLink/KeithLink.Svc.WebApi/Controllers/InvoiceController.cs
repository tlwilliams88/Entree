using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.Configurations;
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
        private readonly IEventLogRepository _log;
        #endregion

        #region ctor
        public InvoiceController(IUserProfileLogic profileLogic, IOnlinePaymentServiceRepository invoiceRepository, IExportSettingLogic exportSettingsLogic,
                                 IImagingLogic invoiceImagingLogic, IEventLogRepository logRepo) : base(profileLogic) {
            _repo = invoiceRepository;
			_exportLogic = exportSettingsLogic;
            _imgLogic = invoiceImagingLogic;
            _log = logRepo;
        }
        #endregion

        #region methods
        /// <summary>
        /// Retrieve customer banks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("banks")]
        public Models.OperationReturnModel<List<CustomerBank>> Get() {
            Models.OperationReturnModel<List<CustomerBank>> retVal = new Models.OperationReturnModel<List<CustomerBank>>();
            try
            {
                retVal.SuccessResponse = _repo.GetAllCustomerBanks(SelectedUserContext);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("Get banks", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve bank information for account
        /// </summary>
        /// <param name="accountNumber">Account number</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("banks/{accountNumber}")]
        public Models.OperationReturnModel<CustomerBank> GetBackAccount(string accountNumber) {
            Models.OperationReturnModel<CustomerBank> retVal = new Models.OperationReturnModel<CustomerBank>();
            try
            {
                retVal.SuccessResponse = _repo.GetBankAccount(this.SelectedUserContext, accountNumber);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("GetBackAccount", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Read a paged list of invoices
        /// </summary>
        /// <param name="paging">Paging options</param>
        /// <param name="forAllCustomers">Invoices for all customers?</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("invoice/")]
        public Models.OperationReturnModel<InvoiceHeaderReturnModel> Invoice(PagingModel paging, bool forAllCustomers = false) {
            Models.OperationReturnModel<InvoiceHeaderReturnModel> retVal = new Models.OperationReturnModel<InvoiceHeaderReturnModel>();
            try
            {
                retVal.SuccessResponse = _repo.GetInvoiceHeaders(this.AuthenticatedUser, SelectedUserContext, paging, forAllCustomers);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("GetInvoiceHeaders", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
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
            HttpResponseMessage ret;
            try
            {
                var list = _repo.GetInvoiceHeaders(this.AuthenticatedUser, SelectedUserContext, request.paging, forAllCustomers);

                if (request.export.Fields != null)
                    _exportLogic.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.Invoice, 0, request.export.Fields, request.export.SelectedType);
                ret = ExportModel<InvoiceModel>(list.PagedResults.Results, request.export);
            }
            catch (Exception ex)
            {
                ret = Request.CreateResponse(HttpStatusCode.InternalServerError);
                ret.ReasonPhrase = ex.Message;
                _log.WriteErrorLog("ExportInvoices", ex);
            }
            return ret;
        }

        /// <summary>
        /// Retrieve export options for invoices
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("invoice/export/")]
        public Models.OperationReturnModel<ExportOptionsModel> ExportInvoices() {
            Models.OperationReturnModel<ExportOptionsModel> retVal = new Models.OperationReturnModel<ExportOptionsModel>();
            try
            {
                retVal.SuccessResponse = _exportLogic.ReadCustomExportOptions(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.Invoice, 0);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("ReadCustomExportOptions", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
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
                retVal.IsSuccess = true;
            }
            catch (Exception ex) {
                retVal.ErrorMessage = string.Format("Could not retrieve invoice images at this time.");
                _log.WriteErrorLog("GetInvoiceImages", ex);
                retVal.IsSuccess = false;
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
        public Models.OperationReturnModel<InvoiceModel> InvoiceTransactions(string invoiceNumber) {
            Models.OperationReturnModel<InvoiceModel> retVal = new Models.OperationReturnModel<InvoiceModel>();
            try
            {
                retVal.SuccessResponse = _repo.GetInvoiceDetails(this.SelectedUserContext, invoiceNumber);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("InvoiceTransactions", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
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
            HttpResponseMessage ret;
            try
            {
                var invoice = _repo.GetInvoiceDetails(this.SelectedUserContext, invoiceNumber);
                if (exportRequest.Fields != null)
                    _exportLogic.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.InvoiceDetail, KeithLink.Svc.Core.Enumerations.List.ListType.Custom, exportRequest.Fields, exportRequest.SelectedType);

                ret = ExportModel<InvoiceItemModel>(invoice.Items, exportRequest);
            }
            catch (Exception ex)
            {
                ret = Request.CreateResponse(HttpStatusCode.InternalServerError);
                ret.ReasonPhrase = ex.Message;
                _log.WriteErrorLog("ExportInvoiceDetail", ex);
            }
            return ret;
        }

        /// <summary>
        /// Retrieve export options for specific invoice
        /// </summary>
        /// <param name="invoiceNumber">Invoice number</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("invoice/export/{invoiceNumber}")]
        public OperationReturnModel<ExportOptionsModel> ExportInvoiceDetail(string invoiceNumber) {
            OperationReturnModel<ExportOptionsModel> retVal = new OperationReturnModel<ExportOptionsModel>();
            try
            {
                retVal.SuccessResponse = _exportLogic.ReadCustomExportOptions(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.InvoiceDetail, 0);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                retVal.ErrorMessage = ex.Message;
                _log.WriteErrorLog("ExportInvoiceDetail", ex);
                retVal.IsSuccess = false;
            }

            return retVal;
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
                retVal.IsSuccess = true;
            }
            catch (Exception ex) {
                retVal.SuccessResponse = false;
                retVal.IsSuccess = false;
                retVal.ErrorMessage = ex.Message;
                _log.WriteErrorLog("MakeInvoicePayment", ex);
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
            try
            {
                List<PaymentTransactionModel> transactionErrors = _repo.ValidatePayment(this.SelectedUserContext, payments);

                // If the payment validation list comes back with a count > 0 then there were errors
                // validating a transaction. It will return the transactions that did not validate correctly.
                if (transactionErrors.Count > 0)
                {
                    returnValue.ErrorMessage = string.Format("The total for Bank Account {0} on {1} must be positive.", transactionErrors.First().AccountNumber, transactionErrors.First().PaymentDate.Value.ToShortDateString());
                    returnValue.SuccessResponse = new PaymentValidationResponseModel()
                    {
                        IsValid = false,
                        PaymentTransactions = transactionErrors,
                    };
                    returnValue.IsSuccess = true;
                }
                else {
                    returnValue.SuccessResponse = new PaymentValidationResponseModel()
                    {
                        IsValid = true,
                        PaymentTransactions = null,
                    };
                    returnValue.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                returnValue.IsSuccess = false;
                returnValue.ErrorMessage = ex.Message;
                _log.WriteErrorLog("ValidatePayment", ex);
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
        public OperationReturnModel<PagedResults<PaymentTransactionModel>> PendingTransactions(PagingModel paging) {
            OperationReturnModel<PagedResults<PaymentTransactionModel>> retVal = new OperationReturnModel<PagedResults<PaymentTransactionModel>>();
            try
            {
                retVal.SuccessResponse = _repo.PendingTransactions(this.SelectedUserContext, paging);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                retVal.IsSuccess = false;
                retVal.ErrorMessage = ex.Message;
                _log.WriteErrorLog("PendingTransactions", ex);
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve paged list of pending transactions
        /// </summary>
        /// <param name="paging">Paging options</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("invoice/transactions/pending")]
        public OperationReturnModel<PagedResults<PaymentTransactionModel>> PendingTransactionsForAllCustomers(PagingModel paging) {
            OperationReturnModel<PagedResults<PaymentTransactionModel>> retVal = new OperationReturnModel<PagedResults<PaymentTransactionModel>>();
            try
            {
                retVal.SuccessResponse = _repo.PendingTransactionsAllCustomers(this.AuthenticatedUser, paging);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                retVal.IsSuccess = false;
                retVal.ErrorMessage = ex.Message;
                _log.WriteErrorLog("PendingTransactionsForAllCustomers", ex);
            }

            return retVal;
        }

        /// <summary>
        /// Export pending transactions
        /// </summary>
        /// <param name="request">Export options</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("invoice/transactions/pending/export/")]
        public HttpResponseMessage ExportOrders(InvoiceExportRequestModel request) {
            HttpResponseMessage ret;
            try
            {
                request.paging.Size = int.MaxValue;
                request.paging.From = 0;

                var transactions = _repo.PendingTransactionsAllCustomers(this.AuthenticatedUser, request.paging);

                if (request.export.Fields != null)
                    _exportLogic.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.PendingTransactions, KeithLink.Svc.Core.Enumerations.List.ListType.Custom, request.export.Fields, request.export.SelectedType);

                ret = ExportModel<PaymentTransactionModel>(transactions.Results, request.export);
            }
            catch (Exception ex)
            {
                ret = Request.CreateResponse(HttpStatusCode.InternalServerError);
                ret.ReasonPhrase = ex.Message;
                _log.WriteErrorLog("ExportOrders", ex);
            }
            return ret;
        }

        /// <summary>
        /// Retrieve export options for pending transactions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("invoice/transactions/pending/export")]
        public OperationReturnModel<ExportOptionsModel> ExportOrders() {
            OperationReturnModel<ExportOptionsModel> retVal = new OperationReturnModel<ExportOptionsModel>();
            try
            {
                retVal.SuccessResponse = _exportLogic.ReadCustomExportOptions
                    (this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.PendingTransactions, 0);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                retVal.IsSuccess = false;
                retVal.ErrorMessage = ex.Message;
                _log.WriteErrorLog("ExportOrders", ex);
            }

            return retVal;
        }
        #endregion
    }
}
