﻿using KeithLink.Svc.Core.Interface.Cart;
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
	public class ShoppingCartController : BaseController
    {
		private readonly IShoppingCartLogic shoppingCartLogic;

		public ShoppingCartController(IUserProfileRepository userProfileRepo, IShoppingCartLogic shoppingCartLogic)
			: base(userProfileRepo)
		{
			this.shoppingCartLogic = shoppingCartLogic;
		}

		[HttpGet]
		[ApiKeyedRoute("cart/{branchId}")]
		public List<ShoppingCart> List(string branchId, bool header = false)
		{
			return shoppingCartLogic.ReadAllCarts(this.AuthenticatedUser, branchId, header);
		}

		[HttpPut]
		[ApiKeyedRoute("cart/")]
		public void Put(ShoppingCart updatedCart, bool deleteomitted = true)
		{
			shoppingCartLogic.UpdateCart(this.AuthenticatedUser, updatedCart, deleteomitted);
		}


		[HttpGet]
		[ApiKeyedRoute("cart/{branchId}/{cartId}")]
		public ShoppingCart Cart(Guid cartId)
		{
			return shoppingCartLogic.ReadCart(this.AuthenticatedUser, cartId);
		}

		
		[HttpPost]
		[ApiKeyedRoute("cart/{branchId}")]
		public NewItem List(string branchId, ShoppingCart cart)
		{
			return new NewItem() { ListItemId = shoppingCartLogic.CreateCart(this.AuthenticatedUser, branchId, cart) };
		}

		[HttpDelete]
		[ApiKeyedRoute("cart/{cartId}")]
		public void DeleteList(Guid cartId)
		{
			shoppingCartLogic.DeleteCart(this.AuthenticatedUser, cartId);
		}

		[HttpPost]
		[ApiKeyedRoute("cart/{cartId}/item")]
		public NewItem AddItem(Guid cartId, ShoppingCartItem newItem)
		{
			return  new NewItem() { ListItemId = shoppingCartLogic.AddItem(this.AuthenticatedUser, cartId, newItem) };
		}

		[HttpPut]
		[ApiKeyedRoute("cart/{cartId}/item")]
		public void UpdateItem(Guid cartId, ShoppingCartItem updatedItem)
		{
			shoppingCartLogic.UpdateItem(this.AuthenticatedUser, cartId, updatedItem);
		}

		[HttpDelete]
		[ApiKeyedRoute("cart/{cartId}/item/{itemId}")]
		public void DeleteItem(Guid cartId, Guid itemId)
		{
			shoppingCartLogic.DeleteItem(this.AuthenticatedUser, cartId, itemId);
		}

    }
}
