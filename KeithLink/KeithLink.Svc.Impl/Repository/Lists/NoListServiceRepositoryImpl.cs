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
	public class NoListServiceRepositoryImpl: IListServiceRepository {
        #region attributes
        #endregion

        #region ctor
        public NoListServiceRepositoryImpl()
		{
		}
        #endregion

        #region methods
		public long? AddItem(long listId, ListItemModel item)
        {
            throw new NotImplementedException();
		}

		public ListModel AddItems(UserProfile user, UserSelectedContext catalogInfo, long listId, List<ListItemModel> items)
        {
            throw new NotImplementedException();
		}

		public void AddNote(UserProfile user, UserSelectedContext catalogInfo, ItemNote newNote)
        {
            throw new NotImplementedException();
		}
        
		public void AddRecentlyViewedItem(UserProfile user, UserSelectedContext catalogInfo, string itemNumber)
        {
            throw new NotImplementedException();
		}
        
        public long CreateList(Guid userId, UserSelectedContext catalogInfo, ListModel list, ListType type)
        {
            throw new NotImplementedException();
		}
		
		public void DeleteItem(long Id)
        {
            throw new NotImplementedException();
		}
		
		public void DeleteItems(List<long> itemIds)
        {
            throw new NotImplementedException();
		}

        public void DeleteList(long Id)
        {
            throw new NotImplementedException();
		}
        
		public void DeleteNote(UserProfile user, UserSelectedContext catalogInfo, string ItemNumber)
        {
            throw new NotImplementedException();
		}
		
        public void DeleteLists(List<long> listIds)
        {
            throw new NotImplementedException();
		}
        
		public List<string> ReadFavorites(UserProfile user, UserSelectedContext catalogInfo)
        {
            throw new NotImplementedException();
		}
        
        public ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, long Id)
        {
            throw new NotImplementedException();
		}
		
		public List<ListModel> ReadListByType(UserProfile user, UserSelectedContext catalogInfo, ListType type)
        {
            throw new NotImplementedException();
		}
		
        public List<string> ReadListLabels(Core.Models.Profile.UserProfile user, UserSelectedContext catalogInfo)
        {
            throw new NotImplementedException();
		}
        
		public List<ListItemModel> ReadNotes(UserProfile user, UserSelectedContext catalogInfo)
        {
            throw new NotImplementedException();
		}
		
        public List<RecentItem> ReadRecent(UserProfile user, UserSelectedContext catalogInfo)
        {
            throw new NotImplementedException();
		}

        public List<ListModel> ReadReminders(UserProfile user, UserSelectedContext catalogInfo)
        {
            throw new NotImplementedException();
        }

        public List<ListModel> ReadUserList(Core.Models.Profile.UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false)
		{
            throw new NotImplementedException();
		}

		public void UpdateItem(ListItemModel item)
		{
            throw new NotImplementedException();
		}

		public void UpdateList(ListModel userList)
        {
            throw new NotImplementedException();
		}
        
		public void CopyList(ListCopyShareModel copyListModel)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
