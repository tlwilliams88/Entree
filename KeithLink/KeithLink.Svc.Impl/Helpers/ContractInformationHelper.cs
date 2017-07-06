using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Helpers
{
    public class ContractInformationHelper
    {
        public static void GetContractCategoriesFromLists(UserSelectedContext catalogInfo, ListModel list, IListService listService)
        {
            Dictionary<string, string> contractdictionary = listService.GetContractInformation(catalogInfo);

            if (contractdictionary != null &&
                contractdictionary.Count > 0) {
                Parallel.ForEach(list.Items, listItem =>
                {
                    if (contractdictionary.ContainsKey(listItem.ItemNumber))
                    {
                        listItem.Category = contractdictionary[listItem.ItemNumber];
                    }
                });
            }
        }
        public static void GetContractCategoriesFromLists(UserSelectedContext catalogInfo, Product prod, IListService listService)
        {
            Dictionary<string, string> contractdictionary = listService.GetContractInformation(catalogInfo);

            if (contractdictionary != null &&
                contractdictionary.Count > 0) {
                if (contractdictionary.ContainsKey(prod.ItemNumber)) {
                    prod.Category = contractdictionary[prod.ItemNumber];
                }
            }
        }
        public static void GetContractCategoriesFromLists(UserSelectedContext catalogInfo, List<Product> prods, IListService listService)
        {
            Dictionary<string, string> contractdictionary = listService.GetContractInformation(catalogInfo);

            if (contractdictionary != null &&
                contractdictionary.Count > 0)
            {
                Parallel.ForEach(prods, prod =>
                {
                    if (contractdictionary.ContainsKey(prod.ItemNumber))
                    {
                        prod.Category = contractdictionary[prod.ItemNumber];
                    }
                });
            }
        }
    }
}
