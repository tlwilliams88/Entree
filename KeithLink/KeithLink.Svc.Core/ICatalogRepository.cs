using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Catalog
{
    public interface ICatalogRepository
    {
        ProductsReturn GetProductsByCategory(string branch, string category, int from, int size, string facetFilters);
        ProductsReturn GetProductsBySearch(string branch, string search, int from, int size, string facetFilters);
        CategoriesReturn GetCategories(int from, int size);
        Product GetProductById(string branch, string id);
    }
}
