using KeithLink.Svc.Core.Enumerations.List;

using KeithLink.Svc.Core.Helpers;

using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions {
	public static class ListExtensions {
        #region methods
        /// <summary>
        /// convert the ListModel to a List object
        /// </summary>
        /// <param name="list">the list model</param>
        /// <returns>List database model</returns>
        public static List ToEFList(this ListModel list) {
            return new KeithLink.Svc.Core.Models.EF.List() {
                DisplayName = list.Name,
                Type = ListType.Custom,
                ReadOnly = list.ReadOnly,
                Items = list.Items == null ? null : list.Items.Select(i => new KeithLink.Svc.Core.Models.EF.ListItem() {
                    Category = i.Category,
                    ItemNumber = i.ItemNumber,
                    Label = i.Label,
                    Par = i.ParLevel,
                    Position = i.Position,
                    Note = i.Notes,
                    Each = i.Each ?? false,
                    Quantity = i.Quantity
                }).ToArray()
            };
        }

        /// <summary>
        /// convert the List database model to a ListModel object
        /// </summary>
        /// <param name="list">the database model</param>
        /// <param name="catalogInfo">the customer information</param>
        /// <returns>ListModel object</returns>
        public static ListModel ToListModel(this List list, UserSelectedContext catalogInfo) {
            return new ListModel() {
                BranchId = list.BranchId,
                IsContractList = list.Type == ListType.Contract,
                IsFavorite = list.Type == ListType.Favorite,
                IsWorksheet = list.Type == ListType.Worksheet,
                IsReminder = list.Type == ListType.Reminder,
                IsMandatory = list.Type == ListType.Mandatory,
                IsRecommended = list.Type == ListType.RecommendedItems,
                Type = list.Type,
                SharedWith = list.Shares != null ? list.Shares.Select(s => s.CustomerId).ToList() : null,
                ListId = list.Id,
                Name = list.DisplayName,
                ReadOnly = list.ReadOnly,
                IsSharing = list.Shares != null ? (list.Shares.Any() && list.CustomerId.Equals(catalogInfo.CustomerId) && 
                                                   list.BranchId.Equals(catalogInfo.BranchId, StringComparison.CurrentCultureIgnoreCase)) 
                                                : false,
                IsShared = !list.CustomerId.Equals(catalogInfo.CustomerId),
                Items = list.Items == null ? null :
                    list.Items.Select(i => new ListItemModel() {
                        Category = i.Category,
                        Type = list.Type,
                        ItemNumber = i.ItemNumber,
                        Label = i.Label,
                        ParLevel = i.Par,
                        ListItemId = i.Id,
                        Position = i.Position,
                        ModifiedUtc = i.ModifiedUtc,
                        CreatedUtc = i.CreatedUtc,
                        FromDate = i.FromDate,
                        ToDate = i.ToDate,
                        Each = i.Each ?? false,
                        Quantity = i.Quantity
                    }).ToList()
            };
        }

        /// <summary>
        /// convert a PagedListModel to a ListReportmodel
        /// </summary>
        /// <param name="list">PagedListModel</param>
        /// <returns>ListReportModel</returns>
        public static ListReportModel ToReportModel(this PagedListModel list) {
            return new ListReportModel() {
                Name = list.Name,
                Items = list.Items == null || list.Items.Results == null ? null :
                    list.Items.Results.Select(i => new ListItemReportModel() {
                        Brand = i.BrandExtendedDescription,
                        ItemNumber = i.ItemNumber,
                        Name = i.Name,
                        PackSize = i.PackSize,
                        ParLevel = i.ParLevel,
                        Notes = i.Notes
                    }).ToList()
            };
        }

        /// <summary>
        /// Converts listitems to ShoppingCartItemReportModel
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<ShoppingCartItemReportModel> ToShoppingCartItemReportList( this ListModel list ) {
            List<ShoppingCartItemReportModel> items = new List<ShoppingCartItemReportModel>();

            foreach (ListItemModel i in list.Items) {
                ShoppingCartItemReportModel item = new ShoppingCartItemReportModel();

                item.ItemNumber = i.ItemNumber;
                item.Name = i.Name;
                item.Brand = i.Brand;
                item.Category = i.Category;
                item.PackSize = i.PackSize;
                if (i.Notes != null) {
                    item.Notes = i.Notes;
                }
                item.Label = i.Label;
                item.ParLevel = i.ParLevel;
                item.Quantity = i.Quantity.ToString();
                item.Each = i.Each;
                item.CasePrice = i.CasePrice;
                item.PackagePrice = i.PackagePrice;

                int pack = 0;
                int.TryParse(i.Pack, out pack);

                item.ExtPrice = PricingHelper.GetPrice((int)i.Quantity, i.CasePriceNumeric, i.PackagePriceNumeric,
                                                       (i.Each ?? false), i.CatchWeight, i.AverageWeight,
                                                       pack).ToString("f2");

                items.Add( item );
            }


            return items;
        }
        #endregion
    }
}
