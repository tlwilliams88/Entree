using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.Profile;

using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Paging;

using KeithLink.Svc.Impl.Helpers;
using KeithLink.Svc.Impl.Logic;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
	[Authorize]
    public class OrderController : BaseController {
        #region attributes
        private readonly IOrderHistoryRequestLogic _historyRequestLogic;
		private readonly IOrderLogic _orderLogic;
        private readonly IOrderHistoryHeaderRepsitory _historyHeaderRepo;
        private readonly IShipDateRepository _shipDayService;
        private readonly IShoppingCartLogic _shoppingCartLogic;
		//private readonly IOrderServiceRepository _orderServiceRepository;
		private readonly IExportSettingLogic _exportLogic;
        private readonly IEventLogRepository _log;
        private readonly IOrderHistoryLogic _historyLogic;
        #endregion

        #region ctor
        public OrderController(IShoppingCartLogic shoppingCartLogic, IOrderLogic orderLogic, IShipDateRepository shipDayRepo,
							   IOrderHistoryRequestLogic historyRequestLogic, IUserProfileLogic profileLogic, IExportSettingLogic exportSettingsLogic, 
                               IEventLogRepository logRepo, IOrderHistoryHeaderRepsitory historyHeaderRepository, IOrderHistoryLogic orderHistoryLogic) : base(profileLogic) {
            _historyRequestLogic = historyRequestLogic;
			_orderLogic = orderLogic;
            _shipDayService = shipDayRepo;
			_shoppingCartLogic = shoppingCartLogic;
			_exportLogic = exportSettingsLogic;
            _log = logRepo;
            _historyHeaderRepo = historyHeaderRepository;
            _historyLogic = orderHistoryLogic;
        }
        #endregion

        #region methods
        /// <summary>
        /// Retrieve possible ship dates
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("order/shipdays")]
        public Models.OperationReturnModel<ShipDateReturn> GetShipDays() {
            Models.OperationReturnModel<ShipDateReturn> retVal = new Models.OperationReturnModel<ShipDateReturn>();
            try
            {
                retVal.SuccessResponse = _shipDayService.GetShipDates(this.SelectedUserContext);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("GetShipDays", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve a orders for the authenticated user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("order/")]
        public Models.OperationReturnModel<List<Order>> Orders() {
            Models.OperationReturnModel<List<Order>> retVal = new Models.OperationReturnModel<List<Order>>();
            try
            {
                retVal.SuccessResponse = _orderLogic.UpdateOrdersForSecurity(AuthenticatedUser, _orderLogic.GetOrders(AuthenticatedUser.UserId, SelectedUserContext));
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("Orders", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve a paged list of orders for the authenticated user
        /// </summary>
        /// <param name="paging">Paging options</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("order/")]
        public Models.OperationReturnModel<PagedResults<Order>> PagedOrders(PagingModel paging) {
            Models.OperationReturnModel<PagedResults<Order>> retVal = new Models.OperationReturnModel<PagedResults<Order>>();
            try
            {
                var results = _orderLogic.GetPagedOrders(AuthenticatedUser.UserId, SelectedUserContext, paging);                
                _orderLogic.UpdateOrdersForSecurity(AuthenticatedUser, results.Results);
                retVal.SuccessResponse = results;
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("PagedOrders", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve orders in the date range for the authenticated user
        /// </summary>
        /// <param name="from">Start date</param>
        /// <param name="to">End date</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("order/date")]
        public Models.OperationReturnModel<List<Order>> OrdersIndate(DateTime from, DateTime to) {
            Models.OperationReturnModel<List<Order>> retVal = new Models.OperationReturnModel<List<Order>>();
            try
            {
                retVal.SuccessResponse = _orderLogic.UpdateOrdersForSecurity(AuthenticatedUser, _orderLogic.GetOrderHeaderInDateRange(SelectedUserContext, from, to));
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("OrdersIndate", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve a summary of order information going back for as many months 
        /// as the parameter sets for the authenticated user
        /// </summary>
        /// <param name="numberOfMonths">Number of months from today</param>
        [HttpGet]
        [ApiKeyedRoute("order/totalbymonth/{numberOfMonths}")]
        public Models.OperationReturnModel<OrderTotalByMonth> GetOrderTotalByMonth(int numberOfMonths) {
            Models.OperationReturnModel<OrderTotalByMonth> retVal = new Models.OperationReturnModel<OrderTotalByMonth>();
            try
            {
                retVal.SuccessResponse = _historyLogic.GetOrderTotalByMonth(SelectedUserContext, numberOfMonths);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("OrdersIndate", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Export orders
        /// </summary>
        /// <param name="exportRequest">Export options</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("order/export/")]
        public HttpResponseMessage ExportOrders(ExportRequestModel exportRequest) {
            HttpResponseMessage ret;
            try
            {
                var orders = _orderLogic.GetOrders(AuthenticatedUser.UserId, SelectedUserContext)
                                        .OrderByDescending(o => o.CreatedDate)
                                        .ToList();
                if (exportRequest.Fields != null)
                    _exportLogic.SaveUserExportSettings(AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.Order, Core.Enumerations.List.ListType.Custom, 
                                                        exportRequest.Fields, exportRequest.SelectedType);

                ret = ExportModel<Order>(orders, exportRequest, SelectedUserContext);
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
        /// Retrieve order export options
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("order/export")]
        public Models.OperationReturnModel<ExportOptionsModel> ExportOrders() {
            Models.OperationReturnModel<ExportOptionsModel> retVal = new Models.OperationReturnModel<ExportOptionsModel>();
            try
            {
                retVal.SuccessResponse = _exportLogic.ReadCustomExportOptions(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.Order, 0);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("OrdersIndate", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve order
        /// </summary>
        /// <param name="orderNumber">Order number</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("order/{orderNumber}")]
        public Models.OperationReturnModel<Order> Orders(string orderNumber) {
            Models.OperationReturnModel<Order> retVal = new Models.OperationReturnModel<Order>();
            try
            {
                retVal.SuccessResponse = _orderLogic.UpdateOrderForEta(AuthenticatedUser, _orderLogic.GetOrder(SelectedUserContext.BranchId, orderNumber.Trim()));
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("OrdersIndate", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Exort a specific order
        /// </summary>
        /// <param name="orderNumber">Order number</param>
        /// <param name="exportRequest">Export options</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("order/export/{orderNumber}")]
        public HttpResponseMessage ExportOrderDetail(string orderNumber, ExportRequestModel exportRequest) {
            HttpResponseMessage ret;
            try
            {
                var order = _orderLogic.UpdateOrderForEta(AuthenticatedUser, _orderLogic.GetOrder(SelectedUserContext.BranchId, orderNumber.Trim()));
                if (exportRequest.Fields != null)
                    _exportLogic.SaveUserExportSettings(AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.OrderDetail, Core.Enumerations.List.ListType.Custom, 
                                                        exportRequest.Fields, exportRequest.SelectedType);

                ret = ExportModel<OrderLine>(order.Items, exportRequest, SelectedUserContext);
            }
            catch (Exception ex)
            {
                ret = Request.CreateResponse(HttpStatusCode.InternalServerError);
                ret.ReasonPhrase = ex.Message;
                _log.WriteErrorLog("ExportOrderDetail", ex);
            }
            return ret;
        }

        /// <summary>
        /// Retrieve export options for a specific order
        /// </summary>
        /// <param name="orderNumber">Order number</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("order/export/{orderNumber}")]
        public Models.OperationReturnModel<ExportOptionsModel> ExportOrderDetail(string orderNumber) {
            Models.OperationReturnModel<ExportOptionsModel> retVal = new Models.OperationReturnModel<ExportOptionsModel>();
            try
            {
                retVal.SuccessResponse = _exportLogic.ReadCustomExportOptions(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.OrderDetail, 0);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("ExportOrderDetail", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Request order history for customer
        /// </summary>
        [HttpPost]
        [ApiKeyedRoute("order/history")]
        public Models.OperationReturnModel<bool> RequestOrderHistoryHeaders() {
            Models.OperationReturnModel<bool> retVal = new Models.OperationReturnModel<bool>();
            try
            {
                _historyRequestLogic.RequestAllOrdersForCustomer(this.SelectedUserContext);
                retVal.SuccessResponse = true;
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("RequestOrderHistoryHeaders", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Request order history update for specific order
        /// </summary>
        /// <param name="orderNumber">Order number</param>
        [HttpPost]
        [ApiKeyedRoute("order/history/{orderNumber}")]
        public Models.OperationReturnModel<bool> RequestOrderHistory(string orderNumber) {
            Models.OperationReturnModel<bool> retVal = new Models.OperationReturnModel<bool>();
            try
            {
                _historyRequestLogic.RequestOrderForCustomer(this.SelectedUserContext, orderNumber);
                retVal.SuccessResponse = true;
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("RequestOrderHistory", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Submit order
        /// </summary>
        /// <param name="cartId">Shopping cart Id</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("order/{cartId}")]
        public Models.OperationReturnModel<SaveOrderReturn> SaveOrder(Guid cartId) {
            Models.OperationReturnModel<SaveOrderReturn> retVal = new Models.OperationReturnModel<SaveOrderReturn>();
            try
            {
                retVal.SuccessResponse = _shoppingCartLogic.SaveAsOrder(this.AuthenticatedUser, this.SelectedUserContext, cartId);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("SaveAsOrder", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Submit change order
        /// </summary>
        /// <param name="orderNumber">Order number</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("order/{orderNumber}/changeorder")]
        public Models.OperationReturnModel<NewOrderReturn> SaveOrder(string orderNumber) {
            Models.OperationReturnModel<NewOrderReturn> retVal = new Models.OperationReturnModel<NewOrderReturn>();
            try
            {
                retVal.SuccessResponse = _orderLogic.SubmitChangeOrder(this.AuthenticatedUser, this.SelectedUserContext, orderNumber);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("SubmitChangeOrder", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve change order
        /// </summary>
        /// <param name="header">Header only?</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("order/changeorder")]
        public Models.OperationReturnModel<List<Order>> GetChangeOrders(bool header = false) {
            Models.OperationReturnModel<List<Order>> ret = new Models.OperationReturnModel<List<Order>>();
            try
            {
                List<Order> changeOrders = _orderLogic.ReadOrders(this.AuthenticatedUser, this.SelectedUserContext, header: header);

                ret.SuccessResponse = changeOrders.Where(x => x.IsChangeOrderAllowed).OrderByDescending(o => o.InvoiceNumber).ToList();
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("changeOrders", ex);
                ret.ErrorMessage = ex.Message;
                ret.IsSuccess = false;
            }

            return ret;
        }

        /// <summary>
        /// Update order
        /// </summary>
        /// <param name="order">Updated order</param>
        /// <param name="deleteOmitted">Delete ommitted items?</param>
        /// <returns></returns>
        [HttpPut]
        [ApiKeyedRoute("order/")]
        public Models.OperationReturnModel<Order> UpdateOrder(Order order, bool deleteOmitted = true) {            
            Models.OperationReturnModel<Order> retVal = new Models.OperationReturnModel<Order>();
            try
            {
                retVal.SuccessResponse = _orderLogic.UpdateOrder(this.SelectedUserContext, this.AuthenticatedUser, order, deleteOmitted);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("UpdateOrder", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Cancel order
        /// </summary>
        /// <param name="commerceId">Order Id</param>
        /// <returns></returns>
        [HttpDelete]
        [ApiKeyedRoute("order/{commerceId}")]
        public Models.OperationReturnModel<NewOrderReturn> CancelOrder(Guid commerceId) {
            Models.OperationReturnModel<NewOrderReturn> retVal = new Models.OperationReturnModel<NewOrderReturn>();
            try
            {
                retVal.SuccessResponse = _orderLogic.CancelOrder(this.AuthenticatedUser, this.SelectedUserContext, commerceId);
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("CancelOrder", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve when orders were last updated
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("order/lastupdate")]
        public Models.OperationReturnModel<OrderHistoryUpdateModel> LastUpdated() {
            Models.OperationReturnModel<OrderHistoryUpdateModel> retVal = new Models.OperationReturnModel<OrderHistoryUpdateModel>();
            try
            {
                retVal.SuccessResponse = new OrderHistoryUpdateModel() { LastUpdated = _historyHeaderRepo.ReadLatestOrderDate(SelectedUserContext) };
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("LastUpdated", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Retrieve unconfirmed orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("order/admin/submittedUnconfirmed")]
        public Models.OperationReturnModel<List<OrderHeader>> GetUnconfirmedOrders() {
            Models.OperationReturnModel<List<OrderHeader>> retVal = new Models.OperationReturnModel<List<OrderHeader>>();
            try
            {
                List<OrderHeader> orders = _orderLogic.GetSubmittedUnconfirmedOrders();
                retVal.SuccessResponse = orders;
                retVal.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("GetUnconfirmedOrders", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// Resubmit unconfirmed order
        /// </summary>
        /// <param name="controlNumber">Control number</param>
        /// <returns></returns>
        [HttpPut]
        [ApiKeyedRoute("order/admin/resubmitUnconfirmed/{controlNumber}")]
        public Models.OperationReturnModel<bool> ResubmitUnconfirmedOrder(int controlNumber) {
            Models.OperationReturnModel<bool> retVal = new Models.OperationReturnModel<bool>();
            try
            {
                retVal.SuccessResponse = _orderLogic.ResendUnconfirmedOrder(this.AuthenticatedUser, controlNumber, this.SelectedUserContext);
                retVal.IsSuccess = retVal.SuccessResponse;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("ResubmitUnconfirmedOrder", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }

        /// <summary>
        /// set the status of an order to "Lost"
        /// </summary>
        /// <param name="trackingNumber"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("order/admin/setlostorder")]
        public Models.OperationReturnModel<string> SetLostOrder(string trackingNumber)
        {
            Models.OperationReturnModel<string> retVal = new Models.OperationReturnModel<string>();
            try
            {
                if (AuthenticatedUser.RoleName.Equals("beksysadmin", StringComparison.CurrentCultureIgnoreCase))
                {
                    retVal.SuccessResponse = _historyLogic.SetLostOrder(trackingNumber);
                    retVal.IsSuccess = true;
                }
                else
                {
                    retVal.ErrorMessage = "Must be a beksysadmin user";
                    retVal.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("SetLostOrder", ex);
                retVal.ErrorMessage = ex.Message;
                retVal.IsSuccess = false;
            }

            return retVal;
        }
        #endregion
    }
}
