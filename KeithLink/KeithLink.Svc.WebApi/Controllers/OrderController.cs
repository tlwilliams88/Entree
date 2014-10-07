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
        private readonly IShoppingCartLogic shoppingCartLogic;
		private readonly IOrderLogic orderLogic;
        #endregion

        #region ctor
        public OrderController(IShoppingCartLogic shoppingCartLogic, IUserProfileLogic profileLogic, IOrderLogic orderLogic): base(profileLogic) {
			this.shoppingCartLogic = shoppingCartLogic;
			this.orderLogic = orderLogic;
		}
        #endregion

        #region methods
        [HttpPost]
		[ApiKeyedRoute("order/{cartId}")]
		public string SaveOrder(Guid cartId)
		{
			return shoppingCartLogic.SaveAsOrder(this.AuthenticatedUser, cartId);
		}

		[HttpGet]
		[ApiKeyedRoute("order/")]
		public List<Order> Orders()
		{
			return orderLogic.ReadOrders(this.AuthenticatedUser, this.RequestCatalogInfo);
		}

		[HttpGet]
		[ApiKeyedRoute("order/{orderNumber}")]
		public Order Orders(string orderNumber)
		{
			return orderLogic.ReadOrder(this.AuthenticatedUser, this.RequestCatalogInfo, orderNumber);
		}

        #endregion
    }
}
