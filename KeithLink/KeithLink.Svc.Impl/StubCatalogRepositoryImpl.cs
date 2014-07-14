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
    }
}
