using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entree.Core.Lists.Models;
using Entree.Core.Lists.Enumerations;

using Entree.Core.Profile.Models;
using Entree.Core.SiteCatalog.Models;

namespace Entree.Core.Cache.Interfaces
{
    public interface ICacheListLogic {
        Dictionary<string, string> GetCachedContractInformation(UserSelectedContext catalogInfo);

        void AddCachedContractInformation(UserSelectedContext catalogInfo, Dictionary<string, string> contractdictionary);

        List<string> GetCachedLabels(UserSelectedContext catalogInfo);

        void AddCachedLabels(UserSelectedContext catalogInfo, List<string> list);

        List<ListModel> GetCachedTypedLists(UserSelectedContext catalogInfo, ListType type, bool headerOnly);

        void AddCachedTypedLists(UserSelectedContext catalogInfo, ListType type, bool headerOnly, List<ListModel> lists);

        List<ListModel> GetCachedCustomerLists(UserSelectedContext catalogInfo);

        void AddCachedCustomerLists(UserSelectedContext catalogInfo, List<ListModel> lists);

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
