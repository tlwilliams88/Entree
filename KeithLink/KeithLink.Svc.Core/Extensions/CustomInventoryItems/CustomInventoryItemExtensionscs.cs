// KeithLink
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Lists;

// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions.CustomInventoryItems {
    public static class CustomInventoryItemExtensionscs {
        /// <summary>
        /// Convert CustomInventoryItem EF Model to ReturnModel
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static CustomInventoryItemReturnModel ToReturnModel(this CustomInventoryItem item) {
            return new CustomInventoryItemReturnModel() {
                Id = item.Id,
                ItemNumber = item.ItemNumber,
                CustomerNumber = item.CustomerNumber,
                BranchId = item.BranchId,
                Name = item.Name,
                Brand = item.Brand,
                Supplier = item.Supplier,
                Pack = item.Pack,
                Size = item.Size,
                Vendor = item.Vendor,
                Each = item.Each,
                CasePrice = item.CasePrice.ToString(),
                PackagePrice = item.PackagePrice.ToString(),
                Label = item.Label
            };
        }

        /// <summary>
        /// Convert CustomInventoryItemReturnModel to CustomInventoryItem model
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static CustomInventoryItem ToModel(this CustomInventoryItemReturnModel item) {
            return new CustomInventoryItem() {
                Id = item.Id,
                ItemNumber = item.ItemNumber,
                CustomerNumber = item.CustomerNumber,
                BranchId = item.BranchId,
                Name = item.Name,
                Brand = item.Brand,
                Supplier = item.Supplier,
                Pack = item.Pack,
                Size = item.Size,
                Vendor = item.Vendor,
                Each = item.Each,
                CasePrice = Decimal.Parse(item.CasePrice),
                PackagePrice = Decimal.Parse(item.PackagePrice),
                Label = item.Label
            };
        }


        /// <summary>
        /// Convert an entire list of CustomInventoryItems to ReturnModels
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static List<CustomInventoryItemReturnModel> ToReturnModelList(this List<CustomInventoryItem> items) {
            List<CustomInventoryItemReturnModel> returnValue = new List<CustomInventoryItemReturnModel>();
            items.ForEach(item => returnValue.Add( item.ToReturnModel() ));

            return returnValue;
        }

        /// <summary>
        /// Convert an entire list of CustomInventoryItemReturnModel to EF Model
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static List<CustomInventoryItem> ToModel(this List<CustomInventoryItemReturnModel> items) {
            List<CustomInventoryItem> returnValue = new List<CustomInventoryItem>();
            items.ForEach(item => returnValue.Add(item.ToModel()));

            return returnValue;
        }

    }
}
