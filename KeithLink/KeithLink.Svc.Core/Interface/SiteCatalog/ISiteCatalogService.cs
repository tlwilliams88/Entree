using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Profile;

namespace KeithLink.Svc.Core.Interface.SiteCatalog
{
    public interface ISiteCatalogService
    {
        ProductsReturn GetProductsByCategory(UserSelectedContext catalogInfo,
                                             string category,
                                             SearchInputModel searchModel,
                                             UserProfile profile);

        ProductsReturn GetProductsBySearch(UserSelectedContext catalogInfo,
                                           string search,
                                           SearchInputModel searchModel,
                                           UserProfile profile);

        ProductsReturn GetHouseProductsByBranch(UserSelectedContext catalogInfo,
                                                string brandControlLabel,
                                                SearchInputModel searchModel,
                                                UserProfile profile);

        ProductsReturn GetRecommendedItemsForCart(UserSelectedContext catalogInfo,
                                                  List<string> cartItems,
                                                  UserProfile profile);
    }
}
