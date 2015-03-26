using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions
{
	public static class ListExtensions
	{
		public static List ToEFList(this ListModel list)
		{
			return new KeithLink.Svc.Core.Models.EF.List(){
				DisplayName = list.Name,
				Type = ListType.Custom,
				ReadOnly = list.ReadOnly,
				Items = list.Items == null ? null : list.Items.Select(i => new KeithLink.Svc.Core.Models.EF.ListItem() { Category = i.Category, ItemNumber = i.ItemNumber, Label = i.Label, Par = i.ParLevel, Position = i.Position, Note = i.Notes, Each = i.Each.Equals(null) ? false : (bool)i.Each}).ToArray()
			};
		}

		public static ListModel ToListModel(this List list,UserSelectedContext catalogInfo)
		{
			return new ListModel()
			{
				BranchId = list.BranchId,
				IsContractList = list.Type == ListType.Contract,
				IsFavorite = list.Type == ListType.Favorite,
				IsWorksheet = list.Type == ListType.Worksheet,
                IsReminder = list.Type == ListType.Reminder,
				IsMandatory = list.Type == ListType.Mandatory,
				IsRecommended = list.Type == ListType.RecommendedItems,
				Type = list.Type,
				SharedWith = list.Shares!= null ? list.Shares.Select(s => s.CustomerId).ToList() : null ,
				ListId = list.Id,
				Name = list.DisplayName,
				ReadOnly = list.ReadOnly,
                IsSharing = list.Shares != null ? (list.Shares.Any() && list.CustomerId.Equals(catalogInfo.CustomerId) && list.BranchId.Equals(catalogInfo.BranchId)) : false,
				IsShared = !list.CustomerId.Equals(catalogInfo.CustomerId),
                Items = list.Items == null ? null :
                    list.Items.Select(i => new ListItemModel() { Category = i.Category, Type = list.Type, ItemNumber = i.ItemNumber, Label = i.Label, ParLevel = i.Par, ListItemId = i.Id, Position = i.Position, ModifiedUtc = i.ModifiedUtc, CreatedUtc = i.CreatedUtc, FromDate = i.FromDate, ToDate = i.ToDate, Each = i.Each.Equals(null) ? false : (bool)i.Each }).ToList()
			};
		}


		public static ListReportModel ToReportModel(this PagedListModel list)
		{
			return new ListReportModel()
			{
				Name = list.Name,
				Items = list.Items == null || list.Items.Results == null ? null :
					list.Items.Results.Select(i => new ListItemReportModel() { Brand = i.BrandExtendedDescription, ItemNumber = i.ItemNumber, Name = i.Name, PackSize = i.PackSize }).ToList()
			};
		}
	}
}
