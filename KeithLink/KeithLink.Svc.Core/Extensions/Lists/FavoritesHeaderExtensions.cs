using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Favorites;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class FavoritesHeaderExtensions {
        #region attributes
        private const string LISTNAME_FAVORITE = "Favorites";
        #endregion

        #region methods

        public static ListModel ToListModel(this FavoritesListHeader header) {
            return new ListModel() {
                BranchId = header.BranchId,
                IsContractList = false,
                IsFavorite = true,
                IsWorksheet = false,
                IsReminder = false,
                IsMandatory = false,
                IsRecommended = false,
                IsCustomInventory = false,
                Type = ListType.Favorite,
                ListId = header.Id,
                Name = LISTNAME_FAVORITE,
                ReadOnly = false,
                Items = null
            };
        }

        public static ListModel ToListModel(this FavoritesListHeader header, List<FavoritesListDetail> items) {
            ListModel retVal = header.ToListModel();
            retVal.Items = items.Select(i => i.ToWebModel())
                                .OrderBy(l => l.Position)
                                .ToList();
            return retVal;
        }

        public static ListModel ToListModel(this FavoritesListHeader header, List<ListItemModel> items) {
            ListModel retVal = header.ToListModel();
            retVal.Items = items.OrderBy(l => l.Position)
                                .ToList();
            return retVal;
        }
        #endregion
    }
}
