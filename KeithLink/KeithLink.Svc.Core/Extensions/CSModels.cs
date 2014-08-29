using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS = KeithLink.Svc.Core.Models.Generated;

namespace KeithLink.Svc.Core.Extensions
{
	public static class CSModels
	{
		public static CS.LineItem ToLineItem(this ListItem listItem, string branchId)
		{
			return new CS.LineItem() { Id = listItem.ListItemId.ToString("B"),  CatalogName = branchId, ParLevel = listItem.ParLevel, LinePosition = listItem.Position.ToString(), Label = listItem.Label, ProductId = listItem.ItemNumber };
		}

		public static CS.LineItem ToLineItem(this ShoppingCartItem cartItem, string branchId)
		{
			return new CS.LineItem() {Id = cartItem.CartItemId.ToString("B"), CatalogName = branchId, Notes = cartItem.Notes, ProductId = cartItem.ItemNumber, Quantity = cartItem.Quantity, Each = cartItem.Each };
		}
	}
}
