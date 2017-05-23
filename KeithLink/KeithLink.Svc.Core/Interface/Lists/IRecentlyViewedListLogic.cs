using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Lists;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IRecentlyViewedListLogic : IBaseListLogic
    {
        List<string> GetRecentlyViewedItemNumbers(UserProfile user, UserSelectedContext catalogInfo);

        void AddOrUpdateRecentlyViewed(UserProfile user,
            UserSelectedContext catalogInfo,
            string itemNumber,
            bool each,
            string catalogId,
            bool active);

    }
}
