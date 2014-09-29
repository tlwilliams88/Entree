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

namespace KeithLink.Svc.Impl.Logic
{
    public class ListLogicImpl: IListLogic
    {
		private const string FAVORITESLIST = "Favorites";
		private readonly string BasketStatus = "CustomerList";

        private readonly IBasketRepository basketRepository;
		private readonly ICatalogRepository catalogRepository;
		private readonly IPriceRepository priceRepository;
		private readonly IItemNoteLogic itemNoteLogic;


		public ListLogicImpl(IBasketRepository basketRepository, ICatalogRepository catalogRepository, IPriceRepository priceRepository, IItemNoteLogic itemNoteLogic)
        {
			this.basketRepository = basketRepository;
			this.catalogRepository = catalogRepository;
			this.priceRepository = priceRepository;
			this.itemNoteLogic = itemNoteLogic;
        }

		public Guid CreateList(Guid userId, string branchId, UserList list)
        {
			var newBasket = new CS.Basket();
			newBasket.BranchId = branchId.ToLower();
			newBasket.DisplayName = list.Name;
			newBasket.Status = BasketStatus;
			newBasket.Name = list.FormattedName(branchId);

			return basketRepository.CreateOrUpdateBasket(userId, branchId.ToLower(), newBasket, list.Items.Select(l => l.ToLineItem(branchId)).ToList());
        }

		public Guid? AddItem(Guid userId, Guid listId, ListItem newItem)
        {

			var basket = basketRepository.ReadBasket(userId, listId);
			if (basket == null)
				return null;

            return basketRepository.AddItem(userId, listId, newItem.ToLineItem(basket.BranchId), basket);
		}

		public void UpdateItem(Guid userId, Guid listId, ListItem updatedItem)
        {
			var basket = basketRepository.ReadBasket(userId, listId);
			if (basket == null)
				return;
						
			basketRepository.UpdateItem(userId, listId, updatedItem.ToLineItem(basket.BranchId));
        }

		public void UpdateList(Guid userId, UserList list)
        {
			var updateBasket = basketRepository.ReadBasket(userId, list.ListId);

			if (updateBasket == null)
				return;

			updateBasket.DisplayName = list.Name;
			updateBasket.Name = list.FormattedName(updateBasket.BranchId);
			var itemsToRemove = new List<Guid>();
			var lineItems = new List<CS.LineItem>();

			if (list.Items != null)
			{
				itemsToRemove = updateBasket.LineItems.Where(b => !list.Items.Any(c => c.ListItemId.ToCommerceServerFormat().Equals(b.Id))).Select(l => l.Id.ToGuid()).ToList();
				lineItems = list.Items.Select(s => s.ToLineItem(updateBasket.BranchId)).ToList();
			}
			else
				itemsToRemove = updateBasket.LineItems.Select(l => l.Id.ToGuid()).ToList();

			basketRepository.CreateOrUpdateBasket(userId, updateBasket.BranchId, updateBasket, lineItems);

			foreach (var toDelete in itemsToRemove)
			{
				basketRepository.DeleteItem(userId, list.ListId, toDelete);
			}						
        }

		public void DeleteList(Guid userId, Guid listId)
        {
			basketRepository.DeleteBasket(userId, listId);
        }

		public void DeleteItem(Guid userId, Guid listId, Guid itemId)
        {
			basketRepository.DeleteItem(userId, listId, itemId);
        }


		public List<UserList> ReadAllLists(UserProfile user, string branchId, bool headerInfoOnly)
        {
			var lists = basketRepository.ReadAllBaskets(user.UserId);


			if (!lists.Where(l => l.Name.Equals(string.Format("l{0}_{1}", branchId.ToLower(),FAVORITESLIST))).Any())
			{
				//favorites list doesn't exist yet, create an empty one
				basketRepository.CreateOrUpdateBasket(user.UserId, branchId.ToLower(), new CS.Basket() { DisplayName = FAVORITESLIST, Status = BasketStatus, BranchId = branchId, Name = string.Format("l{0}_{1}", branchId.ToLower(), FAVORITESLIST) }, null);
				lists = basketRepository.ReadAllBaskets(user.UserId);
			}

			var listForBranch = lists.Where(b => b.BranchId.Equals(branchId, StringComparison.InvariantCultureIgnoreCase) && b.Status.Equals(BasketStatus));
			if (headerInfoOnly)
				return listForBranch.Select(l => new UserList() { ListId = l.Id.ToGuid(), Name = l.DisplayName }).ToList();
			else
			{
				var returnList = listForBranch.Select(b => ToUserList(b)).ToList();
				returnList.ForEach(delegate(UserList list)
				{
					LookupProductDetails(user, list);
				});
				return returnList;
			}
        }

		public UserList ReadList(UserProfile user, Guid listId)
        {
			var basket = basketRepository.ReadBasket(user.UserId, listId);
			if (basket == null)
				return null;

			var cart = ToUserList(basket);

			LookupProductDetails(user, cart);
			return cart;			
        }

		public List<string> ReadListLabels(Guid userId, Guid listId)
        {
			var lists = basketRepository.ReadBasket(userId, listId);
            
            if (lists == null || lists.LineItems == null)
                return null;

			return lists.LineItems.Where(l => l.Label != null).Select(i => i.Label).Distinct().ToList();
        }

		public List<string> ReadListLabels(Guid userId, string branchId)
        {
			var lists = basketRepository.ReadAllBaskets(userId);
			return lists.Where(i => i.LineItems != null && i.Status.Equals(BasketStatus) && i.BranchId.Equals(branchId.ToLower())).SelectMany(l => l.LineItems.Where(b => b.Label != null).Select(i => i.Label)).Distinct().ToList();
        }

		private void LookupProductDetails(UserProfile user, UserList list)
		{
			if (list.Items == null)
				return;

			var activeCart = basketRepository.ReadAllBaskets(user.UserId).Where(b => b.Status.Equals("ShoppingCart") && b.Active.Equals(true));

			var products = catalogRepository.GetProductsByIds(list.BranchId, list.Items.Select(i => i.ItemNumber).Distinct().ToList());
			var favorites = basketRepository.ReadBasket(user.UserId, string.Format("l{0}_{1}", list.BranchId, FAVORITESLIST));
			var pricing = priceRepository.GetPrices(user.BranchId, user.CustomerNumber, DateTime.Now.AddDays(1), products.Products);
			var notes = itemNoteLogic.ReadNotes(user.UserId);


			list.Items.ForEach(delegate (ListItem listItem)
			{

				var prod = products.Products.Where(p => p.ItemNumber.Equals(listItem.ItemNumber)).FirstOrDefault();
				var price = pricing.Where(p => p.ItemNumber.Equals(listItem.ItemNumber)).FirstOrDefault();
				var note = notes.Where(n => n.ItemNumber.Equals(listItem.ItemNumber));

				if (prod != null)
				{
					listItem.Name = prod.Name;
					listItem.PackSize = string.Format("{0} / {1}", prod.Pack, prod.Size);
					listItem.StorageTemp = prod.Nutritional.StorageTemp;
					listItem.Brand = prod.Brand;
					listItem.ReplacedItem = prod.ReplacedItem;
					listItem.ReplacementItem = prod.ReplacementItem;
					listItem.NonStock = prod.NonStock;
					listItem.CNDoc = prod.CNDoc;
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
		public void MarkFavoriteProductsAndNotes(Guid userId, string branchId, ProductsReturn products)
		{
			var list = basketRepository.ReadBasket(userId, string.Format("l{0}_{1}", branchId.ToLower(), FAVORITESLIST));
			var notes = itemNoteLogic.ReadNotes(userId);

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
		
		public void DeleteItems(Guid userId, Guid listId, List<Guid> itemIds)
		{
			foreach(var itemId in itemIds)
				basketRepository.DeleteItem(userId, listId, itemId);
		}


		public UserList AddItems(UserProfile user, Guid listId, List<ListItem> newItems, bool allowDuplicates)
		{
			var basket = basketRepository.ReadBasket(user.UserId, listId);

			if (basket == null)
				return null;

			var lineItems = new List<CS.LineItem>();

			foreach (var item in newItems)
			{
				if (allowDuplicates || !basket.LineItems.Where(l => l.ProductId.Equals(item.ItemNumber)).Any())
					lineItems.Add(item.ToLineItem(basket.BranchId));
			}

			basketRepository.CreateOrUpdateBasket(user.UserId, basket.BranchId, basket, lineItems);

			var updatedList = ToUserList(basketRepository.ReadBasket(user.UserId, listId));

			LookupProductDetails(user, updatedList);

			return updatedList;
		}
	}
}
