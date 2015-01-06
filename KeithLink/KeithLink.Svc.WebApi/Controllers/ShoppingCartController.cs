using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Interface.Orders;
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
		private readonly IOrderServiceRepository orderServiceRepository;
        #endregion

        #region ctor
        public ShoppingCartController(IShoppingCartLogic shoppingCartLogic, IUserProfileLogic profileLogic, IOrderServiceRepository orderServiceRepository) : base(profileLogic) {
			this.shoppingCartLogic = shoppingCartLogic;
			this.orderServiceRepository = orderServiceRepository;
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
			return shoppingCartLogic.ReadCart(this.AuthenticatedUser, this.SelectedUserContext, cartId);
		}

		[HttpPost]
		[ApiKeyedRoute("cart/")]
		public NewCSItem List(ShoppingCart cart)
		{
			return new NewCSItem() { Id = shoppingCartLogic.CreateCart(this.AuthenticatedUser, this.SelectedUserContext, cart) };
		}

		[HttpPost]
		[ApiKeyedRoute("cart/{cartId}/item")]
		public NewCSItem AddItem(Guid cartId, ShoppingCartItem newItem)
		{
			return new NewCSItem() { Id = shoppingCartLogic.AddItem(this.AuthenticatedUser, this.SelectedUserContext, cartId, newItem) };
		}

		[HttpPut]
		[ApiKeyedRoute("cart/{cartId}/item")]
		public void UpdateItem(Guid cartId, ShoppingCartItem updatedItem)
		{
			shoppingCartLogic.UpdateItem(this.AuthenticatedUser, this.SelectedUserContext, cartId, updatedItem);
		}

		[HttpPut]
		[ApiKeyedRoute("cart/")]
		public void Put(ShoppingCart updatedCart, bool deleteomitted = true)
		{
			shoppingCartLogic.UpdateCart(this.SelectedUserContext, this.AuthenticatedUser, updatedCart, deleteomitted);
		}

		[HttpPut]
		[ApiKeyedRoute("cart/{cartId}/active")]
		public void SetActive(Guid cartId)
		{
			orderServiceRepository.SaveUserActiveCart(this.SelectedUserContext, this.AuthenticatedUser.UserId, cartId);
		}

		[HttpDelete]
		[ApiKeyedRoute("cart/{cartId}")]
		public void DeleteList(Guid cartId)
		{
			shoppingCartLogic.DeleteCart(this.AuthenticatedUser, this.SelectedUserContext, cartId);
		}

		[HttpDelete]
		[ApiKeyedRoute("cart/")]
		public void DeleteList(List<Guid> cartIds)
		{
			shoppingCartLogic.DeleteCarts(this.AuthenticatedUser, this.SelectedUserContext, cartIds);
		}


		[HttpDelete]
		[ApiKeyedRoute("cart/{cartId}/item/{itemId}")]
		public void DeleteItem(Guid cartId, Guid itemId)
		{
			shoppingCartLogic.DeleteItem(this.AuthenticatedUser, this.SelectedUserContext, cartId, itemId);
		}
        #endregion
    }
}
