using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Enumerations.List
{
	public enum BasketType
	{
		[Obsolete]
		Notes,
		[Obsolete]
		Favorite,
		[Obsolete]
		Custom,
		Cart,
		[Obsolete]
		RecentlyViewed
	}


	public enum ListType
	{
		Custom,
		Favorite,
		Contract,
		Recent,
		Notes,
		Worksheet,
		ContractItemsAdded,
		ContractItemsDeleted,
		Reminder,
		Mandatory,
		RecommendedItems
	}
}
