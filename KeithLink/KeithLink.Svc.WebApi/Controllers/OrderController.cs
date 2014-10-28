using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Impl.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
	[Authorize]
    public class OrderController : BaseController {
        #region attributes
        private readonly IShoppingCartLogic _shoppingCartLogic;
		private readonly IOrderLogic _orderLogic;
        private IShipDateRepository _shipDayService;
        #endregion

        #region ctor
        public OrderController(IShoppingCartLogic shoppingCartLogic, IOrderLogic orderLogic, IShipDateRepository shipDayRepo, 
                               IUserProfileLogic profileLogic): base(profileLogic) {
			_shoppingCartLogic = shoppingCartLogic;
			_orderLogic = orderLogic;
            _shipDayService = shipDayRepo;
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
			return _orderLogic.ReadOrders(this.AuthenticatedUser, this.SelectedUserContext);
		}

		[HttpGet]
		[ApiKeyedRoute("order/{orderNumber}")]
		public Order Orders(string orderNumber)
		{
			return _orderLogic.ReadOrder(this.AuthenticatedUser, this.SelectedUserContext, orderNumber);
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
        public Models.OperationReturnModel<List<Order>> GetChangeOrders()
        {
            List<Order> changeOrders = _orderLogic.ReadOrders(this.AuthenticatedUser, this.SelectedUserContext);

            Models.OperationReturnModel<List<Order>> ret = new Models.OperationReturnModel<List<Order>>();
            ret.SuccessResponse = changeOrders.Where(x => x.IsChangeOrderAllowed).ToList();
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
        public Models.OperationReturnModel<bool> CancelOrder(Guid commerceId)
        {
            Models.OperationReturnModel<bool> ret = new Models.OperationReturnModel<bool>();
            ret.SuccessResponse = true;
            return ret;
        }
        #endregion
    }
}
