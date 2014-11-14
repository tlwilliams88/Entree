using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.InternalSvc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KeithLink.Svc.InternalSvc
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ListServcie" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select ListServcie.svc or ListServcie.svc.cs at the Solution Explorer and start debugging.
	public class ListServcie : IListServcie {
        #region attributes
        private readonly IInternalListLogic listLogic;
        #endregion

        #region ctor
        public ListServcie(IInternalListLogic listLogic)
		{
			this.listLogic = listLogic;
		}
        #endregion

        #region methods
		public long? AddItem(long listId, ListItemModel item)
		{
			return listLogic.AddItem(listId, item);
		}

		public ListModel AddItems(UserProfile user, UserSelectedContext catalogInfo, long listId, List<ListItemModel> items)
		{
			return listLogic.AddItems(user, catalogInfo, listId, items);
		}
        
		public void AddNote(UserProfile user, UserSelectedContext catalogInfo, ItemNote newNote)
		{
			listLogic.AddNote(user, catalogInfo, newNote);
		}
		
		public void AddRecentlyViewedItem(UserProfile user, UserSelectedContext catalogInfo, string itemNumber)
		{
			listLogic.AddRecentlyViewedItem(user, catalogInfo, itemNumber);
		}
        
        public long CreateList(Guid userId, UserSelectedContext catalogInfo, ListModel list, ListType type)
		{
			return listLogic.CreateList(userId, catalogInfo, list, type);
		}
        
		public void DeleteItem(long Id)
		{
			listLogic.DeleteItem(Id);
		}
		
		public void DeleteList(long Id)
		{
			listLogic.DeleteList(Id);
		}
        
		public void DeleteNote(UserProfile user, UserSelectedContext catalogInfo, string ItemNumber)
		{
			listLogic.DeleteNote(user, catalogInfo, ItemNumber);
		}
        
		public List<string> ReadFavorites(UserProfile user, UserSelectedContext catalogInfo)
		{
			return listLogic.ReadFavorites(user, catalogInfo);
		}

        public ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, long Id)
		{
			return listLogic.ReadList(user, catalogInfo, Id);
		}
        
		public List<ListModel> ReadListByType(UserProfile user, UserSelectedContext catalogInfo, ListType type)
		{
			return listLogic.ReadListByType(user, catalogInfo, type);
		}
		
        public List<string> ReadListLabels(UserProfile user, UserSelectedContext catalogInfo)
		{
			return listLogic.ReadListLabels(user, catalogInfo);
		}
        
		public List<ListItemModel> ReadNotes(UserProfile user, UserSelectedContext catalogInfo)
		{
			return listLogic.ReadNotes(user, catalogInfo);
		}
		
        public List<RecentItem> ReadRecent(UserProfile user, UserSelectedContext catalogInfo)
		{
			return listLogic.ReadRecent(user, catalogInfo);
		}

        public List<ListModel> ReadReminders(UserProfile user, UserSelectedContext catalogInfo) {
            return listLogic.ReadReminders(user, catalogInfo);
        }
        
        public List<ListModel> ReadUserList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly=false)
		{
			return listLogic.ReadUserList(user, catalogInfo, headerOnly);
		}

		public void UpdateItem(ListItemModel item)
		{
			listLogic.UpdateItem(item);
		}
		
		public void UpdateList(Core.Models.Lists.ListModel userList)
		{
			listLogic.UpdateList(userList);
		}
        
		public void CopyList(ListCopyShareModel copyListModel)
		{
			listLogic.CopyList(copyListModel);
		}		

		public void ShareList(ListCopyShareModel shareListModel)
		{
			listLogic.ShareList(shareListModel);
		}
		
		#endregion
	}
}
