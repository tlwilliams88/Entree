using KeithLink.Svc.Core.Enumerations.List;
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
	[GlobalErrorBehaviorAttribute(typeof(ErrorHandler))]
	public class ListServcie : IListServcie
	{
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
        
        public long CreateList(Guid? userId, UserSelectedContext catalogInfo, ListModel list, ListType type)
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

        public ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, long Id, bool includePrice = true)
		{
			return listLogic.ReadList(user, catalogInfo, Id, includePrice);
		}

		public List<ListModel> ReadListByType(UserProfile user, UserSelectedContext catalogInfo, ListType type, bool headerOnly = false)
		{
			return listLogic.ReadListByType(user, catalogInfo, type, headerOnly);
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

		public List<ListCopyResultModel> CopyList(ListCopyShareModel copyListModel)
		{
			return listLogic.CopyList(copyListModel);
		}		

		public void ShareList(ListCopyShareModel shareListModel)
		{
			listLogic.ShareList(shareListModel);
		}
		
		public List<RecommendedItemModel> ReadRecommendedItemsList(UserSelectedContext catalogInfo)
		{
			return listLogic.ReadRecommendedItemsList(catalogInfo);
		}


		public List<Core.Models.Reports.ItemBarcodeModel> GetBarcodeForList(UserProfile user, UserSelectedContext catalogInfo, long Id)
		{
			return listLogic.GetBarcodeForList(user, catalogInfo, Id);
		}


		public PagedListModel ReadPagedList(UserProfile user, UserSelectedContext catalogInfo, long Id, Core.Models.Paging.PagingModel paging)
		{
			return listLogic.ReadPagedList(user, catalogInfo, Id, paging);
		}
		

		public void DeleteItemNumberFromList(long Id, string itemNumber)
		{
			listLogic.DeleteItemNumberFromList(Id, itemNumber);
		}


		public List<InHistoryReturnModel> ItemsInHistoryList(UserSelectedContext catalogInfo, List<string> itemNumbers)
		{
			return listLogic.ItemsInHistoryList(catalogInfo, itemNumbers);
		}
		
		#endregion
	}
}
