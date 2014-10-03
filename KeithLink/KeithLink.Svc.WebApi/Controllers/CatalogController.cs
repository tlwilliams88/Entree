using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core;
using System.Web.Http.Cors;
using System.Dynamic;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.WebApi.Models;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Interface.Profile;

namespace KeithLink.Svc.WebApi.Controllers
{
	[Authorize]
    public class CatalogController : BaseController
    {
        #region attributes
        KeithLink.Svc.Core.Interface.SiteCatalog.ICatalogLogic _catalogLogic;
        #endregion

        #region ctor
        public CatalogController(ICatalogLogic catalogLogic, IUserProfileLogic profileLogic) : base(profileLogic) {
            _catalogLogic = catalogLogic;
        }
        #endregion

        #region methods
        [HttpGet]
        [ApiKeyedRoute("catalog/categories")]
        public CategoriesReturn GetCategories()
        {
            IEnumerable<KeyValuePair<string, string>> pairs = Request.GetQueryNameValuePairs();
            return _catalogLogic.GetCategories(0, 2000);
        }

        [HttpGet]
        [ApiKeyedRoute("catalog/category/{id}/categories")]
        public CategoriesReturn GetSubCategoriesByParentId(string id, [FromUri] SearchInputModel searchModel)
        {
            return _catalogLogic.GetCategories(searchModel.From, searchModel.Size);
        }

        [HttpGet]
        [ApiKeyedRoute("catalog/search/category/{categoryId}/products")]
        public ProductsReturn GetProductsByCategoryId(string categoryId, [FromUri] SearchInputModel searchModel)
        {
			ProductsReturn prods = _catalogLogic.GetProductsByCategory(this.RequestCatalogInfo.BranchId, categoryId, searchModel, this.AuthenticatedUser);
            return prods;
        }

        [HttpGet]
        [ApiKeyedRoute("catalog/search/brands/house/{brandControlLabel}")]
        public ProductsReturn GetProductsByHouseBrand(string brandControlLabel, [FromUri] SearchInputModel searchModel)
        {
            return _catalogLogic.GetHouseProductsByBranch(this.RequestCatalogInfo.BranchId, brandControlLabel, searchModel, this.AuthenticatedUser);
        }

        [HttpGet]
        [ApiKeyedRoute("catalog/category/{id}")]
		public CategoriesReturn GetCategoriesById(string id, [FromUri] SearchInputModel searchModel)
        {
			//TODO: This is not actually getting a category by ID (ID is never used).
			return _catalogLogic.GetCategories(searchModel.From, searchModel.Size);
        }

        [HttpGet]
        [ApiKeyedRoute("catalog/product/{id}")]
        public Product GetProductById(string id)
        {
            IEnumerable<KeyValuePair<string, string>> pairs = Request.GetQueryNameValuePairs();
            Product prod = _catalogLogic.GetProductById(this.RequestCatalogInfo.BranchId, id, this.AuthenticatedUser);

            if (prod == null)
                return new Product();

            return prod;
        }

        [HttpGet]
        [ApiKeyedRoute("catalog/search/{searchTerms}/products")]
		public ProductsReturn GetProductsSearch(string searchTerms, [FromUri] SearchInputModel searchModel)
        {
            ProductsReturn prods = _catalogLogic.GetProductsBySearch(this.RequestCatalogInfo.BranchId, searchTerms, searchModel, this.AuthenticatedUser);
            return prods;
        }

		[HttpGet]
		[AllowAnonymous]
		[ApiKeyedRoute("catalog/divisions")]
		public List<Division> GetDivisions()
		{
			return _catalogLogic.GetDivisions();
		}

        #endregion
    }
}