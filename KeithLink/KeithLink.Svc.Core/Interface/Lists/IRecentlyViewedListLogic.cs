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
	public interface IRecentlyViewedListLogic
	{
		void AddItem(UserProfile user, UserSelectedContext catalogInfo, string itemNumber);
		void Clear(UserProfile user, UserSelectedContext catalogInfo);
		List<RecentItem> Read(UserProfile user, UserSelectedContext catalogInfo);
	}
}
