using KeithLink.Svc.Core.Interface.Lists;
using CS = KeithLink.Svc.Core.Models.Generated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.SiteCatalog;
using CommerceServer.Core.Runtime.Orders;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Models.Profile;
using System.Text.RegularExpressions;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Enumerations.List;

namespace KeithLink.Svc.Impl.Logic
{
	public class ListLogicImpl : IListLogic
	{
		private const string FAVORITESLIST = "Favorites";

		private readonly IBasketRepository basketRepository;
		private readonly ICatalogRepository catalogRepository;
		private readonly IPriceRepository priceRepository;
		private readonly IItemNoteLogic itemNoteLogic;
		private readonly IBasketLogic basketLogic;

		public ListLogicImpl(IBasketRepository basketRepository, ICatalogRepository catalogRepository,
			IPriceRepository priceRepository, IItemNoteLogic itemNoteLogic, IBasketLogic basketLogic)
		{
			this.basketRepository = basketRepository;
			this.catalogRepository = catalogRepository;
			this.priceRepository = priceRepository;
			this.itemNoteLogic = itemNoteLogic;
			this.basketLogic = basketLogic;
		}

		public Guid CreateList(Guid userId, UserSelectedContext catalogInfo, UserList list)
		{
			var newBasket = new CS.Basket();
			newBasket.BranchId = catalogInfo.BranchId.ToLower();
			newBasket.DisplayName = list.Name;
			newBasket.ListType = (int)ListType.Custom;
			newBasket.Name = ListName(list.Name, catalogInfo);
			newBasket.CustomerId = catalogInfo.CustomerId;
			newBasket.IsContractList = list.IsContractList;
			newBasket.ReadOnly = list.ReadOnly;
			newBasket.Shared = true;

			return basketRepository.CreateOrUpdateBasket(userId, catalogInfo.BranchId.ToLower(), newBasket, list.Items.Select(l => l.ToLineItem(catalogInfo.BranchId)).ToList());
		}

		public Guid? AddItem(UserProfile user, UserSelectedContext catalogInfo, Guid listId, ListItem newItem)
		{
			var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, listId);

			if (basket == null)
				return null;

			return basketRepository.AddItem(listId, newItem.ToLineItem(basket.BranchId), basket);
		}

		public void UpdateItem(UserProfile user, Guid listId, ListItem updatedItem, UserSelectedContext catalogInfo)
		{
			var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, listId);

			if (basket == null)
				return;

			basketRepository.UpdateItem(basket.UserId.ToGuid(), listId, updatedItem.ToLineItem(catalogInfo.BranchId.ToLower()));
		}

		public void DeleteItem(UserProfile user, UserSelectedContext catalogInfo, Guid listId, Guid itemId)
		{
			var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, listId);

			if (basket == null)
				return;

			basketRepository.DeleteItem(basket.UserId.ToGuid(), listId, itemId);
		}

		public List<UserList> ReadAllLists(UserProfile user, UserSelectedContext catalogInfo, bool headerInfoOnly)
		{
			var lists = basketLogic.RetrieveAllSharedCustomerBaskets(user, catalogInfo, ListType.Custom, true);


			if (!lists.Where(l => l.Name.Equals(ListName(FAVORITESLIST, catalogInfo)) && !string.IsNullOrEmpty(l.CustomerId) && l.CustomerId.Equals(catalogInfo.CustomerId)).Any())
			{
				//favorites list doesn't exist yet, create an empty one
				basketRepository.CreateOrUpdateBasket(user.UserId, catalogInfo.BranchId.ToLower(), new CS.Basket() { Shared = false, DisplayName = FAVORITESLIST, ListType = (int)ListType.Favorite, BranchId = catalogInfo.BranchId, CustomerId = catalogInfo.CustomerId, Name = ListName(FAVORITESLIST, catalogInfo) }, null);
				lists = basketLogic.RetrieveAllSharedCustomerBaskets(user, catalogInfo, ListType.Custom,true);
			}

			var listForBranch = lists.Where(b => b.BranchId.Equals(catalogInfo.BranchId, StringComparison.InvariantCultureIgnoreCase) &&
				!string.IsNullOrEmpty(b.CustomerId) &&
				b.CustomerId.Equals(catalogInfo.CustomerId));


			if (headerInfoOnly)
				return listForBranch.Select(l => new UserList() { ListId = l.Id.ToGuid(), Name = l.DisplayName, IsContractList = l.IsContractList.HasValue ? l.IsContractList.Value : false, ReadOnly = l.ReadOnly.HasValue ? l.ReadOnly.Value : false }).ToList();
			else
			{
				var returnList = listForBranch.Select(b => ToUserList(b)).ToList();
				returnList.ForEach(delegate(UserList list)
				{
					LookupProductDetails(user, list, catalogInfo);
				});
				return returnList;
			}
		}

		public UserList ReadList(UserProfile user, Guid listId, UserSelectedContext catalogInfo)
		{
			var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, listId);
			if (basket == null)
				return null;

			var list = ToUserList(basket);

			LookupProductDetails(user, list, catalogInfo);
			return list;
		}

		public List<string> ReadListLabels(UserProfile user, UserSelectedContext catalogInfo, Guid listId)
		{
			var lists = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, listId);

			if (lists == null || lists.LineItems == null)
				return null;

			return lists.LineItems.Where(l => l.Label != null).Select(i => i.Label).Distinct().ToList();
		}

		public List<string> ReadListLabels(UserProfile user, UserSelectedContext catalogInfo)
		{
			var lists = basketLogic.RetrieveAllSharedCustomerBaskets(user, catalogInfo, ListType.Custom);
			return lists.Where(i => i.LineItems != null && i.ListType.Equals((int)ListType.Custom) && i.BranchId.Equals(catalogInfo.BranchId.ToLower())).SelectMany(l => l.LineItems.Where(b => b.Label != null).Select(i => i.Label)).Distinct().ToList();
		}

		public void DeleteItems(UserProfile user, UserSelectedContext catalogInfo, Guid listId, List<Guid> itemIds)
		{
			var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, listId);

			if (basket == null && !(basket.ReadOnly.HasValue && basket.ReadOnly.Value))
				return;

			foreach (var itemId in itemIds)
				basketRepository.DeleteItem(basket.UserId.ToGuid(), listId, itemId);
		}

		public void DeleteList(UserProfile user, UserSelectedContext catalogInfo, Guid listId)
		{
			var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, listId);

			if (basket == null)
				return;

			basketRepository.DeleteBasket(basket.UserId.ToGuid(), listId);
		}
		
		public void DeleteLists(UserProfile user, UserSelectedContext catalogInfo, List<Guid> listIds)
		{
			foreach (var listId in listIds)
			{
				var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, listId);
				if(basket != null)
					basketRepository.DeleteBasket(basket.UserId.ToGuid(), listId);
			}
		}

		public UserList AddItems(UserProfile user, UserSelectedContext catalogInfo, Guid listId, List<ListItem> newItems, bool allowDuplicates)
		{
			var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, listId);

			if (basket == null)
				return null;

			var lineItems = new List<CS.LineItem>();

			foreach (var item in newItems)
			{
				if (allowDuplicates || !basket.LineItems.Where(l => l.ProductId.Equals(item.ItemNumber)).Any())
					lineItems.Add(item.ToLineItem(basket.BranchId));
			}

			basketRepository.CreateOrUpdateBasket(basket.UserId.ToGuid(), basket.BranchId, basket, lineItems);

			var updatedList = ToUserList(basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, listId));

			LookupProductDetails(user, updatedList, catalogInfo);

			return updatedList;
		}

		public void UpdateList(UserProfile user, UserList list, UserSelectedContext catalogInfo)
		{
			var updateBasket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, list.ListId);
			
			if (updateBasket == null)
				return;

			updateBasket.DisplayName = list.Name;
			updateBasket.Name = ListName(list.Name, catalogInfo); ;
			var itemsToRemove = new List<Guid>();
			var lineItems = new List<CS.LineItem>();

			if (list.Items != null)
			{
				itemsToRemove = updateBasket.LineItems.Where(b => !list.Items.Any(c => c.ListItemId.ToCommerceServerFormat().Equals(b.Id))).Select(l => l.Id.ToGuid()).ToList();
				lineItems = list.Items.Select(s => s.ToLineItem(updateBasket.BranchId)).ToList();
			}
			else
				itemsToRemove = updateBasket.LineItems.Select(l => l.Id.ToGuid()).ToList();

			basketRepository.CreateOrUpdateBasket(updateBasket.UserId.ToGuid(), updateBasket.BranchId, updateBasket, lineItems);

			foreach (var toDelete in itemsToRemove)
			{
				basketRepository.DeleteItem(updateBasket.UserId.ToGuid(), list.ListId, toDelete);
			}
		}

		
		private void LookupProductDetails(UserProfile user, UserList list, UserSelectedContext catalogInfo)
		{
			if (list.Items == null)
				return;

			var activeCart = basketLogic.RetrieveAllSharedCustomerBaskets(user, catalogInfo, ListType.Cart).Where(b => b.Active.Equals(true));

			var products = catalogRepository.GetProductsByIds(list.BranchId, list.Items.Select(i => i.ItemNumber).Distinct().ToList());
			var favorites = basketRepository.ReadBasket(user.UserId, ListName(FAVORITESLIST, catalogInfo));
			var pricing = priceRepository.GetPrices(catalogInfo.BranchId, catalogInfo.CustomerId, DateTime.Now.AddDays(1), products.Products);
			var notes = itemNoteLogic.ReadNotes(user, catalogInfo);


			list.Items.ForEach(delegate(ListItem listItem)
			{

				var prod = products.Products.Where(p => p.ItemNumber.Equals(listItem.ItemNumber)).FirstOrDefault();
				var price = pricing.Where(p => p.ItemNumber.Equals(listItem.ItemNumber)).FirstOrDefault();
				var note = notes.Where(n => n.ItemNumber.Equals(listItem.ItemNumber));

				if (prod != null)
				{
					listItem.Name = prod.Name;
					listItem.PackSize = string.Format("{0} / {1}", prod.Pack, prod.Size);
					listItem.StorageTemp = prod.Nutritional.StorageTemp;
					listItem.Brand = prod.BrandExtendedDescription;
					listItem.ReplacedItem = prod.ReplacedItem;
					listItem.ReplacementItem = prod.ReplacementItem;
					listItem.NonStock = prod.NonStock;
					listItem.ChildNutrition = prod.ChildNutrition;
					listItem.CatchWeight = prod.CatchWeight;
				}
				if (favorites != null)
				{
					listItem.Favorite = favorites.LineItems.Where(i => i.ProductId.Equals(listItem.ItemNumber)).Any();
				}
				if (price != null)
				{
					listItem.PackagePrice = price.PackagePrice.ToString();
					listItem.CasePrice = price.CasePrice.ToString();

				}

				if (activeCart.Any()) //Is there an active cart? If so get item counts
				{
					listItem.QuantityInCart = activeCart.First().LineItems.Where(b => b.ProductId.Equals(listItem.ItemNumber)).Sum(l => l.Quantity);
				}

				if (note.Any())
					listItem.Notes = note.First().Note;

			});

		}

		/// <summary>
		/// Checks if any of the products are in the user's Favorites list. 
		/// If so, their Favorite property is set to "true"
		/// </summary>
		/// <param name="branchId">The branch/catalog to use</param>
		/// <param name="products">List of products</param>
		public void MarkFavoriteProductsAndNotes(UserProfile user, string branchId, ProductsReturn products, UserSelectedContext catalogInfo)
		{
			var list = basketRepository.ReadBasket(user.UserId, ListName(FAVORITESLIST, catalogInfo));
			var notes = itemNoteLogic.ReadNotes(user, catalogInfo);

			if (list == null || list.LineItems == null)
				return;

			products.Products.ForEach(delegate(Product product)
			{
				product.Favorite = list.LineItems.Where(i => i.ProductId.Equals(product.ItemNumber)).Any();
				var note = notes.Where(n => n.ItemNumber.Equals(product.ItemNumber));
				if (note.Any())
					product.Notes = note.First().Note;

			});
		}

		private UserList ToUserList(CS.Basket basket)
		{
			return new UserList()
			{
				ListId = basket.Id.ToGuid(),
				Name = basket.DisplayName,
				BranchId = basket.BranchId,
				IsContractList = basket.IsContractList.HasValue ? basket.IsContractList.Value : false,
				ReadOnly = basket.ReadOnly.HasValue ? basket.ReadOnly.Value : false,
				Items = basket.LineItems == null ? null : basket.LineItems.Select(l => new ListItem()
				{
					ItemNumber = l.ProductId,
					Label = l.Label,
					ListItemId = l.Id.ToGuid(),
					ParLevel = l.ParLevel == null ? 0M : (decimal)l.ParLevel,
					Position = string.IsNullOrEmpty(l.LinePosition) ? 0 : int.Parse(l.LinePosition)
				}).ToList()
			};

		}
				

		private string ListName(string name, UserSelectedContext catalogInfo)
		{
			return string.Format("l{0}_{1}_{2}", catalogInfo.BranchId.ToLower(), catalogInfo.CustomerId, Regex.Replace(name, @"\s+", ""));
		}
			

	}
}
