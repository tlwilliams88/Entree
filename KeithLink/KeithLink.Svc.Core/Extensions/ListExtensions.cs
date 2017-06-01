using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Enumerations.List;

using KeithLink.Svc.Core.Helpers;

using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.History;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Lists.Contract;
using KeithLink.Svc.Core.Models.Lists.Favorites;
using KeithLink.Svc.Core.Models.Lists.MandatoryItem;
using KeithLink.Svc.Core.Models.Lists.Notes;
using KeithLink.Svc.Core.Models.Lists.RecentlyViewed;
using KeithLink.Svc.Core.Models.Lists.RecentlyOrdered;
using KeithLink.Svc.Core.Models.Lists.RecommendedItem;
using KeithLink.Svc.Core.Models.Lists.ReminderItem;
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;
using KeithLink.Svc.Core.Models.Lists.CustomList;

namespace KeithLink.Svc.Core.Extensions
{
    public static class ListExtensions
    {
        #region methods
        /// <summary>
        /// convert the ListModel to a List object
        /// </summary>
        /// <param name="list">the list model</param>
        /// <returns>List database model</returns>
        public static List ToEFList(this ListModel list)
        {
            return new KeithLink.Svc.Core.Models.EF.List()
            {
                DisplayName = list.Name,
                Type = ListType.Custom,
                ReadOnly = list.ReadOnly,
                Items = list.Items == null ? null : list.Items.Select(i => new KeithLink.Svc.Core.Models.EF.ListItem()
                {
                    Category = i.Category,
                    ItemNumber = i.ItemNumber,
                    Label = i.Label,
                    Par = i.ParLevel,
                    Position = i.Position,
                    Note = i.Notes,
                    Each = i.Each ?? false,
                    Quantity = i.Quantity,
                    CatalogId = i.CatalogId,
                    CustomInventoryItemId = i.CustomInventoryItemId
                }).OrderBy(l => l.Position).ToArray()
            };
        }

        /// <summary>
        /// convert the List database model to a ListModel object
        /// </summary>
        /// <param name="list">the database model</param>
        /// <param name="catalogInfo">the customer information</param>
        /// <returns>ListModel object</returns>
        public static ListModel ToListModel(this List list, UserSelectedContext catalogInfo)
        {
            return new ListModel() {
                BranchId = list.BranchId,
                IsContractList = list.Type == ListType.Contract,
                IsFavorite = list.Type == ListType.Favorite,
                IsWorksheet = list.Type == ListType.Worksheet,
                IsReminder = list.Type == ListType.Reminder,
                IsMandatory = list.Type == ListType.Mandatory,
                IsRecommended = list.Type == ListType.RecommendedItems,
                Type = list.Type,
                SharedWith = list.Shares != null ? list.Shares.Select(s => s.CustomerId).ToList() : null,
                ListId = list.Id,
                Name = list.DisplayName,
                ReadOnly = list.ReadOnly,
                IsSharing = list.Shares != null ? (list.Shares.Any() && list.CustomerId.Equals(catalogInfo.CustomerId) &&
                                                   list.BranchId.Equals(catalogInfo.BranchId, StringComparison.CurrentCultureIgnoreCase))
                                                : false,
                IsShared = !list.CustomerId.Equals(catalogInfo.CustomerId),
                Items = list.Items == null ? null :
                    list.Items.Select(i => new ListItemModel() {
                        Category = i.Category,
                        Type = list.Type,
                        ItemNumber = i.ItemNumber,
                        Label = i.Label,
                        ParLevel = i.Par,
                        ListItemId = i.Id,
                        Position = i.Position,
                        ModifiedUtc = i.ModifiedUtc,
                        CreatedUtc = i.CreatedUtc,
                        Delta = (i.CreatedUtc.AddDays
                            (Constants.CONTENTMGMT_CONTRACTITEMS_THRESHOLD) > DateTime.Now) ? Constants.CONTENTMGMT_CONTRACTITEMS_NEWADDED + 
                                " " + Constants.CONTENTMGMT_CONTRACTITEMS_ACTIVE :
                            (i.ToDate != null && i.ToDate.Value < DateTime.Now) ? Constants.CONTENTMGMT_CONTRACTITEMS_NEWDELETED :
                            Constants.CONTENTMGMT_CONTRACTITEMS_ACTIVE,
                        FromDate = i.FromDate,
                        ToDate = i.ToDate,
                        Each = i.Each ?? false,
                        Quantity = i.Quantity,
                        CatalogId = i.CatalogId,
                        CustomInventoryItemId = i.CustomInventoryItemId.HasValue ? i.CustomInventoryItemId.Value : 0
                    } ).OrderBy( l => l.Position ).ToList()
            };
        }

        public static ListModel ToListModel(this ContractListHeader header, UserSelectedContext catalogInfo)
        {
            return new ListModel()
            {
                BranchId = header.BranchId,
                IsContractList = true,
                IsFavorite = false,
                IsWorksheet = false,
                IsReminder = false,
                IsMandatory = false,
                IsRecommended = false,
                IsCustomInventory = false,
                Type = ListType.Contract,
                ListId = header.Id,
                Name = header.Name,
                ReadOnly = true,
                Items = header.Items == null ? null :
                    header.Items.Select(i => new ListItemModel()
                    {
                        ListItemId = i.Id,
                        Type = ListType.Contract,
                        Category = i.Category,
                        ItemNumber = i.ItemNumber,
                        //        Delta = (i.CreatedUtc.AddDays
                        //            (Constants.CONTENTMGMT_CONTRACTITEMS_THRESHOLD) > DateTime.Now) ? Constants.CONTENTMGMT_CONTRACTITEMS_NEWADDED +
                        //                " " + Constants.CONTENTMGMT_CONTRACTITEMS_ACTIVE :
                        //            (i.ToDate != null && i.ToDate.Value < DateTime.Now) ? Constants.CONTENTMGMT_CONTRACTITEMS_NEWDELETED :
                        //            Constants.CONTENTMGMT_CONTRACTITEMS_ACTIVE,
                        //        FromDate = i.FromDate,
                        //        ToDate = i.ToDate,
                        Position = i.LineNumber,
                        ModifiedUtc = i.ModifiedUtc,
                        CreatedUtc = i.CreatedUtc,
                        Each = i.Each ?? false,
                        CatalogId = i.CatalogId
                    }).OrderBy(l => l.Position).ToList()
            };
        }

        public static ListModel ToListModel(this HistoryListHeader header, UserSelectedContext catalogInfo)
        {
            return new ListModel()
            {
                BranchId = header.BranchId,
                IsContractList = false,
                IsFavorite = false,
                IsWorksheet = true,
                IsReminder = false,
                IsMandatory = false,
                IsRecommended = false,
                IsCustomInventory = false,
                Type = ListType.Worksheet,
                ListId = header.Id,
                Name = header.Name,
                ReadOnly = true,
                Items = header.Items == null ? null :
                    header.Items.Select(i => new ListItemModel()
                    {
                        ListItemId = i.Id,
                        Type = ListType.Worksheet,
                        //        Category = i.Category,
                        ItemNumber = i.ItemNumber,
                        //        Delta = (i.CreatedUtc.AddDays
                        //            (Constants.CONTENTMGMT_CONTRACTITEMS_THRESHOLD) > DateTime.Now) ? Constants.CONTENTMGMT_CONTRACTITEMS_NEWADDED +
                        //                " " + Constants.CONTENTMGMT_CONTRACTITEMS_ACTIVE :
                        //            (i.ToDate != null && i.ToDate.Value < DateTime.Now) ? Constants.CONTENTMGMT_CONTRACTITEMS_NEWDELETED :
                        //            Constants.CONTENTMGMT_CONTRACTITEMS_ACTIVE,
                        //        FromDate = i.FromDate,
                        //        ToDate = i.ToDate,
                        Position = i.LineNumber,
                        ModifiedUtc = i.ModifiedUtc,
                        CreatedUtc = i.CreatedUtc,
                        Each = i.Each ?? false,
                        CatalogId = i.CatalogId
                    }).OrderBy(l => l.Position).ToList()
            };
        }

        public static ListModel ToListModel(this FavoritesListHeader header, UserSelectedContext catalogInfo)
        {
            return new ListModel()
            {
                BranchId = header.BranchId,
                IsContractList = false,
                IsFavorite = true,
                IsWorksheet = false,
                IsReminder = false,
                IsMandatory = false,
                IsRecommended = false,
                IsCustomInventory = false,
                Type = ListType.Favorite,
                ListId = header.Id,
                Name = header.Name,
                ReadOnly = false,
                Items = header.Items == null ? null :
                    header.Items.Select(i => new ListItemModel()
                    {
                        ListItemId = i.Id,
                        //        Category = i.Category,
                        Type = ListType.Favorite,
                        ItemNumber = i.ItemNumber,
                        //        Label = i.Label,
                        //        ParLevel = i.Par,
                        //        ListItemId = i.Id,
                        //        Position = i.LineNumber,
                        ModifiedUtc = i.ModifiedUtc,
                        CreatedUtc = i.CreatedUtc,
                        Each = i.Each ?? false,
                        //        Quantity = i.Quantity,
                        CatalogId = i.CatalogId
                        //        CustomInventoryItemId = i.CustomInventoryItemId.HasValue ? i.CustomInventoryItemId.Value : 0
                    }).OrderBy(l => l.Position).ToList()
            };
        }

        public static ListModel ToListModel(this NotesListHeader header, UserSelectedContext catalogInfo)
        {
            return new ListModel()
            {
                BranchId = header.BranchId,
                IsContractList = false,
                IsFavorite = false,
                IsWorksheet = false,
                IsReminder = false,
                IsMandatory = false,
                IsRecommended = false,
                IsCustomInventory = false,
                Type = ListType.Notes,
                ListId = header.Id,
                Name = header.Name,
                ReadOnly = false,
                Items = header.Items == null ? null :
                    header.Items.Select(i => new ListItemModel()
                    {
                        ListItemId = i.Id,
                        //        Category = i.Category,
                        Type = ListType.Notes,
                        ItemNumber = i.ItemNumber,
                        //        Label = i.Label,
                        //        ParLevel = i.Par,
                        ModifiedUtc = i.ModifiedUtc,
                        CreatedUtc = i.CreatedUtc,
                        Each = i.Each ?? false,
                        Notes = i.Note,
                        //        Quantity = i.Quantity,
                        CatalogId = i.CatalogId
                    }).OrderBy(l => l.Position).ToList()
            };
        }

        public static ListModel ToListModel(this RecommendedItemsListHeader header, UserSelectedContext catalogInfo)
        {
            return new ListModel()
            {
                BranchId = header.BranchId,
                IsContractList = false,
                IsFavorite = false,
                IsWorksheet = false,
                IsReminder = false,
                IsMandatory = false,
                IsRecommended = true,
                IsCustomInventory = false,
                Type = ListType.RecommendedItems,
                ListId = header.Id,
                Name = header.Name,
                ReadOnly = false,
                Items = header.Items == null ? null :
                    header.Items.Select(i => new ListItemModel()
                    {
                        ListItemId = i.Id,
                        //        Category = i.Category,
                        Type = ListType.RecommendedItems,
                        ItemNumber = i.ItemNumber,
                        //        Label = i.Label,
                        //        ParLevel = i.Par,
                        //        Position = i.LineNumber,
                        ModifiedUtc = i.ModifiedUtc,
                        CreatedUtc = i.CreatedUtc,
                        Each = i.Each ?? false,
                        Notes = i.Note,
                        //        Quantity = i.Quantity,
                        CatalogId = i.CatalogId
                    }).OrderBy(l => l.Position).ToList()
            };
        }

        public static ListModel ToListModel(this RecentlyViewedListHeader header, UserSelectedContext catalogInfo)
        {
            return new ListModel()
            {
                BranchId = header.BranchId,
                IsContractList = false,
                IsFavorite = false,
                IsWorksheet = false,
                IsReminder = false,
                IsMandatory = false,
                IsRecommended = false,
                IsCustomInventory = false,
                Type = ListType.Recent,
                ListId = header.Id,
                Name = header.Name,
                ReadOnly = false,
                Items = header.Items == null ? null :
                    header.Items.Select(i => new ListItemModel()
                    {
                        ListItemId = i.Id,
                        //        Category = i.Category,
                        Type = ListType.Recent,
                        ItemNumber = i.ItemNumber,
                        //        Label = i.Label,
                        //        ParLevel = i.Par,
                        //        ListItemId = i.Id,
                        //        Position = i.LineNumber,
                        ModifiedUtc = i.ModifiedUtc,
                        CreatedUtc = i.CreatedUtc,
                        Each = i.Each ?? false,
                        //        Quantity = i.Quantity,
                        CatalogId = i.CatalogId
                        //        CustomInventoryItemId = i.CustomInventoryItemId.HasValue ? i.CustomInventoryItemId.Value : 0
                    }).OrderBy(l => l.Position).ToList()
            };
        }

        public static ListModel ToListModel(this InventoryValuationListHeader header, UserSelectedContext catalogInfo)
        {
            return new ListModel()
            {
                BranchId = header.BranchId,
                IsContractList = false,
                IsFavorite = false,
                IsWorksheet = false,
                IsReminder = false,
                IsMandatory = false,
                IsRecommended = false,
                IsCustomInventory = false,
                Type = ListType.InventoryValuation,
                ListId = header.Id,
                Name = header.Name,
                ReadOnly = false,
                Items = header.Items == null ? null :
                    header.Items.Select(i => new ListItemModel()
                    {
                        ListItemId = i.Id,
                        //        Category = i.Category,
                        Type = ListType.InventoryValuation,
                        ItemNumber = i.ItemNumber,
                        //        Label = i.Label,
                        //        ParLevel = i.Par,
                        //        ListItemId = i.Id,
                        //        Position = i.LineNumber,
                        ModifiedUtc = i.ModifiedUtc,
                        CreatedUtc = i.CreatedUtc,
                        Each = i.Each ?? false,
                        //        Quantity = i.Quantity,
                        CatalogId = i.CatalogId
                        //        CustomInventoryItemId = i.CustomInventoryItemId.HasValue ? i.CustomInventoryItemId.Value : 0
                    }).OrderBy(l => l.Position).ToList()
            };
        }

        public static ListModel ToListModel(this RecentlyOrderedListHeader header, UserSelectedContext catalogInfo)
        {
            return new ListModel()
            {
                BranchId = header.BranchId,
                IsContractList = false,
                IsFavorite = false,
                IsWorksheet = false,
                IsReminder = false,
                IsMandatory = false,
                IsRecommended = false,
                IsCustomInventory = false,
                Type = ListType.RecentOrderedNonBEK,
                //SharedWith = list.Shares != null ? list.Shares.Select(s => s.CustomerId).ToList() : null,
                ListId = header.Id,
                Name = header.Name,
                ReadOnly = true,
                //IsSharing = list.Shares != null ? (list.Shares.Any() && list.CustomerId.Equals(catalogInfo.CustomerId) &&
                //                                   list.BranchId.Equals(catalogInfo.BranchId, StringComparison.CurrentCultureIgnoreCase))
                //                                : false,
                //IsShared = !list.CustomerId.Equals(catalogInfo.CustomerId),
                Items = header.Items == null ? null :
                    header.Items.Select(i => new ListItemModel()
                    {
                        ListItemId = i.Id,
                        //        Category = i.Category,
                        Type = ListType.RecentOrderedNonBEK,
                        ItemNumber = i.ItemNumber,
                        //        Label = i.Label,
                        //        ParLevel = i.Par,
                        //        ListItemId = i.Id,
                        //        Position = i.LineNumber,
                        ModifiedUtc = i.ModifiedUtc,
                        CreatedUtc = i.CreatedUtc,
                        //        Delta = (i.CreatedUtc.AddDays
                        //            (Constants.CONTENTMGMT_CONTRACTITEMS_THRESHOLD) > DateTime.Now) ? Constants.CONTENTMGMT_CONTRACTITEMS_NEWADDED +
                        //                " " + Constants.CONTENTMGMT_CONTRACTITEMS_ACTIVE :
                        //            (i.ToDate != null && i.ToDate.Value < DateTime.Now) ? Constants.CONTENTMGMT_CONTRACTITEMS_NEWDELETED :
                        //            Constants.CONTENTMGMT_CONTRACTITEMS_ACTIVE,
                        //        FromDate = i.FromDate,
                        //        ToDate = i.ToDate,
                        Each = i.Each ?? false,
                        //        Quantity = i.Quantity,
                        CatalogId = i.CatalogId
                        //        CustomInventoryItemId = i.CustomInventoryItemId.HasValue ? i.CustomInventoryItemId.Value : 0
                    }).OrderBy(l => l.Position).ToList()
            };
        }

        public static ListModel ToListModel(this ReminderItemsListHeader header, UserSelectedContext catalogInfo)
        {
            return new ListModel()
            {
                BranchId = header.BranchId,
                IsContractList = false,
                IsFavorite = false,
                IsWorksheet = false,
                IsReminder = true,
                IsMandatory = false,
                IsRecommended = false,
                IsCustomInventory = false,
                Type = ListType.Reminder,
                //SharedWith = list.Shares != null ? list.Shares.Select(s => s.CustomerId).ToList() : null,
                ListId = header.Id,
                Name = header.Name,
                ReadOnly = false,
                //IsSharing = list.Shares != null ? (list.Shares.Any() && list.CustomerId.Equals(catalogInfo.CustomerId) &&
                //                                   list.BranchId.Equals(catalogInfo.BranchId, StringComparison.CurrentCultureIgnoreCase))
                //                                : false,
                //IsShared = !list.CustomerId.Equals(catalogInfo.CustomerId),
                Items = header.Items == null ? null :
                    header.Items.Select(i => new ListItemModel()
                    {
                        ListItemId = i.Id,
                        Type = ListType.Reminder,
                        //        Type = header.Type,
                        ItemNumber = i.ItemNumber,
                        //        Label = i.Label,
                        //        ParLevel = i.Par,
                        //        ListItemId = i.Id,
                        //        Position = i.LineNumber,
                        ModifiedUtc = i.ModifiedUtc,
                        CreatedUtc = i.CreatedUtc,
                        //        Delta = (i.CreatedUtc.AddDays
                        //            (Constants.CONTENTMGMT_CONTRACTITEMS_THRESHOLD) > DateTime.Now) ? Constants.CONTENTMGMT_CONTRACTITEMS_NEWADDED +
                        //                " " + Constants.CONTENTMGMT_CONTRACTITEMS_ACTIVE :
                        //            (i.ToDate != null && i.ToDate.Value < DateTime.Now) ? Constants.CONTENTMGMT_CONTRACTITEMS_NEWDELETED :
                        //            Constants.CONTENTMGMT_CONTRACTITEMS_ACTIVE,
                        //        FromDate = i.FromDate,
                        //        ToDate = i.ToDate,
                        Each = i.Each ?? false,
                        Notes = i.Note,
                        //        Quantity = i.Quantity,
                        CatalogId = i.CatalogId
                        //        CustomInventoryItemId = i.CustomInventoryItemId.HasValue ? i.CustomInventoryItemId.Value : 0
                    }).OrderBy(l => l.Position).ToList()
            };
        }

        public static ListModel ToListModel(this MandatoryItemsListHeader header, UserSelectedContext catalogInfo)
        {
            return new ListModel()
            {
                BranchId = header.BranchId,
                IsContractList = false,
                IsFavorite = false,
                IsWorksheet = false,
                IsReminder = false,
                IsMandatory = true,
                IsRecommended = false,
                IsCustomInventory = false,
                Type = ListType.Mandatory,
                //SharedWith = list.Shares != null ? list.Shares.Select(s => s.CustomerId).ToList() : null,
                ListId = header.Id,
                Name = header.Name,
                ReadOnly = false,
                //IsSharing = list.Shares != null ? (list.Shares.Any() && list.CustomerId.Equals(catalogInfo.CustomerId) &&
                //                                   list.BranchId.Equals(catalogInfo.BranchId, StringComparison.CurrentCultureIgnoreCase))
                //                                : false,
                //IsShared = !list.CustomerId.Equals(catalogInfo.CustomerId),
                Items = header.Items == null ? null :
                    header.Items.Select(i => new ListItemModel()
                    {
                        ListItemId = i.Id,
                        //        Category = i.Category,
                        Type = ListType.Mandatory,
                        ItemNumber = i.ItemNumber,
                        //        Label = i.Label,
                        //        ParLevel = i.Par,
                        //        ListItemId = i.Id,
                        //        Position = i.LineNumber,
                        ModifiedUtc = i.ModifiedUtc,
                        CreatedUtc = i.CreatedUtc,
                        //        Delta = (i.CreatedUtc.AddDays
                        //            (Constants.CONTENTMGMT_CONTRACTITEMS_THRESHOLD) > DateTime.Now) ? Constants.CONTENTMGMT_CONTRACTITEMS_NEWADDED +
                        //                " " + Constants.CONTENTMGMT_CONTRACTITEMS_ACTIVE :
                        //            (i.ToDate != null && i.ToDate.Value < DateTime.Now) ? Constants.CONTENTMGMT_CONTRACTITEMS_NEWDELETED :
                        //            Constants.CONTENTMGMT_CONTRACTITEMS_ACTIVE,
                        //        FromDate = i.FromDate,
                        //        ToDate = i.ToDate,
                        Each = i.Each ?? false,
                        Notes = i.Note,
                        //        Quantity = i.Quantity,
                        CatalogId = i.CatalogId
                        //        CustomInventoryItemId = i.CustomInventoryItemId.HasValue ? i.CustomInventoryItemId.Value : 0
                    }).OrderBy(l => l.Position).ToList()
            };
        }

        public static ListModel ToListModel(this CustomListHeader header, UserSelectedContext catalogInfo)
        {
            return new ListModel()
            {
                BranchId = header.BranchId,
                IsContractList = false,
                IsFavorite = false,
                IsWorksheet = false,
                IsReminder = false,
                IsMandatory = false,
                IsRecommended = false,
                IsCustomInventory = false,
                Type = ListType.Custom,
                //SharedWith = list.Shares != null ? list.Shares.Select(s => s.CustomerId).ToList() : null,
                ListId = header.Id,
                Name = header.Name,
                ReadOnly = false,
                //IsSharing = list.Shares != null ? (list.Shares.Any() && list.CustomerId.Equals(catalogInfo.CustomerId) &&
                //                                   list.BranchId.Equals(catalogInfo.BranchId, StringComparison.CurrentCultureIgnoreCase))
                //                                : false,
                //IsShared = !list.CustomerId.Equals(catalogInfo.CustomerId),
                Items = header.Items == null ? null :
                    header.Items.Select(i => new ListItemModel()
                    {
                        ListItemId = i.Id,
                        //        Category = i.Category,
                        Type = ListType.Custom,
                        ItemNumber = i.ItemNumber,
                        //        Label = i.Label,
                        //        ParLevel = i.Par,
                        //        ListItemId = i.Id,
                        //        Position = i.LineNumber,
                        ModifiedUtc = i.ModifiedUtc,
                        CreatedUtc = i.CreatedUtc,
                        //        Delta = (i.CreatedUtc.AddDays
                        //            (Constants.CONTENTMGMT_CONTRACTITEMS_THRESHOLD) > DateTime.Now) ? Constants.CONTENTMGMT_CONTRACTITEMS_NEWADDED +
                        //                " " + Constants.CONTENTMGMT_CONTRACTITEMS_ACTIVE :
                        //            (i.ToDate != null && i.ToDate.Value < DateTime.Now) ? Constants.CONTENTMGMT_CONTRACTITEMS_NEWDELETED :
                        //            Constants.CONTENTMGMT_CONTRACTITEMS_ACTIVE,
                        //        FromDate = i.FromDate,
                        //        ToDate = i.ToDate,
                        Each = i.Each ?? false,
                        //Notes = i.Note,
                        //        Quantity = i.Quantity,
                        CatalogId = i.CatalogId
                        //        CustomInventoryItemId = i.CustomInventoryItemId.HasValue ? i.CustomInventoryItemId.Value : 0
                    }).OrderBy(l => l.Position).ToList()
            };
        }

        public static PagedListModel ToPagedList(this ListModel returnList, PagingModel paging)
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

        /// <summary>
        /// convert a PagedListModel to a ListReportmodel
        /// </summary>
        /// <param name="list">PagedListModel</param>
        /// <returns>ListReportModel</returns>
        public static ListReportModel ToReportModel(this ListModel list)
        {
            return new ListReportModel()
            {
                Name = list.Name,
                Items = list.Items.OrderBy(m => m.Position).Select(i => new ListItemReportModel()
                    {
                        Brand = i.BrandExtendedDescription,
                        ItemNumber = i.ItemNumber,
                        Name = i.Name,
                        PackSize = i.PackSize,
                        Each = (i.Each != null) ? ((i.Each == true) ? "Y" : null) : null,
                        ParLevel = i.ParLevel,
                        Notes = i.Notes
                    }).ToList()
            };
        }

        /// <summary>
        /// Converts listitems to ShoppingCartItemReportModel
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<ShoppingCartItemReportModel> ToShoppingCartItemReportList(this PagedListModel list)
        {
            List<ShoppingCartItemReportModel> items = new List<ShoppingCartItemReportModel>();

            foreach (ListItemModel i in list.Items.Results)
            {
                ShoppingCartItemReportModel item = new ShoppingCartItemReportModel();

                item.ItemNumber = i.ItemNumber;
                item.Name = i.Name;
                item.Brand = i.BrandExtendedDescription;
                item.Category = i.ItemClass;
                item.PackSize = i.PackSize;
                if (i.Notes != null)
                {
                    item.Notes = i.Notes;
                }
                item.Label = i.Label;
                item.ParLevel = i.ParLevel;
                item.Quantity = i.Quantity;
                item.Each = i.Each;
                item.CasePrice = i.CasePrice.ToDouble().Value;
                item.PackagePrice = i.PackagePrice.ToDouble().Value;
                

                item.ExtPrice = PricingHelper.GetPrice((int)i.Quantity, i.CasePriceNumeric, i.PackagePriceNumeric,
                                                       (i.Each ?? false), i.CatchWeight, i.AverageWeight,
                                                       i.Pack.ToInt(1));

                items.Add(item);
            }

            return items;
        }
        #endregion
    }
}
