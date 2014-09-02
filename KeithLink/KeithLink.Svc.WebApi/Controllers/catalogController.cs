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
	//[Authorize]
    public class CatalogController : BaseController
    {
        #region attributes
        KeithLink.Svc.Core.Interface.SiteCatalog.ICatalogLogic _catalogLogic;
        KeithLink.Svc.Core.Interface.SiteCatalog.IProductImageRepository _imgRepository;
		private readonly IListLogic _listLogic;
        #endregion

        #region ctor
        public CatalogController(ICatalogLogic catalogLogic, IListLogic listLogic,
                                 IUserProfileRepository userProfileRepo, IProductImageRepository imgRepository)
            : base(userProfileRepo)
        {
            _catalogLogic = catalogLogic;
            _imgRepository = imgRepository;
            _listLogic = listLogic;
        }
        #endregion

        #region methods
        [HttpGet]
        [Route("catalog/categories")]
        public CategoriesReturn GetCategories()
        {
            IEnumerable<KeyValuePair<string, string>> pairs = Request.GetQueryNameValuePairs();
            return _catalogLogic.GetCategories(0, 2000);
        }

        [HttpGet]
        [Route("catalog/category/{id}/categories")]
        public CategoriesReturn GetSubCategoriesByParentId(string id, [FromUri] SearchInputModel searchModel)
        {
            return _catalogLogic.GetCategories(searchModel.From, searchModel.Size);
        }

        [HttpGet]
        [Route("catalog/search/category/{branchId}/{categoryId}/products")]
        public ProductsReturn GetProductsByCategoryId(string branchId, string categoryId, [FromUri] SearchInputModel searchModel)
        {

			ProductsReturn prods = _catalogLogic.GetProductsByCategory(branchId, categoryId, searchModel, this.AuthenticatedUser);
			
			if(this.AuthenticatedUser != null)
				_listLogic.MarkFavoriteProducts(this.AuthenticatedUser.UserId, branchId, prods);

            return prods;
        }

        [HttpGet]
        [Route("catalog/category/{id}")]
		public CategoriesReturn GetCategoriesById(string id, [FromUri] SearchInputModel searchModel)
        {
			//TODO: This is not actually getting a category by ID (ID is never used).
			return _catalogLogic.GetCategories(searchModel.From, searchModel.Size);
        }

        [HttpGet]
        [Route("catalog/product/{branchId}/{id}")]
        public Product GetProductById(string branchId, string id)
        {
            IEnumerable<KeyValuePair<string, string>> pairs = Request.GetQueryNameValuePairs();
            Product prod = _catalogLogic.GetProductById(branchId, id, this.AuthenticatedUser);

            if (prod == null)
            {
                prod = new Product();
            } 
            else 
            {
                ProductsReturn prods = new ProductsReturn() { Products = new List<Product>() { prod } };

                if (this.AuthenticatedUser != null)
                    _listLogic.MarkFavoriteProducts(this.AuthenticatedUser.UserId, branchId, prods);

                prod.ProductImages = _imgRepository.GetImageList(prod.ItemNumber).ProductImages;
            }
            return prod;
        }

        [HttpGet]
        [Route("catalog/search/{branchId}/{searchTerms}/products")]
		public ProductsReturn GetProductsSearch(string branchId, string searchTerms, [FromUri] SearchInputModel searchModel)
        {
            ProductsReturn prods = _catalogLogic.GetProductsBySearch(branchId, searchTerms, searchModel, this.AuthenticatedUser);

			if (this.AuthenticatedUser != null)
				_listLogic.MarkFavoriteProducts(this.AuthenticatedUser.UserId, branchId, prods);

            return prods;
        }

        #endregion
    }
}