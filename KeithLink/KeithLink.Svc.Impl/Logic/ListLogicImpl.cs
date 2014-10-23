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
	//public class ListLogicImpl : IListLogic
	//{
	//	private const string FAVORITESLIST = "Favorites";

	//	private readonly IBasketRepository basketRepository;
	//	private readonly ICatalogRepository catalogRepository;
	//	private readonly IPriceRepository priceRepository;
	//	private readonly IBasketLogic basketLogic;


	//	public ListLogicImpl(IBasketRepository basketRepository, ICatalogRepository catalogRepository,
	//		IPriceRepository priceRepository, IBasketLogic basketLogic)
	//	{
	//		this.basketRepository = basketRepository;
	//		this.catalogRepository = catalogRepository;
	//		this.priceRepository = priceRepository;
	//		this.basketLogic = basketLogic;

	//	}

	//	public long CreateList(Guid userId, UserSelectedContext catalogInfo, ListModel list)
	//	{
	//		//return client.CreateList(new KeithLink.Svc.Core.Models.EF.List(){
	//		//	BranchId = catalogInfo.BranchId,
	//		//	CustomerId = catalogInfo.CustomerId,
	//		//	DisplayName = list.Name,
	//		//	UserId = userId,
	//		//	Type = KeithLink.Svc.Core.Models.EF.ListType.Custom,
	//		//	Items = list.Items.Select(i => new KeithLink.Svc.Core.Models.EF.ListItem() { ItemNumber = i.ItemNumber, Label = i.Label, Par = i.ParLevel }).ToArray()
	//		//});	
	//		return 1;
	//	}

	//	public long? AddItem(UserProfile user, UserSelectedContext catalogInfo, long listId, ListItemModel newItem)
	//	{
	//		return null; // client.AddItem(listId, new Core.Models.EF.ListItem() { ItemNumber = newItem.ItemNumber, Label = newItem.Label, Par = newItem.ParLevel });			
	//	}

	//	public void UpdateItem(ListItemModel updatedItem)
	//	{
	//		//client.UpdateItem(new Core.Models.EF.ListItem() { ItemNumber = updatedItem.ItemNumber, Id = updatedItem.ListItemId, Label = updatedItem.Label, Par = updatedItem.ParLevel});
	//		//var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, listId);

	//		//if (basket == null)
	//		//	return;

	//		//basketRepository.UpdateItem(basket.UserId.ToGuid(), listId, updatedItem.ToLineItem(catalogInfo.BranchId.ToLower()));
	//	}

	//	public void DeleteItem(long itemId)
	//	{
	//		//client.DeleteItem(itemId);
	//		//var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, listId);

	//		//if (basket == null)
	//		//	return;

	//		//basketRepository.DeleteItem(basket.UserId.ToGuid(), listId, itemId);
	//	}

	//	public List<ListModel> ReadAllLists(UserProfile user, UserSelectedContext catalogInfo, bool headerInfoOnly)
	//	{
	//		//var list = client.ReadUserList(user, catalogInfo, false);

	//		//if (!list.Where(l => l.Type.Equals(Core.Models.EF.ListType.Favorite)).Any())
	//		//{
	//		//	//client.CreateList(new Core.Models.EF.List() { Type = Core.Models.EF.ListType.Favorite, DisplayName = "Favorites", UserId = user.UserId, CustomerId = catalogInfo.CustomerId, BranchId = catalogInfo.BranchId });
	//		//	list = client.ReadUserList(user, catalogInfo, false);
	//		//}

	//		//if (headerInfoOnly)
	//		//	return list.Select(l => new ListModel() { ListId = l.Id, Name = l.DisplayName, IsContractList = l.Type == Core.Models.EF.ListType.Contract }).ToList();
	//		//else
	//		//{
	//		//	var returnList = list.Select(b => ToUserList(b)).ToList();
	//		//	//returnList.ForEach(delegate(ListModel list)
	//		//	//{
	//		//	//	LookupProductDetails(user, list, catalogInfo);
	//		//	//});
	//		//	return returnList;
	//		//}
	//		return null;
	//	}

	//	public ListModel ReadList(long listId)
	//	{
	//		//var list = client.ReadList(listId);
	//		//if (list == null)
	//		//	return null;

	//		//var returnList = ToUserList(list);
	//		return null;
	//		//var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, listId);
	//		//if (basket == null)
	//		//	return null;

	//		//var list = ToUserList(basket);

	//		//LookupProductDetails(user, list, catalogInfo);
	//		//return returnList;
			
	//	}

	//	public List<string> ReadListLabels(long listId)
	//	{
	//		//var list = client.ReadList(listId);

	//		//if (list == null)
	//		//	return null;

	//		//return list.Items.Select(l => l.Label).ToList();
	//		return null;
	//		//var lists = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, listId);

	//		//if (lists == null || lists.LineItems == null)
	//		//	return null;

	//		//return lists.LineItems.Where(l => l.Label != null).Select(i => i.Label).Distinct().ToList();
	//	}

	//	public List<string> ReadListLabels(UserProfile user, UserSelectedContext catalogInfo)
	//	{
	//		//var list = client.ReadUserList(user, catalogInfo, true).ToList();

	//		//return list.SelectMany(l => l.Items.Select(i => i.Label)).ToList();
	//		return null;
	//		//var lists = basketLogic.RetrieveAllSharedCustomerBaskets(user, catalogInfo, ListType.Custom);
	//		//return lists.Where(i => i.LineItems != null && i.ListType.Equals((int)ListType.Custom) && i.BranchId.Equals(catalogInfo.BranchId.ToLower())).SelectMany(l => l.LineItems.Where(b => b.Label != null).Select(i => i.Label)).Distinct().ToList();
	//	}

	//	public void DeleteItems(List<long> itemIds)
	//	{
	//		//foreach (var itemId in itemIds)
	//			//client.DeleteItem(itemId);
	//		//var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, listId);

	//		//if (basket == null && !(basket.ReadOnly.HasValue && basket.ReadOnly.Value))
	//		//	return;

	//		//foreach (var itemId in itemIds)
	//		//	basketRepository.DeleteItem(basket.UserId.ToGuid(), listId, itemId);
	//	}

	//	public void DeleteList(long listId)
	//	{
	//		//client.DeleteList(listId);
	//		//var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, listId);

	//		//if (basket == null)
	//		//	return;

	//		//basketRepository.DeleteBasket(basket.UserId.ToGuid(), listId);
	//	}
		
	//	public void DeleteLists(List<long> listIds)
	//	{
	//		foreach (var listId in listIds)
	//		{
	//			//client.DeleteList(listId);
			
	//			//var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, listId);
	//			//if(basket != null)
	//			//	basketRepository.DeleteBasket(basket.UserId.ToGuid(), listId);
	//		}
	//	}

	//	public ListModel AddItems(UserProfile user, UserSelectedContext catalogInfo, long listId, List<ListItemModel> newItems, bool allowDuplicates)
	//	{
	//		//client.AddItems(listId, newItems.ToArray());

	//		return ReadList(listId);

	//		//client.AddItems(listId, 
	//		//var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, listId);

	//		//if (basket == null)
	//		//	return null;

	//		//var lineItems = new List<CS.LineItem>();

	//		//foreach (var item in newItems)
	//		//{
	//		//	if (allowDuplicates || !basket.LineItems.Where(l => l.ProductId.Equals(item.ItemNumber)).Any())
	//		//		lineItems.Add(item.ToLineItem(basket.BranchId));
	//		//}

	//		//basketRepository.CreateOrUpdateBasket(basket.UserId.ToGuid(), basket.BranchId, basket, lineItems);

	//		//var updatedList = ToUserList(basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, listId));

	//		//LookupProductDetails(user, updatedList, catalogInfo);

	//		//return updatedList;
	//		return null;
	//	}

	//	public void UpdateList(UserProfile user, ListModel list, UserSelectedContext catalogInfo)
	//	{
	//		//client.UpdateList(list);
	//		//var currentList = client.ReadList(list.ListId);

	//		//if (currentList == null)
	//		//	return;

	//		//currentList.DisplayName = list.Name;

	//		//client.CreateUpdateList(currentList);

	//		//if (currentList.Items != null)
	//		//	foreach (var item in currentList.Items.Where(i => !list.Items.Any(l => l.ListItemId.Equals(i.Id))))
	//		//		client.DeleteItem(item.Id);

	//		//foreach (var updateItem in list.Items)
	//		//{
	//		//	if (updateItem.ListItemId != null)
	//		//		client.UpdateItem(new Core.Models.EF.ListItem() { Id = updateItem.ListItemId, Par = updateItem.ParLevel, Label = updateItem.Label, ItemNumber = updateItem.ItemNumber });
	//		//	else
	//		//		client.AddItem(currentList.Id, new Core.Models.EF.ListItem() { Par = updateItem.ParLevel, Label = updateItem.Label, ItemNumber = updateItem.ItemNumber });

	//		//}


	//		//var updateBasket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, list.ListId);
			
	//		//if (updateBasket == null)
	//		//	return;

	//		//updateBasket.DisplayName = list.Name;
	//		//updateBasket.Name = ListName(list.Name, catalogInfo); ;
	//		//var itemsToRemove = new List<Guid>();
	//		//var lineItems = new List<CS.LineItem>();

	//		//if (list.Items != null)
	//		//{
	//		//	itemsToRemove = updateBasket.LineItems.Where(b => !list.Items.Any(c => c.ListItemId.ToCommerceServerFormat().Equals(b.Id))).Select(l => l.Id.ToGuid()).ToList();
	//		//	lineItems = list.Items.Select(s => s.ToLineItem(updateBasket.BranchId)).ToList();
	//		//}
	//		//else
	//		//	itemsToRemove = updateBasket.LineItems.Select(l => l.Id.ToGuid()).ToList();

	//		//basketRepository.CreateOrUpdateBasket(updateBasket.UserId.ToGuid(), updateBasket.BranchId, updateBasket, lineItems);

	//		//foreach (var toDelete in itemsToRemove)
	//		//{
	//		//	basketRepository.DeleteItem(updateBasket.UserId.ToGuid(), list.ListId, toDelete);
	//		//}
	//	}
				
	//	private void LookupProductDetails(UserProfile user, ListModel list, UserSelectedContext catalogInfo)
	//	{
	//		if (list.Items == null)
	//			return;

	//		var activeCart = basketLogic.RetrieveAllSharedCustomerBaskets(user, catalogInfo, ListType.Cart).Where(b => b.Active.Equals(true));

	//		var products = catalogRepository.GetProductsByIds(list.BranchId, list.Items.Select(i => i.ItemNumber).Distinct().ToList());
	//		var favorites = basketRepository.ReadBasket(user.UserId, ListName(FAVORITESLIST, catalogInfo));
	//		var pricing = priceRepository.GetPrices(catalogInfo.BranchId, catalogInfo.CustomerId, DateTime.Now.AddDays(1), products.Products);
			


	//		list.Items.ForEach(delegate(ListItemModel listItem)
	//		{

	//			var prod = products.Products.Where(p => p.ItemNumber.Equals(listItem.ItemNumber)).FirstOrDefault();
	//			var price = pricing.Where(p => p.ItemNumber.Equals(listItem.ItemNumber)).FirstOrDefault();
				
	//			if (prod != null)
	//			{
	//				listItem.Name = prod.Name;
	//				listItem.PackSize = string.Format("{0} / {1}", prod.Pack, prod.Size);
	//				listItem.StorageTemp = prod.Nutritional.StorageTemp;
	//				listItem.Brand = prod.BrandExtendedDescription;
	//				listItem.ReplacedItem = prod.ReplacedItem;
	//				listItem.ReplacementItem = prod.ReplacementItem;
	//				listItem.NonStock = prod.NonStock;
	//				listItem.ChildNutrition = prod.ChildNutrition;
	//				listItem.CatchWeight = prod.CatchWeight;
	//			}
	//			if (favorites != null)
	//			{
	//				listItem.Favorite = favorites.LineItems.Where(i => i.ProductId.Equals(listItem.ItemNumber)).Any();
	//			}
	//			if (price != null)
	//			{
	//				listItem.PackagePrice = price.PackagePrice.ToString();
	//				listItem.CasePrice = price.CasePrice.ToString();

	//			}

	//			if (activeCart.Any()) //Is there an active cart? If so get item counts
	//			{
	//				listItem.QuantityInCart = activeCart.First().LineItems.Where(b => b.ProductId.Equals(listItem.ItemNumber)).Sum(l => l.Quantity);
	//			}

				

	//		});

	//	}

	//	/// <summary>
	//	/// Checks if any of the products are in the user's Favorites list. 
	//	/// If so, their Favorite property is set to "true"
	//	/// </summary>
	//	/// <param name="branchId">The branch/catalog to use</param>
	//	/// <param name="products">List of products</param>
	//	public void MarkFavoriteProductsAndNotes(UserProfile user, string branchId, ProductsReturn products, UserSelectedContext catalogInfo)
	//	{
	//		var list = basketRepository.ReadBasket(user.UserId, ListName(FAVORITESLIST, catalogInfo));

	//		if (list == null || list.LineItems == null)
	//			return;

	//		products.Products.ForEach(delegate(Product product)
	//		{
	//			product.Favorite = list.LineItems.Where(i => i.ProductId.Equals(product.ItemNumber)).Any();
	//		});
	//	}

	//	private ListModel ToUserList(KeithLink.Svc.Core.Models.EF.List list)
	//	{
	//		return new ListModel()
	//		{
	//			ListId = list.Id,
	//			Name = list.DisplayName,
	//			BranchId = list.BranchId,
	//			IsContractList = list.Type == Core.Models.EF.ListType.Contract,
	//			ReadOnly = false, //TODO: Add Readonly
	//			Items = list.Items == null ? null : list.Items.Select(l => new ListItemModel()
	//			{
	//				ItemNumber = l.ItemNumber,
	//				Label = l.Label,
	//				ListItemId = l.Id,
	//				ParLevel = l.Par
	//				//Category = l.Category //TODO: Add Category
	//			}).ToList()
	//		};

	//	}
				

	//	private string ListName(string name, UserSelectedContext catalogInfo)
	//	{
	//		return string.Format("l{0}_{1}_{2}", catalogInfo.BranchId.ToLower(), catalogInfo.CustomerId, Regex.Replace(name, @"\s+", ""));
	//	}
			

	//}
}
