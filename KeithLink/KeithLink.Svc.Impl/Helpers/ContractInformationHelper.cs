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
                    contractdictionary = contract.Items
                                                 .ToDictionary(li => li.ItemNumber, li => li.Category.Trim());
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
