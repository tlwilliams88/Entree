using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
	[Authorize]
	public class ShoppingCartController : BaseController
	{
		#region attributes
		private readonly IShoppingCartLogic shoppingCartLogic;
		private readonly IOrderServiceRepository orderServiceRepository;
		#endregion

		#region ctor
		public ShoppingCartController(IShoppingCartLogic shoppingCartLogic, IUserProfileLogic profileLogic, IOrderServiceRepository orderServiceRepository)
			: base(profileLogic)
		{
			this.shoppingCartLogic = shoppingCartLogic;
			this.orderServiceRepository = orderServiceRepository;
		}
		#endregion

		#region methods
		/// <summary>
		/// Retrieve user shopping carts (orders not yet submitted)
		/// </summary>
		/// <param name="header">Header level info only?</param>
		/// <returns></returns>
		[HttpGet]
		[ApiKeyedRoute("cart/")]
		public List<ShoppingCart> List(bool header = false)
		{
			return shoppingCartLogic.ReadAllCarts(this.AuthenticatedUser, this.SelectedUserContext, header);
		}

		/// <summary>
		/// Retrieve a specific cart
		/// </summary>
		/// <param name="cartId">Cart id</param>
		/// <returns></returns>
		[HttpGet]
		[ApiKeyedRoute("cart/{cartId}")]
		public ShoppingCart Cart(Guid cartId)
		{
			return shoppingCartLogic.ReadCart(this.AuthenticatedUser, this.SelectedUserContext, cartId);
		}

		/// <summary>
		/// Create a user cart
		/// </summary>
		/// <param name="cart">Cart</param>
		/// <returns></returns>
		[HttpPost]
		[ApiKeyedRoute("cart/")]
		public NewCSItem Cart(ShoppingCart cart)
		{
			return new NewCSItem() { Id = shoppingCartLogic.CreateCart(this.AuthenticatedUser, this.SelectedUserContext, cart) };
		}

		/// <summary>
		/// Create a new cart from the quick add screen
		/// </summary>
		/// <param name="items">Items for the new cart</param>
		/// <returns></returns>
		[HttpPost]
		[ApiKeyedRoute("cart/quickadd")]
		public QuickAddReturnModel QuickAddCart(List<QuickAddItemModel> items)
		{
			return shoppingCartLogic.CreateQuickAddCart(this.AuthenticatedUser, this.SelectedUserContext, items);
		}

		/// <summary>
		/// Validate if items are valid in the quick add screen
		/// </summary>
		/// <param name="items">Items to validate</param>
		/// <returns></returns>
		[HttpPost]
		[ApiKeyedRoute("cart/quickadd/validate")]
		public List<ItemValidationResultModel> Validate(List<QuickAddItemModel> items)
		{
			return shoppingCartLogic.ValidateItems(this.SelectedUserContext, items);
		}

		/// <summary>
		/// Add a new item to an existing cart
		/// </summary>
		/// <param name="cartId">Cart id</param>
		/// <param name="newItem">Item</param>
		/// <returns></returns>
		[HttpPost]
		[ApiKeyedRoute("cart/{cartId}/item")]
		public NewCSItem AddItem(Guid cartId, ShoppingCartItem newItem)
		{
			return new NewCSItem() { Id = shoppingCartLogic.AddItem(this.AuthenticatedUser, this.SelectedUserContext, cartId, newItem) };
		}

		/// <summary>
		/// Update item on an existing cart
		/// </summary>
		/// <param name="cartId">Cart id</param>
		/// <param name="updatedItem">Item</param>
		[HttpPut]
		[ApiKeyedRoute("cart/{cartId}/item")]
		public void UpdateItem(Guid cartId, ShoppingCartItem updatedItem)
		{
			shoppingCartLogic.UpdateItem(this.AuthenticatedUser, this.SelectedUserContext, cartId, updatedItem);
		}

		/// <summary>
		/// Update existing cart
		/// </summary>
		/// <param name="updatedCart">Cart</param>
		/// <param name="deleteomitted">Delete items ommitted from the request?</param>
		[HttpPut]
		[ApiKeyedRoute("cart/")]
		public void Put(ShoppingCart updatedCart, bool deleteomitted = true)
		{
			shoppingCartLogic.UpdateCart(this.SelectedUserContext, this.AuthenticatedUser, updatedCart, deleteomitted);
		}

		/// <summary>
		/// Set a cart as the active cart. (Is this still used?)
		/// </summary>
		/// <param name="cartId"></param>
		[HttpPut]
		[ApiKeyedRoute("cart/{cartId}/active")]
		public void SetActive(Guid cartId)
		{
			orderServiceRepository.SaveUserActiveCart(this.SelectedUserContext, this.AuthenticatedUser.UserId, cartId);
		}

		/// <summary>
		/// Delete a cart
		/// </summary>
		/// <param name="cartId">Cart id</param>
		[HttpDelete]
		[ApiKeyedRoute("cart/{cartId}")]
		public void DeleteCart(Guid cartId)
		{
			shoppingCartLogic.DeleteCart(this.AuthenticatedUser, this.SelectedUserContext, cartId);
		}

		/// <summary>
		/// Delete a list of carts
		/// </summary>
		/// <param name="cartIds">List of cart ids</param>
		[HttpDelete]
		[ApiKeyedRoute("cart/")]
		public void DeleteCarts(List<Guid> cartIds)
		{
			shoppingCartLogic.DeleteCarts(this.AuthenticatedUser, this.SelectedUserContext, cartIds);
		}

		/// <summary>
		/// Delete item from a cart
		/// </summary>
		/// <param name="cartId">Cart id</param>
		/// <param name="itemId">Item id</param>
		[HttpDelete]
		[ApiKeyedRoute("cart/{cartId}/item/{itemId}")]
		public void DeleteItem(Guid cartId, Guid itemId)
		{
			shoppingCartLogic.DeleteItem(this.AuthenticatedUser, this.SelectedUserContext, cartId, itemId);
		}
		#endregion
	}
}
