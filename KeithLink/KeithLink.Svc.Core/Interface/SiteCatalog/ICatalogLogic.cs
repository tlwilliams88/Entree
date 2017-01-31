using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Profile;

namespace KeithLink.Svc.Core.Interface.SiteCatalog
{
    public interface ICatalogLogic
    {
        void AddPricingInfo(ProductsReturn prods, UserSelectedContext context, SearchInputModel searchModel);

        CategoriesReturn GetCategories(int from, int size, string catalogType);

        List<Division> GetDivisions();

        ProductsReturn GetHouseProductsByBranch(UserSelectedContext catalogInfo, string search, SearchInputModel searchModel, UserProfile profile);

        Product GetProductById(UserSelectedContext catalogInfo, string id, UserProfile profile, string catalogType);

        Product GetProductByIdOrUPC(UserSelectedContext catalogInfo, string idorupc, UserProfile profile);

        ProductsReturn GetProductsByCategory(UserSelectedContext catalogInfo, string category, SearchInputModel searchModel, UserProfile profile);

        ProductsReturn GetProductsByIds(string branch, List<string> ids);

        ProductsReturn GetProductsByIdsWithPricing(UserSelectedContext catalogInfo, List<string> ids);

        ProductsReturn GetProductsByItemNumbers(UserSelectedContext context, List<string> ids, SearchInputModel searchModel, UserProfile profile);
        ProductsReturn GetProductsBySearch(UserSelectedContext catalogInfo, string search, SearchInputModel searchModel, UserProfile profile);


        //Catalog Methods
        string GetBranchId(string bekBranchId, string catalogType);

        List<String> GetExternalBranchIds(string bekBranchId);

        bool IsSpecialtyCatalog(string catalogType, string branchId = null);


        bool IsCatalogIdBEK(string catalogId);


        string GetCatalogTypeFromCatalogId(string catalogId);


        Dictionary<string, int> GetHitsForCatalogs(UserSelectedContext catalogInfo, string search, SearchInputModel searchModel);
    }
}
