﻿using KeithLink.Svc.Core.Enumerations.List;

using KeithLink.Svc.Core.Interface.Lists;

using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System.Collections.Generic;
using System.Linq;
using KeithLink.Svc.Core.Models.Lists;
using System;

namespace KeithLink.Svc.Impl.Logic.Lists {
    public class FavoritesListLogicImpl : IFavoritesListLogic {
        #region attributes
        private readonly IFavoritesListRepository _favoritesRepo;
        #endregion

        #region ctor
        public FavoritesListLogicImpl(IFavoritesListRepository favoritesRepo)
        {
            _favoritesRepo = favoritesRepo;
        }
        #endregion

        #region methods

        public List<string> GetFavoritedItemNumbers(UserProfile user, UserSelectedContext catalogInfo)
        {
            List<ListModel> list = ReadList(user, catalogInfo, false);

            return list[0].Items.Select(i => i.ItemNumber).ToList();
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
            throw new NotImplementedException();
        }
        #endregion
    }
}
