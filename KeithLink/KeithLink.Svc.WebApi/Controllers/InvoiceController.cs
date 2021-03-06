﻿﻿using KeithLink.Common.Core.Interfaces.Logging;

﻿using KeithLink.Svc.Core.Extensions.Orders;

using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.Profile;

using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.Invoices.Imaging.Document;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.SiteCatalog;

using KeithLink.Svc.WebApi.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web.Http;

 using KeithLink.Svc.Core.Interface.Lists;
 using KeithLink.Svc.Core.Interface.SiteCatalog;
 using KeithLink.Svc.Impl.Helpers;

namespace KeithLink.Svc.WebApi.Controllers {
    /// <summary>
    /// end points for online bill payment
    /// </summary>
	[Authorize]
    public class InvoiceController : BaseController {
        #region attributes
        private readonly IExportInvoicesService _invService;
        private readonly IListService _listService;
        private readonly IOnlinePaymentsLogic _invLogic;
		private readonly IExportSettingLogic _exportLogic;
        private readonly IOrderLogic _orderLogic;
        private readonly IImagingLogic _imgLogic;
        private readonly IEventLogRepository _log;
        private readonly ICatalogLogic _catalogLogic;
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="profileLogic"></param>
        /// <param name="invoiceLogic"></param>
        /// <param name="exportSettingsLogic"></param>
        /// <param name="orderLogic"></param>
        /// <param name="invoiceImagingLogic"></param>
        /// <param name="logRepo"></param>
        /// <param name="invService"></param>
		public InvoiceController(IUserProfileLogic profileLogic, IOnlinePaymentsLogic invoiceLogic, IExportSettingLogic exportSettingsLogic,
                                 IOrderLogic orderLogic, IImagingLogic invoiceImagingLogic, IEventLogRepository logRepo, ICatalogLogic catalogLogic,
                                 IExportInvoicesService invService, IListService listService) : base(profileLogic) {
            _invLogic = invoiceLogic;
            _orderLogic = orderLogic;
			_exportLogic = exportSettingsLogic;
            _imgLogic = invoiceImagingLogic;
            _log = logRepo;
            _catalogLogic = catalogLogic;
            _invService = invService;
            _listService = listService;
        }
        #endregion

        #region methods
        /// <summary>
        /// Retrieve customer banks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("banks")]
        public OperationReturnModel<List<CustomerBank>> Get(string customerId = null, string branchId = null) {
            OperationReturnModel<List<CustomerBank>> retVal = new OperationReturnModel<List<CustomerBank>>();
            try
            {
                if (customerId != null && branchId != null)
                {
                    retVal.SuccessResponse = _invLogic.GetAllBankAccounts
                        (new Core.Models.SiteCatalog.UserSelectedContext() { CustomerId=customerId, BranchId=branchId });
                }
                else
                {
                    retVal.SuccessResponse = _invLogic.GetAllBankAccounts(SelectedUserContext);
                }
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
        public OperationReturnModel<CustomerBank> GetBackAccount(string accountNumber) {
            OperationReturnModel<CustomerBank> retVal = new OperationReturnModel<CustomerBank>();
            try
            {
                retVal.SuccessResponse = _invLogic.GetBankAccount(this.SelectedUserContext, accountNumber);
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
        /// ReadSP a paged list of invoices
        /// </summary>
        /// <param name="paging">Paging options</param>
        /// <param name="forAllCustomers">Invoices for all customers?</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("invoice/{customerBranch}/{customerNumber}")]
        public OperationReturnModel<InvoiceHeaderReturnModel> Invoice(PagingModel paging, [FromUri] string customerBranch, [FromUri] string customerNumber) {
            OperationReturnModel<InvoiceHeaderReturnModel> retVal = new OperationReturnModel<InvoiceHeaderReturnModel>();
            try
            {
                retVal.SuccessResponse = _invLogic.GetInvoiceHeaders(this.AuthenticatedUser, 
                                                                     new UserSelectedContext() {
                                                                         BranchId = customerBranch,
                                                                         CustomerId = customerNumber
                                                                     },  
                                                                     paging, 
                                                                     false);
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

        [HttpPost]
        [ApiKeyedRoute("invoice/mycustomers")]
        public OperationReturnModel<InvoiceCustomers> MyCustomers(PagingModel paging, [FromUri]bool forAllCustomers = false)
        {
            OperationReturnModel<InvoiceCustomers> retVal = new OperationReturnModel<InvoiceCustomers>();
            try
            {
                retVal.SuccessResponse = _invLogic.GetInvoiceCustomers(this.AuthenticatedUser, SelectedUserContext, paging, forAllCustomers);
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
        public HttpResponseMessage ExportInvoices(ExportRequestModel request, bool forAllCustomers = true) {
            HttpResponseMessage ret;
            try {

                List<InvoiceModel> exportData = _invService.GetExportableInvoiceModels(AuthenticatedUser, 
                                                                                       SelectedUserContext,
                                                                                       request, 
                                                                                       forAllCustomers);

                ret = ExportModel<InvoiceModel>(exportData, request, SelectedUserContext);
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("ExportInvoices", ex);
                ret = Request.CreateResponse(HttpStatusCode.InternalServerError);
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
        /// <param name="customerId">Optional parameter to supply customerId</param>
        /// <param name="branchId">Optional parameter to supply branchId</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("invoice/image/{invoiceNumber}")]
        public OperationReturnModel<List<Base64Image>> GetInvoiceImages(string invoiceNumber, string customerId = null, string branchId = null) {
            OperationReturnModel<List<Base64Image>> retVal = new OperationReturnModel<List<Base64Image>>();
            try {
                if (customerId != null && branchId != null)
                {
                    retVal.SuccessResponse = _imgLogic.GetInvoiceImages
                        (new Core.Models.SiteCatalog.UserSelectedContext() { CustomerId = customerId, BranchId = branchId }, invoiceNumber);
                }
                else
                {
                    retVal.SuccessResponse = _imgLogic.GetInvoiceImages(this.SelectedUserContext, invoiceNumber);
                }
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
        public Models.OperationReturnModel<InvoiceModel> InvoiceDetails(string invoiceNumber) {
            Models.OperationReturnModel<InvoiceModel> retVal = new Models.OperationReturnModel<InvoiceModel>();
            try
            {
                InvoiceModel inv = _invLogic.GetInvoiceDetails(this.SelectedUserContext, invoiceNumber);

                Order order = _orderLogic.GetOrder(SelectedUserContext.BranchId, invoiceNumber);
                if (order != null)
                {
                    inv.Items = order.Items.Select(i => i.ToInvoiceItem()).ToList();
                }

                    retVal.SuccessResponse = inv;
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
        /// Invoice transaction details
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="branchId"></param>
        /// <param name="customerNumber"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("invoice/transactions/{branchId}/{customerNumber}/{invoiceNumber}")]
        public Models.OperationReturnModel<List<InvoiceTransactionModel>> InvoiceTransactions(string branchId, string customerNumber, string invoiceNumber) {
            Models.OperationReturnModel<List<InvoiceTransactionModel>> retVal = new Models.OperationReturnModel<List<InvoiceTransactionModel>>();
            try
            {
                UserSelectedContext customerContext = new UserSelectedContext() {
                    BranchId = branchId,
                    CustomerId = customerNumber
                };
                List<InvoiceTransactionModel> transactions = _invLogic.GetInvoiceTransactions(customerContext, invoiceNumber);

                retVal.SuccessResponse = transactions;
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
            try {
                InvoiceModel invoice = _invService.GetExportableInvoice(AuthenticatedUser,
                                                                        SelectedUserContext,
                                                                        exportRequest,
                                                                        invoiceNumber);

                invoice.Items = _invService.GetExportableInvoiceItems(AuthenticatedUser,
                                                                      SelectedUserContext,
                                                                      exportRequest,
                                                                      invoiceNumber,
                                                                      _listService.GetContractInformation(SelectedUserContext));

                ItemOrderHistoryHelper.GetItemOrderHistories(_catalogLogic, SelectedUserContext, invoice.Items);

                ret = ExportModel<InvoiceItemModel>(invoice.Items,
                                                    exportRequest,
                                                    SelectedUserContext,
                                                    invoice);
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("ExportInvoiceDetail", ex);
                ret = Request.CreateResponse(HttpStatusCode.InternalServerError);
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
                _invLogic.MakeInvoicePayment(this.SelectedUserContext, this.AuthenticatedUser.EmailAddress, payments);

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
                List<PaymentTransactionModel> transactionErrors = _invLogic.ValidatePayment(this.SelectedUserContext, payments);

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
                retVal.SuccessResponse = _invLogic.PendingTransactions(this.SelectedUserContext, paging);
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
                retVal.SuccessResponse = _invLogic.PendingTransactionsAllCustomers(this.AuthenticatedUser, paging);
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

                var transactions = _invLogic.PendingTransactionsAllCustomers(this.AuthenticatedUser, request.paging);

                if (request.export.Fields != null)
                    _exportLogic.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.PendingTransactions, KeithLink.Svc.Core.Enumerations.List.ListType.Custom, request.export.Fields, request.export.SelectedType);

                ret = ExportModel<PaymentTransactionModel>(transactions.Results, request.export, SelectedUserContext);
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("ExportOrders", ex);
                ret = Request.CreateResponse(HttpStatusCode.InternalServerError);
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
