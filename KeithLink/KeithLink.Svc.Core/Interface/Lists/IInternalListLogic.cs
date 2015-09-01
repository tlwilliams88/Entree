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

        List<ListCopyResultModel> CopyList(ListCopyShareModel copyListModel);

        long CreateList(Guid? userId, UserSelectedContext catalogInfo, ListModel list, ListType type);
		
		void DeleteItem(long Id);
		
        void DeleteList(long Id);

		void DeleteItemNumberFromList(long Id, string itemNumber);
		
        void DeleteNote(UserProfile user, UserSelectedContext catalogInfo, string ItemNumber);

        List<ItemBarcodeModel> GetBarcodeForList(UserProfile user, UserSelectedContext catalogInfo, long Id);

        List<InHistoryReturnModel> ItemsInHistoryList(UserSelectedContext catalogInfo, List<string> itemNumbers);

        List<string> ReadFavorites(UserProfile user, UserSelectedContext catalogInfo);
        
        ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, long Id, bool includePrice = true);

		List<ListModel> ReadListByType(UserProfile user, UserSelectedContext catalogInfo, ListType type, bool headerOnly = false);
        
        List<string> ReadListLabels(UserProfile user, UserSelectedContext catalogInfo);
        
		List<ListItemModel> ReadNotes(UserProfile user, UserSelectedContext catalogInfo);

        PagedListModel ReadPagedList(UserProfile user, UserSelectedContext catalogInfo, long Id, PagingModel paging);

        List<RecentItem> ReadRecent(UserProfile user, UserSelectedContext catalogInfo);

        List<RecommendedItemModel> ReadRecommendedItemsList(UserSelectedContext catalogInfo);

        List<ListModel> ReadReminders(UserProfile user, UserSelectedContext catalogInfo);

        List<ListModel> ReadUserList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false);

        void ShareList(ListCopyShareModel shareListModel);

        void UpdateItem(ListItemModel item);
		
        void UpdateList(ListModel userList);
	}
}
