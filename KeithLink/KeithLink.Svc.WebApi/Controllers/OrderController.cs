using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Interface.Configuration;
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
        private readonly IShipDateRepository _shipDayService;
        private readonly IShoppingCartLogic _shoppingCartLogic;
		private readonly IOrderServiceRepository _orderServiceRepository;
		private readonly IExportSettingServiceRepository _exportSettingRepository;
        #endregion

        #region ctor
        public OrderController(IShoppingCartLogic shoppingCartLogic, IOrderLogic orderLogic, IShipDateRepository shipDayRepo,
							   IOrderHistoryRequestLogic historyRequestLogic, IUserProfileLogic profileLogic, IOrderServiceRepository orderServiceRepository, 
                               IExportSettingServiceRepository exportSettingRepository)
			: base(profileLogic)
		{
            _historyRequestLogic = historyRequestLogic;
			_orderLogic = orderLogic;
            _shipDayService = shipDayRepo;
			_shoppingCartLogic = shoppingCartLogic;
			this._orderServiceRepository = orderServiceRepository;
			this._exportSettingRepository = exportSettingRepository;
        }
        #endregion

        #region methods
        [HttpGet]
        [ApiKeyedRoute("order/shipdays")]
        public ShipDateReturn GetShipDays() {
            return _shipDayService.GetShipDates(this.SelectedUserContext);
        }

		[HttpGet]
		[ApiKeyedRoute("order/")]
		public List<Order> Orders()
		{
            return _orderLogic.UpdateOrdersForSecurity(this.AuthenticatedUser,
                _orderServiceRepository.GetCustomerOrders(this.AuthenticatedUser.UserId, this.SelectedUserContext));
		}

		[HttpPost]
		[ApiKeyedRoute("order/")]
		public PagedResults<Order> PagedOrders(PagingModel paging)
		{
			var results = _orderServiceRepository.GetPagedOrders(this.AuthenticatedUser.UserId, this.SelectedUserContext, paging);
			_orderLogic.UpdateOrdersForSecurity(this.AuthenticatedUser, results.Results);
			return results;
		}

		[HttpGet]
		[ApiKeyedRoute("order/date")]
		public List<Order> OrdersIndate(DateTime from, DateTime to)
		{
			return _orderLogic.UpdateOrdersForSecurity(this.AuthenticatedUser,
                _orderServiceRepository.GetOrderHeaderInDateRange(this.AuthenticatedUser.UserId, this.SelectedUserContext, from, to));
		}
        
		[HttpPost]
		[ApiKeyedRoute("order/export/")]
		public HttpResponseMessage ExportOrders(ExportRequestModel exportRequest)
		{
            //var orders = _orderLogic.ReadOrders(this.AuthenticatedUser, this.SelectedUserContext);
            var orders = _orderServiceRepository.GetCustomerOrders(this.AuthenticatedUser.UserId, this.SelectedUserContext);
			if (exportRequest.Fields != null)
				_exportSettingRepository.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.Order, KeithLink.Svc.Core.Enumerations.List.ListType.Custom, exportRequest.Fields, exportRequest.SelectedType);
			
			return ExportModel<Order>(orders, exportRequest);
		}

		[HttpGet]
		[ApiKeyedRoute("order/export")]
		public ExportOptionsModel ExportOrders()
		{
			return _exportSettingRepository.ReadCustomExportOptions(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.Order, 0);
		}

		[HttpGet]
		[ApiKeyedRoute("order/{orderNumber}")]
		public Order Orders(string orderNumber)
		{
			//return _orderLogic.ReadOrder(this.AuthenticatedUser, this.SelectedUserContext, orderNumber);
            try {
                return _orderLogic.UpdateOrderForEta(this.AuthenticatedUser,
                    _orderServiceRepository.GetOrder(SelectedUserContext.BranchId, orderNumber.Trim()));
            } catch (Exception ex) {
                return null;
            }
		}

		[HttpPost]
		[ApiKeyedRoute("order/export/{orderNumber}")]
		public HttpResponseMessage ExportOrderDetail(string orderNumber, ExportRequestModel exportRequest)
		{
			var order = _orderLogic.ReadOrder(this.AuthenticatedUser, this.SelectedUserContext, orderNumber);
			if (exportRequest.Fields != null)
				_exportSettingRepository.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.OrderDetail, KeithLink.Svc.Core.Enumerations.List.ListType.Custom, exportRequest.Fields, exportRequest.SelectedType);

			return ExportModel<OrderLine>(order.Items, exportRequest);			
		}

		[HttpGet]
		[ApiKeyedRoute("order/export/{orderNumber}")]
		public ExportOptionsModel ExportOrderDetail(string orderNumber)
		{
			return _exportSettingRepository.ReadCustomExportOptions(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.OrderDetail, 0);
		}


        [HttpPost]
        [ApiKeyedRoute("order/history")]
        public void RequestOrderHistoryHeaders() {
            _historyRequestLogic.RequestAllOrdersForCustomer(this.SelectedUserContext);
        }

        [HttpPost]
        [ApiKeyedRoute("order/history/{orderNumber}")]
        public void RequestOrderHistory(string orderNumber) {
            _historyRequestLogic.RequestOrderForCustomer(this.SelectedUserContext, orderNumber);
        }

        [HttpPost]
        [ApiKeyedRoute("order/{cartId}")]
        public NewOrderReturn SaveOrder(Guid cartId) {
            return _shoppingCartLogic.SaveAsOrder(this.AuthenticatedUser, this.SelectedUserContext, cartId);
        }

        [HttpPost]
        [ApiKeyedRoute("order/{orderNumber}/changeorder")]
        public NewOrderReturn SaveOrder(string orderNumber)
        {
            return _orderLogic.SubmitChangeOrder(this.AuthenticatedUser, this.SelectedUserContext, orderNumber);
        }

        [HttpGet]
        [ApiKeyedRoute("order/changeorder")]
		public Models.OperationReturnModel<List<Order>> GetChangeOrders(bool header = false)
        {
            List<Order> changeOrders = _orderLogic.ReadOrders(this.AuthenticatedUser, this.SelectedUserContext, header: header);

            Models.OperationReturnModel<List<Order>> ret = new Models.OperationReturnModel<List<Order>>();
            ret.SuccessResponse = changeOrders.Where(x => x.IsChangeOrderAllowed).OrderByDescending(o => o.InvoiceNumber).ToList();
            return ret;
        }

        [HttpPut]
        [ApiKeyedRoute("order/")]
        public Order UpdateOrder(Order order, bool deleteOmitted = true)
        {
            return _orderLogic.UpdateOrder(this.SelectedUserContext, this.AuthenticatedUser, order, deleteOmitted);
        }

        [HttpDelete]
        [ApiKeyedRoute("order/{commerceId}")]
        public NewOrderReturn CancelOrder(Guid commerceId)
        {
            return _orderLogic.CancelOrder(this.AuthenticatedUser, this.SelectedUserContext, commerceId);
        }

		[HttpGet]
		[ApiKeyedRoute("order/lastupdate")]
		public OrderHistoryUpdateModel LastUpdated()
		{
			return new OrderHistoryUpdateModel() { LastUpdated = _orderServiceRepository.ReadLatestUpdatedDate(this.SelectedUserContext) };
		}

        [HttpGet]
        [ApiKeyedRoute("order/admin/submittedUnconfirmed")]
        public Models.OperationReturnModel<List<OrderHeader>> GetUnconfirmedOrders()
        {
            List<OrderHeader> orders = _orderServiceRepository.GetSubmittedUnconfirmedOrders();
            return new Models.OperationReturnModel<List<OrderHeader>>() { SuccessResponse = orders };
        }

        [HttpPut]
        [ApiKeyedRoute("order/admin/resubmitUnconfirmed/{controlNumber}")]
        public Models.OperationReturnModel<bool> ResubmitUnconfirmedOrder(int controlNumber)
        {
            return new Models.OperationReturnModel<bool>() 
                {
                    SuccessResponse = _orderLogic.ResendUnconfirmedOrder(this.AuthenticatedUser, controlNumber, this.SelectedUserContext) 
                };
        }

        #endregion
    }
}
