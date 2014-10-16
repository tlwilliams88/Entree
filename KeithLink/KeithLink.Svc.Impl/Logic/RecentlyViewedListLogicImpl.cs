using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS = KeithLink.Svc.Core.Models.Generated;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Interface.SiteCatalog;

namespace KeithLink.Svc.Impl.Logic
{
	public class RecentlyViewedListLogicImpl: IRecentlyViewedListLogic
	{
		private readonly string RecentlyViewedListNameFormat = "rv_{0}_RecentItems";
		private readonly string BasketStatus = "RecentlyViewed";
		
		private readonly IBasketRepository basketRepository;
		private readonly IProductImageRepository productImageRepository;
		private readonly ICatalogRepository catalogRepository;
		

		public RecentlyViewedListLogicImpl(IBasketRepository basketRepository,
			IProductImageRepository productImageRepository,
			ICatalogRepository catalogRepository)
		{
			this.basketRepository = basketRepository;
			this.productImageRepository = productImageRepository;
			this.catalogRepository = catalogRepository;
		}

		public void AddItem(UserProfile user, UserSelectedContext catalogInfo, string itemNumber)
		{
			var basket = basketRepository.ReadBasket(user.UserId, RecentlyViewedListName(catalogInfo));

			if (string.IsNullOrEmpty(basket.CustomerId) || string.IsNullOrEmpty(catalogInfo.CustomerId))
			{
				//This basket is new, set value and update
				var newBasket = new CS.Basket();

				newBasket.CustomerId = catalogInfo.CustomerId;
				newBasket.BranchId = catalogInfo.BranchId.ToLower();
				newBasket.DisplayName = "Recent Items";
				newBasket.Status = BasketStatus;
				newBasket.Name = RecentlyViewedListName(catalogInfo);
				

				basketRepository.CreateOrUpdateBasket(user.UserId, catalogInfo.BranchId.ToLower(), newBasket, new List<CS.LineItem>() { new CS.LineItem() { ProductId = itemNumber, CatalogName = catalogInfo.BranchId } });
			}
			else
			{
				//Does item already exist? If so, just update the ModifiedDate
				var existingItem = basket.LineItems.Where(l => l.ProductId.Equals(itemNumber));
				if (existingItem.Any())
				{
					existingItem.First().Properties["DateModified"] = DateTime.Now;
					basketRepository.UpdateItem(user.UserId, basket.Id.ToGuid(), existingItem.First());
				}
				else
				{
					if (basket.LineItems.Count >= Configuration.RecentItemsToKeep)
						basketRepository.DeleteItem(user.UserId, basket.Id.ToGuid(), basket.LineItems.OrderBy(l => l.Properties["DateModified"]).FirstOrDefault().Id.ToGuid());

					basketRepository.AddItem(basket.Id.ToGuid(), new CS.LineItem() { ProductId = itemNumber, CatalogName = catalogInfo.BranchId }, basket);
				}
			}
		}

		public void Clear(UserProfile user, UserSelectedContext catalogInfo)
		{
			basketRepository.DeleteBasket(user.UserId, RecentlyViewedListName(catalogInfo));
		}

		public List<RecentItem> Read(UserProfile user, UserSelectedContext catalogInfo)
		{
			var basket = basketRepository.ReadBasket(user.UserId, RecentlyViewedListName(catalogInfo));
			var returnList = basket.LineItems.OrderByDescending(o => o.Properties["DateModified"]).Select(l => new RecentItem() { ItemNumber = l.ProductId }).ToList();

			PopulateProductDetails(catalogInfo,returnList);

			return returnList;
		}

		private void PopulateProductDetails(UserSelectedContext catalogInfo, List<RecentItem> returnList)
		{
			if (returnList == null)
				return;

			var products = catalogRepository.GetProductsByIds(catalogInfo.BranchId, returnList.Select(i => i.ItemNumber).Distinct().ToList());

			returnList.ForEach(delegate(RecentItem item)
			{
				item.Images = productImageRepository.GetImageList(item.ItemNumber).ProductImages;
				item.Name = products.Products.Where(p => p.ItemNumber.Equals(item.ItemNumber)).FirstOrDefault().Name;
			});
					
		}

		private string RecentlyViewedListName(UserSelectedContext catalogInfo)
		{
			return string.Format(RecentlyViewedListNameFormat, catalogInfo.CustomerId);
		}
	}
}
