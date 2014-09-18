using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.SiteCatalog
{
    public interface ICatalogRepository
    {
        ProductsReturn GetProductsByCategory(string branch, string category, SearchInputModel searchModel);
		ProductsReturn GetProductsBySearch(string branch, string search, SearchInputModel searchModel);
        ProductsReturn GetHouseProductsByBranch(string branchId, string brandControlLabel, SearchInputModel searchModel);
        CategoriesReturn GetCategories(int from, int size);
        Product GetProductById(string branch, string id);
		ProductsReturn GetProductsByIds(string branch, List<string> ids);
    }
}
