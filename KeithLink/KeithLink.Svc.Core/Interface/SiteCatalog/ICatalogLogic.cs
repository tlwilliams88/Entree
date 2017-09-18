using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Profile;

namespace KeithLink.Svc.Core.Interface.SiteCatalog
{
    public interface ICatalogLogic {
        List<OrderHistoryFile> GetLastFiveOrderHistory(UserSelectedContext catalogInfo, string itemNumber);

        void AddPricingInfo(ProductsReturn prods, UserSelectedContext context, SearchInputModel searchModel);

        CategoriesReturn GetCategories(int from, int size, string catalogType);

        List<Division> GetDivisions();

        ShallowProduct GetShallowProductById(string branchId, string id, string catalogType);
            
        Product GetProductById(UserSelectedContext catalogInfo, string id, UserProfile profile, string catalogType);

        Product GetProductByIdOrUPC(UserSelectedContext catalogInfo, string idorupc, UserProfile profile);

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

        string GetCategoryName(string category, SearchInputModel searchModel);

        void AddProductImageInfo(Product ret);

        Dictionary<string, int> GetHitsForCatalogs(UserSelectedContext catalogInfo, string search, SearchInputModel searchModel);
    }
}
