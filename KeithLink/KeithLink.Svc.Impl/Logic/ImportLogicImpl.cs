using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Enumerations.List;

namespace KeithLink.Svc.Impl.Logic
{
	public class ImportLogicImpl: IImportLogic
	{
		private IListServiceRepository listServiceRepository;
		private ICatalogLogic catalogLogic;
		private IEventLogRepository eventLogRepository;
		private IShoppingCartLogic shoppingCartLogic;

		public ImportLogicImpl(IListServiceRepository listServiceRepository, ICatalogLogic catalogLogic, IEventLogRepository eventLogRepository,IShoppingCartLogic shoppingCartLogic)
		{
			this.listServiceRepository = listServiceRepository;
			this.catalogLogic = catalogLogic;
			this.eventLogRepository = eventLogRepository;
			this.shoppingCartLogic = shoppingCartLogic;
		}

		public ListImportModel ImportList(UserProfile user, UserSelectedContext catalogInfo, string csvFile)
		{
			try
			{
				var importReturn = new ListImportModel();
				var newList = new ListModel() { Name = string.Format("Imported List - {0}", DateTime.Now.ToShortDateString()), BranchId = catalogInfo.BranchId};

				var rows = csvFile.Split('\n');

				var items = rows.Skip(1).Select(i => i.Split(',')).Select(l => new ListItemModel() { ItemNumber = l[0].ToString() }).Where(x => !string.IsNullOrEmpty(x.ItemNumber)).ToList();

				//Verify the user has these products in their catalog
				var prods = catalogLogic.GetProductsByIds(catalogInfo.BranchId, items.Select(i => i.ItemNumber).Distinct().ToList(), user);

				//Item counts don't match, which means there are items in the import that don't belong to this catalog
				if (prods.Products.Select(p => p.ItemNumber).Distinct().Count() != items.Select(i => i.ItemNumber).Distinct().Count())
				{
					importReturn.WarningMessage = "Some items were not imported because they were not found in the current catalog.";

					newList.Items = new List<ListItemModel>();
					foreach (var item in items)
						if (prods.Products.Where(p => p.ItemNumber.Equals(item.ItemNumber)).Any())
							newList.Items.Add(item);
				}
				else
					newList.Items = items; //All items are in the user's catalog


				importReturn.ListId = 	listServiceRepository.CreateList(user.UserId, catalogInfo, newList, ListType.Custom);

				importReturn.Success = true;

				return importReturn;
			}
			catch (Exception ex)
			{
				eventLogRepository.WriteErrorLog(string.Format("List Import Error for Customer {0}", catalogInfo.CustomerId), ex);
				return new ListImportModel() { Success = false, ErrorMessage = "An error has occurred while processing the import file" };
			}
		}


		public OrderImportModel ImportOrder(UserProfile user, UserSelectedContext catalogInfo, Core.Models.Orders.OrderImportOptions options, string fileContents)
		{
			var returnModel = new OrderImportModel();

			var newCart = new ShoppingCart { 
                Name = string.Format("Imported Order - {0}", DateTime.Now.ToString("g")), 
				BranchId = catalogInfo.BranchId };

			var items = new List<ShoppingCartItem>();

			switch (options.FileFormat)
			{
				case FileFormat.CSV:
					var rows = fileContents.Split('\n');
					items = rows
                        .Skip(options.IgnoreFirstLine ? 1 : 0)
                        .Select(i => i.Split(','))
                        .Where(f => f[0] == "y")
                        .Select(l => new ShoppingCartItem() { 
                            ItemNumber = DetermineItemNumber(l, options), 
                            Quantity = DetermineQuantity(l, options) })
                        .Where(x => !string.IsNullOrEmpty(x.ItemNumber))
                        .ToList();
					break;
				case FileFormat.Tab:
					var tabrows = fileContents.Split('\n');
					items = tabrows
                        .Skip(options.IgnoreFirstLine ? 1 : 0)
                        .Select(i => i.Split((char)9))
                        .Select(l => new ShoppingCartItem() { 
                            ItemNumber = DetermineItemNumber(l, options), 
                            Quantity = DetermineQuantity(l, options) })
                        .Where(x => !string.IsNullOrEmpty(x.ItemNumber))
                        .ToList();
					break;
				case FileFormat.Excel:
					break;
			}

			if (options.IgnoreZeroQuantities)
				items = items.Where(i => i.Quantity > 0).ToList();


			newCart.Items = items;

			var cartId = shoppingCartLogic.CreateCart(user, catalogInfo, newCart);
			
			return returnModel;
			
		}

		private string DetermineItemNumber(string[] l, OrderImportOptions options)
		{
			switch (options.ItemNumber)
			{
				case ItemNumberType.ItemNumberOrUPC:
					//TODO: Determine if UPC or ItemNumber??? Lookup if UPC

					return l[0];
				case ItemNumberType.UPC:
					//TODO: Lookup ItemNumber

					return l[0];
				default: //ItemNumber
					//Just return value
					return l[0];

			}
		}

		private decimal DetermineQuantity(string[] l, OrderImportOptions options)
		{

			if(options.Contents == FileContentType.ItemOnly)
				return 0;

			var returnDecimal = l[1].ToDecimal();
			return returnDecimal.HasValue ? returnDecimal.Value : 0;
		}
	}
}
