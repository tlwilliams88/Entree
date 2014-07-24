﻿using System;
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
        public ProductsReturn GetProductsByCategory(string branch, string category, string elasticSearchEndpoint, int from = 0, int size = 500)
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
                KeithLink.Svc.Impl.Models.Generated.Category currCat = (KeithLink.Svc.Impl.Models.Generated.Category)cat;

                if (currCat.ChildCategories != null && currCat.ChildCategories.Count > 0)
                {
                    foreach (var childCat in currCat.ChildCategories)
                    {
                        KeithLink.Svc.Impl.Models.Generated.Category currChildCat = (KeithLink.Svc.Impl.Models.Generated.Category)childCat;
                        Category c = new Category() { Description = currChildCat.DisplayName, Id = currChildCat.Id, Name = currChildCat.DisplayName};
                        ret.Categories.Add(c);
                    }
                }
            }

            return ret;
        }


        public CategoriesReturn GetCategories(string endpoint, int from = 0, int size = 2000)
        {
            throw new NotImplementedException();
        }


        public ProductsReturn GetProductsBySearch(string branch, string search, string endpoint, int from = 0, int size = 500)
        {
            throw new NotImplementedException();
        }

        public Product GetProductById(string branch, string id, string endpoint)
        {
            throw new NotImplementedException();
        }
    }
}
