﻿using Entree.Core;
using Entree.Core.Enumerations.List;
using Entree.Core.Interface.Cache;
using Entree.Core.Interface.Lists;
using Entree.Core.Models.EF;
using Entree.Core.Models.Lists;
using Entree.Core.Models.SiteCatalog;
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
        public static Dictionary<string, Core.Models.Lists.CustomInventoryItemReturnModel> GetCustomInventoryInformation
            (UserSelectedContext catalogInfo, IListRepository listRepo, ICacheRepository cache)
        {
            Dictionary<string, Core.Models.Lists.CustomInventoryItemReturnModel> inventorydictionary = new Dictionary<string, Core.Models.Lists.CustomInventoryItemReturnModel>();

            Dictionary<string, Core.Models.Lists.CustomInventoryItemReturnModel> cachedInventorydictionary = cache.GetItem<Dictionary<string, Core.Models.Lists.CustomInventoryItemReturnModel>>(CACHE_GROUPNAME,
                                                                                                             CACHE_PREFIX,
                                                                                                             CACHE_NAME,
                                                                                                             string.Format("CustomInventoryDictionary_{0}_{1}",
                                                                                                                           catalogInfo.BranchId,
                                                                                                                           catalogInfo.CustomerId));

            if (cachedInventorydictionary == null)
            {
                //List inventory = listRepo.ReadListForCustomer(catalogInfo, true)
                //                         .Where(i => i.Type == ListType.CustomInventory)
                //                         .FirstOrDefault();
                //if (inventory != null)
                //{
                //    inventorydictionary = new Dictionary<string, Core.Models.Lists.CustomInventoryItemReturnModel>();
                //    foreach (var itm in inventory.Items)
                //    {
                //        if (inventorydictionary.Keys.Contains(itm.ItemNumber) == false)
                //        {
                //            inventorydictionary.Add(itm.ItemNumber, new Core.Models.Lists.CustomInventoryItemReturnModel()
                //            {
                //                ItemNumber = itm.ItemNumber,
                //                Name = itm.Name,
                //                Brand = itm.Brand,
                //                Vendor = itm.Vendor,
                //                Pack = itm.Pack,
                //                Size = itm.Size,
                //                CasePrice = itm.CasePrice,
                //                PackagePrice = itm.PackagePrice,
                //                Each = (itm.Each != null) ? itm.Each.Value : false
                //            });
                //        }
                //    }
                //}
                //cache.AddItem<Dictionary<string, Core.Models.Lists.CustomInventoryItemReturnModel>>(CACHE_GROUPNAME,
                //                                           CACHE_PREFIX,
                //                                           CACHE_NAME,
                //                                           string.Format("CustomInventoryDictionary_{0}_{1}",
                //                                                         catalogInfo.BranchId,
                //                                                         catalogInfo.CustomerId), TimeSpan.FromMinutes(5), inventorydictionary);

            }
            else
            {
                inventorydictionary = cachedInventorydictionary;
            }

            return inventorydictionary;
        }

        public static void AddCustomInventoryItemInformationIfCustomerHasCustomInventory
            (Dictionary<string, Core.Models.Lists.CustomInventoryItemReturnModel> inventorydictionary, ref ListItemModel item)
        {
            //if (inventorydictionary.Count > 0)
            //{
            //    if (inventorydictionary.ContainsKey(item.ItemNumber))
            //    {
            //        Core.Models.Lists.CustomInventoryItemReturnModel customInvItem = (Core.Models.Lists.CustomInventoryItemReturnModel)inventorydictionary[item.ItemNumber];
            //        item.Name = customInvItem.Name;
            //        item.BrandExtendedDescription = customInvItem.Brand;
            //        item.Pack = customInvItem.Pack;
            //        item.Size = customInvItem.Size;
            //        item.PackSize = string.Format("{0} / {1}", item.Pack, item.Size);
            //        item.Vendor1 = customInvItem.Vendor;
            //        item.CasePrice = customInvItem.CasePrice;
            //        item.Each = customInvItem.Each;
            //        item.PackagePrice = customInvItem.PackagePrice;
            //        item.IsValid = true;
            //    }
            //}
        }

        public static void RemoveCache(ICacheRepository cache, string branchId, string customerNumber)
        {
            cache.RemoveItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, 
                string.Format("CustomInventoryDictionary_{0}_[1]", branchId, customerNumber)); //Invalidate cache
        }
    }
}
