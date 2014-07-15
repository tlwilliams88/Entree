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
            new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1, Weight = "1lb" }, 
            new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M, Weight = "2lb"  }, 
            new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M, Weight = "3lb"  },
            new Product { Id = 4, Name = "Hammer Four", Category = "Hardware", Price = 16.94M, Weight = "4lb"  },
            new Product { Id = 5, Name = "Hammer Five", Category = "Hardware", Price = 16.95M, Weight = "5lb"  } 
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

        public IEnumerable<Product> GetProductsForCategory(string category)
        {
            if (String.IsNullOrEmpty(category))
            {
                return products;
            }
            else
            {
                IEnumerable<Product> prods = products.Where(x => x.Category == category);
                return prods;
            }
        }


        public CategoriesReturn GetCategories()
        {
            return new CategoriesReturn() { Categories = categories.ToList() };
        }
    }
}
