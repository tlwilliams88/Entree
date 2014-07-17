using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core;
using RT = KeithLink.Svc.Impl.RequestTemplates;

namespace KeithLink.Svc.Impl
{
    public class CommerceServerCatalogRepositoryImpl : ICatalogRepository
    {
        public IEnumerable<Product> GetProductsForCategory(string category)
        {
            throw new NotImplementedException();
        }

        public CategoriesReturn GetCategories()
        {
            var categories = RT.Catalog.GetCategoryWithChildProducts(
                    "Backpacks",
                    "Adventure Works Catalog",
                    string.IsNullOrWhiteSpace("") ? "0" : "0",
                    "10",
                    "true",
                    null,
                    null,
                    null,
                    null,
                    null);

            CategoriesReturn ret = new CategoriesReturn();
            ret.Categories = new List<Category>(); 
            foreach (var cat in categories)
            {
                KeithLink.Svc.Impl.Models.Generated.Category currCat = (KeithLink.Svc.Impl.Models.Generated.Category)cat;
                Category c = new Category() { Description = currCat.Description, Id = currCat.Id, Name = currCat.Name };
                if (currCat.ChildCategories != null && currCat.ChildCategories.Count > 0)
                {
                }

                ret.Categories.Add(c);
            }

            return ret;
        }
    }
}
