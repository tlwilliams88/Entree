using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Interface.Profile;
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
        #endregion

        #region ctor
        public OrderController(IShoppingCartLogic shoppingCartLogic, IUserProfileLogic profileLogic): base(profileLogic) {
			this.shoppingCartLogic = shoppingCartLogic;
		}
        #endregion

        #region methods
        [HttpPost]
		[ApiKeyedRoute("order/{cartId}")]
		public string SaveOrder(Guid cartId)
		{
			return shoppingCartLogic.SaveAsOrder(this.AuthenticatedUser, cartId);
		}
        #endregion
    }
}
