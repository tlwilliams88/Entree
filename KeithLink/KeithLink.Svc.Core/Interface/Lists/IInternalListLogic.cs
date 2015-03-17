using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Lists
{
	public interface IInternalListLogic
	{
		long? AddItem(long listId, ListItemModel item);

		ListModel AddItems(UserProfile user, UserSelectedContext catalogInfo, long listId, List<ListItemModel> items);
		
        void AddNote(UserProfile user, UserSelectedContext catalogInfo, ItemNote newNote);

		void AddRecentlyViewedItem(UserProfile user, UserSelectedContext catalogInfo, string itemNumber);
		
        long CreateList(Guid userId, UserSelectedContext catalogInfo, ListModel list, ListType type);
		
		void DeleteItem(long Id);
		
        void DeleteList(long Id);
		
        void DeleteNote(UserProfile user, UserSelectedContext catalogInfo, string ItemNumber);
		
		List<string> ReadFavorites(UserProfile user, UserSelectedContext catalogInfo);
        
        ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, long Id);
		
		List<ListModel> ReadListByType(UserProfile user, UserSelectedContext catalogInfo, ListType type);
        
        List<string> ReadListLabels(UserProfile user, UserSelectedContext catalogInfo);
        
		List<ListItemModel> ReadNotes(UserProfile user, UserSelectedContext catalogInfo);
		
        List<RecentItem> ReadRecent(UserProfile user, UserSelectedContext catalogInfo);

        List<ListModel> ReadReminders(UserProfile user, UserSelectedContext catalogInfo);

        List<ListModel> ReadUserList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false);

        void UpdateItem(ListItemModel item);
		
        void UpdateList(ListModel userList);

		void CopyList(ListCopyShareModel copyListModel);
		void ShareList(ListCopyShareModel shareListModel);

		List<RecommendedItemModel> ReadRecommendedItemsList(UserSelectedContext catalogInfo);

		List<ItemBarcodeModel> GetBarcodeForList(UserProfile user, UserSelectedContext catalogInfo, long Id);

		PagedListModel ReadPagedList(UserProfile user, UserSelectedContext catalogInfo, long Id, PagingModel paging);
	}
}
