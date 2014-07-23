using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core;

namespace KeithLink.Svc.Impl
{
    public class StubCatalogRepositoryImpl : ICatalogRepository
    {
        Product[] products = new Product[] 
        { 
            new Product { ItemNumber = "1", Description = "Tomato Soup", CategoryId = "Groceries", CasePrice = "1.00", Size = "1lb" }, 
            new Product { ItemNumber = "2", Description = "Yo-yo", CategoryId = "Toys", CasePrice = "3.75", Size = "2lb"  }, 
            new Product { ItemNumber = "3", Description = "Hammer", CategoryId = "Hardware", CasePrice = "16.99", Size = "3lb"  },
            new Product { ItemNumber = "4", Description = "Hammer Four", CategoryId = "Hardware", CasePrice = "16.94", Size = "4lb"  },
            new Product { ItemNumber = "5", Description = "Hammer Five", CategoryId = "Hardware", CasePrice = "16.95", Size = "5lb"  } 
        };

        Category[] categories = new Category[]
        {
            new Category() { Name = "Shrimp", Description="Shrimp, Headless, Raw", Id="FS940"
                , SubCategories = new Category[] {
                    new Category() { Id = "FS942", Name="Jumbo Shrimp", Description="Shrimp, Headless, Raw 12 Ct"},
                    new Category() { Id = "FS941", Name="Popcorn Shrimp", Description="Shrimp, Headless, Raw 65 Ct"}}},
            new Category() { Name = "Cakes", Description="Cakes Silly", Id="CS940"
                , SubCategories = new Category[] {
                    new Category() { Id = "CS942", Name="Cakes, Decorated", Description="Decorated Cakes"},
                    new Category() { Id = "CS941", Name="Cakes, Bulk", Description="Un-decorated cakes"}}},
            new Category() { Name = "Pork", Description="Soo weee!!!", Id="PS940"
                , SubCategories = new Category[] {
                    new Category() { Id = "PS942", Name="Pork Shoulder", Description="Bulk Pork Shoulder"},
                    new Category() { Id = "PS941", Name="Pork, Chops", Description="Pork Chops"}}},
        };

        public ProductsReturn GetProductsByCategory(string branch, string category, string elasticSearchEndpoint)
        {
            if (String.IsNullOrEmpty(category))
            {
                return new ProductsReturn() { Products = products.ToList() };
            }
            else
            {
                IEnumerable<Product> prods = products.Where(x => x.CategoryId == category);
                return new ProductsReturn() { Products = prods.ToList() };
            }
        }

        public CategoriesReturn GetCategories()
        {
            return new CategoriesReturn() { Categories = categories.ToList() };
        }


        public CategoriesReturn GetCategories(string endpoint)
        {
            throw new NotImplementedException();
        }


        public ProductsReturn GetProductsBySearch(string branch, string search, string endpoint)
        {
            throw new NotImplementedException();
        }

        public Product GetProductById(string branch, string id, string endpoint)
        {
            throw new NotImplementedException();
        }
    }
}
