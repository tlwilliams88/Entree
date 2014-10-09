using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
	[Authorize]
	public class ShoppingCartController : BaseController {
        #region attributes
        private readonly IShoppingCartLogic shoppingCartLogic;
        #endregion

        #region ctor
        public ShoppingCartController(IShoppingCartLogic shoppingCartLogic, IUserProfileLogic profileLogic) : base(profileLogic) {
			this.shoppingCartLogic = shoppingCartLogic;
		}
        #endregion

        #region methods
        [HttpGet]
		[ApiKeyedRoute("cart/")]
		public List<ShoppingCart> List(bool header = false)
		{
			return shoppingCartLogic.ReadAllCarts(this.AuthenticatedUser, this.SelectedUserContext, header);
		}

		[HttpGet]
		[ApiKeyedRoute("cart/{cartId}")]
		public ShoppingCart Cart(Guid cartId)
		{
			return shoppingCartLogic.ReadCart(this.AuthenticatedUser, cartId);
		}

		[HttpPost]
		[ApiKeyedRoute("cart/")]
		public NewItem List(ShoppingCart cart)
		{
			return new NewItem() { ListItemId = shoppingCartLogic.CreateCart(this.AuthenticatedUser, this.SelectedUserContext, cart) };
		}

		[HttpPost]
		[ApiKeyedRoute("cart/{cartId}/item")]
		public NewItem AddItem(Guid cartId, ShoppingCartItem newItem)
		{
			return new NewItem() { ListItemId = shoppingCartLogic.AddItem(this.AuthenticatedUser, cartId, newItem) };
		}

		[HttpPut]
		[ApiKeyedRoute("cart/{cartId}/item")]
		public void UpdateItem(Guid cartId, ShoppingCartItem updatedItem)
		{
			shoppingCartLogic.UpdateItem(this.AuthenticatedUser, cartId, updatedItem);
		}

		[HttpPut]
		[ApiKeyedRoute("cart/")]
		public void Put(ShoppingCart updatedCart, bool deleteomitted = true)
		{
			shoppingCartLogic.UpdateCart(this.SelectedUserContext, this.AuthenticatedUser, updatedCart, deleteomitted);
		}

		[HttpDelete]
		[ApiKeyedRoute("cart/{cartId}")]
		public void DeleteList(Guid cartId)
		{
			shoppingCartLogic.DeleteCart(this.AuthenticatedUser, cartId);
		}

		[HttpDelete]
		[ApiKeyedRoute("cart/")]
		public void DeleteList(List<Guid> cartIds)
		{
			shoppingCartLogic.DeleteCarts(this.AuthenticatedUser.UserId, cartIds);
		}


		[HttpDelete]
		[ApiKeyedRoute("cart/{cartId}/item/{itemId}")]
		public void DeleteItem(Guid cartId, Guid itemId)
		{
			shoppingCartLogic.DeleteItem(this.AuthenticatedUser, cartId, itemId);
		}
        #endregion
    }
}
