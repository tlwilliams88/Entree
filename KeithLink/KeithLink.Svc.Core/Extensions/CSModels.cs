using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
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

		public static CS.LineItem ToLineItem(this ShoppingCartItem cartItem)
		{
			return new CS.LineItem() {
                Id = cartItem.CartItemId.ToCommerceServerFormat(), 
                CatalogName = cartItem.CatalogId, 
                Notes = cartItem.Notes, 
                ProductId = cartItem.ItemNumber, 
                DisplayName = cartItem.Name,
                Quantity = cartItem.Quantity,
                LinePosition = cartItem.Position.ToString(),
                Each = cartItem.Each,
                CatchWeight = cartItem.CatchWeight,
                Label = cartItem.Label,
                ParLevel = cartItem.ParLevel,
                IsCombinedQuantity = cartItem.IsCombinedQuantity,
            };
		}

		public static Division ToDivision(this CS.Catalog catalog)
		{
			return new Division() { Id = catalog.Id, Name = catalog.DisplayName };
		}

        public static Customer ToCustomer(this CS.Organization org) {
            Customer customer = new Customer() {
                CustomerId = Guid.Parse(org.Id),
                AccountId = String.IsNullOrEmpty(org.ParentOrganizationId) ? new Nullable<Guid>() : Guid.Parse(org.ParentOrganizationId),
                ContractId = org.ContractNumber,
                DisplayName = string.Format("{0} - {1}", org.CustomerNumber, org.Name),
                CustomerBranch = org.BranchNumber,
                CustomerName = org.Name,
                CustomerNumber = org.CustomerNumber,
                DsrNumber = org.DsrNumber,
                IsPoRequired = org.IsPoRequired.HasValue ? org.IsPoRequired.Value : false,
                IsPowerMenu = org.IsPowerMenu.HasValue ? org.IsPowerMenu.Value : false,
                // TODO - fill this in from real data source
                Phone = org.PreferredAddress != null
                && !String.IsNullOrEmpty(org.PreferredAddress.Telephone)
                && !org.PreferredAddress.Telephone.Equals("0000000000") ? org.PreferredAddress.Telephone : string.Empty, // get from address profile
                Email = string.Empty,
                PointOfContact = string.Empty,
                TermCode = org.TermCode,
                KPayCustomer = org.AchType == "2" || org.AchType == "3",
                DsmNumber = org.DsmNumber,
                NationalId = org.NationalId,
                NationalNumber = org.NationalNumber,
                NationalSubNumber = org.NationalSubNumber,
                RegionalId = org.RegionalId,
                RegionalNumber = org.RegionalNumber,
                IsKeithNetCustomer = org.IsKeithnetCustomer != null && org.IsKeithnetCustomer.ToLower() == "y" ? true : false,
                NationalIdDesc = !String.IsNullOrEmpty(org.NationalIdDesc) ? org.NationalIdDesc.Trim() : String.Empty,
                NationalNumberSubDesc = !String.IsNullOrEmpty(org.NationalNumberSubDesc) ? org.NationalNumberSubDesc.Trim() : String.Empty,
                RegionalIdDesc = !String.IsNullOrEmpty(org.RegionalIdDesc) ? org.RegionalIdDesc.Trim() : String.Empty,
                RegionalNumberDesc = !String.IsNullOrEmpty(org.RegionalNumberDesc) ? org.RegionalNumberDesc.Trim() : String.Empty,
                CanViewPricing = org.CanViewPricing ?? true
            };

            // fill in the address
            customer.Address = org.PreferredAddress != null ? new Address() {
                StreetAddress =
                    !String.IsNullOrEmpty(org.PreferredAddress.Line1) && !String.IsNullOrEmpty(org.PreferredAddress.Line2)
                    ? org.PreferredAddress.Line1 + System.Environment.NewLine + org.PreferredAddress.Line2
                    : !String.IsNullOrEmpty(org.PreferredAddress.Line1) ? org.PreferredAddress.Line1 : string.Empty,
                City = !String.IsNullOrEmpty(org.PreferredAddress.City) ? org.PreferredAddress.City : string.Empty,
                RegionCode = !String.IsNullOrEmpty(org.PreferredAddress.StateProvinceCode) ? org.PreferredAddress.StateProvinceCode : string.Empty,
                PostalCode = !String.IsNullOrEmpty(org.PreferredAddress.ZipPostalCode) ? org.PreferredAddress.ZipPostalCode : string.Empty
            }
                    : new Address() { StreetAddress = string.Empty, City = string.Empty, RegionCode = string.Empty, PostalCode = string.Empty };

            return customer;
        }
	}
}
