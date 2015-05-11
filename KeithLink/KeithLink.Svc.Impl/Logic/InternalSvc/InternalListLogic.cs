using KeithLink.Svc.Core.Interface.Lists;
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
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Impl.Helpers;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Common.Core.Logging;
using System.Collections.Concurrent;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Models.Messaging.Queue;
using KeithLink.Svc.Core.Extensions.Messaging;

namespace KeithLink.Svc.Impl.Logic.InternalSvc
{
	public class InternalListLogic : IInternalListLogic {
        #region attributes
        private readonly IUnitOfWork unitOfWork;
		private readonly IListRepository listRepository;
		private readonly IListItemRepository listItemRepository;
		private readonly ICatalogLogic catalogLogic;
		private readonly ICacheRepository listCacheRepository;
		private readonly IPriceLogic priceLogic;
		private readonly IProductImageRepository productImageRepository;
		private readonly IListShareRepository listShareRepository;
		private readonly IUserActiveCartRepository userActiveCartRepository;
		private readonly ICustomerRepository customerRepository;
		private readonly IEventLogRepository eventLogRepository;
		private readonly IGenericQueueRepository queueRepository;


		private const string CACHE_GROUPNAME = "UserList";
		private const string CACHE_NAME = "UserList";
		private const string CACHE_PREFIX = "Default";
        #endregion

        #region ctor
        public InternalListLogic(IUnitOfWork unitOfWork, IListRepository listRepository,
			IListItemRepository listItemRepository,
			ICatalogLogic catalogLogic, ICacheRepository listCacheRepository, IPriceLogic priceLogic, IProductImageRepository productImageRepository, IListShareRepository listShareRepository,
			IUserActiveCartRepository userActiveCartRepository, ICustomerRepository customerRepository, IEventLogRepository eventLogRepository, IGenericQueueRepository queueRepository)
		{
			this.listRepository = listRepository;
			this.unitOfWork = unitOfWork;
			this.listItemRepository = listItemRepository;
			this.catalogLogic = catalogLogic;
			this.listCacheRepository = listCacheRepository;
			this.priceLogic = priceLogic;
			this.productImageRepository = productImageRepository;
			this.listShareRepository = listShareRepository;
			this.userActiveCartRepository = userActiveCartRepository;
			this.customerRepository = customerRepository;
			this.eventLogRepository = eventLogRepository;
			this.queueRepository = queueRepository;
		}
        #endregion

        #region methods
		public long? AddItem(long listId, ListItemModel newItem)
		{
			var list = listRepository.ReadById(listId);

			var position = 1;

			if (list.Items == null)
				list.Items = new List<ListItem>();
			else
				if(list.Items.Any())
					position = list.Items.Max(i => i.Position) + 1;



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
				Position = position,
			};

			list.Items.Add(item);
			listRepository.CreateOrUpdate(list);
			unitOfWork.SaveChanges();

			if (list.Type == ListType.RecommendedItems)
				GenerateNewRecommendItemNotification(list.CustomerId, list.BranchId); //Send a notification that new recommended items have been added

			listCacheRepository.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", list.Id)); //Invalidate cache
			return item.Id;
		}

		private void GenerateNewRecommendItemNotification(string customerId, string branchId)
		{
			try
			{
				var notifcation = new HasNewsNotification()
				{
					CustomerNumber = customerId,
					BranchId = branchId,
					Subject = "New recommended items",
					Notification = "New recommended item(s) have been added to your list"
				};
				queueRepository.PublishToQueue(notifcation.ToJson(), Configuration.RabbitMQNotificationServer,
							Configuration.RabbitMQNotificationUserNamePublisher, Configuration.RabbitMQNotificationUserPasswordPublisher,
							Configuration.RabbitMQVHostNotification, Configuration.RabbitMQExchangeNotification);
			}
			catch (Exception ex)
			{
				eventLogRepository.WriteInformationLog("Error creating new recommended item notification", ex);
			}
		}

		public ListModel AddItems(UserProfile user, UserSelectedContext catalogInfo, long listId, List<ListItemModel> items)
		{
			var list = listRepository.ReadById(listId);
			var nextPosition = 1;
			if (list.Items == null)
				list.Items = new List<ListItem>();
			else
				if(list.Items.Any())
					nextPosition = list.Items.Max(i => i.Position) + 1;

			foreach (var item in items)
			{
				if ((list.Type == ListType.Favorite || list.Type == ListType.Reminder) && list.Items.Where(i => i.ItemNumber.Equals(item.ItemNumber)).Any())
					continue;

                list.Items.Add(
                    new ListItem() { 
                        ItemNumber = item.ItemNumber, 
                        Label = item.Label, 
                        Par = item.ParLevel, 
                        Each = !item.Each.Equals(null) ? item.Each : false,
 						Position = nextPosition
                    });
				nextPosition++;
			}

			listRepository.CreateOrUpdate(list);
			unitOfWork.SaveChanges();

			if (list.Type == ListType.RecommendedItems)
				GenerateNewRecommendItemNotification(list.CustomerId, list.BranchId); //Send a notification that new recommended items have been added

			listCacheRepository.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", list.Id)); //Invalidate cache
			return ReadList(user, catalogInfo, listId);
		}
        
		public void AddNote(UserProfile user, UserSelectedContext catalogInfo, ItemNote newNote)
		{
			var list = listRepository.ReadListForCustomer(catalogInfo, true).Where(i => i.Type == ListType.Notes).FirstOrDefault();
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
            var list = listRepository.Read(i => i.UserId == user.UserId && i.Type == ListType.Recent && i.CustomerId.Equals(catalogInfo.CustomerId), l => l.Items).FirstOrDefault();

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
        
        public long CreateList(Guid? userId, UserSelectedContext catalogInfo, ListModel list, ListType type)
		{
			var newList = list.ToEFList();

			//Set initial positions
			if (newList.Items != null && newList.Items.Any() && newList.Items.Max(i => i.Position) == 0)
			{
				var position = 1;
				foreach (var item in newList.Items)
				{
					item.Position = position;
					position++;
				}
			}

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
			listCacheRepository.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", listId)); //Invalidate cache
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
			var list = listRepository.ReadListForCustomer(catalogInfo, true).Where(l => l.Type == ListType.Notes).FirstOrDefault();
			if (list == null)
			{
				listItemRepository.Delete(list.Items.Where(i => i.ItemNumber.Equals(ItemNumber)).FirstOrDefault());
				unitOfWork.SaveChanges();
			}
		}

		private void LookupPrices(UserProfile user, List<ListItemModel> listItems, UserSelectedContext catalogInfo)
		{
			if (listItems == null || listItems.Count == 0 || user == null)
				return;
			var stopWatch = new System.Diagnostics.Stopwatch(); //Temp code while tweaking performance. This should be removed
			
			stopWatch.Start();

			var prices = priceLogic.GetPrices(catalogInfo.BranchId, catalogInfo.CustomerId, DateTime.Now.AddDays(1), listItems.GroupBy(g => g.ItemNumber).Select(i => new Product() { ItemNumber = i.First().ItemNumber }).Distinct().ToList());
			stopWatch.Stop();
			var priceTime = stopWatch.ElapsedMilliseconds;

			var priceHash = prices.Prices.ToDictionary(p => p.ItemNumber);

			Parallel.ForEach(listItems, listItem =>
			{
				var price = priceHash.ContainsKey(listItem.ItemNumber) ? priceHash[listItem.ItemNumber] : null;				
				if (price != null)
				{
					listItem.PackagePrice = price.PackagePrice.ToString();
					listItem.CasePrice = price.CasePrice.ToString();
				}
			});

			eventLogRepository.WriteInformationLog(string.Format("Lookup Prices for {0} Items. Price Time: {1}ms", listItems.Count, priceTime));

		}
		
        private void LookupProductDetails(UserProfile user, ListModel list, UserSelectedContext catalogInfo)
		{
			if (list.Items == null || list.Items.Count == 0)
				return;
			int totalProcessed = 0;
			ProductsReturn products = new ProductsReturn() { Products = new List<Product>() };
			
			while (totalProcessed < list.Items.Count)
			{
				var batch = list.Items.Skip(totalProcessed).Take(50).Select(i => i.ItemNumber).ToList();

				var tempProducts = catalogLogic.GetProductsByIds(list.BranchId, batch);

				products.Products.AddRange(tempProducts.Products);
				totalProcessed += 50;
			}

			var productHash = products.Products.GroupBy(p => p.ItemNumber).Select(i => i.First()).ToDictionary(p => p.ItemNumber);
			
			Parallel.ForEach(list.Items, listItem =>
			{
				var prod = productHash.ContainsKey(listItem.ItemNumber) ? productHash[listItem.ItemNumber] : null;
				if (prod != null)
				{
					listItem.Name = prod.Name;
					listItem.PackSize = string.Format("{0} / {1}", prod.Pack, prod.Size);
					listItem.BrandExtendedDescription = prod.BrandExtendedDescription;
					listItem.Description = prod.Description;
					listItem.Brand = prod.BrandExtendedDescription;
					listItem.StorageTemp = prod.Nutritional.StorageTemp;
					listItem.ReplacedItem = prod.ReplacedItem;
					listItem.ReplacementItem = prod.ReplacementItem;
					listItem.NonStock = prod.NonStock;
					listItem.ChildNutrition = prod.ChildNutrition;
					listItem.CatchWeight = prod.CatchWeight;
					listItem.ItemClass = prod.ItemClass;
					listItem.CategoryId = prod.CategoryId;
					listItem.CategoryName = prod.CategoryName;
					listItem.UPC = prod.UPC;
					listItem.VendorItemNumber = prod.VendorItemNumber;
					listItem.Cases = prod.Cases;
					listItem.Kosher = prod.Kosher;
					listItem.ManufacturerName = prod.ManufacturerName;
					listItem.ManufacturerNumber = prod.ManufacturerNumber;
					listItem.AverageWeight = prod.AverageWeight;
					listItem.Nutritional = new Nutritional()
					{
						CountryOfOrigin = prod.Nutritional.CountryOfOrigin,
						GrossWeight = prod.Nutritional.GrossWeight,
						HandlingInstructions = prod.Nutritional.HandlingInstructions,
						Height = prod.Nutritional.Height,
						Length = prod.Nutritional.Length,
						Ingredients = prod.Nutritional.Ingredients,
						Width = prod.Nutritional.Width
					};

				}
				
			});
			
		}

		private void MarkFavoritesAndAddNotes(UserProfile user, ListModel list, UserSelectedContext catalogInfo)
		{
			if (list.Items == null || list.Items.Count == 0)
				return;

			var notes = listRepository.Read(l => l.CustomerId.Equals(catalogInfo.CustomerId, StringComparison.CurrentCultureIgnoreCase) && l.BranchId.Equals(catalogInfo.BranchId) && l.Type == ListType.Notes, i => i.Items).FirstOrDefault();
			var favorites = listRepository.Read(l => l.UserId == user.UserId && l.CustomerId.Equals(catalogInfo.CustomerId, StringComparison.CurrentCultureIgnoreCase) && l.BranchId.Equals(catalogInfo.BranchId) && l.Type == ListType.Favorite, i => i.Items).FirstOrDefault();

			var notesHash = new Dictionary<string, ListItem>();
			var favHash = new Dictionary<string, ListItem>();

			if (notes != null && notes.Items != null)
				notesHash = notes.Items.ToDictionary(n => n.ItemNumber);
			if (favorites != null && favorites.Items != null)
				favHash = favorites.Items.ToDictionary(f => f.ItemNumber);

			
			Parallel.ForEach(list.Items, listItem =>
			{
				listItem.Favorite = favHash.ContainsKey(listItem.ItemNumber);// favorites.Items.Where(l => l.ItemNumber.Equals(listItem.ItemNumber)).Any();
				listItem.Notes = notesHash.ContainsKey(listItem.ItemNumber) ? notesHash[listItem.ItemNumber].Note : null;// notes.Items.Where(n => n.ItemNumber.Equals(listItem.ItemNumber)).Select(i => i.Note).FirstOrDefault();
			});

		}
		
		private void PopulateProductDetails(UserSelectedContext catalogInfo, List<RecentItem> returnList)
		{
			if (returnList == null)
				return;

			var products = catalogLogic.GetProductsByIds(catalogInfo.BranchId, returnList.Select(i => i.ItemNumber).Distinct().ToList());

			returnList.ForEach(delegate(RecentItem item)
			{
				var product = products.Products.Where(p => p.ItemNumber.Equals(item.ItemNumber)).FirstOrDefault();
				if (product != null)
				{
					item.Name = product.Name;
				}
			});

		}

		public List<string> ReadFavorites(UserProfile user, UserSelectedContext catalogInfo)
		{
            var list = listRepository.Read(l => l.UserId == user.UserId && l.CustomerId.Equals(catalogInfo.CustomerId) && l.Type == ListType.Favorite, i => i.Items).ToList();

			if (list == null)
				return null;

			return list.SelectMany(i => i.Items.Select(x => x.ItemNumber)).ToList();
		}

		public ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, long Id, bool includePrice = true)
		{
			var cachedList = listCacheRepository.GetItem<ListModel>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", Id));
			if (cachedList != null)
			{
				var clonedList = cachedList.Clone();

				MarkFavoritesAndAddNotes(user, clonedList, catalogInfo);
				
				var sharedlist = listRepository.Read(l => l.Id.Equals(Id), i => i.Items).FirstOrDefault();

				clonedList.IsSharing = sharedlist.Shares.Any() && sharedlist.CustomerId.Equals(catalogInfo.CustomerId) && sharedlist.BranchId.Equals(catalogInfo.BranchId, StringComparison.InvariantCultureIgnoreCase);
				clonedList.IsShared = !sharedlist.CustomerId.Equals(catalogInfo.CustomerId);
				
				if (includePrice)
					LookupPrices(user, clonedList.Items, catalogInfo);
				
				return clonedList;
			}

			var list = listRepository.Read(l => l.Id.Equals(Id), i => i.Items).FirstOrDefault();

			if (list == null)
				return null;

			var returnList = list.ToListModel(catalogInfo);


			LookupProductDetails(user, returnList, catalogInfo);
			listCacheRepository.AddItem<ListModel>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", Id), TimeSpan.FromHours(2), returnList);

			var listClone = returnList.Clone();

			MarkFavoritesAndAddNotes(user, listClone, catalogInfo);
			
			if(includePrice)
				LookupPrices(user, listClone.Items, catalogInfo);
			
			return listClone;
		}

		//private Core.Models.Generated.Basket GetUserActiveCart(UserSelectedContext catalogInfo, UserProfile user)
		//{
		//	var userActiveCart = userActiveCartRepository.Read(u => u.UserId == user.UserId && u.CustomerId.Equals(catalogInfo.CustomerId) && u.BranchId.Equals(catalogInfo.BranchId)).FirstOrDefault();
		//	var customer = customerRepository.GetCustomerByCustomerNumber(catalogInfo.CustomerId, catalogInfo.BranchId);

		//	KeithLink.Svc.Core.Models.Generated.Basket activeCart = null;

		//	if (userActiveCart != null && customer != null)
		//	{
		//		activeCart = basketRepository.ReadBasket(customer.CustomerId, userActiveCart.CartId);
		//	}
		//	return activeCart;
		//}

		public List<ListModel> ReadListByType(UserProfile user, UserSelectedContext catalogInfo, ListType type, bool headerOnly = false)
		{
			var list = listRepository.ReadListForCustomer(catalogInfo, headerOnly).Where(l => l.Type == type && l.CustomerId.Equals(catalogInfo.CustomerId) && l.BranchId.Equals(catalogInfo.BranchId, StringComparison.InvariantCultureIgnoreCase)).ToList();

			if (list == null)
				return null;

			if (headerOnly)
				return list.Select(l => new ListModel()
				{
					ListId = l.Id,
					Name = l.DisplayName,
					IsContractList = l.Type == ListType.Contract,
					IsFavorite = l.Type == ListType.Favorite,
					IsWorksheet = l.Type == ListType.Worksheet,
					IsReminder = l.Type == ListType.Reminder,
					IsMandatory = l.Type == ListType.Mandatory,
					IsRecommended = l.Type == ListType.RecommendedItems,
					SharedWith = l.Shares.Select(s => s.CustomerId).ToList(),
					IsSharing = l.Shares.Any() && l.CustomerId.Equals(catalogInfo.CustomerId) && l.BranchId.Equals(catalogInfo.BranchId),
					IsShared = !l.CustomerId.Equals(catalogInfo.CustomerId)
				}).ToList();
			else
			{
				var returnList = list.Select(b => b.ToListModel(catalogInfo)).ToList();

				var processedList = new List<ListModel>();
				//Lookup product details for each item
				returnList.ForEach(delegate(ListModel listItem)
				{
					var cachedList = listCacheRepository.GetItem<ListModel>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", listItem.ListId));
					if (cachedList != null)
					{
						processedList.Add(cachedList);
						return;
					}

					LookupProductDetails(user, listItem, catalogInfo);
					processedList.Add(listItem);
					listCacheRepository.AddItem<ListModel>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", listItem.ListId), TimeSpan.FromHours(2), listItem);

				});

				foreach(var tempList in returnList)
					LookupPrices(user, tempList.Items, catalogInfo);
			
				return returnList;
			}
		}

		public List<string> ReadListLabels(UserProfile user, UserSelectedContext catalogInfo)
		{
			return listRepository.Read(l => l.CustomerId.Equals(catalogInfo.CustomerId, StringComparison.CurrentCultureIgnoreCase) && l.BranchId.Equals(catalogInfo.BranchId) && (l.Type == ListType.Custom || l.Type == ListType.Favorite), i => i.Items).SelectMany(i => i.Items.Where(l => !string.IsNullOrEmpty(l.Label)).Select(b => b.Label)).Distinct().ToList();
		}
		
        public List<ListItemModel> ReadNotes(UserProfile user, UserSelectedContext catalogInfo)
		{
			var notes = listRepository.Read(l => l.CustomerId.Equals(catalogInfo.CustomerId, StringComparison.CurrentCultureIgnoreCase) && l.BranchId.Equals(catalogInfo.BranchId) && l.Type == ListType.Notes, i => i.Items).FirstOrDefault();

			if (notes == null)
				return new List<ListItemModel>();

			return notes.Items.Select(x => new ListItemModel() { ItemNumber = x.ItemNumber, Notes = x.Note }).ToList();
		}

		public List<RecentItem> ReadRecent(UserProfile user, UserSelectedContext catalogInfo)
		{
			var list = listRepository.Read(i => i.UserId == user.UserId  && i.Type == ListType.Recent && i.CustomerId.Equals(catalogInfo.CustomerId), l => l.Items);
			var returnItems = list.SelectMany(i => i.Items.Select(l => new RecentItem() { ItemNumber = l.ItemNumber, ModifiedOn = l.ModifiedUtc })).ToList();
			PopulateProductDetails(catalogInfo, returnItems);
			returnItems.ForEach(delegate(RecentItem item)
			{
				item.Images = productImageRepository.GetImageList(item.ItemNumber).ProductImages;
			});
			return returnItems.OrderByDescending(l => l.ModifiedOn).ToList();

		}

        public List<ListModel> ReadReminders(UserProfile user, UserSelectedContext catalogInfo) {
            var list = listRepository.Read(l => l.CustomerId.Equals(catalogInfo.CustomerId) && l.BranchId.Equals(catalogInfo.BranchId) && (l.Type == ListType.Reminder || l.Type == ListType.Mandatory), i => i.Items).ToList();

            if (list == null)
                return null;

            var returnList = list.Select(b => b.ToListModel(catalogInfo)).ToList();

            returnList.ForEach(delegate(ListModel listItem) {
                LookupProductDetails(user, listItem, catalogInfo);
				LookupPrices(user, listItem.Items, catalogInfo);
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
					IsRecommended = l.Type == ListType.RecommendedItems,
					ReadOnly = l.ReadOnly || (!user.IsInternalUser && l.Type.Equals(ListType.RecommendedItems) || (!user.IsInternalUser && l.Type.Equals(ListType.Mandatory)) ),
					SharedWith = l.Shares.Select(s => s.CustomerId).ToList(),
					IsSharing = l.Shares.Any() && l.CustomerId.Equals(catalogInfo.CustomerId) && l.BranchId.Equals(catalogInfo.BranchId),
					IsShared = !l.CustomerId.Equals(catalogInfo.CustomerId)}).ToList();
            else {
                var returnList = list.Select(b => b.ToListModel(catalogInfo)).ToList();
				
				var processedList = new List<ListModel>();
                //Lookup product details for each item
                returnList.ForEach(delegate(ListModel listItem) {
					var cachedList = listCacheRepository.GetItem<ListModel>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", listItem.ListId));
                    if (cachedList != null) {
                        processedList.Add(cachedList);
                        return;
                    }

                    LookupProductDetails(user, listItem, catalogInfo);
                    processedList.Add(listItem);
					listCacheRepository.AddItem<ListModel>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", listItem.ListId), TimeSpan.FromHours(2), listItem);

                });
                //Mark favorites and add notes
                processedList.ForEach(delegate(ListModel listItem) {
                    MarkFavoritesAndAddNotes(user, listItem, catalogInfo);
                });

                return processedList;
            }
        }

		private IEnumerable<List> ReadListForCustomer(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly)
		{
            if (String.IsNullOrEmpty(catalogInfo.CustomerId))
                return new List<List>();

			var list = listRepository.ReadListForCustomer(catalogInfo, headerOnly).Where(l => l.Type == ListType.Custom ||
				(l.UserId == user.UserId && l.Type == ListType.Favorite) || l.Type == ListType.Contract || l.Type == ListType.Worksheet || l.Type == ListType.ContractItemsAdded
				|| l.Type == ListType.ContractItemsDeleted || l.Type == ListType.Reminder  || l.Type == ListType.RecommendedItems || (l.Type == ListType.Mandatory));
			return list;
		}
		
        public void UpdateItem(ListItemModel item)
		{
			listItemRepository.Update(new ListItem() { Id = item.ListItemId, ItemNumber = item.ItemNumber, Label = item.Label, Par = item.ParLevel, Position = item.Position});
			unitOfWork.SaveChanges();

			var updatedItem = listItemRepository.Read(i => i.Id.Equals(item.ListItemId), l => l.ParentList).FirstOrDefault();

			if (updatedItem != null && updatedItem.ParentList != null)
				listCacheRepository.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", updatedItem.ParentList.Id)); //Invalidate cache

		}

        public void UpdateList(ListModel userList)
		{
			bool itemsAdded = false;
			var currentList = listRepository.Read(l => l.Id.Equals(userList.ListId), i => i.Items).FirstOrDefault();

			if (currentList == null)
				return;

			currentList.DisplayName = userList.Name;
            
			if (userList.Items == null)
				userList.Items = new List<ListItemModel>();

			if (currentList.Items == null && userList.Items != null)
				currentList.Items = new List<ListItem>();

            //if contract of history, replace all items with new items
            if (userList.Type == ListType.Worksheet || userList.Type == ListType.Contract)
            {
                foreach (var li in currentList.Items.ToList())
                {
                    listItemRepository.Delete(li);
                }
                
                foreach (var li in userList.Items.ToList())
                {
                    currentList.Items.Add(new ListItem() { ItemNumber = li.ItemNumber, Par = li.ParLevel, Label = li.Label, Each = li.Each });
                }
            }
            else
            {
                foreach (var updateItem in userList.Items)
                {
                    if (updateItem.IsDelete)
                    {
                        var itemToDelete = currentList.Items.Where(i => i.Id.Equals(updateItem.ListItemId)).FirstOrDefault();
                        if (itemToDelete != null)
                            listItemRepository.Delete(itemToDelete);
                    }
                    else
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
                            item.Each = updateItem.Each;
                        }
                        else
                        {
                            if ((currentList.Type == ListType.Favorite || currentList.Type == ListType.Reminder) && currentList.Items.Where(i => i.ItemNumber.Equals(updateItem.ItemNumber)).Any())
                                continue;

                            currentList.Items.Add(new ListItem() { ItemNumber = updateItem.ItemNumber, Par = updateItem.ParLevel, Label = updateItem.Label, Each = updateItem.Each });
							itemsAdded = true;
                        }
                    }
                }
            }

            unitOfWork.SaveChanges();

			if (currentList.Type == ListType.RecommendedItems && itemsAdded)
				GenerateNewRecommendItemNotification(currentList.CustomerId, currentList.BranchId); //Send a notification that new recommended items have been added

			listCacheRepository.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", currentList.Id)); //Invalidate cache
		}

		public List<ListCopyResultModel> CopyList(ListCopyShareModel copyListModel)
		{
			var listToCopy = listRepository.ReadById(copyListModel.ListId);

			var listToCreate = new List<List>();
			foreach (var customer in copyListModel.Customers)
			{
				var newList = new List() { DisplayName = string.Format("Copied - {0}", listToCopy.DisplayName), UserId = listToCopy.UserId, CustomerId = customer.CustomerNumber, BranchId = customer.CustomerBranch, Type = ListType.Custom, ReadOnly = false };
				newList.Items = new List<ListItem>();
				foreach (var item in listToCopy.Items)
					newList.Items.Add(new ListItem() { Category = item.Category, ItemNumber = item.ItemNumber, Label = item.Label, Par = item.Par, Position = item.Position });
				listRepository.Create(newList);
				listToCreate.Add(newList);

			}

			unitOfWork.SaveChanges();

			return listToCreate.Select(l => new ListCopyResultModel() { CustomerId = l.CustomerId, BranchId = l.BranchId, NewListId = l.Id }).ToList();
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

			var cachedList = listCacheRepository.GetItem<ListModel>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", listToShare.Id));
			if (cachedList != null)
			{
				cachedList.SharedWith = listToShare.Shares.Select(s => s.CustomerId).ToList();
				listCacheRepository.AddItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", listToShare.Id), TimeSpan.FromHours(2), cachedList);
			}
		}
        
		public List<RecommendedItemModel> ReadRecommendedItemsList(UserSelectedContext catalogInfo)
		{
			var list = listRepository.Read(l => l.Type == ListType.RecommendedItems && l.CustomerId.Equals(catalogInfo.CustomerId) && l.BranchId.Equals(catalogInfo.BranchId)).FirstOrDefault();

			if (list == null || list.Items == null)
				return new List<RecommendedItemModel>();

			var returnItems = list.Items.Where(i => (i.FromDate == null || i.FromDate <= DateTime.Now) && (i.ToDate == null || i.ToDate >= DateTime.Now)).Select(r => new RecommendedItemModel() { ItemNumber = r.ItemNumber }).ToList();

			var products = catalogLogic.GetProductsByIds(catalogInfo.BranchId, returnItems.Select(i => i.ItemNumber).Distinct().ToList());

			returnItems.ForEach(delegate(RecommendedItemModel item)
			{
				var product = products.Products.Where(p => p.ItemNumber.Equals(item.ItemNumber)).FirstOrDefault();
				if (product != null)
				{
					item.Name = product.Name;
				}
			});

			returnItems.ForEach(delegate(RecommendedItemModel item)
			{
				item.Images = productImageRepository.GetImageList(item.ItemNumber).ProductImages;
			});

			return returnItems;
		}

		public List<Core.Models.Reports.ItemBarcodeModel> GetBarcodeForList(UserProfile user, UserSelectedContext catalogInfo, long Id)
		{
			var cachedList = listCacheRepository.GetItem<ListModel>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", Id));
			if (cachedList != null)
			{
				return cachedList.Items.Select(i => new ItemBarcodeModel() { ItemNumber = i.ItemNumber, Name = i.Name, PackSize = i.PackSize }).ToList();
			}

			var list = listRepository.Read(l => l.Id.Equals(Id), i => i.Items).FirstOrDefault();

			if (list == null)
				return null;

			var returnList = list.ToListModel(catalogInfo);
			LookupNameAndPackSize(user, returnList, catalogInfo);

			return returnList.Items.Select(i => new ItemBarcodeModel() { ItemNumber = i.ItemNumber, Name = i.Name, PackSize = i.PackSize }).ToList();
			
		}

		private void LookupNameAndPackSize(UserProfile user, ListModel list, UserSelectedContext catalogInfo)
		{
			if (list.Items == null || list.Items.Count == 0)
				return;

			var products = catalogLogic.GetProductsByIds(list.BranchId, list.Items.Select(i => i.ItemNumber).Distinct().ToList());
			

			var productHash = products.Products.ToDictionary(p => p.ItemNumber);
			
			Parallel.ForEach(list.Items, listItem =>
			{
				var prod = productHash.ContainsKey(listItem.ItemNumber) ? productHash[listItem.ItemNumber] : null;
				if (prod != null)
				{
					listItem.Name = prod.Name;
					listItem.PackSize = string.Format("{0} / {1}", prod.Pack, prod.Size);	
				}
				
			});

		}

		public PagedListModel ReadPagedList(UserProfile user, UserSelectedContext catalogInfo, long Id, Core.Models.Paging.PagingModel paging)
		{
			var cachedList = listCacheRepository.GetItem<ListModel>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", Id));
			if (cachedList != null)
			{
				var cachedReturnList = cachedList.ShallowCopy();

				MarkFavoritesAndAddNotes(user, cachedReturnList, catalogInfo);

				var sharedlist = listRepository.Read(l => l.Id.Equals(Id)).FirstOrDefault();

				cachedReturnList.IsSharing = sharedlist.Shares.Any() && sharedlist.CustomerId.Equals(catalogInfo.CustomerId) && sharedlist.BranchId.Equals(catalogInfo.BranchId, StringComparison.InvariantCultureIgnoreCase);
				cachedReturnList.IsShared = !sharedlist.CustomerId.Equals(catalogInfo.CustomerId);

				var cachedPagedList = ToPagedList(paging, cachedReturnList);
				LookupPrices(user, cachedPagedList.Items.Results, catalogInfo);

				return cachedPagedList;
			}
			
			var list = listRepository.Read(l => l.Id.Equals(Id), l => l.Items).FirstOrDefault();
			
			
			if (list == null)
				return null;
			var tempList = list.ToListModel(catalogInfo);

			LookupProductDetails(user, tempList, catalogInfo);
			listCacheRepository.AddItem<ListModel>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", Id), TimeSpan.FromHours(2), tempList);

			var returnList = tempList.ShallowCopy();

			MarkFavoritesAndAddNotes(user, returnList, catalogInfo);
			
			var pagedList = ToPagedList(paging, returnList);
			
			LookupPrices(user, pagedList.Items.Results, catalogInfo);
			

			return pagedList;
		}

		private PagedListModel ToPagedList(PagingModel paging, ListModel returnList)
		{
			var pagedList = new PagedListModel()
			{
				BranchId = returnList.BranchId,
				IsContractList = returnList.IsContractList,
				IsFavorite = returnList.IsFavorite,
				IsMandatory = returnList.IsMandatory,
				IsRecommended = returnList.IsRecommended,
				IsReminder = returnList.IsReminder,
				IsShared = returnList.IsShared,
				IsSharing = returnList.IsSharing,
				IsWorksheet = returnList.IsWorksheet,
				ListId = returnList.ListId,
				Name = returnList.Name,
				ReadOnly = returnList.ReadOnly,
				SharedWith = returnList.SharedWith,
				Type = returnList.Type
			};

			if (returnList.Items != null)
				pagedList.Items = returnList.Items.AsQueryable<ListItemModel>().GetPage<ListItemModel>(paging, "Position");

			return pagedList;
		}

		public void DeleteItemNumberFromList(long Id, string itemNumber)
		{
			var list = listRepository.Read(l => l.Id.Equals(Id), i => i.Items).FirstOrDefault();

			if (list == null)
				return;

			foreach (var item in list.Items.Where(i => i.ItemNumber.Equals(itemNumber)).ToList())
				listItemRepository.Delete(item);
			
			unitOfWork.SaveChanges();
		}

		public List<InHistoryReturnModel> ItemsInHistoryList(UserSelectedContext catalogInfo, List<string> itemNumbers)
		{
			var returnModel = new BlockingCollection<InHistoryReturnModel>();
			
			var list = listRepository.Read(l => l.CustomerId.Equals(catalogInfo.CustomerId) && l.BranchId.Equals(catalogInfo.BranchId, StringComparison.CurrentCultureIgnoreCase) && l.Type == ListType.Worksheet, i => i.Items).FirstOrDefault();

			if (list == null)
				return itemNumbers.Select(i => new InHistoryReturnModel() { ItemNumber = i, InHistory = false }).ToList();
			else
			{
				Parallel.ForEach(itemNumbers, item =>
				{
					returnModel.Add(new InHistoryReturnModel() { InHistory = list.Items.Where(i => i.ItemNumber.Equals(item)).Any(), ItemNumber = item });
				});
			}
			return returnModel.ToList();
		}

		#endregion

	}
}
