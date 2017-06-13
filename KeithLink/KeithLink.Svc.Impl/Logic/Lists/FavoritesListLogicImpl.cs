using System;
using System.Collections.Generic;
using System.Linq;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Favorites;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

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
            return GetFavoritesList(user.UserId, catalogInfo, true)
                        .Items
                        .Select(i => i.ItemNumber)
                        .ToList();
        }

        public ListModel GetFavoritesList(Guid userId, UserSelectedContext catalogInfo, bool headerOnly) {
            FavoritesListHeader header = _headerRepo.GetFavoritesList(userId, catalogInfo);
        
            if (header == null) {
                return null;
            } else {
                List<FavoritesListDetail> items = null;

                if(!headerOnly) {
                    items = _detailRepo.GetFavoritesListDetails(header.Id);
                }

                return header.ToListModel(items);
            }
        }

        public ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long Id) {
            return GetFavoritesList(user.UserId, catalogInfo, false);
        }

        public void Save(UserProfile user, UserSelectedContext catalogInfo, FavoritesListDetail model) {
            // try to find the parent header id if it is not in the model
            if (model.ParentFavoritesHeaderId == 0) {
                FavoritesListHeader header = _headerRepo.GetFavoritesList(user.UserId, catalogInfo);

                if (header == null) {
                    // create the header
                    model.ParentFavoritesHeaderId = _headerRepo.SaveFavoriteListHeader(new FavoritesListHeader()
                    {
                        BranchId = catalogInfo.BranchId,
                        CustomerNumber = catalogInfo.CustomerId,
                        UserId = user.UserId
                    });
                } else {
                    model.ParentFavoritesHeaderId = header.Id;
                }
            }

            _detailRepo.SaveFavoriteListDetail(model);
        }

        #endregion
    }
}
