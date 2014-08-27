﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.SiteCatalog;
using RT = KeithLink.Svc.Impl.RequestTemplates;
using KeithLink.Svc.Core.Interface.SiteCatalog;

namespace KeithLink.Svc.Impl.Repository.SiteCatalog
{
    public class CommerceServerCatalogRepositoryImpl : ICatalogRepository
    {
        public ProductsReturn GetProductsByCategory(string branch, string category, int from, int size, string facetFilters)
        {
            throw new NotImplementedException();
        }

        public ProductsReturn GetProductsByIds(string branch, List<string> ids)
        {
            throw new NotImplementedException();
        }

        public CategoriesReturn GetCategories()
        {
            CategoriesReturn ret = new CategoriesReturn();
            ret.Categories = new List<Category>(); 

            var categories = RT.Catalog.GetTopLevelCategories("FAM", "500", "true", null);
            foreach (var cat in categories)
            {
				KeithLink.Svc.Core.Models.Generated.Category currCat = (KeithLink.Svc.Core.Models.Generated.Category)cat;

                if (currCat.ChildCategories != null && currCat.ChildCategories.Count > 0)
                {
                    foreach (var childCat in currCat.ChildCategories)
                    {
						KeithLink.Svc.Core.Models.Generated.Category currChildCat = (KeithLink.Svc.Core.Models.Generated.Category)childCat;
                        Category c = new Category() { Description = currChildCat.DisplayName, Id = currChildCat.Id, Name = currChildCat.DisplayName};
                        ret.Categories.Add(c);
                    }
                }
            }

            return ret;
        }


        public CategoriesReturn GetCategories(int from = 0, int size = 2000)
        {
            throw new NotImplementedException();
        }


		public ProductsReturn GetProductsBySearch(string branch, string search, int from, int size, string facetFilters, string sortField, string sortDir)
        {
            throw new NotImplementedException();
        }

        public Product GetProductById(string branch, string id)
        {
            throw new NotImplementedException();
        }

		public ProductsReturn GetProductsByCategory(string branch, string category, int from, int size, string facetFilters, string sortField, string sortDir)
		{
			throw new NotImplementedException();
		}
	}
}
