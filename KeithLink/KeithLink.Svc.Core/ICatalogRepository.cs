using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core
{
    public interface ICatalogRepository
    {
        ProductsReturn GetProductsByCategory(string branch, string category, int from, int size);
        ProductsReturn GetProductsBySearch(string branch, string search, int from, int size);
        CategoriesReturn GetCategories(int from, int size);
        Product GetProductById(string branch, string id);
    }
}
