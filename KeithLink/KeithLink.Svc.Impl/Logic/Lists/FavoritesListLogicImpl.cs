using KeithLink.Svc.Core.Enumerations.List;

using KeithLink.Svc.Core.Interface.Lists;

using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System.Collections.Generic;
using System.Linq;
using KeithLink.Svc.Core.Models.Lists;
using System;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Models.Lists.Favorites;

namespace KeithLink.Svc.Impl.Logic.Lists {
    public class FavoritesListLogicImpl : IFavoritesListLogic {
        #region attributes
        private readonly IFavoriteListDetailRepository _detailRepo;
        private readonly IFavoriteListHeaderRepository _headerRepo;
        #endregion

        #region ctor
        public FavoritesListLogicImpl(IFavoriteListDetailRepository detailRepository, IFavoriteListHeaderRepository headerRepository) {
            _detailRepo = detailRepository;
            _headerRepo = headerRepository;
        }
        #endregion

        #region methods

        public List<string> GetFavoritedItemNumbers(UserProfile user, UserSelectedContext catalogInfo) {
            return GetFavoritesList(user.UserName, catalogInfo, true)
                        .Items
                        .Select(i => i.ItemNumber)
                        .ToList();
        }

        public ListModel GetFavoritesList(string userId, UserSelectedContext catalogInfo, bool headerOnly) {
            FavoritesListHeader header = _headerRepo.GetFavoritesList(userId, catalogInfo);

            if (header == null) {
                return null;
            } else {
                if(!headerOnly) {
                    header.Items = _detailRepo.GetFavoritesListDetails(header.Id);
                }

                return header.ToListModel(catalogInfo);
            }
        }

        public List<ListModel> ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly)
        {
            return _favoritesRepo.GetFavoritesList(user.UserId.ToString(), catalogInfo, headerOnly);
        }

        public void AddOrUpdateFavorite(UserProfile user, 
                                UserSelectedContext catalogInfo,
                                string itemNumber,
                                bool each,
                                string catalogId,
                                bool active)
        {
            _favoritesRepo.AddOrUpdateFavorite(user.UserId.ToString(),
                catalogInfo.CustomerId,
                catalogInfo.BranchId,
                itemNumber,
                each,
                catalogId,
                active);
        }

        public ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long Id)
        {
            return ReadList(user, catalogInfo, false)[0];
        }
        #endregion
    }
}
