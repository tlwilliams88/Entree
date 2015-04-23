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
using KeithLink.Svc.Impl.Helpers;
using System.IO;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Interface.Configuration;

namespace KeithLink.Svc.WebApi.Controllers
{
	[Authorize]
    public class CatalogController : BaseController
    {
        #region attributes
        KeithLink.Svc.Core.Interface.SiteCatalog.ICatalogLogic _catalogLogic;
		private readonly IExportSettingServiceRepository _exportSettingRepository;
       
        #endregion

        #region ctor
		public CatalogController(ICatalogLogic catalogLogic, IUserProfileLogic profileLogic, IExportSettingServiceRepository exportSettingRepository)
			: base(profileLogic)
		{
            _catalogLogic = catalogLogic;
			this._exportSettingRepository = exportSettingRepository;
        }
        #endregion

        #region methods
		/// <summary>
		/// Retrieve all product categories
		/// </summary>
		/// <returns>List of Categories</returns>
        [HttpGet]
        [ApiKeyedRoute("catalog/categories")]
        public CategoriesReturn GetCategories()
        {
            IEnumerable<KeyValuePair<string, string>> pairs = Request.GetQueryNameValuePairs();
            return _catalogLogic.GetCategories(0, 2000);
        }

		/// <summary>
		/// Retrieve Sub-Categories
		/// </summary>
		/// <param name="id">Parent Category</param>
		/// <param name="searchModel">Contains From and Size for specifing items to be retreived</param>
		/// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("catalog/category/{id}/categories")]
        public CategoriesReturn GetSubCategoriesByParentId(string id, [FromUri] SearchInputModel searchModel)
        {
            return _catalogLogic.GetCategories(searchModel.From, searchModel.Size);
        }

		/// <summary>
		/// Retrieve Products for a Category
		/// </summary>
		/// <param name="categoryId">Category Id</param>
		/// <param name="searchModel"></param>
		/// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("catalog/search/category/{categoryId}/products")]
        public ProductsReturn GetProductsByCategoryId(string categoryId, [FromUri] SearchInputModel searchModel)
        {
			ProductsReturn prods = _catalogLogic.GetProductsByCategory(this.SelectedUserContext, categoryId, searchModel, this.AuthenticatedUser);
            return prods;
        }

		/// <summary>
		/// Retrieve Products by house brand
		/// </summary>
		/// <param name="brandControlLabel">House brand</param>
		/// <param name="searchModel"></param>
		/// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("catalog/search/brands/house/{brandControlLabel}")]
        public ProductsReturn GetProductsByHouseBrand(string brandControlLabel, [FromUri] SearchInputModel searchModel)
        {
            return _catalogLogic.GetHouseProductsByBranch(this.SelectedUserContext, brandControlLabel, searchModel, this.AuthenticatedUser);
        }

		/// <summary>
		/// Get Categories
		/// </summary>
		/// <param name="id">This is not currently being used</param>
		/// <param name="searchModel"></param>
		/// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("catalog/category/{id}")]
		public CategoriesReturn GetCategoriesById(string id, [FromUri] SearchInputModel searchModel)
        {
			//TODO: This is not actually getting a category by ID (ID is never used).
			return _catalogLogic.GetCategories(searchModel.From, searchModel.Size);
        }

		/// <summary>
		/// Get Product by Id
		/// </summary>
		/// <param name="id">Product Id (itemnumber)</param>
		/// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("catalog/product/{id}")]
        public Product GetProductById(string id)
        {
            IEnumerable<KeyValuePair<string, string>> pairs = Request.GetQueryNameValuePairs();
            Product prod = _catalogLogic.GetProductById(this.SelectedUserContext, id, this.AuthenticatedUser);

            if (prod == null)
                return new Product();

            return prod;
        }

		/// <summary>
		/// Get Product by Id or UPC
		/// </summary>
		/// <param name="idorupc">Can be Itemnumber or UPC</param>
		/// <returns></returns>
		[HttpGet]
		[ApiKeyedRoute("catalog/product/scan/{idorupc}")]
		public Product GetProductByIdorUPC(string idorupc)
		{
			Product prod = _catalogLogic.GetProductByIdorUPC(this.SelectedUserContext, idorupc, this.AuthenticatedUser);
					
			return prod;
		}

		/// <summary>
		/// Search products
		/// </summary>
		/// <param name="searchTerms">Product Search term</param>
		/// <param name="searchModel"></param>
		/// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("catalog/search/{searchTerms}/products")]
		public ProductsReturn GetProductsSearch(string searchTerms, [FromUri] SearchInputModel searchModel)
        {
            ProductsReturn prods = _catalogLogic.GetProductsBySearch(this.SelectedUserContext, searchTerms, searchModel, this.AuthenticatedUser);
            return prods;
        }
			
		/// <summary>
		/// Retrieve divisions
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[AllowAnonymous]
		[ApiKeyedRoute("catalog/divisions")]
		public List<Division> GetDivisions()
		{
			return _catalogLogic.GetDivisions();
		}

		#region Exports

		/// <summary>
		/// Get Catalog export options
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[ApiKeyedRoute("catalog/export")]
		public ExportOptionsModel ExportProducts()
		{
			return _exportSettingRepository.ReadCustomExportOptions(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.Products, 0);
		}

		/// <summary>
		/// Export Catalog using provided search term
		/// </summary>
		/// <param name="searchTerms">Search term</param>
		/// <param name="searchModel"></param>
		/// <param name="exportRequest">Export options</param>
		/// <returns></returns>
		[HttpPost]
		[ApiKeyedRoute("catalog/export/{searchTerms}/products")]
		public HttpResponseMessage ProductSearchExport(string searchTerms, [FromUri] SearchInputModel searchModel, ExportRequestModel exportRequest)
		{
			searchModel.Size = 500;

			ProductsReturn prods = _catalogLogic.GetProductsBySearch(this.SelectedUserContext, searchTerms, searchModel, this.AuthenticatedUser);
			if (exportRequest.Fields != null)
				_exportSettingRepository.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.Products, KeithLink.Svc.Core.Enumerations.List.ListType.Custom, exportRequest.Fields, exportRequest.SelectedType);
			return ExportModel<Product>(prods.Products, exportRequest);	

		}

		/// <summary>
		/// Export products for a Category
		/// </summary>
		/// <param name="categoryId">Category Id</param>
		/// <param name="searchModel"></param>
		/// <param name="exportRequest">Export options</param>
		/// <returns></returns>
		[HttpPost]
		[ApiKeyedRoute("catalog/export/category/{categoryId}/products")]
		public HttpResponseMessage GetProductsByCategoryIdExport(string categoryId, [FromUri] SearchInputModel searchModel, ExportRequestModel exportRequest)
		{
			searchModel.Size = 500;

			ProductsReturn prods = _catalogLogic.GetProductsByCategory(this.SelectedUserContext, categoryId, searchModel, this.AuthenticatedUser);
			if (exportRequest.Fields != null)
				_exportSettingRepository.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.Products, KeithLink.Svc.Core.Enumerations.List.ListType.Custom, exportRequest.Fields, exportRequest.SelectedType);
			return ExportModel<Product>(prods.Products, exportRequest);	
		}

		/// <summary>
		/// Export products for a house brand
		/// </summary>
		/// <param name="brandControlLabel">House brand</param>
		/// <param name="searchModel"></param>
		/// <param name="exportRequest">Export options</param>
		/// <returns></returns>
		[HttpPost]
		[ApiKeyedRoute("catalog/export/brands/house/{brandControlLabel}")]
		public HttpResponseMessage GetProductsByHouseBrandExport(string brandControlLabel, [FromUri] SearchInputModel searchModel, ExportRequestModel exportRequest)
		{
			searchModel.Size = 500;
			ProductsReturn prods = _catalogLogic.GetHouseProductsByBranch(this.SelectedUserContext, brandControlLabel, searchModel, this.AuthenticatedUser);
			if (exportRequest.Fields != null)
				_exportSettingRepository.SaveUserExportSettings(this.AuthenticatedUser.UserId, Core.Models.Configuration.EF.ExportType.Products, KeithLink.Svc.Core.Enumerations.List.ListType.Custom, exportRequest.Fields, exportRequest.SelectedType);
			return ExportModel<Product>(prods.Products, exportRequest);	
		}	

		#endregion

        #endregion
    }
}