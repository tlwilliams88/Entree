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
        private readonly IFavoriteListDetailsRepository _detailRepo;
        private readonly IFavoriteListHeadersRepository _headerRepo;
        #endregion

        #region ctor
        public FavoritesListLogicImpl(IFavoriteListDetailsRepository detailRepository, IFavoriteListHeadersRepository headerRepository) {
            _detailRepo = detailRepository;
            _headerRepo = headerRepository;
        }
        #endregion

        #region methods
        public List<string> GetFavoritedItemNumbers(UserProfile user, UserSelectedContext catalogInfo) {
            ListModel list = GetFavoritesList(user.UserId, catalogInfo, false);
            if (list != null && list.Items != null) {
                return list
                        .Items
                        .Select(i => i.ItemNumber)
                        .ToList();
            }

            return new List<string>();
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

        public ListModel SaveList(UserProfile user, UserSelectedContext catalogInfo, ListModel list)
        {
            FavoritesListHeader header = _headerRepo.GetFavoritesList(user.UserId, catalogInfo);

            if (header == null)
            {
                foreach (var item in list.Items)
                {
                    FavoritesListDetail detail = item.ToFavoritesListDetail(0);
                    detail.Active = !item.IsDelete;
                    Save(user, catalogInfo, detail);
                }
            }
            else
            {
                foreach (var item in list.Items)
                {
                    FavoritesListDetail detail = item.ToFavoritesListDetail(header.Id);
                    detail.Active = !item.IsDelete;
                    Save(user, catalogInfo, detail);
                }
            }

            return GetFavoritesList(user.UserId, catalogInfo, false);
        }

        public void Save(UserProfile user, UserSelectedContext catalogInfo, FavoritesListDetail model) {
            // try to find the parent header id if it is not in the model
            if (model.HeaderId == 0) {
                FavoritesListHeader header = _headerRepo.GetFavoritesList(user.UserId, catalogInfo);

                if (header == null) {
                    // create the header
                    model.HeaderId = _headerRepo.SaveFavoriteListHeader(new FavoritesListHeader()
                    {
                        BranchId = catalogInfo.BranchId,
                        CustomerNumber = catalogInfo.CustomerId,
                        UserId = user.UserId
                    });
                } else {
                    model.HeaderId = header.Id;
                }
            }

            _detailRepo.SaveFavoriteListDetail(model);
        }

        public long CreateList(UserProfile user, UserSelectedContext catalogInfo) {
            FavoritesListHeader header = _headerRepo.GetFavoritesList(user.UserId, catalogInfo);

            if (header == null) {
                // create the header
                return _headerRepo.SaveFavoriteListHeader(new FavoritesListHeader() {
                                                                                        BranchId = catalogInfo.BranchId,
                                                                                        CustomerNumber = catalogInfo.CustomerId,
                                                                                        UserId = user.UserId
                                                                                    });
            } else {
                return header.Id;
            }
        }
        #endregion
    }
}
