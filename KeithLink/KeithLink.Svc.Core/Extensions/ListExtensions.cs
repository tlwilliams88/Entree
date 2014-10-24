using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Lists;
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
				Type = KeithLink.Svc.Core.Models.EF.ListType.Custom,
				ReadOnly = list.ReadOnly,
				Items = list.Items == null ? null : list.Items.Select(i => new KeithLink.Svc.Core.Models.EF.ListItem() { Category = i.Category, ItemNumber = i.ItemNumber, Label = i.Label, Par = i.ParLevel, Position = i.Position, Note = i.Notes }).ToArray()
			};
		}

		public static ListModel ToListModel(this List list)
		{
			return new ListModel()
			{
				BranchId = list.BranchId,
				IsContractList = list.Type == ListType.Contract,
				IsFavorite = list.Type == ListType.Favorite,
				IsWorksheet = list.Type == ListType.Worksheet,
				ListId = list.Id,
				Name = list.DisplayName,
				ReadOnly = list.ReadOnly,
				Items = list.Items == null ? null : list.Items.Select(i => new ListItemModel() { Category = i.Category, ItemNumber = i.ItemNumber, Label = i.Label, ParLevel = i.Par, ListItemId = i.Id, Position = i.Position }).ToList()
			};
		}
	}
}
