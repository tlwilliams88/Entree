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
    public class OrderController : BaseController
    {

		private readonly IShoppingCartLogic shoppingCartLogic;

		public OrderController(IUserProfileRepository userProfileRepo, IShoppingCartLogic shoppingCartLogic): base(userProfileRepo)
		{
			this.shoppingCartLogic = shoppingCartLogic;
		}

		[HttpPost]
		[Route("order/{cartId}")]
		public string SaveOrder(Guid cartId)
		{
			return shoppingCartLogic.SaveAsOrder(this.AuthenticatedUser, cartId);
		}

    }
}
