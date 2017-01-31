using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using System.Dynamic;

namespace KeithLink.Svc.Impl.Repository.SiteCatalog
{
    public class StubCatalogRepositoryImpl : ICatalogRepository
    {
        Product[] products = new Product[] 
        { 
            new Product { ItemNumber = "1", Description = "Tomato Soup", CategoryCode = "Groceries", CasePrice = "1.00", Size = "1lb" }, 
            new Product { ItemNumber = "2", Description = "Yo-yo", CategoryCode = "Toys", CasePrice = "3.75", Size = "2lb"  }, 
            new Product { ItemNumber = "3", Description = "Hammer", CategoryCode = "Hardware", CasePrice = "16.99", Size = "3lb"  },
            new Product { ItemNumber = "4", Description = "Hammer Four", CategoryCode = "Hardware", CasePrice = "16.94", Size = "4lb"  },
            new Product { ItemNumber = "5", Description = "Hammer Five", CategoryCode = "Hardware", CasePrice = "16.95", Size = "5lb"  } 
        };

        Category[] categories = new Category[]
        {
            new Category() { Name = "Shrimp", Description="Shrimp, Headless, Raw", Id="FS940"
                , SubCategories = new SubCategory[] {
                    new SubCategory() { Id = "FS942", Name="Jumbo Shrimp", Description="Shrimp, Headless, Raw 12 Ct"},
                    new SubCategory() { Id = "FS941", Name="Popcorn Shrimp", Description="Shrimp, Headless, Raw 65 Ct"}}},
            new Category() { Name = "Cakes", Description="Cakes Silly", Id="CS940"
                , SubCategories = new SubCategory[] {
                    new SubCategory() { Id = "CS942", Name="Cakes, Decorated", Description="Decorated Cakes"},
                    new SubCategory() { Id = "CS941", Name="Cakes, Bulk", Description="Un-decorated cakes"}}},
            new Category() { Name = "Pork", Description="Soo weee!!!", Id="PS940"
                , SubCategories = new SubCategory[] {
                    new SubCategory() { Id = "PS942", Name="Pork Shoulder", Description="Bulk Pork Shoulder"},
                    new SubCategory() { Id = "PS941", Name="Pork, Chops", Description="Pork Chops"}}},
        };

		public ProductsReturn GetProductsByCategory(string branch, string category, SearchInputModel searchModel)
        {
            if (String.IsNullOrEmpty(category))
            {
                return new ProductsReturn() { Products = products.ToList() };
            }
            else
            {
                IEnumerable<Product> prods = products.Where(x => x.CategoryCode == category);
                return new ProductsReturn() { Products = prods.ToList() };
            }
        }

        public ProductsReturn GetHouseProductsByBranch(string branch, string controlLabel, SearchInputModel searchModel)
        {
            return new ProductsReturn();
        }

        public CategoriesReturn GetCategories()
        {
            return new CategoriesReturn() { Categories = categories.ToList() };
        }


        public CategoriesReturn GetCategories(int from = 0, int size = 2000, string catalogType = "BEK")
        {
            throw new NotImplementedException();
        }


        public ProductsReturn GetProductsBySearch(string branch, string search, SearchInputModel searchModel)
        {
            throw new NotImplementedException();
        }

        public Product GetProductById(string branch, string id)
        {
            throw new NotImplementedException();
        }


		public ProductsReturn GetProductsByIds(string branch, List<string> ids)
		{
			throw new NotImplementedException();
		}

		public ProductsReturn GetProductsByItemNumbers(string branch, List<string> ids, SearchInputModel model = null)
		{
			throw new NotImplementedException();
		}

		public ProductsReturn GetProductsByCategory(UserSelectedContext catalogInfo, string category, SearchInputModel searchModel)
		{
			throw new NotImplementedException();
		}

        public ProductsReturn GetProductsBySearch(UserSelectedContext catalogInfo, string search, SearchInputModel searchModel)
		{
			throw new NotImplementedException();
		}

		public ProductsReturn GetHouseProductsByBranch(UserSelectedContext catalogInfo, string brandControlLabel, SearchInputModel searchModel)
		{
			throw new NotImplementedException();
		}


        public int GetHitsForSearchInIndex(UserSelectedContext catalogInfo, string searchTerm, SearchInputModel searchModel)
        {
            throw new NotImplementedException();
        }

        public List<string> SeekSpecialFilters(string facetFilters)
        {
            throw new NotImplementedException();
        }

        public ProductsReturn GetProductNumbersByCategory(UserSelectedContext catalogInfo, string category, SearchInputModel searchModel)
        {
            throw new NotImplementedException();
        }

        public void AddSpecialFiltersToFacets(ExpandoObject facets, string count = null)
        {
            throw new NotImplementedException();
        }

        public ProductsReturn GetProductNumbersBySearch(UserSelectedContext catalogInfo, string search, SearchInputModel searchModel)
        {
            throw new NotImplementedException();
        }

        public ProductsReturn GetHouseProductNumbersByBranch(UserSelectedContext catalogInfo, string brandControlLabel, SearchInputModel searchModel)
        {
            throw new NotImplementedException();
        }
    }
}
