using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core
{
    public interface ICatalogRepository
    {
        ProductsReturn GetProductsByCategory(string branch, string category, string endpoint, int from, int size);
        ProductsReturn GetProductsBySearch(string branch, string search, string endpoint, int from, int size);
        CategoriesReturn GetCategories();
        CategoriesReturn GetCategories(string endpoint, int from = 0, int size = 2000);
        Product GetProductById(string branch, string id, string endpoint);
    }
}
