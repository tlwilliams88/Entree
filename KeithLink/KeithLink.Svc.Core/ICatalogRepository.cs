using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core
{
    public interface ICatalogRepository
    {
        ProductsReturn GetProductsByCategory(string branch, string category, string endpoint);
        ProductsReturn GetProductsBySearch(string branch, string search, string endpoint);
        CategoriesReturn GetCategories();
        CategoriesReturn GetCategories(string endpoint);
        Product GetProductById(string branch, string id, string endpoint);
    }
}
