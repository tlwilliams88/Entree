using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
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
		List<ListModel> ReadUserList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly=false);
		[OperationContract]
		long CreateList(Guid userId, UserSelectedContext catalogInfo, ListModel list, ListType type);
		[OperationContract]
		long? AddItem(long listId, ListItemModel item);
		[OperationContract]
		void UpdateItem(ListItemModel item);
		[OperationContract]
		ListModel AddItems(UserProfile user, UserSelectedContext catalogInfo, long listId, List<ListItemModel> items);
		[OperationContract]
		ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, long Id);
		[OperationContract]
		List<string> ReadListLabels(UserProfile user, UserSelectedContext catalogInfo);
		[OperationContract]
		void DeleteItem(long Id);
		[OperationContract]
		void DeleteList(long Id);
		[OperationContract]
		void UpdateList(ListModel userList);
		[OperationContract]
		void AddNote(UserProfile user, UserSelectedContext catalogInfo, ItemNote newNote);
		[OperationContract]
		void DeleteNote(UserProfile user, UserSelectedContext catalogInfo, string ItemNumber);
		[OperationContract]
		void AddRecentlyViewedItem(UserProfile user, UserSelectedContext catalogInfo, string itemNumber);
		[OperationContract]
		List<RecentItem> ReadRecent(UserProfile user, UserSelectedContext catalogInfo);
		[OperationContract]
		List<string> ReadFavorites(UserProfile user, UserSelectedContext catalogInfo);
		[OperationContract]
		List<ListItemModel> ReadNotes(UserProfile user, UserSelectedContext catalogInfo);
		[OperationContract]
		List<ListModel> ReadContractList(UserProfile user, UserSelectedContext catalogInfo);
		
	}
}
