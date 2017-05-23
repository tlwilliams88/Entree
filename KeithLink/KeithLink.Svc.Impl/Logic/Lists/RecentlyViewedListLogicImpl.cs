using KeithLink.Svc.Core.Enumerations.List;

using KeithLink.Svc.Core.Interface.Lists;

using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System.Collections.Generic;
using System.Linq;
using KeithLink.Svc.Core.Models.Lists;
using System;

namespace KeithLink.Svc.Impl.Logic.Lists {
    public class RecentlyViewedListLogicImpl : IRecentlyViewedListLogic
    {
        #region attributes
        private readonly IRecentlyViewedListRepository _recentlyViewedRepo;
        #endregion

        #region ctor
        public RecentlyViewedListLogicImpl(IRecentlyViewedListRepository recentlyViewedRepo)
        {
            _recentlyViewedRepo = recentlyViewedRepo;
        }
        #endregion

        #region methods

        public List<string> GetRecentlyViewedItemNumbers(UserProfile user, UserSelectedContext catalogInfo)
        {
            List<ListModel> list = ReadList(user, catalogInfo, false);

            return list[0].Items.Select(i => i.ItemNumber).ToList();
        }

        public List<ListModel> ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly)
        {
            return _recentlyViewedRepo.GetRecentlyViewedList(user.UserId.ToString(), catalogInfo, headerOnly);
        }

        public void AddRecentlyViewedItem(UserProfile user, UserSelectedContext catalogInfo, string itemNumber)
        {
            AddOrUpdateRecentlyViewed(user, catalogInfo, itemNumber, false, catalogInfo.BranchId, true);
        }

        public void AddOrUpdateRecentlyViewed(UserProfile user, 
                                UserSelectedContext catalogInfo,
                                string itemNumber,
                                bool each,
                                string catalogId,
                                bool active)
        {
            _recentlyViewedRepo.AddOrUpdateRecentlyViewed(user.UserId.ToString(),
                catalogInfo.CustomerId,
                catalogInfo.BranchId,
                itemNumber,
                each,
                catalogId,
                active);
        }

        public ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long Id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
