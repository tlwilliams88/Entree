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


		public ListLogicImpl(IBasketRepository basketRepository, ICatalogRepository catalogRepository, IPriceRepository priceRepository)
        {
			this.basketRepository = basketRepository;
			this.catalogRepository = catalogRepository;
			this.priceRepository = priceRepository;
        }

		public Guid CreateList(Guid userId, string branchId, UserList list)
        {
			var newBasket = new CS.Basket();
			newBasket.BranchId = branchId;
			newBasket.DisplayName = list.Name;
			newBasket.Status = BasketStatus;
			newBasket.Name = list.FormattedName(branchId);

			return basketRepository.CreateOrUpdateBasket(userId, branchId, newBasket, list.Items.Select(l => l.ToLineItem(branchId)).ToList());
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
				itemsToRemove = updateBasket.LineItems.Where(b => !list.Items.Any(c => c.ListItemId.ToString("B").Equals(b.Id))).Select(l => l.Id.ToGuid()).ToList();
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


			if (!lists.Where(l => l.Name.Equals(FAVORITESLIST)).Any())
			{
				//favorites list doesn't exist yet, create an empty one
				basketRepository.CreateOrUpdateBasket(user.UserId, branchId, new CS.Basket() { DisplayName = FAVORITESLIST, Status = BasketStatus, BranchId = branchId, Name = string.Format("l{0}_{1}", branchId, FAVORITESLIST)  }, null);
				lists = basketRepository.ReadAllBaskets(user.UserId);
			}

			var listForBranch = lists.Where(b => b.BranchId.Equals(branchId) && b.Status.Equals(BasketStatus));
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
			return lists.Where(i => i.LineItems != null && i.Status.Equals(BasketStatus) && i.BranchId.Equals(branchId)).SelectMany(l => l.LineItems.Where(b => b.Label != null).Select(i => i.Label)).Distinct().ToList();
        }

		private void LookupProductDetails(UserProfile user, UserList list)
		{
			if (list.Items == null)
				return;

			var products = catalogRepository.GetProductsByIds(list.BranchId, list.Items.Select(i => i.ItemNumber).Distinct().ToList());
			var favorites = basketRepository.ReadBasket(user.UserId, string.Format("l{0}_{1}", list.BranchId, FAVORITESLIST));
			var pricing = priceRepository.GetPrices(user.BranchId, user.CustomerNumber, DateTime.Now.AddDays(1), products.Products); 

			list.Items.ForEach(delegate (ListItem listItem)
			{

				var prod = products.Products.Where(p => p.ItemNumber.Equals(listItem.ItemNumber)).FirstOrDefault();
				var price = pricing.Where(p => p.ItemNumber.Equals(listItem.ItemNumber)).FirstOrDefault();
				
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
			});
			
		}
		
		/// <summary>
		/// Checks if any of the products are in the user's Favorites list. 
		/// If so, their Favorite property is set to "true"
		/// </summary>
		/// <param name="branchId">The branch/catalog to use</param>
		/// <param name="products">List of products</param>
		public void MarkFavoriteProducts(Guid userId, string branchId, ProductsReturn products)
		{
			var list = basketRepository.ReadBasket(userId, string.Format("l{0}_{1}", branchId, FAVORITESLIST));

			if (list == null || list.LineItems == null)
				return;

			products.Products.ForEach(delegate(Product product)
			{
				product.Favorite = list.LineItems.Where(i => i.ProductId.Equals(product.ItemNumber)).Any();
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
		
	}
}
