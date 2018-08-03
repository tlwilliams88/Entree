﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Cache
{
    public interface ICacheListLogic {
        Dictionary<string, string> GetCachedContractInformation(UserSelectedContext catalogInfo);

        void AddCachedContractInformation(UserSelectedContext catalogInfo, Dictionary<string, string> contractdictionary);

        List<string> GetCachedLabels(UserSelectedContext catalogInfo);

        void AddCachedLabels(UserSelectedContext catalogInfo, List<string> list);

        List<ListModel> GetCachedTypedLists(UserSelectedContext catalogInfo, ListType type, bool headerOnly);

        void AddCachedTypedLists(UserSelectedContext catalogInfo, ListType type, bool headerOnly, List<ListModel> lists);

        List<ListModel> GetCachedCustomerLists(UserSelectedContext catalogInfo, bool headerOnly);

        void AddCachedCustomerLists(UserSelectedContext catalogInfo, List<ListModel> lists, bool headerOnly);

        ListModel GetCachedSpecificList(UserSelectedContext catalogInfo, ListType type, long Id);

        void AddCachedSpecificList(UserSelectedContext catalogInfo, ListType type, long Id, ListModel list);

        void RemoveSpecificCachedList(ListModel list);

        void RemoveTypeOfListsCache(UserSelectedContext catalogInfo, ListType type);

        void ClearCustomersListCaches(UserProfile user, UserSelectedContext catalogInfo, List<ListModel> lists);

        void ClearCustomersListCaches(UserProfile user, string customerNumber, string branchId, List<ListModel> lists);

        void ClearCustomersLabelsCache(UserSelectedContext catalogInfo);

        void ClearCustomersLabelsCache(string customerNumber, string branchId);
    }
}
