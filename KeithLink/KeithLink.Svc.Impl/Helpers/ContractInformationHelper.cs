using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Helpers
{
    /* For now this helper is in the Impl assembly (for testing), but should only be called from a controller in WebApi */
    public class ContractInformationHelper
    {
        public static void GetContractCategoriesFromLists(UserSelectedContext catalogInfo, Product prod, IListService listService)
        {
            Dictionary<string, string> contractdictionary = listService.GetContractInformation(catalogInfo);

            if (contractdictionary != null &&
                contractdictionary.Count > 0) {
                if (contractdictionary.ContainsKey(prod.ItemNumber)) {
                    prod.Category = contractdictionary[prod.ItemNumber];
                    prod.Detail = string.Format("{0} / {1} / {2} / {3} / {4}",
                                                prod.Name,
                                                prod.ItemNumber,
                                                prod.BrandExtendedDescription,
                                                contractdictionary[prod.ItemNumber],
                                                prod.PackSize);
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
                        prod.Detail = string.Format("{0} / {1} / {2} / {3} / {4}",
                                                    prod.Name,
                                                    prod.ItemNumber,
                                                    prod.BrandExtendedDescription,
                                                    contractdictionary[prod.ItemNumber],
                                                    prod.PackSize);
                    }
                });
            }
        }
        public static void GetContractCategoriesFromLists(UserSelectedContext catalogInfo, List<ShoppingCartItem> prods, IListService listService)
        {
            Dictionary<string, string> contractdictionary = listService.GetContractInformation(catalogInfo);

            if (contractdictionary != null &&
                contractdictionary.Count > 0)
            {
                Parallel.ForEach(prods, prod =>
                {
                    if (contractdictionary.ContainsKey(prod.ItemNumber))
                    {
                        prod.Detail = string.Format("{0} / {1} / {2} / {3} / {4}",
                                                    prod.Name,
                                                    prod.ItemNumber,
                                                    prod.BrandExtendedDescription,
                                                    contractdictionary[prod.ItemNumber],
                                                    prod.PackSize);
                    }
                });
            }
        }
        public static void GetContractCategoriesFromLists(UserSelectedContext catalogInfo, List<OrderLine> prods, IListService listService)
        {
            Dictionary<string, string> contractdictionary = listService.GetContractInformation(catalogInfo);

            if (contractdictionary != null &&
                contractdictionary.Count > 0)
            {
                Parallel.ForEach(prods, prod =>
                {
                    if (contractdictionary.ContainsKey(prod.ItemNumber))
                    {
                        prod.Detail = string.Format("{0} / {1} / {2} / {3} / {4}", 
                                                    prod.Name, 
                                                    prod.ItemNumber, 
                                                    prod.BrandExtendedDescription, 
                                                    contractdictionary[prod.ItemNumber],
                                                    prod.PackSize);
                    }
                });
            }
        }
        public static void GetContractCategoriesFromLists(UserSelectedContext catalogInfo, List<ItemUsageReportItemModel> prods, IListService listService)
        {
            Dictionary<string, string> contractdictionary = listService.GetContractInformation(catalogInfo);

            if (contractdictionary != null &&
                contractdictionary.Count > 0)
            {
                Parallel.ForEach(prods, prod =>
                {
                    if (contractdictionary.ContainsKey(prod.ItemNumber))
                    {
                        prod.Detail = string.Format("{0} / {1} / {2} / {3} / {4}",
                                                    prod.Name,
                                                    prod.ItemNumber,
                                                    prod.Brand,
                                                    contractdictionary[prod.ItemNumber],
                                                    prod.PackSize);
                    }
                });
            }
        }
    }
}
