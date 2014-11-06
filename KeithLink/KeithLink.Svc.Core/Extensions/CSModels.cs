using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Models.SiteCatalog;
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
		public static CS.LineItem ToLineItem(this ListItemModel listItem, string branchId)
		{
			//return new CS.LineItem() { 
			//	Id = listItem.ListItemId.ToCommerceServerFormat(),  
			//	CatalogName = branchId, 
			//	ParLevel = listItem.ParLevel, 
			//	LinePosition = listItem.Position.ToString(), 
			//	Label = listItem.Label, 
			//	ProductId = listItem.ItemNumber,
			//	CatchWeight = listItem.CatchWeight,
			//	Category = listItem.Category
			//};
			return null;
		}

		public static CS.LineItem ToLineItem(this ShoppingCartItem cartItem, string branchId)
		{
			return new CS.LineItem() {
                Id = cartItem.CartItemId.ToCommerceServerFormat(), 
                CatalogName = branchId, 
                Notes = cartItem.Notes, 
                ProductId = cartItem.ItemNumber, 
                DisplayName = cartItem.Name,
                Quantity = cartItem.Quantity, 
                Each = cartItem.Each,
                CatchWeight = cartItem.CatchWeight
            };
		}

		public static Division ToDivision(this CS.Catalog catalog)
		{
			return new Division() { Id = catalog.Id, Name = catalog.DisplayName };
		}
	}
}
