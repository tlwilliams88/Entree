﻿using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Cart
{
	public interface IShoppingCartLogic
	{
		Guid CreateCart(UserProfile user, UserSelectedContext catalogInfo, ShoppingCart cart, string catalogId = null);
		Guid? AddItem(UserProfile user, UserSelectedContext catalogInfo, Guid cartId, ShoppingCartItem newItem);
		
		void UpdateItem(UserProfile user,  UserSelectedContext catalogInfo,  Guid cartId, ShoppingCartItem updatedItem);
		void UpdateCart(UserSelectedContext catalogInfo, UserProfile user, ShoppingCart cart, bool deleteOmmitedItems);

		void DeleteCart(UserProfile user, UserSelectedContext catalogInfo, Guid cartId);
		void DeleteCarts(UserProfile user, UserSelectedContext catalogInfo, List<Guid> cartIds);
		void DeleteItem(UserProfile user, UserSelectedContext catalogInfo, Guid cartId, Guid itemId);

		List<ShoppingCart> ReadAllCarts(UserProfile user, UserSelectedContext catalogInfo, bool headerInfoOnly);
		ShoppingCart ReadCart(UserProfile user, UserSelectedContext catalogInfo, Guid cartId);

        /// <summary>
        /// Method to check for a cart that has been submitted
        /// </summary>
        bool IsSubmitted(UserProfile user, UserSelectedContext catalogInfo, Guid cartId);

        System.IO.MemoryStream CartReport(UserProfile user, UserSelectedContext context, Guid cartId, PagedListModel list, PrintListModel options);

        SaveOrderReturn SaveAsOrder(UserProfile user,  UserSelectedContext catalogInfo, Guid cartId);

		void SetActive(UserProfile user, UserSelectedContext catalogInfo, Guid cartId);

		List<ItemValidationResultModel> ValidateItems(UserSelectedContext catalogInfo, UserProfile user, List<QuickAddItemModel> productsToValidate);

		QuickAddReturnModel CreateQuickAddCart(UserProfile user, UserSelectedContext catalogInfo, List<QuickAddItemModel> items);

	}
}
