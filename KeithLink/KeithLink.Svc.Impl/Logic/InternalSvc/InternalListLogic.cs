﻿using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.SiteCatalog;

namespace KeithLink.Svc.Impl.Logic.InternalSvc
{
	public class InternalListLogic : IInternalListLogic {
        #region attributes
        private readonly IUnitOfWork unitOfWork;
		private readonly IListRepository listRepository;
		private readonly IListItemRepository listItemRepository;
		private readonly IBasketLogic basketLogic;
		private readonly ICatalogLogic catalogLogic;
		private readonly IListCacheRepository listCacheRepository;
		private readonly IPriceLogic priceLogic;
		private readonly IProductImageRepository productImageRepository;
		private readonly IListShareRepository listShareRepository;
        #endregion

        #region ctor
        public InternalListLogic(IUnitOfWork unitOfWork, IListRepository listRepository,
			IListItemRepository listItemRepository, IBasketLogic basketLogic,
			ICatalogLogic catalogLogic, IListCacheRepository listCacheRepository, IPriceLogic priceLogic, IProductImageRepository productImageRepository, IListShareRepository listShareRepository)
		{
			this.listRepository = listRepository;
			this.unitOfWork = unitOfWork;
			this.listItemRepository = listItemRepository;
			this.basketLogic = basketLogic;
			this.catalogLogic = catalogLogic;
			this.listCacheRepository = listCacheRepository;
			this.priceLogic = priceLogic;
			this.productImageRepository = productImageRepository;
			this.listShareRepository = listShareRepository;
		}
        #endregion

        #region methods
		public long? AddItem(long listId, ListItemModel newItem)
		{
			var list = listRepository.ReadById(listId);
			if (list.Items == null)
				list.Items = new List<ListItem>();

			if (list.Type == ListType.Favorite || list.Type == ListType.Reminder) //Don't allow duplicates
			{
				var dupItem = list.Items.Where(i => i.ItemNumber.Equals(newItem.ItemNumber)).FirstOrDefault();
				if (dupItem != null)
					return dupItem.Id;
			}

			var item = new ListItem()
			{
				ItemNumber = newItem.ItemNumber,
				Label = newItem.Label,
				Par = newItem.ParLevel,
				Position = newItem.Position,
			};

			list.Items.Add(item);
			listRepository.CreateOrUpdate(list);
			unitOfWork.SaveChanges();
			listCacheRepository.RemoveItem(string.Format("UserList_{0}", list.Id)); //Invalidate cache
			return item.Id;
		}

		public ListModel AddItems(UserProfile user, UserSelectedContext catalogInfo, long listId, List<ListItemModel> items)
		{
			var list = listRepository.ReadById(listId);
			if (list.Items == null)
				list.Items = new List<ListItem>();

			foreach (var item in items)
			{
				if (list.Type == ListType.Favorite && list.Items.Where(i => i.ItemNumber.Equals(item.ItemNumber)).Any())
					continue;

				list.Items.Add(new ListItem() { ItemNumber = item.ItemNumber, Label = item.Label, Par = item.ParLevel });
			}

			listRepository.CreateOrUpdate(list);
			unitOfWork.SaveChanges();
			listCacheRepository.RemoveItem(string.Format("UserList_{0}", list.Id)); //Invalidate cache
			return ReadList(user, catalogInfo, listId);
		}
        
		public void AddNote(UserProfile user, UserSelectedContext catalogInfo, ItemNote newNote)
		{
			var list = listRepository.ReadListForCustomer(user, catalogInfo, true).Where(i => i.Type == ListType.Notes).FirstOrDefault();
			if (list == null)
			{
				var newNotes = new List()
				{
					Type = ListType.Notes,
					CustomerId = catalogInfo.CustomerId,
					BranchId = catalogInfo.BranchId,
					DisplayName = "Notes",
					ReadOnly = false,
					UserId = user.UserId,
					Items = new List<ListItem>() { new ListItem() { ItemNumber = newNote.ItemNumber, Note = newNote.Note } }

				};
				listRepository.Create(newNotes);
			}
			else
			{
				var existingItem = list.Items.Where(i => i.ItemNumber.Equals(newNote.ItemNumber)).FirstOrDefault();

				if(existingItem != null)
				{
					existingItem.Note = newNote.Note;
					listItemRepository.Update(existingItem);
				}
				else
				{
					var createNote = new ListItem() { Note = newNote.Note, ItemNumber = newNote.ItemNumber, ParentList = list };
					listItemRepository.Create(createNote);
				}
			}
			unitOfWork.SaveChanges();

		}
        
		public void AddRecentlyViewedItem(UserProfile user, UserSelectedContext catalogInfo, string itemNumber)
		{
			var list = listRepository.Read(i => i.UserId.Equals(user.UserId) && i.Type == ListType.Recent && i.CustomerId.Equals(catalogInfo.CustomerId), l => l.Items).FirstOrDefault();

			if (list == null)
			{
				//Create a new recently viewed list
				this.CreateList(user.UserId, catalogInfo, new ListModel()
				{
					Name = "Recent",
					BranchId = catalogInfo.BranchId,
					Items = new List<ListItemModel>() { 
						new ListItemModel() 
						{ 
							ItemNumber = itemNumber
						} 
					}
				}, ListType.Recent);

			}
			else
			{
				var item = list.Items.Where(i => i.ItemNumber.Equals(itemNumber)).FirstOrDefault();
				if (item != null)
				{
					listItemRepository.Update(item);
				}
				else
				{
					if (list.Items.Count >= Configuration.RecentItemsToKeep)
						listItemRepository.Delete(list.Items.OrderBy(i => i.ModifiedUtc).FirstOrDefault());

					list.Items.Add(new ListItem() { ItemNumber = itemNumber });
				}
			}

			unitOfWork.SaveChanges();

		}
        
        public long CreateList(Guid userId, UserSelectedContext catalogInfo, ListModel list, ListType type)
		{
			var newList = list.ToEFList();
			newList.BranchId = catalogInfo.BranchId;
			newList.CustomerId = catalogInfo.CustomerId;
			newList.UserId = userId;
			newList.Type = type;
			listRepository.CreateOrUpdate(newList);
			unitOfWork.SaveChanges();
			return newList.Id;
		}

		public void DeleteItem(long Id)
		{
			var item = listItemRepository.Read(i => i.Id.Equals(Id), l => l.ParentList).FirstOrDefault();
			var listId = item.ParentList.Id;
			listItemRepository.Delete(item);
			unitOfWork.SaveChanges();
			listCacheRepository.RemoveItem(string.Format("UserList_{0}", listId)); //Invalidate cache
		}

		public void DeleteList(long Id)
		{
			var list = listRepository.Read(l => l.Id.Equals(Id), i => i.Items).FirstOrDefault();

			list.Items.ToList().ForEach(delegate(ListItem item)
			{
				listItemRepository.Delete(item);
			});

			listRepository.Delete(list);
			unitOfWork.SaveChanges();
		}
		
		public void DeleteNote(UserProfile user, UserSelectedContext catalogInfo, string ItemNumber)
		{
			var list = listRepository.ReadListForCustomer(user, catalogInfo, true).FirstOrDefault();
			if (list == null)
			{
				listItemRepository.Delete(list.Items.Where(i => i.ItemNumber.Equals(ItemNumber)).FirstOrDefault());
				unitOfWork.SaveChanges();
			}
		}
        
        private void LookupProductDetails(UserProfile user, ListModel list, UserSelectedContext catalogInfo)
		{
			if (list.Items == null || list.Items.Count == 0)
				return;

			var products = catalogLogic.GetProductsByIds(list.BranchId, list.Items.Select(i => i.ItemNumber).Distinct().ToList(), user);
			var prices = priceLogic.GetPrices(catalogInfo.BranchId, catalogInfo.CustomerId, DateTime.Now.AddDays(1), list.Items.Select(i => new Product() { ItemNumber = i.ItemNumber }).ToList());

			Parallel.ForEach(list.Items, listItem =>
			{
				var prod = products.Products.Where(p => p.ItemNumber.Equals(listItem.ItemNumber)).FirstOrDefault();
				var price = prices.Prices.Where(p => p.ItemNumber.Equals(listItem.ItemNumber)).FirstOrDefault();
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
                    listItem.TempZone = prod.TempZone;

				}
				if (price != null)
				{
					listItem.PackagePrice = price.PackagePrice == null ? null : price.PackagePrice.ToString();
					listItem.CasePrice = price.CasePrice == null ? null : price.CasePrice.ToString();
				}
			});

		}

		private void MarkFavoritesAndAddNotes(UserProfile user, ListModel list, UserSelectedContext catalogInfo, IEnumerable<KeithLink.Svc.Core.Models.Generated.Basket> activeCart)
		{
			if (list.Items == null)
				return;

			var notes = listRepository.ReadListForCustomer(user, catalogInfo, false).Where(l => l.Type.Equals(ListType.Notes)).FirstOrDefault();
			var favorites = listRepository.ReadListForCustomer(user, catalogInfo, false).Where(l => l.Type.Equals(ListType.Favorite) && l.UserId.Equals(user.UserId)).FirstOrDefault();
			Parallel.ForEach(list.Items, listItem =>
			{
				if (favorites != null && favorites.Items != null)
				{
					listItem.Favorite = favorites.Items.Where(l => l.ItemNumber.Equals(listItem.ItemNumber)).Any();
				}


				if (activeCart.Any()) //Is there an active cart? If so get item counts
				{
					listItem.QuantityInCart = activeCart.First().LineItems.Where(b => b.ProductId.Equals(listItem.ItemNumber)).Sum(l => l.Quantity);
				}

				if (notes != null && notes.Items != null)
					listItem.Notes = notes.Items.Where(n => n.ItemNumber.Equals(listItem.ItemNumber)).Select(i => i.Note).FirstOrDefault();
			});

		}
		
		private void PopulateProductDetails(UserSelectedContext catalogInfo, List<RecentItem> returnList, UserProfile user)
		{
			if (returnList == null)
				return;

			var products = catalogLogic.GetProductsByIds(catalogInfo.BranchId, returnList.Select(i => i.ItemNumber).Distinct().ToList(), user);

			returnList.ForEach(delegate(RecentItem item)
			{
				var product = products.Products.Where(p => p.ItemNumber.Equals(item.ItemNumber)).FirstOrDefault();
				if (product != null)
				{
					item.Images = product.ProductImages;
					item.Name = product.Name;
				}
			});

		}

		public List<string> ReadFavorites(UserProfile user, UserSelectedContext catalogInfo)
		{
			var list = listRepository.Read(l => l.UserId.Equals(user.UserId) && l.CustomerId.Equals(catalogInfo.CustomerId) && l.Type == ListType.Favorite, i => i.Items).ToList();

			if (list == null)
				return null;

			return list.SelectMany(i => i.Items.Select(x => x.ItemNumber)).ToList();
		}

		public ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, long Id)
		{
			var activeCart = basketLogic.RetrieveAllSharedCustomerBaskets(user, catalogInfo, Core.Enumerations.List.ListType.Cart).Where(b => b.Active.Equals(true));

			var cachedList = listCacheRepository.GetItem<ListModel>(string.Format("UserList_{0}", Id));
			if (cachedList != null)
			{
				MarkFavoritesAndAddNotes(user, cachedList, catalogInfo, activeCart);

				//var sharedlist = listRepository.Read(l => l.Id.Equals(Id), i => i.Items).FirstOrDefault();

				//cachedList.IsSharing = sharedlist.Shares.Any() && sharedlist.CustomerId.Equals(catalogInfo.CustomerId) && sharedlist.BranchId.Equals(catalogInfo.BranchId);
				//cachedList.IsShared = !sharedlist.CustomerId.Equals(catalogInfo.CustomerId);

				return cachedList;
			}

			var list = listRepository.Read(l => l.Id.Equals(Id), i => i.Items).FirstOrDefault();

			if (list == null)
				return null;

			var returnList = list.ToListModel(catalogInfo);

			LookupProductDetails(user, returnList, catalogInfo);
			listCacheRepository.AddItem<ListModel>(string.Format("UserList_{0}", Id), returnList);
			MarkFavoritesAndAddNotes(user, returnList, catalogInfo, activeCart);
			return returnList;
		}
		
        public List<ListModel> ReadListByType(UserProfile user, UserSelectedContext catalogInfo, ListType type)
		{
			var list = listRepository.ReadListForCustomer(user, catalogInfo, false).Where(l => l.Type.Equals(type) && l.CustomerId.Equals(catalogInfo.CustomerId) && l.BranchId.Equals(catalogInfo.BranchId)).ToList();

			if (list == null)
				return null;

			return list.Select(b => b.ToListModel(catalogInfo)).ToList();
		}

		public List<string> ReadListLabels(UserProfile user, UserSelectedContext catalogInfo)
		{
			return listRepository.ReadListForCustomer(user, catalogInfo, false).Where(l => l.Type.Equals(ListType.Custom) || l.Type.Equals(ListType.Favorite)).SelectMany(i => i.Items.Where(l => !string.IsNullOrEmpty(l.Label)).Select(b => b.Label)).Distinct().ToList();
		}
		
        public List<ListItemModel> ReadNotes(UserProfile user, UserSelectedContext catalogInfo)
		{
			var list = listRepository.Read(l => l.UserId.Equals(user.UserId) && l.CustomerId.Equals(catalogInfo.CustomerId) && l.Type == ListType.Notes, i => i.Items).ToList();

			if (list == null)
				return null;

			return list.SelectMany(i => i.Items.Select(x => new ListItemModel() { ItemNumber = x.ItemNumber, Notes = x.Note })).ToList();
		}

		public List<RecentItem> ReadRecent(UserProfile user, UserSelectedContext catalogInfo)
		{
			var list = listRepository.Read(i => i.UserId.Equals(user.UserId) && i.Type == ListType.Recent && i.CustomerId.Equals(catalogInfo.CustomerId), l => l.Items);
			var returnItems = list.SelectMany(i => i.Items.Select(l => new RecentItem() { ItemNumber = l.ItemNumber, ModifiedOn = l.ModifiedUtc })).ToList();
			PopulateProductDetails(catalogInfo, returnItems, user);
			returnItems.ForEach(delegate(RecentItem item)
			{
				item.Images = productImageRepository.GetImageList(item.ItemNumber).ProductImages;
			});
			return returnItems.OrderByDescending(l => l.ModifiedOn).ToList();

		}

        public List<ListModel> ReadReminders(UserProfile user, UserSelectedContext catalogInfo) {
            var list = listRepository.Read(l => l.UserId.Equals(user.UserId) && l.CustomerId.Equals(catalogInfo.CustomerId) && (l.Type == ListType.Reminder || l.Type == ListType.Mandatory), i => i.Items).ToList();

            if (list == null)
                return null;

            var returnList = list.Select(b => b.ToListModel(catalogInfo)).ToList();

            returnList.ForEach(delegate(ListModel listItem) {
                LookupProductDetails(user, listItem, catalogInfo);
            });

            return returnList;
        }

        public List<ListModel> ReadUserList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false) {
			var list = ReadListForCustomer(user, catalogInfo, headerOnly);

			

            if (list == null)
                return null;

            if (!list.Where(l => l.Type.Equals(ListType.Favorite)).Any()) {
                this.CreateList(user.UserId, catalogInfo, new ListModel() { Name = "Favorites", BranchId = catalogInfo.BranchId }, ListType.Favorite);
				list = ReadListForCustomer(user, catalogInfo, headerOnly);
            }

            if (!list.Where(l => l.Type.Equals(ListType.Reminder)).Any()) {
                this.CreateList(user.UserId, catalogInfo, new ListModel() { Name = "Reminder", BranchId = catalogInfo.BranchId }, ListType.Reminder);
				list = ReadListForCustomer(user, catalogInfo, headerOnly);
            }
 
            if (headerOnly)
                return list.Select(l => new ListModel() { ListId = l.Id, Name = l.DisplayName, 
					IsContractList = l.Type == ListType.Contract, 
					IsFavorite = l.Type == ListType.Favorite, 
					IsWorksheet = l.Type == ListType.Worksheet, 
					IsReminder = l.Type == ListType.Reminder,
 					IsMandatory = l.Type == ListType.Mandatory,
					ReadOnly = l.ReadOnly,
					SharedWith = l.Shares.Select(s => s.CustomerId).ToList(),
					IsSharing = l.Shares.Any() && l.CustomerId.Equals(catalogInfo.CustomerId) && l.BranchId.Equals(catalogInfo.BranchId),
					IsShared = !l.CustomerId.Equals(catalogInfo.CustomerId)}).ToList();
            else {
                var returnList = list.Select(b => b.ToListModel(catalogInfo)).ToList();
                var activeCart = basketLogic.RetrieveAllSharedCustomerBaskets(user, catalogInfo, Core.Enumerations.List.ListType.Cart).Where(b => b.Active.Equals(true));

                var processedList = new List<ListModel>();
                //Lookup product details for each item
                returnList.ForEach(delegate(ListModel listItem) {
                    var cachedList = listCacheRepository.GetItem<ListModel>(string.Format("UserList_{0}", listItem.ListId));
                    if (cachedList != null) {
                        processedList.Add(cachedList);
                        return;
                    }

                    LookupProductDetails(user, listItem, catalogInfo);
                    processedList.Add(listItem);
                    listCacheRepository.AddItem<ListModel>(string.Format("UserList_{0}", listItem.ListId), listItem);

                });
                //Mark favorites and add notes
                processedList.ForEach(delegate(ListModel listItem) {
                    MarkFavoritesAndAddNotes(user, listItem, catalogInfo, activeCart);
                });

                return processedList;
            }
        }

		private List<List> ReadListForCustomer(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly)
		{
			var list = listRepository.ReadListForCustomer(user, catalogInfo, headerOnly).Where(l => l.Type.Equals(ListType.Custom) ||
				(l.UserId.Equals(user.UserId) && l.Type.Equals(ListType.Favorite)) || l.Type.Equals(ListType.Contract) || l.Type.Equals(ListType.Worksheet) || l.Type.Equals(ListType.ContractItemsAdded)
				|| l.Type.Equals(ListType.ContractItemsDeleted) || l.Type.Equals(ListType.Reminder) || (user.IsDSR && l.Type.Equals(ListType.Mandatory))).ToList();
			return list;
		}
		
        public void UpdateItem(ListItemModel item)
		{
			listItemRepository.Update(new ListItem() { Id = item.ListItemId, ItemNumber = item.ItemNumber, Label = item.Label, Par = item.ParLevel, Position = item.Position});
			unitOfWork.SaveChanges();

			var updatedItem = listItemRepository.Read(i => i.Id.Equals(item.ListItemId), l => l.ParentList).FirstOrDefault();

			if (updatedItem != null && updatedItem.ParentList != null)
				listCacheRepository.RemoveItem(string.Format("UserList_{0}", updatedItem.ParentList.Id)); //Invalidate cache

		}

        public void UpdateList(ListModel userList)
		{
			var currentList = listRepository.Read(l => l.Id.Equals(userList.ListId), i => i.Items).FirstOrDefault();

			if (currentList == null)
				return;

			currentList.DisplayName = userList.Name;
            
			if (userList.Items == null)
				userList.Items = new List<ListItemModel>();

			if (currentList.Items != null)
				currentList.Items.Where(i => !userList.Items.Any(l => l.ListItemId.Equals(i.Id))).ToList().ForEach(delegate(ListItem item)
				{
					listItemRepository.Delete(item);
				});


			if (currentList.Items == null && userList.Items != null)
				currentList.Items = new List<ListItem>();


			foreach (var updateItem in userList.Items)
			{
				if (string.IsNullOrEmpty(updateItem.ItemNumber))
					continue;

				if (updateItem.ListItemId != 0)
				{
					var item = currentList.Items.Where(i => i.Id.Equals(updateItem.ListItemId)).FirstOrDefault();
					item.ItemNumber = updateItem.ItemNumber;
					item.Label = updateItem.Label;
					item.Par = updateItem.ParLevel;
					item.Position = updateItem.Position;
				}
				else
					currentList.Items.Add(new ListItem() { ItemNumber = updateItem.ItemNumber, Par = updateItem.ParLevel, Label = updateItem.Label });

			}

			unitOfWork.SaveChanges();
			listCacheRepository.RemoveItem(string.Format("UserList_{0}", currentList.Id)); //Invalidate cache
		}

		public void CopyList(ListCopyShareModel copyListModel)
		{
			var listToCopy = listRepository.ReadById(copyListModel.ListId);

			foreach (var customer in copyListModel.Customers)
			{
				var newList = new List() { DisplayName = string.Format("Copied - {0}", listToCopy.DisplayName), UserId = listToCopy.UserId, CustomerId = customer.CustomerNumber, BranchId = customer.CustomerBranch, Type = ListType.Custom, ReadOnly = false };
				newList.Items = new List<ListItem>();
				foreach (var item in listToCopy.Items)
					newList.Items.Add(new ListItem() { Category = item.Category, ItemNumber = item.ItemNumber, Label = item.Label, Par = item.Par, Position = item.Position });
				listRepository.Create(newList);

			}

			unitOfWork.SaveChanges();
		}
		
		public void ShareList(ListCopyShareModel shareListModel)
		{
			var listToShare = listRepository.ReadById(shareListModel.ListId);

			if (listToShare == null)
				return;

			foreach (var customer in shareListModel.Customers)
			{
				if(!listToShare.Shares.Any(s => s.CustomerId.Equals(customer.CustomerNumber) && s.BranchId.Equals(customer.CustomerBranch)))
					listToShare.Shares.Add(new ListShare() { CustomerId = customer.CustomerNumber, BranchId = customer.CustomerBranch });
			}

			var itemsToRemove = listToShare.Shares.Where(l => !shareListModel.Customers.Any(c => c.CustomerNumber.Equals(l.CustomerId) && c.CustomerBranch.Equals(l.BranchId))).Select(l => l).ToList();

			foreach (var item in itemsToRemove)
				listShareRepository.Delete(item);
			
			listRepository.Update(listToShare);
			unitOfWork.SaveChanges();

			var cachedList = listCacheRepository.GetItem<ListModel>(string.Format("UserList_{0}", listToShare.Id));
			if (cachedList != null)
			{
				cachedList.SharedWith = listToShare.Shares.Select(s => s.CustomerId).ToList();
				listCacheRepository.AddItem(string.Format("UserList_{0}", listToShare.Id), cachedList);
			}
		}
        
		#endregion


		
	}
}
