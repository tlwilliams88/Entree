using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
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
		long CreateList(Guid userId, UserSelectedContext catalogInfo, ListModel list, ListType type);
		long? AddItem(long listId, ListItemModel item);
		ListModel AddItems(UserProfile user, UserSelectedContext catalogInfo, long listId, List<ListItemModel> items);

		List<ListModel> ReadUserList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false, bool returnAllItems = false);
		ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, long Id, bool returnAllItems = false);

		List<string> ReadListLabels(UserProfile user, UserSelectedContext catalogInfo);

		void UpdateItem(ListItemModel item);
		void UpdateList(ListModel userList);		

		void DeleteItem(long Id);
		void DeleteList(long Id);


		void AddNote(UserProfile user, UserSelectedContext catalogInfo, ItemNote newNote);
		void DeleteNote(UserProfile user, UserSelectedContext catalogInfo, string ItemNumber);

		void AddRecentlyViewedItem(UserProfile user, UserSelectedContext catalogInfo, string itemNumber);
		List<RecentItem> ReadRecent(UserProfile user, UserSelectedContext catalogInfo);

		List<string> ReadFavorites(UserProfile user, UserSelectedContext catalogInfo);
		List<ListItemModel> ReadNotes(UserProfile user, UserSelectedContext catalogInfo);

		List<ListModel> ReadListByType(UserProfile user, UserSelectedContext catalogInfo, ListType type, bool returnAllItems = false);
	}
}
