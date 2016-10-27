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
    public class CustomInventoryHelper
    {
        private const string CACHE_GROUPNAME = "CustomInventory";
        private const string CACHE_NAME = "CustomInventory";
        private const string CACHE_PREFIX = "Default";
        public static Dictionary<string, CustomInventoryItem> GetCustomInventoryInformation
            (UserSelectedContext catalogInfo, IListRepository listRepo, ICacheRepository cache)
        {
            Dictionary<string, CustomInventoryItem> inventorydictionary = new Dictionary<string, CustomInventoryItem>();

            Dictionary<string, CustomInventoryItem> cachedInventorydictionary = cache.GetItem<Dictionary<string, CustomInventoryItem>>(CACHE_GROUPNAME,
                                                                                                             CACHE_PREFIX,
                                                                                                             CACHE_NAME,
                                                                                                             string.Format("CustomInventoryDictionary_{0}_{1}",
                                                                                                                           catalogInfo.BranchId,
                                                                                                                           catalogInfo.CustomerId));

            if (cachedInventorydictionary == null)
            {
                List inventory = listRepo.ReadListForCustomer(catalogInfo, true)
                                         .Where(i => i.Type == ListType.CustomInventory)
                                         .FirstOrDefault();
                if (inventory != null)
                {
                    inventorydictionary = inventory.Items
                                                 .ToDictionary(li => li.ItemNumber,
                                                               li => new CustomInventoryItem()
                                                               {
                                                                   ItemNumber = li.ItemNumber.Trim(),
                                                                   Name = li.Name.Trim(),
                                                                   Vendor = li.Vendor.Trim(),
                                                                   Pack = li.Pack.Trim(),
                                                                   Size = li.Size.Trim(),
                                                                   CasePrice = li.CasePrice.Trim(),
                                                                   PackagePrice = li.PackagePrice.Trim(),
                                                                   Each = (li.Each != null) ? li.Each.Value : false
                                                               });
                }
                cache.AddItem<Dictionary<string, CustomInventoryItem>>(CACHE_GROUPNAME,
                                                           CACHE_PREFIX,
                                                           CACHE_NAME,
                                                           string.Format("CustomInventoryDictionary_{0}_{1}",
                                                                         catalogInfo.BranchId,
                                                                         catalogInfo.CustomerId), TimeSpan.FromHours(2), inventorydictionary);

            }
            else
            {
                inventorydictionary = cachedInventorydictionary;
            }

            return inventorydictionary;
        }

        public static void AddCustomInventoryItemInformationIfCustomerHasCustomInventory
            (Dictionary<string, CustomInventoryItem> inventorydictionary, ref ListItemModel item)
        {
            if (inventorydictionary.Count > 0)
            {
                if (inventorydictionary.ContainsKey(item.ItemNumber))
                {
                    CustomInventoryItem customInvItem = (CustomInventoryItem)inventorydictionary[item.ItemNumber];
                    item.Name = customInvItem.Name;
                    item.Pack = customInvItem.Pack;
                    item.Size = customInvItem.Size;
                    item.PackSize = string.Format("{0} / {1}", item.Pack, item.Size);
                    item.Vendor1 = customInvItem.Vendor;
                    item.CasePrice = customInvItem.CasePrice;
                    item.Each = customInvItem.Each;
                    item.PackagePrice = customInvItem.PackagePrice;
                }
            }
        }

        public static void RemoveCache(ICacheRepository cache, string branchId, string customerNumber)
        {
            cache.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, 
                string.Format("CustomInventoryDictionary_{0}_[1]", branchId, customerNumber)); //Invalidate cache
        }
    }
}
