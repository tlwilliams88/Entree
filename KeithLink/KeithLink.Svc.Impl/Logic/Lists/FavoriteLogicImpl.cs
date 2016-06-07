using KeithLink.Svc.Core.Enumerations.List;

using KeithLink.Svc.Core.Interface.Lists;

using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System.Collections.Generic;
using System.Linq;

namespace KeithLink.Svc.Impl.Logic.Lists {
    public class FavoriteLogicImpl : IFavoriteLogic {
        #region attributes
        private readonly IListRepository _repo;
        #endregion

        #region ctor
        public FavoriteLogicImpl(IListRepository listRepository) {
            _repo = listRepository;
        }
        #endregion

        #region methods
        public List<string> GetFavoritedItemNumbers(UserProfile user, UserSelectedContext catalogInfo) {
            var list = _repo.Read(l => l.UserId == user.UserId && 
                                       l.CustomerId.Equals(catalogInfo.CustomerId) && 
                                       l.Type == ListType.Favorite, 
                                  i => i.Items)
                            .ToList();

            if(list == null) {
                return new List<string>();
            } else {
                return list.SelectMany(i => i.Items.Select(x => x.ItemNumber)).ToList();
            }
        }
        #endregion
    }
}
