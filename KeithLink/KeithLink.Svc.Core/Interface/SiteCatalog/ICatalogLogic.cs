using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Profile;

namespace KeithLink.Svc.Core.Interface.SiteCatalog
{
    public interface ICatalogLogic
    {
		ProductsReturn GetProductsByCategory(UserSelectedContext catalogInfo, string category, SearchInputModel searchModel, UserProfile profile);
        ProductsReturn GetProductsBySearch(UserSelectedContext catalogInfo, string search, SearchInputModel searchModel, UserProfile profile);
		ProductsReturn GetHouseProductsByBranch(UserSelectedContext catalogInfo, string search, SearchInputModel searchModel, UserProfile profile);
        ProductsReturn GetProductsByIds(string branch, List<string> ids, UserProfile profile);
        Product GetProductById(UserSelectedContext catalogInfo, string id, UserProfile profile);
        CategoriesReturn GetCategories(int from, int size);
		List<Division> GetDivisions();
    }
}
