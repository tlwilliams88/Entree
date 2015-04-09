using KeithLink.Svc.Core.Enumerations.List;
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

namespace KeithLink.Svc.WebApi.Repository.Lists
{
	public class ListServiceRepositoryImpl: IListServiceRepository {
        #region attributes
        private com.benekeith.ListService.IListServcie serviceClient;
        #endregion

        #region ctor
        public ListServiceRepositoryImpl(com.benekeith.ListService.IListServcie serviceClient)
		{
			this.serviceClient = serviceClient;
		}
        #endregion

        #region methods
		public long? AddItem(long listId, ListItemModel item)
		{
			return serviceClient.AddItem(listId, item);
		}

		public ListModel AddItems(UserProfile user, UserSelectedContext catalogInfo, long listId, List<ListItemModel> items)
		{
			return serviceClient.AddItems(user, catalogInfo, listId, items.ToArray());
		}

		public void AddNote(UserProfile user, UserSelectedContext catalogInfo, ItemNote newNote)
		{
			serviceClient.AddNote(user, catalogInfo, newNote);
		}
        
		public void AddRecentlyViewedItem(UserProfile user, UserSelectedContext catalogInfo, string itemNumber)
		{
			serviceClient.AddRecentlyViewedItem(user, catalogInfo, itemNumber);
		}
        
        public long CreateList(Guid? userId, UserSelectedContext catalogInfo, ListModel list, ListType type)
		{
			return serviceClient.CreateList(userId, catalogInfo, list, type);
		}
		
		public void DeleteItem(long Id)
		{
			serviceClient.DeleteItem(Id);
		}
		
		public void DeleteItems(List<long> itemIds)
		{
			foreach (var Id in itemIds)
				serviceClient.DeleteItem(Id);
		}

        public void DeleteList(long Id)
		{
			serviceClient.DeleteList(Id);
		}
        
		public void DeleteNote(UserProfile user, UserSelectedContext catalogInfo, string ItemNumber)
		{
			serviceClient.DeleteNote(user, catalogInfo, ItemNumber);
		}
		
        public void DeleteLists(List<long> listIds)
		{
			foreach (var Id in listIds)
				serviceClient.DeleteList(Id);
		}
        
		public List<string> ReadFavorites(UserProfile user, UserSelectedContext catalogInfo)
		{
			return serviceClient.ReadFavorites(user, catalogInfo).ToList();
		}

		public ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, long Id, bool includePrice = true)
		{
			return serviceClient.ReadList(user, catalogInfo, Id, includePrice);
		}
		
		public List<ListModel> ReadListByType(UserSelectedContext catalogInfo, ListType type)
		{
			return serviceClient.ReadListByType(catalogInfo, type).ToList();
		}
		
        public List<string> ReadListLabels(Core.Models.Profile.UserProfile user, UserSelectedContext catalogInfo)
		{
			return serviceClient.ReadListLabels(user, catalogInfo).ToList();
		}
        
		public List<ListItemModel> ReadNotes(UserProfile user, UserSelectedContext catalogInfo)
		{
			return serviceClient.ReadNotes(user, catalogInfo).ToList();
		}
		
        public List<RecentItem> ReadRecent(UserProfile user, UserSelectedContext catalogInfo)
		{
			return serviceClient.ReadRecent(user, catalogInfo).ToList();
		}

        public List<ListModel> ReadReminders(UserProfile user, UserSelectedContext catalogInfo) {
            return serviceClient.ReadReminders(user, catalogInfo).ToList();
        }

        public List<ListModel> ReadUserList(Core.Models.Profile.UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false)
		{
			var list = serviceClient.ReadUserList(user, catalogInfo, headerOnly);

			if (list == null)
				return null;
			return list.ToList();
		}

		public void UpdateItem(ListItemModel item)
		{
			serviceClient.UpdateItem(item);
		}

		public void UpdateList(ListModel userList)
		{
			serviceClient.UpdateList(userList);
		}

		public void CopyList(ListCopyShareModel copyListModel)
		{
			serviceClient.CopyList(copyListModel);
		}

		public void ShareList(ListCopyShareModel shareListModel)
		{
			serviceClient.ShareList(shareListModel);
		} 
		
		#endregion


		public List<RecommendedItemModel> ReadRecommendedItemsList(UserSelectedContext catalogInfo)
		{
			return serviceClient.ReadRecommendedItemsList(catalogInfo).ToList();
		}


		public List<Core.Models.Reports.ItemBarcodeModel> GetBarcodeForList(UserProfile user, UserSelectedContext catalogInfo, long Id)
		{
			var list = serviceClient.GetBarcodeForList(user, catalogInfo, Id);

			if (list == null)
				return null;
			return list.ToList();
		}


		public PagedListModel ReadPagedList(UserProfile user, UserSelectedContext catalogInfo, long Id, Core.Models.Paging.PagingModel paging)
		{
			return serviceClient.ReadPagedList(user, catalogInfo, Id, paging);
		}
	}
}
