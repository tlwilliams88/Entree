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
	public interface IListServiceRepository
	{
		long CreateList(Guid userId, UserSelectedContext catalogInfo, ListModel list, ListType type);
		long? AddItem(long listId, ListItemModel item);
		
		List<ListModel> ReadUserList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false);
		ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, long Id);


		ListModel AddItems(UserProfile user, UserSelectedContext catalogInfo, long listId, List<ListItemModel> items);
		void UpdateItem(ListItemModel item);

		List<string> ReadListLabels(UserProfile user, UserSelectedContext catalogInfo);

		void DeleteItem(long Id);
		void DeleteList(long Id);
		void DeleteLists(List<long> listIds);
		void DeleteItems(List<long> itemIds);


		void UpdateList(ListModel userList);


		void AddNote(UserProfile user, UserSelectedContext catalogInfo, ItemNote newNote);
		void DeleteNote(UserProfile user, UserSelectedContext catalogInfo, string ItemNumber);

		void AddRecentlyViewedItem(UserProfile user, UserSelectedContext catalogInfo, string itemNumber);
		List<RecentItem> ReadRecent(UserProfile user, UserSelectedContext catalogInfo);


		List<string> ReadFavorites(UserProfile user, UserSelectedContext catalogInfo);
		List<ListItemModel> ReadNotes(UserProfile user, UserSelectedContext catalogInfo);

		List<ListModel> ReadContractList(UserProfile user, UserSelectedContext catalogInfo);
		
	}
}
