using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KeithLink.Svc.InternalSvc.Interfaces
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IListServcie" in both code and config file together.
	[ServiceContract]
	public interface IListServcie
	{
        [OperationContract]
        ListModel AddItems(UserProfile user, UserSelectedContext catalogInfo, long listId, List<ListItemModel> items);

        [OperationContract]
        long? AddItem(long listId, ListItemModel item);

        [OperationContract]
        void AddNote(UserProfile user, UserSelectedContext catalogInfo, ItemNote newNote);

        [OperationContract]
        void AddRecentlyViewedItem(UserProfile user, UserSelectedContext catalogInfo, string itemNumber);

        [OperationContract]
		long CreateList(Guid? userId, UserSelectedContext catalogInfo, ListModel list, ListType type);

        [OperationContract]
        void DeleteItem(long Id);

        [OperationContract]
        void DeleteList(long Id);

        [OperationContract]
        void DeleteNote(UserProfile user, UserSelectedContext catalogInfo, string ItemNumber);

        [OperationContract]
        List<string> ReadFavorites(UserProfile user, UserSelectedContext catalogInfo);

        [OperationContract]
		ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, long Id, bool includePrice = true);

        [OperationContract]
        List<ListModel> ReadListByType(UserSelectedContext catalogInfo, ListType type);

        [OperationContract]
		List<string> ReadListLabels(UserProfile user, UserSelectedContext catalogInfo);

        [OperationContract]
        List<ListItemModel> ReadNotes(UserProfile user, UserSelectedContext catalogInfo);

        [OperationContract]
		List<RecentItem> ReadRecent(UserProfile user, UserSelectedContext catalogInfo);

        [OperationContract]
        List<ListModel> ReadReminders(UserProfile user, UserSelectedContext catalogInfo);

        [OperationContract]
        List<ListModel> ReadUserList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false);

        [OperationContract]
        void UpdateItem(ListItemModel item);

        [OperationContract]
        void UpdateList(ListModel userList);

		[OperationContract]
		List<ListCopyResultModel> CopyList(ListCopyShareModel copyListModel);

		[OperationContract]
		void ShareList(ListCopyShareModel shareListModel);

		[OperationContract]
		List<RecommendedItemModel> ReadRecommendedItemsList(UserSelectedContext catalogInfo);

		[OperationContract]
		List<ItemBarcodeModel> GetBarcodeForList(UserProfile user, UserSelectedContext catalogInfo, long Id);

		[OperationContract]
		PagedListModel ReadPagedList(UserProfile user, UserSelectedContext catalogInfo, long Id, Core.Models.Paging.PagingModel paging);

		[OperationContract]
		void DeleteItemNumberFromList(long Id, string itemNumber);

		[OperationContract]
		List<InHistoryReturnModel> ItemsInHistoryList(UserSelectedContext catalogInfo, List<string> itemNumbers);
    }
}
