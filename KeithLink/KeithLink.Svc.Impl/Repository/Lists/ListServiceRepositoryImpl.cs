using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Lists
{
	public class ListServiceRepositoryImpl: IListServiceRepository
	{
		private com.benekeith.ListService.IListServcie serviceClient;

		public ListServiceRepositoryImpl(com.benekeith.ListService.IListServcie serviceClient)
		{
			this.serviceClient = serviceClient;
		}

		public long CreateList(Guid userId, UserSelectedContext catalogInfo, ListModel list, ListType type)
		{
			return serviceClient.CreateList(userId, catalogInfo, list, type);
		}

		public long? AddItem(long listId, ListItemModel item)
		{
			return serviceClient.AddItem(listId, item);
		}
		
		public List<ListModel> ReadUserList(Core.Models.Profile.UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false)
		{
			var list = serviceClient.ReadUserList(user, catalogInfo, headerOnly);

			if (list == null)
				return null;
			return list.ToList();
		}


		public ListModel AddItems(UserProfile user, UserSelectedContext catalogInfo, long listId, List<ListItemModel> items)
		{
			return serviceClient.AddItems(user, catalogInfo, listId, items.ToArray());
		}

		public void UpdateItem(ListItemModel item)
		{
			serviceClient.UpdateItem(item);
		}

		public ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, long Id)
		{
			return serviceClient.ReadList(user, catalogInfo, Id);
		}

		public List<string> ReadListLabels(Core.Models.Profile.UserProfile user, UserSelectedContext catalogInfo)
		{
			return serviceClient.ReadListLabels(user, catalogInfo).ToList();
		}


		public void DeleteItem(long Id)
		{
			serviceClient.DeleteItem(Id);
		}

		public void DeleteList(long Id)
		{
			serviceClient.DeleteList(Id);
		}


		public void DeleteLists(List<long> listIds)
		{
			foreach (var Id in listIds)
				serviceClient.DeleteList(Id);
		}

		public void DeleteItems(List<long> itemIds)
		{
			foreach (var Id in itemIds)
				serviceClient.DeleteItem(Id);
		}

		public void UpdateList(ListModel userList)
		{
			serviceClient.UpdateList(userList);
		}


		public void AddNote(UserProfile user, UserSelectedContext catalogInfo, ItemNote newNote)
		{
			serviceClient.AddNote(user, catalogInfo, newNote);
		}

		public void DeleteNote(UserProfile user, UserSelectedContext catalogInfo, string ItemNumber)
		{
			serviceClient.DeleteNote(user, catalogInfo, ItemNumber);
		}


		public void AddRecentlyViewedItem(UserProfile user, UserSelectedContext catalogInfo, string itemNumber)
		{
			serviceClient.AddRecentlyViewedItem(user, catalogInfo, itemNumber);
		}


		public List<RecentItem> ReadRecent(UserProfile user, UserSelectedContext catalogInfo)
		{
			return serviceClient.ReadRecent(user, catalogInfo).ToList();
		}


		public List<string> ReadFavorites(UserProfile user, UserSelectedContext catalogInfo)
		{
			return serviceClient.ReadFavorites(user, catalogInfo).ToList();
		}

		public List<ListItemModel> ReadNotes(UserProfile user, UserSelectedContext catalogInfo)
		{
			return serviceClient.ReadNotes(user, catalogInfo).ToList();
		}


		public List<ListModel> ReadContractList(UserProfile user, UserSelectedContext catalogInfo)
		{
			return serviceClient.ReadContractList(user, catalogInfo).ToList();
		}
	}
}
