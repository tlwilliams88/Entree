using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Extensions.Messaging;

using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;

using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Configuration.EF;
using KeithLink.Svc.Core.Models.Customers.EF;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Messaging.Queue;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Models.SiteCatalog;

using KeithLink.Svc.Impl.Repository.EF.Operational;

using Microsoft.Reporting.WinForms;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Impl.Helpers;

namespace KeithLink.Svc.Impl.Logic.Lists {
    public class ListLogicImpl : IListLogic {
        #region attributes
        private readonly ICacheRepository           _cache;
        private readonly ICatalogLogic              _catalogLogic;
        private readonly ICustomerRepository        _customerRepo;
        private readonly IExternalCatalogRepository _externalCatalogRepo;
        private readonly IItemHistoryRepository     _itemHistoryRepo;
        private readonly IListItemRepository        _listItemRepo;
        private readonly IListRepository            _listRepo;
        private readonly IListShareRepository       _listShareRepo;
        private readonly IEventLogRepository        _log;
        private readonly IPriceLogic                _priceLogic;
        private readonly IProductImageRepository    _productImageRepo;
        private readonly IGenericQueueRepository    _queueRepo;
        private readonly ISettingsRepository        _settingsRepo;
        private readonly IUnitOfWork                _uow;

        private const string CACHE_GROUPNAME = "UserList";
        private const string CACHE_NAME = "UserList";
        private const string CACHE_PREFIX = "Default";
        #endregion

        #region ctor
        public ListLogicImpl(IUnitOfWork unitOfWork, IListRepository listRepository, IListItemRepository listItemRepository, 
                            ICatalogLogic catalogLogic, ICacheRepository listCacheRepository, IPriceLogic priceLogic,
                            IProductImageRepository productImageRepository, IListShareRepository listShareRepository, ICustomerRepository customerRepository, 
                            IEventLogRepository eventLogRepository, IGenericQueueRepository queueRepository, ISettingsRepository settingsRepo,
                            IItemHistoryRepository itemHistoryRepository, IExternalCatalogRepository externalCatalogRepository) {
            _cache = listCacheRepository;
            _catalogLogic = catalogLogic;
            _customerRepo = customerRepository;
            _itemHistoryRepo = itemHistoryRepository;
            _externalCatalogRepo = externalCatalogRepository;
            _listItemRepo = listItemRepository;
            _listRepo = listRepository;
            _listShareRepo = listShareRepository;
            _log = eventLogRepository;
            _priceLogic = priceLogic;
            _productImageRepo = productImageRepository;
            _queueRepo = queueRepository;
            _settingsRepo = settingsRepo;
            _uow = unitOfWork;
        }
        #endregion

        #region methods
        /// <summary>
        /// add a new list item to the list
        /// </summary>
        /// <param name="listId">the list's unique id</param>
        /// <param name="newItem">the item being added</param>
        /// <returns>the list item's unique id</returns>
        public long? AddItem(long listId, ListItemModel newItem) {
            var list = _listRepo.ReadById(listId);

            var position = 1;

            if(list.Items == null)
                list.Items = new List<ListItem>();
            else
                if(list.Items.Any())
                position = list.Items.Max(i => i.Position) + 1;

            //Don't allow duplicates
            if(list.Type == ListType.Favorite || list.Type == ListType.Reminder) {
                var dupItem = list.Items.Where(i => i.ItemNumber.Equals(newItem.ItemNumber)).FirstOrDefault();
                if(dupItem != null)
                    return dupItem.Id;
            }

            var item = new ListItem() {
                ItemNumber = newItem.ItemNumber,
                Label = newItem.Label,
                Par = newItem.ParLevel,
                Position = position,
                Quantity = newItem.Quantity,
                Each = newItem.Each ?? false,
                CatalogId = newItem.CatalogId
            };

            list.Items.Add(item);
            _listRepo.CreateOrUpdate(list);
            _uow.SaveChanges();

            if(list.Type == ListType.RecommendedItems)
                GenerateNewRecommendItemNotification(list.CustomerId, list.BranchId); //Send a notification that new recommended items have been added

            _cache.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", list.Id)); //Invalidate cache

            return item.Id;
        }

        public ListModel AddItems(UserProfile user, UserSelectedContext catalogInfo, long listId, List<ListItemModel> items) {
            var list = _listRepo.ReadById(listId);
            var nextPosition = 1;
            if(list.Items == null)
                list.Items = new List<ListItem>();
            else
                if(list.Items.Any())
                nextPosition = list.Items.Max(i => i.Position) + 1;

            foreach(var item in items) {
                if((list.Type == ListType.Favorite || list.Type == ListType.Reminder) && list.Items.Where(i => i.ItemNumber.Equals(item.ItemNumber)).Any())
                    continue;

                list.Items.Add(
                    new ListItem() {
                        ItemNumber = item.ItemNumber,
                        Label = item.Label,
                        Par = item.ParLevel,
                        Each = !item.Each.Equals(null) ? item.Each : false,
                        Position = nextPosition,
                        Quantity = item.Quantity,
                        CatalogId = item.CatalogId
                    });
                nextPosition++;
            }

            _listRepo.CreateOrUpdate(list);
            _uow.SaveChanges();

            if(list.Type == ListType.RecommendedItems)
                GenerateNewRecommendItemNotification(list.CustomerId, list.BranchId); //Send a notification that new recommended items have been added

            _cache.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", list.Id)); //Invalidate cache

            return ReadList(user, catalogInfo, listId);
        }

        public void AddNote(UserProfile user, UserSelectedContext catalogInfo, ItemNote newNote) {
            var list = _listRepo.ReadListForCustomer(catalogInfo, true).Where(i => i.Type == ListType.Notes).FirstOrDefault();
            if(list == null) {
                var newNotes = new List() {
                    Type = ListType.Notes,
                    CustomerId = catalogInfo.CustomerId,
                    BranchId = catalogInfo.BranchId,
                    DisplayName = "Notes",
                    ReadOnly = false,
                    UserId = user.UserId,
                    Items = new List<ListItem>() { new ListItem() { ItemNumber = newNote.ItemNumber, Note = newNote.Note, CatalogId = newNote.CatalogId } }

                };

                _listRepo.Create(newNotes);
            } else {
                var existingItem = list.Items.Where(i => i.ItemNumber.Equals(newNote.ItemNumber)).FirstOrDefault();

                if(existingItem != null) {
                    existingItem.Note = newNote.Note;
                    existingItem.CatalogId = newNote.CatalogId;

                    _listItemRepo.Update(existingItem);
                } else {
                    var createNote = new ListItem() { Note = newNote.Note, ItemNumber = newNote.ItemNumber, ParentList = list, CatalogId = newNote.CatalogId };

                    _listItemRepo.Create(createNote);
                }
            }

            _uow.SaveChanges();
        }

        public void AddRecentlyOrderedItems(UserProfile user, UserSelectedContext catalogInfo, RecentNonBEKList newlist) {
            List list = _listRepo.Read(i => i.UserId == user.UserId && 
                                           i.Type == ListType.RecentOrderedNonBEK &&
                                           i.BranchId == newlist.Catalog &&
                                           i.CustomerId.Equals(catalogInfo.CustomerId), 
                                      l => l.Items)
                                .FirstOrDefault();

            if(list == null) {
                //Create a new recently ordered list
                this.CreateList(user.UserId, new UserSelectedContext() { CustomerId = catalogInfo.CustomerId, BranchId = newlist.Catalog }, 
                    new ListModel() {
                    Name = "Recent Orders",
                    BranchId = newlist.Catalog,
                    Items = new List<ListItemModel>()
                }, ListType.RecentOrderedNonBEK);
                // grab a pointer to the newly created list
                list = _listRepo.Read(i => i.UserId == user.UserId &&
                               i.Type == ListType.RecentOrderedNonBEK &&
                               i.BranchId == newlist.Catalog &&
                               i.CustomerId.Equals(catalogInfo.CustomerId),
                          l => l.Items)
                    .FirstOrDefault();
                list.Items = new List<ListItem>();
            }
            else
            {
                list.Items = list.Items.ToList();
            }
            // Identify specific warehouse - needed for product lookup
            Dictionary<string, string> externalCatalogDict = 
                _externalCatalogRepo.ReadAll().ToDictionary(e => e.BekBranchId.ToLower(), e => e.ExternalBranchId);
            foreach (string itemNumber in newlist.Items.Select(i => i.ItemNumber).ToList()) {
                // Insert newest at the start of the list while filtering out duplicates
                var item = list.Items.Where(i => i.ItemNumber.Equals(itemNumber)).FirstOrDefault();
                if (item != null)
                {
                    _listItemRepo.Delete(item);
                }
                ((List<ListItem>)list.Items).Insert(0, new ListItem() { ItemNumber = itemNumber, CatalogId = externalCatalogDict[catalogInfo.BranchId.ToLower()] });
            }
            // tailor list number to configured value
            if (list.Items.Count >= Configuration.RecentItemsToKeep)
            {
                while (list.Items.Count >= Configuration.RecentItemsToKeep)
                {
                    _listItemRepo.Delete(list.Items.Last());
                }
            }

            _uow.SaveChanges();
        }

        public void AddRecentlyViewedItem(UserProfile user, UserSelectedContext catalogInfo, string itemNumber)
        {
            var list = _listRepo.Read(i => i.UserId == user.UserId &&
                                           i.Type == ListType.Recent &&
                                           i.CustomerId.Equals(catalogInfo.CustomerId),
                                      l => l.Items)
                                .FirstOrDefault();

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
            else {
                var item = list.Items.Where(i => i.ItemNumber.Equals(itemNumber)).FirstOrDefault();
                if (item != null)
                {
                    _listItemRepo.Update(item);
                }
                else {
                    if (list.Items.Count >= Configuration.RecentItemsToKeep)
                        _listItemRepo.Delete(list.Items.OrderBy(i => i.ModifiedUtc).FirstOrDefault());

                    list.Items.Add(new ListItem() { ItemNumber = itemNumber });
                }
            }

            _uow.SaveChanges();
        }

        /// <summary>
        /// copies the list for sharing with other users
        /// </summary>
        /// <param name="copyListModel">ListCopyShareModel</param>
        /// <returns>the copied list</returns>
        public List<ListCopyResultModel> CopyList(ListCopyShareModel copyListModel) {
            var listToCopy = _listRepo.ReadById(copyListModel.ListId);

            var listToCreate = new List<List>();
            Dictionary<string, ExternalCatalog> externalCatalogDict = _externalCatalogRepo.ReadAll().ToDictionary(e => e.BekBranchId.ToLower());

            foreach(var customer in copyListModel.Customers) {
                var newList = new List() {
                    DisplayName = string.Format("Copied - {0}", listToCopy.DisplayName),
                    UserId = listToCopy.UserId,
                    CustomerId = customer.CustomerNumber,
                    BranchId = customer.CustomerBranch,
                    Type = ListType.Custom,
                    ReadOnly = false
                };

                ExternalCatalog currentExtCatalog = externalCatalogDict[customer.CustomerBranch.ToLower()];

                newList.Items = new List<ListItem>();
                foreach(var item in listToCopy.Items) {
                    newList.Items.Add(new ListItem() {
                        Category = item.Category,
                        ItemNumber = item.ItemNumber,
                        Label = item.Label,
                        Par = item.Par,
                        Position = item.Position,
                        Each = item.Each,
                        CatalogId = IsBekBranch(item.CatalogId) ? customer.CustomerBranch : currentExtCatalog.ExternalBranchId
                    });
                }

                _listRepo.Create(newList);
                listToCreate.Add(newList);
            }

            _uow.SaveChanges();

            return listToCreate.Select(l => new ListCopyResultModel() { CustomerId = l.CustomerId, BranchId = l.BranchId, NewListId = l.Id }).ToList();
        }

        public long CreateList(Guid? userId, UserSelectedContext catalogInfo, ListModel list, ListType type) {
            var newList = list.ToEFList();

            //Set initial positions
            if(newList.Items != null && 
               newList.Items.Any() && 
               newList.Items.Max(i => i.Position) == 0) {
                var position = 1;
                foreach(var item in newList.Items) {
                    item.Position = position;
                    position++;
                }
            }

            newList.BranchId = catalogInfo.BranchId;
            newList.CustomerId = catalogInfo.CustomerId;
            newList.UserId = userId;
            newList.Type = type;

            _listRepo.CreateOrUpdate(newList);

            _uow.SaveChanges();

            return newList.Id;
        }

        public void DeleteItem(long Id) {
            var item = _listItemRepo.Read(i => i.Id.Equals(Id), l => l.ParentList).FirstOrDefault();
            var listId = item.ParentList.Id;

            _listItemRepo.Delete(item);

            _uow.SaveChanges();

            _cache.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", listId)); //Invalidate cache
        }

        public void DeleteItems(List<long> itemIds) {
            foreach(long itemId in itemIds) {
                DeleteItem(itemId);
            }
        }

        public void DeleteItemNumberFromList(long Id, string itemNumber) {
            var list = _listRepo.Read(l => l.Id.Equals(Id), i => i.Items).FirstOrDefault();

            if(list == null) {
                return;
            } else {
                foreach(var item in list.Items.Where(i => i.ItemNumber.Equals(itemNumber)).ToList()) {
                    _listItemRepo.Delete(item);
                }

                _uow.SaveChanges();
            }
        }

        public void DeleteList(long Id) {
            var list = _listRepo.Read(l => l.Id.Equals(Id), i => i.Items).FirstOrDefault();

            list.Items.ToList().ForEach(delegate (ListItem item) {
                _listItemRepo.Delete(item);
            });

            _listRepo.Delete(list);

            _uow.SaveChanges();
        }

        public void DeleteLists(List<long> listIds) {
            foreach(long listId in listIds) {
                DeleteList(listId);
            }
        }

        public void DeleteNote(UserProfile user, UserSelectedContext catalogInfo, string ItemNumber) {
            var list = _listRepo.ReadListForCustomer(catalogInfo, true)
                                .Where(l => l.Type == ListType.Notes)
                                .FirstOrDefault();

            if(list != null) {
                _listItemRepo.Delete(list.Items.Where(i => i.ItemNumber.Equals(ItemNumber)).FirstOrDefault());

                _uow.SaveChanges();
            }
        }

        public void DeleteRecent(UserProfile user, UserSelectedContext catalogInfo) {
            List<List> listcol = null;
            if (user.UserId != null &&
               catalogInfo.CustomerId != null)
                listcol =
                    _listRepo.Read(i => i.UserId == user.UserId &&
                                        i.Type == ListType.Recent &&
                                        i.CustomerId.Equals(catalogInfo.CustomerId),
                                   l => l.Items)
                             .ToList();
            else
                listcol =
                    _listRepo.Read(i => i.UserId == user.UserId &&
                                        i.Type == ListType.Recent,
                                   l => l.Items)
                             .ToList();

            List list = (List)listcol[0];
            list.Items.ToList().ForEach(delegate(ListItem item) {
                _listItemRepo.Delete(item);
            });

            _listRepo.Delete(list);
            _uow.SaveChanges();
        }

        public void DeleteRecentlyOrdered(UserProfile user, UserSelectedContext catalogInfo)
        {
            List<List> listcol = null;
            if (user.UserId != null &&
               catalogInfo.CustomerId != null)
                listcol =
                    _listRepo.Read(i => i.UserId == user.UserId &&
                                        i.Type == ListType.RecentOrderedNonBEK &&
                                        i.CustomerId.Equals(catalogInfo.CustomerId),
                                   l => l.Items)
                             .ToList();
            else
                listcol =
                    _listRepo.Read(i => i.UserId == user.UserId &&
                                        i.Type == ListType.RecentOrderedNonBEK,
                                   l => l.Items)
                             .ToList();

            List list = (List)listcol[0];
            list.Items.ToList().ForEach(delegate (ListItem item) {
                _listItemRepo.Delete(item);
            });

            _listRepo.Delete(list);
            _uow.SaveChanges();
        }

        private void GenerateNewRecommendItemNotification(string customerId, string branchId) {
            try {
                var notifcation = new HasNewsNotification() {
                    CustomerNumber = customerId,
                    BranchId = branchId,
                    Subject = "New recommended items",
                    Notification = "New recommended item(s) have been added to your list"
                };
                _queueRepo.PublishToQueue(notifcation.ToJson(), Configuration.RabbitMQNotificationServer, Configuration.RabbitMQNotificationUserNamePublisher, 
                                          Configuration.RabbitMQNotificationUserPasswordPublisher, Configuration.RabbitMQVHostNotification, Configuration.RabbitMQExchangeNotification);
            } catch(Exception ex) {
                _log.WriteInformationLog("Error creating new recommended item notification", ex);
            }
        }

        public List<ItemBarcodeModel> GetBarcodeForList(UserProfile user, UserSelectedContext catalogInfo, long Id) {
            var cachedList = _cache.GetItem<ListModel>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", Id));
            if(cachedList == null) {
                var list = _listRepo.Read(l => l.Id.Equals(Id), i => i.Items).FirstOrDefault();

                if(list == null)
                    return null;

                var returnList = list.ToListModel(catalogInfo);
                LookupNameAndPackSize(user, returnList, catalogInfo);

                return returnList.Items.Select(i => new ItemBarcodeModel() { ItemNumber = i.ItemNumber, Name = i.Name, PackSize = i.PackSize,
                    BarCode = GetBarcode("*" + i.ItemNumber + "*") }).ToList();
            } else {
                return cachedList.Items.Select(i => new ItemBarcodeModel() { ItemNumber = i.ItemNumber, Name = i.Name, PackSize = i.PackSize,
                    BarCode = GetBarcode("*" + i.ItemNumber + "*") }).ToList();
            }
        }

        public byte[] GetBarcode(string text)
        {
            System.Drawing.Bitmap b;
            BarcodeLib.Barcode bar = new BarcodeLib.Barcode(text);
            bar.Alignment = BarcodeLib.AlignmentPositions.LEFT;
            bar.IncludeLabel = false;
            bar.RotateFlipType = System.Drawing.RotateFlipType.RotateNoneFlipNone;
            b = (System.Drawing.Bitmap)bar.Encode(BarcodeLib.TYPE.CODE39Extended, text, 250, 40);
            byte[] data;
            using (System.IO.MemoryStream ms = new MemoryStream())
            {
                b.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                data = ms.ToArray();
            }
            return data;
        }

        public ItemHistory[] GetItemsHistoryList(UserSelectedContext userContext, string[] itemNumbers) {
            ItemHistory[] itemStatistics = _itemHistoryRepo.Read(f => f.BranchId.Equals(userContext.BranchId) && 
                                                                      f.CustomerNumber.Equals(userContext.CustomerId))
                                                           .Where(f => itemNumbers.Contains(f.ItemNumber))
                                                           .ToArray();
            return itemStatistics;
        }

        public bool IsBekBranch(string catalogId) {
            return catalogId != null &&
                   catalogId.Length == 3 &&
                   catalogId.StartsWith("f", true, System.Globalization.CultureInfo.CurrentCulture);
        }

        //public List<InHistoryReturnModel> ItemsInHistoryList(UserSelectedContext catalogInfo, List<string> itemNumbers) {
        //    var returnModel = new BlockingCollection<InHistoryReturnModel>();

        //    var list = _listRepo.Read(l => l.CustomerId.Equals(catalogInfo.CustomerId) && l.BranchId.Equals(catalogInfo.BranchId, StringComparison.CurrentCultureIgnoreCase) && l.Type == ListType.Worksheet, i => i.Items).FirstOrDefault();

        //    if(list == null)
        //        return itemNumbers.Select(i => new InHistoryReturnModel() { ItemNumber = i, InHistory = false }).ToList();
        //    else {
        //        Parallel.ForEach(itemNumbers, item => {
        //            returnModel.Add(new InHistoryReturnModel() { InHistory = list.Items.Where(i => i.ItemNumber.Equals(item)).Any(), ItemNumber = item });
        //        });
        //    }
        //    return returnModel.ToList();
        //}

        private void LookupNameAndPackSize(UserProfile user, ListModel list, UserSelectedContext catalogInfo) {
            if(list.Items == null || list.Items.Count == 0)
                return;

            var products = _catalogLogic.GetProductsByIds(list.BranchId, list.Items.Select(i => i.ItemNumber).Distinct().ToList());
            var productHash = products.Products.ToDictionary(p => p.ItemNumber);

            Parallel.ForEach(list.Items, listItem => {
                var prod = productHash.ContainsKey(listItem.ItemNumber) ? productHash[listItem.ItemNumber] : null;
                if(prod != null) {
                    listItem.IsValid = true;
                    listItem.Name = prod.Name;
                    listItem.PackSize = string.Format("{0} / {1}", prod.Pack, prod.Size);
                }

            });
        }

        private void LookupPrices(UserProfile user, List<ListItemModel> listItems, UserSelectedContext catalogInfo) {
            if(listItems == null || listItems.Count == 0 || user == null)
                return;

            var prices = new PriceReturn() { Prices = new List<Price>() };

            prices.AddRange(_priceLogic.GetPrices(catalogInfo.BranchId, catalogInfo.CustomerId, DateTime.Now.AddDays(1),
                                                  listItems.GroupBy(g => g.ItemNumber)
                                                           .Select(i => new Product() {
                                                               ItemNumber = i.First().ItemNumber,
                                                               CatchWeight = i.First().CatchWeight,
                                                               PackagePriceNumeric = i.First().PackagePriceNumeric,
                                                               CasePriceNumeric = i.First().CasePriceNumeric,
                                                               CategoryName = i.First().CategoryName,
                                                               CatalogId = i.First().CatalogId,
                                                               Unfi = i.First().Unfi
                                                           })
                                                           .Distinct()
                                                           .ToList()
                                                  )
                           );

            Dictionary<string, Price> priceHash = prices.Prices.ToDictionary(p => p.ItemNumber);

            Parallel.ForEach(listItems, listItem => {
                var price = priceHash.ContainsKey(listItem.ItemNumber) ? priceHash[listItem.ItemNumber] : null;
                if(price != null) {
                    listItem.PackagePrice = price.PackagePrice.ToString();
                    listItem.CasePrice = price.CasePrice.ToString();
                    listItem.DeviatedCost = price.DeviatedCost ? "Y" : "N";
                }
            });
        }

        private void LookupProductDetails(UserProfile user, ListModel list, UserSelectedContext catalogInfo) {
            if(list.Items == null || list.Items.Count == 0)
                return;
            int totalProcessed = 0;
            ProductsReturn products = new ProductsReturn() { Products = new List<Product>() };

            while(totalProcessed < list.Items.Count) {
                var batch = list.Items.Skip(totalProcessed)
                                      .Take(50)
                                      .Select(i => i.ItemNumber)
                                      .ToList();

                var tempProducts = _catalogLogic.GetProductsByIds(catalogInfo.BranchId, batch);

                products.Products.AddRange(tempProducts.Products);
                totalProcessed += 50;
            }


            var productHash = products.Products.GroupBy(p => p.ItemNumber)
                                               .Select(i => i.First())
                                               .ToDictionary(p => p.ItemNumber);
            List<ItemHistory> itemStatistics = _itemHistoryRepo.Read(f => f.BranchId.Equals(catalogInfo.BranchId, StringComparison.InvariantCultureIgnoreCase) && 
                                                                          f.CustomerNumber.Equals(catalogInfo.CustomerId))
                                                               .ToList();
            Parallel.ForEach(list.Items, listItem => {
                var prod = productHash.ContainsKey(listItem.ItemNumber) ? productHash[listItem.ItemNumber] : null;

                if(prod != null) {
                    listItem.IsValid = true;
                    listItem.Name = prod.Name;
                    listItem.Pack = prod.Pack;
                    listItem.PackSize = string.Format("{0} / {1}", prod.Pack, prod.Size);
                    listItem.BrandExtendedDescription = prod.BrandExtendedDescription;
                    listItem.Description = prod.Description;
                    listItem.Brand = prod.BrandExtendedDescription;
                    listItem.ReplacedItem = prod.ReplacedItem;
                    listItem.ReplacementItem = prod.ReplacementItem;
                    listItem.NonStock = prod.NonStock;
                    listItem.ChildNutrition = prod.ChildNutrition;
                    listItem.SellSheet = prod.SellSheet;
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
                    listItem.TempZone = prod.TempZone;
                    listItem.IsSpecialtyCatalog = prod.CatalogId.StartsWith("UNFI", StringComparison.InvariantCultureIgnoreCase);
                    listItem.Unfi = prod.Unfi;
                    if(prod.Nutritional != null) {
                        listItem.StorageTemp = prod.Nutritional.StorageTemp;
                        listItem.Nutritional = new Nutritional() {
                            CountryOfOrigin = prod.Nutritional.CountryOfOrigin,
                            GrossWeight = prod.Nutritional.GrossWeight,
                            HandlingInstructions = prod.Nutritional.HandlingInstructions,
                            Height = prod.Nutritional.Height,
                            Length = prod.Nutritional.Length,
                            Ingredients = prod.Nutritional.Ingredients,
                            Width = prod.Nutritional.Width
                        };
                    }
                    listItem.ItemStatistics = new KeithLink.Svc.Core.Models.Customers.ItemHistoryModel() {
                        CaseAverage = itemStatistics.Where(f => f.ItemNumber.Equals(listItem.ItemNumber) && 
                                                                f.UnitOfMeasure.Equals("C"))
                                                    .Select(p => p.AverageUse)
                                                    .FirstOrDefault(),
                        PackageAverage = itemStatistics.Where(f => f.ItemNumber.Equals(listItem.ItemNumber) && 
                                                                   f.UnitOfMeasure.Equals("P"))
                                                       .Select(p => p.AverageUse)
                                                       .FirstOrDefault()
                    };
                }

            });
        }

        private void MarkFavoritesAndAddNotes(UserProfile user, ListModel list, UserSelectedContext catalogInfo) {
            if(list.Items == null || list.Items.Count == 0)
                return;

            var notes = _listRepo.Read(l => l.CustomerId.Equals(catalogInfo.CustomerId) &&
                                            l.BranchId.Equals(catalogInfo.BranchId, StringComparison.CurrentCultureIgnoreCase) &&
                                            l.Type == ListType.Notes,
                                       i => i.Items)
                                 .FirstOrDefault();
            var favorites = _listRepo.Read(l => l.UserId == user.UserId &&
                                                l.CustomerId.Equals(catalogInfo.CustomerId) &&
                                                l.BranchId.Equals(catalogInfo.BranchId, StringComparison.CurrentCultureIgnoreCase) &&
                                                l.Type == ListType.Favorite,
                                           i => i.Items)
                                     .FirstOrDefault();

            var notesHash = new Dictionary<string, ListItem>();
            var favHash = new Dictionary<string, ListItem>();

            if(notes != null && 
               notes.Items != null)
                notesHash = notes.Items
                                 .GroupBy(i => i.ItemNumber)
                                 .ToDictionary(n => n.Key, n => n.First());
            if(favorites != null && 
               favorites.Items != null)
                favHash = favorites.Items
                                   .GroupBy(i => i.ItemNumber)
                                   .ToDictionary(f => f.Key, f => f.First());

            Parallel.ForEach(list.Items, listItem => {
                listItem.Favorite = favHash.ContainsKey(listItem.ItemNumber);
                listItem.Notes = notesHash.ContainsKey(listItem.ItemNumber) ? notesHash[listItem.ItemNumber].Note : null;
            });
        }

        private void PopulateProductDetails(UserSelectedContext catalogInfo, List<RecentItem> returnList) {
            if(returnList == null)
                return;

            var products = _catalogLogic.GetProductsByIds(catalogInfo.BranchId, 
                                                          returnList.Select(i => i.ItemNumber)
                                                                    .Distinct()
                                                                    .ToList());
            returnList.ForEach(delegate (RecentItem item) {
                var product = products.Products.Where(p => p.ItemNumber.Equals(item.ItemNumber))
                                               .FirstOrDefault();
                if(product != null) {
                    item.Name = product.Name;
                }
            });
        }

        private void PopulateProductDetails(UserSelectedContext catalogInfo, List<RecentNonBEKItem> returnList)
        {
            if (returnList == null)
                return;

            var products = _catalogLogic.GetProductsByIds(returnList[0].CatalogId,
                                                          returnList.Select(i => i.ItemNumber)
                                                                    .Distinct()
                                                                    .ToList());
            returnList.ForEach(delegate (RecentNonBEKItem item) {
                var product = products.Products.Where(p => p.ItemNumber.Equals(item.ItemNumber))
                                               .FirstOrDefault();
                if (product != null)
                {
                    item.Name = product.Name;
                    item.Upc = product.UPC;
                }
            });
        }

        //public List<string> ReadFavorites(UserProfile user, UserSelectedContext catalogInfo) {
        //    var list = _listRepo.Read(l => l.UserId == user.UserId && l.CustomerId.Equals(catalogInfo.CustomerId) && l.Type == ListType.Favorite, i => i.Items).ToList();

        //    if(list == null)
        //        return null;

        //    return list.SelectMany(i => i.Items.Select(x => x.ItemNumber)).ToList();
        //}

        /// <summary>
        /// read a list 
        /// </summary>
        /// <param name="user">the user that the list belongs to</param>
        /// <param name="catalogInfo">the customer information that the list belongs to</param>
        /// <param name="Id">the unique identifier of the list</param>
        /// <param name="includePrice">return prices with the list</param>
        /// <returns>ListModel with header and detail info</returns>
        public ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, long Id, bool includePrice = true) {
            ListModel listClone = null;

            var cachedList = _cache.GetItem<ListModel>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", Id));
            if (cachedList != null)
            {
                listClone = cachedList.Clone();

                MarkFavoritesAndAddNotes(user, listClone, catalogInfo);

                var sharedlist = _listRepo.Read(l => l.Id.Equals(Id), i => i.Items)
                                          .FirstOrDefault();

                listClone.IsSharing = sharedlist.Shares.Any() &&
                                       sharedlist.CustomerId.Equals(catalogInfo.CustomerId) &&
                                       sharedlist.BranchId.Equals(catalogInfo.BranchId, StringComparison.InvariantCultureIgnoreCase);
                listClone.IsShared = !sharedlist.CustomerId.Equals(catalogInfo.CustomerId);
            }
            else
            {
                var list = _listRepo.Read(l => l.Id.Equals(Id), i => i.Items)
                                    .FirstOrDefault();

                if (list == null)
                    return null;

                var returnList = list.ToListModel(catalogInfo);

                LookupProductDetails(user, returnList, catalogInfo);
                _cache.AddItem<ListModel>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", Id), TimeSpan.FromHours(2), returnList);

                listClone = returnList.Clone();

                MarkFavoritesAndAddNotes(user, listClone, catalogInfo);
            }

            if (includePrice)
                LookupPrices(user, listClone.Items, catalogInfo);

            if (listClone.Type == ListType.Worksheet | // a.k.a. History
                listClone.Type == ListType.Contract |
                listClone.Type == ListType.RecommendedItems |
                listClone.Type == ListType.Mandatory)
            {
                ApplyDefaultListSort(user, listClone);
            }

            return listClone;
        }

        private void ApplyDefaultListSort(UserProfile user, ListModel listClone)
        {
            string setting = _settingsRepo.ReadByUser(user.UserId)
                                          .Where(s => s.Key.Equals("sortpreferences", StringComparison.CurrentCultureIgnoreCase))
                                          .Select(s => s.Value)
                                          .FirstOrDefault();
            if (setting != null && setting.IndexOf("lis", StringComparison.CurrentCultureIgnoreCase) > -1)
            {
                setting = setting.Substring(setting.IndexOf("lis", StringComparison.CurrentCultureIgnoreCase) + 3);
                if (setting.IndexOf("ato", StringComparison.CurrentCultureIgnoreCase) > -1)
                {
                    setting = setting.Substring(0, setting.IndexOf("ato", StringComparison.CurrentCultureIgnoreCase));
                }
                if (setting.Length > 0) // if user has setup a default sort before and deleted it, or has no default sort yet, we
                                        // need this if to handle that
                {
                    setting = ListSortHelper.GetSort(setting);
                    listClone.Items = ListSortHelper.SortOrderItems(setting, listClone.Items);
                }
            }
        }

        public List<ListModel> ReadListByType(UserProfile user, UserSelectedContext catalogInfo, ListType type, bool headerOnly = false) {
            var list = _listRepo.ReadListForCustomer(catalogInfo, headerOnly)
                                .Where(l => l.Type == type && 
                                            l.CustomerId.Equals(catalogInfo.CustomerId) && 
                                            l.BranchId.Equals(catalogInfo.BranchId, StringComparison.InvariantCultureIgnoreCase))
                                .ToList();

            if(list == null)
                return null;

            if(headerOnly)
                return list.Select(l => new ListModel() {
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
            else {
                var returnList = list.Select(b => b.ToListModel(catalogInfo))
                                     .ToList();

                var processedList = new List<ListModel>();
                //Lookup product details for each item
                returnList.ForEach(delegate (ListModel listItem) {
                    var cachedList = _cache.GetItem<ListModel>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", listItem.ListId));
                    if(cachedList != null) {
                        processedList.Add(cachedList);
                        return;
                    }

                    LookupProductDetails(user, listItem, catalogInfo);
                    processedList.Add(listItem);
                    _cache.AddItem<ListModel>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", listItem.ListId), TimeSpan.FromHours(2), listItem);

                });

                foreach(var tempList in processedList)
                    LookupPrices(user, tempList.Items, catalogInfo);

                return processedList;
            }
        }

        public List<string> ReadListLabels(UserProfile user, UserSelectedContext catalogInfo) {
            return _listRepo.Read(l => l.CustomerId.Equals(catalogInfo.CustomerId) &&
                                       l.BranchId.Equals(catalogInfo.BranchId, StringComparison.InvariantCultureIgnoreCase) && 
                                       (l.Type == ListType.Custom || 
                                        l.Type == ListType.Favorite), 
                                  i => i.Items)
                            .SelectMany(i => i.Items.Where(l => !string.IsNullOrEmpty(l.Label))
                                                    .Select(b => b.Label))
                            .Distinct()
                            .ToList();
        }

        //public List<ListItemModel> ReadNotes(UserProfile user, UserSelectedContext catalogInfo) {
        //    var notes = _listRepo.Read(l => l.CustomerId.Equals(catalogInfo.CustomerId, StringComparison.CurrentCultureIgnoreCase) && 
        //                                    l.BranchId.Equals(catalogInfo.BranchId) && 
        //                                    l.Type == ListType.Notes, 
        //                                i => i.Items)
        //                         .FirstOrDefault();

        //    if(notes == null) {
        //        return new List<ListItemModel>();
        //    } else {
        //        return notes.Items.Select(x => new ListItemModel() { ItemNumber = x.ItemNumber, Notes = x.Note }).ToList();
        //    }
        //}

        public PagedListModel ReadPagedList(UserProfile user, UserSelectedContext catalogInfo, long Id, Core.Models.Paging.PagingModel paging) {
            var cachedList = _cache.GetItem<ListModel>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", Id));
            if(cachedList != null) {
                var cachedReturnList = cachedList.ShallowCopy();

                MarkFavoritesAndAddNotes(user, cachedReturnList, catalogInfo);

                var sharedlist = _listRepo.Read(l => l.Id.Equals(Id)).FirstOrDefault();

                cachedReturnList.IsSharing = sharedlist.Shares.Any() && 
                                             sharedlist.CustomerId.Equals(catalogInfo.CustomerId) && 
                                             sharedlist.BranchId.Equals(catalogInfo.BranchId, StringComparison.InvariantCultureIgnoreCase);
                cachedReturnList.IsShared = !sharedlist.CustomerId.Equals(catalogInfo.CustomerId);

                var cachedPagedList = ToPagedList(paging, cachedReturnList);
                LookupPrices(user, cachedPagedList.Items.Results, catalogInfo);

                return cachedPagedList;
            }

            var list = _listRepo.Read(l => l.Id.Equals(Id), l => l.Items)
                                .FirstOrDefault(); // Not returned catalog ID here


            if(list == null)
                return null;
            var tempList = list.ToListModel(catalogInfo);

            LookupProductDetails(user, tempList, catalogInfo);
            _cache.AddItem<ListModel>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", Id), TimeSpan.FromHours(2), tempList);

            var returnList = tempList.ShallowCopy();

            MarkFavoritesAndAddNotes(user, returnList, catalogInfo);



            var pagedList = ToPagedList(paging, returnList);

            LookupPrices(user, pagedList.Items.Results, catalogInfo);


            return pagedList;
        }

        public List<RecentItem> ReadRecent(UserProfile user, UserSelectedContext catalogInfo) {
            var list = _listRepo.Read(i => i.UserId == user.UserId && 
                                           i.Type == ListType.Recent && 
                                           i.CustomerId.Equals(catalogInfo.CustomerId), 
                                      l => l.Items);
            var returnItems = list.SelectMany(i => i.Items.Select(l => new RecentItem() { ItemNumber = l.ItemNumber, ModifiedOn = l.ModifiedUtc }))
                                  .ToList();

            PopulateProductDetails(catalogInfo, returnItems);

            returnItems.ForEach(delegate (RecentItem item) {
                item.Images = _productImageRepo.GetImageList(item.ItemNumber).ProductImages;
            });

            return returnItems.OrderByDescending(l => l.ModifiedOn)
                              .ToList();
        }

        public RecentNonBEKList ReadRecentOrder(UserProfile user, UserSelectedContext catalogInfo)
        {
            try
            {
                var list = _listRepo.Read(i => i.UserId == user.UserId &&
                                          i.Type == ListType.RecentOrderedNonBEK &&
                                          i.BranchId.Equals(catalogInfo.BranchId) &&
                                          i.CustomerId.Equals(catalogInfo.CustomerId),
                                          l => l.Items).ToList();
                List<RecentNonBEKItem> returnItems = list.SelectMany(i => i.Items
                    .Select(l => new RecentNonBEKItem()
                    {
                        ItemNumber = l.ItemNumber,
                        CatalogId = l.CatalogId,
                        ModifiedOn = l.ModifiedUtc
                    }))
                    .ToList();

                PopulateProductDetails(catalogInfo, returnItems);

                returnItems.ForEach(delegate (RecentNonBEKItem item)
                {
                    item.Images = _productImageRepo.GetImageList(item.Upc, false).ProductImages;
                });

                return new RecentNonBEKList() { Catalog = catalogInfo.BranchId, Items = returnItems };
            }catch (Exception ex)
            {
                DeleteRecentlyOrdered(user, catalogInfo);
                _log.WriteInformationLog(" Getting recently ordered items list failed, reset list", ex);
                return null;
            }
        }

        public List<RecommendedItemModel> ReadRecommendedItemsList(UserSelectedContext catalogInfo) {
            var list = _listRepo.Read(l => l.Type == ListType.RecommendedItems && l.CustomerId.Equals(catalogInfo.CustomerId) && l.BranchId.Equals(catalogInfo.BranchId)).FirstOrDefault();

            if(list == null || list.Items == null)
                return new List<RecommendedItemModel>();

            var returnItems = list.Items.Where(i => (i.FromDate == null || i.FromDate <= DateTime.Now) && (i.ToDate == null || i.ToDate >= DateTime.Now)).Select(r => new RecommendedItemModel() { ItemNumber = r.ItemNumber }).ToList();

            var products = _catalogLogic.GetProductsByIds(catalogInfo.BranchId, returnItems.Select(i => i.ItemNumber).Distinct().ToList());

            returnItems.ForEach(delegate (RecommendedItemModel item) {
                var product = products.Products.Where(p => p.ItemNumber.Equals(item.ItemNumber)).FirstOrDefault();
                if(product != null) {
                    item.Name = product.Name;
                }
            });

            returnItems.ForEach(delegate (RecommendedItemModel item) {
                item.Images = _productImageRepo.GetImageList(item.ItemNumber).ProductImages;
            });

            return returnItems;
        }

        public List<ListModel> ReadReminders(UserProfile user, UserSelectedContext catalogInfo) {
            var list = _listRepo.Read(l => l.CustomerId.Equals(catalogInfo.CustomerId) && 
                                           l.BranchId.Equals(catalogInfo.BranchId) && 
                                           (l.Type == ListType.Reminder || 
                                            l.Type == ListType.Mandatory), 
                                      i => i.Items)
                                .ToList();
            if(list == null)
                return null;

            var returnList = list.Select(b => b.ToListModel(catalogInfo))
                                 .ToList();

            returnList.ForEach(delegate (ListModel listItem) {
                LookupProductDetails(user, listItem, catalogInfo);
                LookupPrices(user, listItem.Items, catalogInfo);
            });

            return returnList;
        }

        public List<ListModel> ReadUserList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false) {
            var list = ReadListForCustomer(user, catalogInfo, headerOnly);

            if(list == null)
                return null;

            if(!list.Where(l => l.Type.Equals(ListType.Favorite)).Any()) {
                this.CreateList(user.UserId, catalogInfo, new ListModel() { Name = "Favorites", BranchId = catalogInfo.BranchId }, ListType.Favorite);
                list = ReadListForCustomer(user, catalogInfo, headerOnly);
            }

            if(!list.Where(l => l.Type.Equals(ListType.Reminder)).Any()) {
                this.CreateList(user.UserId, catalogInfo, new ListModel() { Name = "Reminder", BranchId = catalogInfo.BranchId }, ListType.Reminder);
                list = ReadListForCustomer(user, catalogInfo, headerOnly);
            }

            if(headerOnly)
                return list.Select(l => new ListModel() {
                                            ListId = l.Id,
                                            Name = l.DisplayName,
                                            IsContractList = l.Type == ListType.Contract,
                                            IsFavorite = l.Type == ListType.Favorite,
                                            IsWorksheet = l.Type == ListType.Worksheet,
                                            IsReminder = l.Type == ListType.Reminder,
                                            IsMandatory = l.Type == ListType.Mandatory,
                                            IsRecommended = l.Type == ListType.RecommendedItems,
                                            ReadOnly = l.ReadOnly || (!user.IsInternalUser && l.Type.Equals(ListType.RecommendedItems) || (!user.IsInternalUser && l.Type.Equals(ListType.Mandatory))),
                                            SharedWith = l.Shares.Select(s => s.CustomerId).ToList(),
                                            IsSharing = l.Shares.Any() && l.CustomerId.Equals(catalogInfo.CustomerId) && l.BranchId.Equals(catalogInfo.BranchId),
                                            IsShared = !l.CustomerId.Equals(catalogInfo.CustomerId)})
                           .ToList();
            else {
                var returnList = list.Select(b => b.ToListModel(catalogInfo))
                                     .ToList();

                var processedList = new List<ListModel>();
                //Lookup product details for each item
                returnList.ForEach(delegate (ListModel listItem) {
                    var cachedList = _cache.GetItem<ListModel>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", listItem.ListId));
                    if(cachedList != null) {
                        processedList.Add(cachedList);
                        return;
                    }

                    LookupProductDetails(user, listItem, catalogInfo);
                    processedList.Add(listItem);
                    _cache.AddItem<ListModel>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", listItem.ListId), TimeSpan.FromHours(2), listItem);

                });
                //Mark favorites and add notes
                processedList.ForEach(delegate (ListModel listItem) {
                    MarkFavoritesAndAddNotes(user, listItem, catalogInfo);
                });

                return processedList;
            }
        }

        private IEnumerable<List> ReadListForCustomer(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly) {
            if(String.IsNullOrEmpty(catalogInfo.CustomerId))
                return new List<List>();

            var list = _listRepo.ReadListForCustomer(catalogInfo, headerOnly)
                                .Where(l => l.Type == ListType.Custom ||
                                            (l.UserId == user.UserId && 
                                             l.Type == ListType.Favorite) || 
                                            l.Type == ListType.Contract || 
                                            l.Type == ListType.Worksheet || 
                                            l.Type == ListType.ContractItemsAdded || 
                                            l.Type == ListType.ContractItemsDeleted || 
                                            l.Type == ListType.Reminder || 
                                            l.Type == ListType.RecommendedItems || 
                                            l.Type == ListType.Mandatory);
            return list;
        }

        public void ShareList(ListCopyShareModel shareListModel) {
            var listToShare = _listRepo.ReadById(shareListModel.ListId);

            if(listToShare == null)
                return;

            foreach(var customer in shareListModel.Customers) {
                if(!listToShare.Shares.Any(s => s.CustomerId.Equals(customer.CustomerNumber) && 
                                                s.BranchId.Equals(customer.CustomerBranch, StringComparison.InvariantCultureIgnoreCase)))
                    listToShare.Shares.Add(new ListShare() { CustomerId = customer.CustomerNumber, BranchId = customer.CustomerBranch });
            }

            var itemsToRemove = listToShare.Shares.Where(l => !shareListModel.Customers.Any(c => c.CustomerNumber.Equals(l.CustomerId) && 
                                                                                                 c.CustomerBranch.Equals(l.BranchId)))
                                                  .Select(l => l)
                                                  .ToList();

            foreach(var item in itemsToRemove)
                _listShareRepo.Delete(item);

            _listRepo.Update(listToShare);
            _uow.SaveChanges();

            var cachedList = _cache.GetItem<ListModel>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", listToShare.Id));
            if(cachedList != null) {
                cachedList.SharedWith = listToShare.Shares.Select(s => s.CustomerId)
                                                          .ToList();
                _cache.AddItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", listToShare.Id), TimeSpan.FromHours(2), cachedList);
            }
        }

        private PagedListModel ToPagedList(PagingModel paging, ListModel returnList) {
            var pagedList = new PagedListModel() {
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

            if(returnList.Items != null)
                pagedList.Items = returnList.Items.AsQueryable<ListItemModel>().GetPage<ListItemModel>(paging, "Position");

            return pagedList;
        }

        /// <summary>
        /// update the list item in the database and update cache for the item
        /// </summary>
        /// <param name="item"></param>
        public void UpdateItem(ListItemModel item) {
            _listItemRepo.Update(new ListItem() {
                Id = item.ListItemId,
                ItemNumber = item.ItemNumber,
                Label = item.Label,
                Par = item.ParLevel,
                Position = item.Position,
                Each = item.Each
            });
            _uow.SaveChanges();

            var updatedItem = _listItemRepo.Read(i => i.Id.Equals(item.ListItemId), l => l.ParentList)
                                           .FirstOrDefault();

            if(updatedItem != null && updatedItem.ParentList != null) {
                _cache.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", updatedItem.ParentList.Id)); //Invalidate cache
            }
        }

        /// <summary>
        /// update an entire list
        /// </summary>
        /// <param name="userList"></param>
        public void UpdateList(ListModel userList) {
            bool itemsAdded = false;
            var currentList = _listRepo.Read(l => l.Id.Equals(userList.ListId), i => i.Items)
                                       .FirstOrDefault();

            if(currentList == null)
                return;

            currentList.DisplayName = userList.Name;

            if(userList.Items == null)
                userList.Items = new List<ListItemModel>();

            if(currentList.Items == null && 
               userList.Items != null)
                currentList.Items = new List<ListItem>();

            //if contract of history, replace all items with new items
            if(userList.Type == ListType.Worksheet || 
               userList.Type == ListType.Contract) {
                foreach(var li in currentList.Items.ToList()) {
                    _listItemRepo.Delete(li);
                }

                foreach(var li in userList.Items.ToList()) {
                    currentList.Items.Add(new ListItem() { ItemNumber = li.ItemNumber, Par = li.ParLevel, Label = li.Label, Each = li.Each });
                }
            } else {
                foreach(var updateItem in userList.Items) {
                    if(updateItem.IsDelete) {
                        var itemToDelete = currentList.Items.Where(i => i.Id.Equals(updateItem.ListItemId))
                                                            .FirstOrDefault();
                        if(itemToDelete != null)
                            _listItemRepo.Delete(itemToDelete);
                    } else {
                        if(string.IsNullOrEmpty(updateItem.ItemNumber))
                            continue;

                        if(updateItem.ListItemId != 0) {
                            var item = currentList.Items.Where(i => i.Id.Equals(updateItem.ListItemId))
                                                        .FirstOrDefault();
                            item.ItemNumber = updateItem.ItemNumber;
                            item.Label = updateItem.Label;
                            item.Par = updateItem.ParLevel;
                            item.Position = updateItem.Position;
                            item.Each = updateItem.Each;
                            item.Quantity = updateItem.Quantity;
                        } else {
                            if((currentList.Type == ListType.Favorite || 
                                currentList.Type == ListType.Reminder) &&
                               currentList.Items.Where(i => i.ItemNumber.Equals(updateItem.ItemNumber))
                                                .Any()) {
                                continue;
                            }
                            currentList.Items.Add(new ListItem() {
                                ItemNumber = updateItem.ItemNumber,
                                Par = updateItem.ParLevel,
                                Label = updateItem.Label,
                                Each = updateItem.Each,
                                Quantity = updateItem.Quantity
                            });
                            itemsAdded = true;
                        }
                    }
                }
            }

            _uow.SaveChanges();

            if(currentList.Type == ListType.RecommendedItems && 
                                   itemsAdded)
                GenerateNewRecommendItemNotification(currentList.CustomerId, currentList.BranchId); //Send a notification that new recommended items have been added

            _cache.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, string.Format("UserList_{0}", currentList.Id)); //Invalidate cache
        }




        public Stream BuildReportFromList(PrintListModel options, long listId, UserSelectedContext userContext, UserProfile userProfile) {
            //if(!string.IsNullOrEmpty(options.Paging.Terms)) {
            //    //Build filter
            //    options.Paging.Filter = new FilterInfo() {
            //        Field = "ItemNumber",
            //        FilterType = "contains",
            //        Value = options.Paging.Terms,
            //        Condition = "||",
            //        Filters = new List<FilterInfo>() { new FilterInfo() { Condition = "||", Field = "Label", Value = options.Paging.Terms, FilterType = "contains" },
            //            new FilterInfo() { Condition = "||", Field = "Name", Value = options.Paging.Terms, FilterType = "contains" } }
            //    };
            //}

            //options.Paging.Size = int.MaxValue;
            //options.Paging.From = 0;

            //if(options.Paging.Sort.Count == 1 && options.Paging.Sort[0].Field == null) {
            //    options.Paging.Sort = new List<SortInfo>();
            //}

            //ListModel list = ReadList(userProfile, userContext, listId, true);

            //if(list == null)
            //    return null;

            //StringBuilder sortinfo = new StringBuilder();
            //foreach(SortInfo si in options.Paging.Sort) {
            //    if(sortinfo.Length > 0)
            //        sortinfo.Append(";");
            //    sortinfo.Append(si.Field);
            //    sortinfo.Append(",");
            //    sortinfo.Append(si.Order);
            //}
            //list.Items = SortOrderItems(sortinfo.ToString(), list.Items);
            //int ind = 1;
            //foreach(ListItemModel item in list.Items) {
            //    item.Position = ind++;
            //}

            //ListReportModel printModel = list.ToReportModel();

            //ReportViewer rv = new ReportViewer();
            //rv.ProcessingMode = ProcessingMode.Local;
            //string deviceInfo = KeithLink.Svc.Core.Constants.SET_REPORT_SIZE_LANDSCAPE;
            //Assembly assembly = Assembly.Load("Keithlink.Svc.Impl");
            //// HACK for dynamically changing column widths doesn't work in run-time reportviewer.  choosing from multiple reports.
            //string rptName = ChooseReportFromOptions(options, userContext);
            //_log.WriteInformationLog("ListServiceRepositoryImpl.BuildReportFromList rptName=" + rptName);
            //Stream rdlcStream = assembly.GetManifestResourceStream(rptName);
            //_log.WriteInformationLog("ListServiceRepositoryImpl.BuildReportFromList rdlcStream=" + rdlcStream.ToString());
            //rv.LocalReport.LoadReportDefinition(rdlcStream);
            //rv.LocalReport.SetParameters(MakeReportOptionsForPrintListReport(options, printModel.Name, userContext));
            //GatherInfoAboutItems(listId, options, printModel, userContext, userProfile);
            //rv.LocalReport.DataSources.Add(new ReportDataSource("ListItems", printModel.Items));
            //byte[] bytes = rv.LocalReport.Render("PDF", deviceInfo);
            //Stream stream = new MemoryStream(bytes);
            //return stream;

            return null;
        }

        ///// <summary>
        ///// Sort list items given an unparsed string with sort information
        ///// </summary>
        ///// <param name="sortinfo">A string with unparsed sort info for the list items (of the form "fld1,ord1[;fld2,ord2]")</param>
        ///// <param name="items">A list of shopping cart items</param>
        ///// <returns>A list of shopping cart items in the order described by sortinfo with position calibrated to that sort</returns>
        //private List<ListItemModel> SortOrderItems(string sortinfo, List<ListItemModel> items) {
        //    IQueryable<ListItemModel> stmt = items.AsQueryable();
        //    string[] sortpairs = sortinfo.Split(';');
        //    int ind = 0;
        //    foreach(string sortpair in sortpairs) {
        //        string fld = sortpair.Substring(0, sortpair.IndexOf(','));
        //        string ord = sortpair.Substring(sortpair.IndexOf(',') + 1);
        //        stmt = stmt.OrderingHelper(fld, ord.Equals("desc", StringComparison.CurrentCultureIgnoreCase), ind > 0);
        //        ind++;
        //    }
        //    return stmt.ToList();
        //}

        //private string ChooseReportFromOptions(PrintListModel options, UserSelectedContext userContext) { // Choose different Report for different columns ; grouping doesn't change column widths so no different name
        //    Customer customer = _profileLogic.GetCustomerByCustomerNumber(userContext.CustomerId, userContext.BranchId);
        //    if(customer != null) {
        //        if((options.ShowParValues) & (customer.CanViewPricing) & (options.ShowNotes))
        //            return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_YesParYesPriceYesNotes;
        //        else if((options.ShowParValues) & (customer.CanViewPricing) & (options.ShowNotes == false))
        //            return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_YesParYesPriceNoNotes;
        //        else if((options.ShowParValues) & (customer.CanViewPricing == false) & (options.ShowNotes))
        //            return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_YesParNoPriceYesNotes;
        //        else if((options.ShowParValues) & (customer.CanViewPricing == false) & (options.ShowNotes == false))
        //            return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_YesParNoPriceNoNotes;
        //        else if((options.ShowParValues == false) & (customer.CanViewPricing) & (options.ShowNotes))
        //            return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_NoParYesPriceYesNotes;
        //        else if((options.ShowParValues == false) & (customer.CanViewPricing) & (options.ShowNotes == false))
        //            return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_NoParYesPriceNoNotes;
        //        else if((options.ShowParValues == false) & (customer.CanViewPricing == false) & (options.ShowNotes))
        //            return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_NoParNoPriceYesNotes;
        //        else if((options.ShowParValues == false) & (customer.CanViewPricing == false) & (options.ShowNotes == false))
        //            return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_NoParNoPriceNoNotes;
        //    }
        //    return KeithLink.Svc.Core.Constants.REPORT_PRINTLIST_NoParNoPriceNoNotes;
        //}

        //private ReportParameter[] MakeReportOptionsForPrintListReport(PrintListModel options, string listName, UserSelectedContext userContext) {
        //    Customer customer = _profileLogic.GetCustomerByCustomerNumber(userContext.CustomerId, userContext.BranchId);
        //    ReportParameter[] parameters = new ReportParameter[5];
        //    parameters[0] = new ReportParameter("ListName", customer.CustomerName + ", " + listName);
        //    parameters[1] = new ReportParameter("ShowNotes", options.ShowNotes.ToString());
        //    parameters[2] = new ReportParameter("ShowPar", options.ShowParValues.ToString());
        //    bool groupbylabel = false;
        //    if((options.Paging != null) && (options.Paging.Sort != null) && (options.Paging.Sort.Count > 0) &&
        //        (options.Paging.Sort[0].Field.Equals("label", StringComparison.CurrentCultureIgnoreCase))) {
        //        groupbylabel = true;
        //    }
        //    parameters[3] = new ReportParameter("GroupByLabel", (groupbylabel).ToString());
        //    parameters[4] = new ReportParameter("ShowPrices", customer.CanViewPricing.ToString());
        //    return parameters;
        //}

        //private void GatherInfoAboutItems(long listId, PrintListModel options, ListReportModel printModel, UserSelectedContext userContext, UserProfile userProfile) {
        //    Customer customer = _profileLogic.GetCustomerByCustomerNumber(userContext.CustomerId, userContext.BranchId);
        //    ListModel listModel = ReadList(userProfile, userContext, listId, true);
        //    List<ListItemModel> itemHash = listModel.Items.ToList();
        //    string[] itemkeys = itemHash.Select(i => i.ItemNumber).ToArray();
        //    ItemHistory[] itemHistories = serviceClient.GetItemsHistoryList(userContext, itemkeys);
        //    foreach(ListItemReportModel item in printModel.Items) {
        //        var itemInfo = itemHash.Where(i => i.ItemNumber == item.ItemNumber).FirstOrDefault();
        //        if((customer != null) && (customer.CanViewPricing)) {
        //            StringBuilder priceInfo = new StringBuilder();
        //            if(itemInfo != null) {
        //                if((itemInfo.PackagePrice != null) && (itemInfo.PackagePrice.Equals("0.00") == false)) {
        //                    priceInfo.Append("$");
        //                    priceInfo.Append(itemInfo.PackagePrice);
        //                    priceInfo.Append("/Pack");
        //                    item.Price = priceInfo.ToString();
        //                    priceInfo.Append(" - ");
        //                }
        //                if(itemInfo.CasePrice != null) {
        //                    priceInfo.Append("$");
        //                    priceInfo.Append(itemInfo.CasePrice);
        //                    priceInfo.Append("/Case");
        //                    item.Price = priceInfo.ToString();
        //                }
        //            }
        //        }
        //        // to make the option not to sort by label not reorder the items we null the label
        //        if((options.Paging != null) && (options.Paging.Sort != null) && (options.Paging.Sort.Count > 0) &&
        //            (options.Paging.Sort[0].Field.Equals("label", StringComparison.CurrentCultureIgnoreCase))) {
        //            item.Label = itemInfo.Label;
        //        } else {
        //            item.Label = null;
        //        }
        //        ItemHistory itemStats = itemHistories.Where(f => f.ItemNumber == item.ItemNumber).FirstOrDefault();
        //        if(itemStats != null) {
        //            StringBuilder AVG8WK = new StringBuilder();
        //            AVG8WK.Append(itemStats.AverageUse);
        //            if(itemStats.UnitOfMeasure.Equals(KeithLink.Svc.Core.Constants.ITEMHISTORY_AVERAGEUSE_PACKAGE))
        //                AVG8WK.Append(" Pack");
        //            else if(itemStats.UnitOfMeasure.Equals(KeithLink.Svc.Core.Constants.ITEMHISTORY_AVERAGEUSE_CASE))
        //                AVG8WK.Append(" Case");
        //            if((itemStats.AverageUse > 1) | (itemStats.AverageUse == 0))
        //                AVG8WK.Append("s");
        //            item.AvgUse = AVG8WK.ToString();
        //        } else {
        //            item.AvgUse = "0 Cases";
        //        }
        //    }
        //}

        #endregion
    }
}
