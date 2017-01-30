﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System.Dynamic;

namespace KeithLink.Svc.Core.Interface.SiteCatalog
{
    public interface ICatalogRepository
    {
        CategoriesReturn GetCategories(int from, int size, string catalogType);
		ProductsReturn GetHouseProductsByBranch(UserSelectedContext catalogInfo, string brandControlLabel, SearchInputModel searchModel);
        ProductsReturn GetHouseProductNumbersByBranch(UserSelectedContext catalogInfo, string brandControlLabel, SearchInputModel searchModel);
        Product GetProductById(string branch, string id);
		ProductsReturn GetProductsByCategory(UserSelectedContext catalogInfo, string category, SearchInputModel searchModel);
        ProductsReturn GetProductNumbersByCategory(UserSelectedContext catalogInfo, string category, SearchInputModel searchModel);
        ProductsReturn GetProductsByIds(string branch, List<string> ids);
		ProductsReturn GetProductsBySearch(UserSelectedContext catalogInfo, string search, SearchInputModel searchModel);
        int GetHitsForSearchInIndex(UserSelectedContext catalogInfo, string searchTerm, SearchInputModel searchModel);
        List<string> SeekSpecialFilters(string facetFilters);
        void AddSpecialFiltersToFacets(ExpandoObject facets, string count = null);
        ProductsReturn GetProductNumbersBySearch(UserSelectedContext catalogInfo, string search, SearchInputModel searchModel);
    }
}
