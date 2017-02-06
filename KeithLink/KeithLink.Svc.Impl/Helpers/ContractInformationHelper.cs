using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Helpers
{
    public class ContractInformationHelper
    {
        private const string CACHE_GROUPNAME = "ContractInformation";
        private const string CACHE_NAME = "ContractInformation";
        private const string CACHE_PREFIX = "Default";
        public static Dictionary<string, string> GetContractInformation(UserSelectedContext catalogInfo, IListRepository listRepo, ICacheRepository cache)
        {
            Dictionary<string, string> contractdictionary = new Dictionary<string, string>();

            Dictionary<string, string> cachedContractdictionary = cache.GetItem<Dictionary<string, string>>(CACHE_GROUPNAME,
                                                                                                             CACHE_PREFIX,
                                                                                                             CACHE_NAME,
                                                                                                             string.Format("ContractDictionary_{0}_{1}",
                                                                                                                           catalogInfo.BranchId,
                                                                                                                           catalogInfo.CustomerId));

            if (cachedContractdictionary == null)
            {
                List contract = listRepo.ReadListForCustomer(catalogInfo, true)
                                         .Where(i => i.Type == ListType.Contract)
                                         .FirstOrDefault();
                if (contract != null)
                {
                    // When we apply contract categories to other lists, on contracts that have the same itemnumber 
                    // for case and package lines have the same itemnumber twice.So the dictionary blows up trying 
                    // to put the two entries for the same itemnumber in...
                    // The dictionary just applies the category to that same item used in other lists. So the only 
                    // negative is if they specify the itemnumber/case as being in a different category than the 
                    // item /package combination. Nothing changes in how it is used in an order or anything.
                    contractdictionary = contract.Items
                                                 .GroupBy(li => li.ItemNumber, StringComparer.CurrentCultureIgnoreCase)
                                                 .ToDictionary(g => g.Key, 
                                                               g => g.First().Category.Trim());
                }
                cache.AddItem<Dictionary<string, string>>(CACHE_GROUPNAME,
                                                           CACHE_PREFIX,
                                                           CACHE_NAME,
                                                           string.Format("ContractDictionary_{0}_{1}",
                                                                         catalogInfo.BranchId,
                                                                         catalogInfo.CustomerId), TimeSpan.FromHours(2), contractdictionary);

            }
            else
            {
                contractdictionary = cachedContractdictionary;
            }

            return contractdictionary;
        }

        public static string AddContractInformationIfInContract(Dictionary<string, string> contractdictionary, ListItemModel item)
        {
            string itmcategory = null;
            if (contractdictionary.Count > 0)
            {
                if (contractdictionary.ContainsKey(item.ItemNumber))
                {
                    itmcategory = contractdictionary[item.ItemNumber];
                }
            }

            return itmcategory;
        }

    }
}
