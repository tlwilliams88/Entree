﻿using System.Collections.Generic;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Customers.EF;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IListService {
        ListModel CreateList(UserProfile user,
                             UserSelectedContext catalogInfo,
                             ListType type,
                             ListModel list);

        void AddItem(UserProfile user,
                     UserSelectedContext catalogInfo,
                     ListType type,
                     long headerId,
                     ListItemModel item);

        ListModel UpdateList(UserProfile user, UserSelectedContext catalogInfo, ListType type, ListModel list);

        List<ListModel> CopyList(UserProfile user, UserSelectedContext catalogInfo, ListCopyShareModel copyListModel);

        void DeleteList(UserProfile user, UserSelectedContext catalogInfo, ListType type,
                        ListModel list);

        Dictionary<string, ListItemModel> GetNotesHash(UserSelectedContext catalogInfo);

        Dictionary<string, ListItemModel> GetFavoritesHash(UserProfile user, UserSelectedContext catalogInfo);

        List<RecentItem> ReadRecent(UserProfile user, UserSelectedContext catalogInfo);

        RecentNonBEKList ReadRecentOrder(UserProfile user, UserSelectedContext catalogInfo, string catalog);

        ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, ListType type, long Id,
                           bool includePrice = true);

        List<ListModel> ReadListByType(UserProfile user, UserSelectedContext catalogInfo, ListType type, bool headerOnly = false);

        List<ListModel> ReadUserList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false);

        PagedListModel ReadPagedList(UserProfile user, UserSelectedContext catalogInfo, ListType type,
                                     long Id, PagingModel paging);

        void SaveItem(UserProfile user, UserSelectedContext catalogInfo, ListType type,
                      long headerId, ListItemModel genericItemProperties);

        void SaveItems(UserProfile user, UserSelectedContext catalogInfo, ListType type,
                       long headerId, List<ListItemModel> items);

        List<string> ReadLabels(UserProfile user, UserSelectedContext catalogInfo);

        Dictionary<string, string> GetContractInformation(UserSelectedContext catalogInfo);

        ListModel MarkFavoritesAndAddNotes(UserProfile user, ListModel list, UserSelectedContext catalogInfo);

        List<Product> MarkFavoritesAndAddNotes(UserProfile user, List<Product> list, UserSelectedContext catalogInfo);

        ItemHistory[] GetItemsHistoryList(UserSelectedContext userContext, string[] itemNumbers);

        List<ItemBarcodeModel> GetBarcodeForList(UserProfile user, UserSelectedContext catalogInfo, ListType type, long Id);

        long? AddCustomInventory(UserProfile user, UserSelectedContext catalogInfo, ListType type, long listId, long customInventoryItemId);

        List<long?> AddCustomInventoryItems(UserProfile user, UserSelectedContext catalogInfo, ListType type, long listId, List<long> customInventoryItemIds);

        void DeleteItem(UserProfile user, UserSelectedContext catalogInfo, ListType type,
                        long headerId, string itemNumber);

        void DeleteItems(UserProfile user, UserSelectedContext catalogInfo, ListType type,
                         long headerId, List<string> itemNumbers);
    }
}