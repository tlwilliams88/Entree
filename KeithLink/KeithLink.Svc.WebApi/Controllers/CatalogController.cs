﻿using KeithLink.Common.Core.Extensions;

using KeithLink.Svc.Core.Enumerations.List;

using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Marketing;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;

using KeithLink.Svc.Core.Models.Configuration.EF;
using KeithLink.Svc.Core.Models.Marketing;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.SiteCatalog;

using KeithLink.Svc.Impl.Helpers;

using KeithLink.Svc.WebApi.Models;

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.IO;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Models.Customers;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Orders;

namespace KeithLink.Svc.WebApi.Controllers {
    /// <summary>
    /// CatalogController
    /// </summary>
	[Authorize]
    public class CatalogController : BaseController {
        #region attributes
        private readonly ICatalogCampaignService _campaignService;
        private readonly ICatalogCampaignLogic _campaignLogic;
        private readonly ISiteCatalogService _catalogService;
        private readonly ICatalogLogic _catalogLogic;
		private readonly IExportSettingLogic _exportSettingRepository;
        private readonly IEventLogRepository _elRepo;
        private readonly IListService _listService;
        #endregion

        #region ctor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="catalogLogic"></param>
        /// <param name="profileLogic"></param>
        /// <param name="exportSettingsLogic"></param>
        /// <param name="elRepo"></param>
        /// <param name="campaignService"></param>
        /// <param name="campaignLogic"></param>
        public CatalogController(ICatalogLogic catalogLogic, IUserProfileLogic profileLogic, IListService listService,
            IExportSettingLogic exportSettingsLogic, IEventLogRepository elRepo, ICatalogCampaignService campaignService,
            ICatalogCampaignLogic campaignLogic, ISiteCatalogService catalogService) : base(profileLogic) {

            _campaignLogic = campaignLogic;
            _campaignService = campaignService;
            _catalogLogic = catalogLogic;
            _exportSettingRepository = exportSettingsLogic;
            _catalogService = catalogService;
            _listService = listService;

            this._elRepo = elRepo;
        }
        #endregion

        #region methods
        /// <summary>
        /// Retrieve all product categories
        /// </summary>
        /// <param name="catalogType">Catalog Type</param>
        /// <returns>List of Categories</returns>
        [HttpGet]
        [ApiKeyedRoute("catalog/{catalogType}/categories")]
        public OperationReturnModel<CategoriesReturn> GetCategories(string catalogType)
        {
            OperationReturnModel<CategoriesReturn> ret = new OperationReturnModel<CategoriesReturn>();
            try
            {
                IEnumerable<KeyValuePair<string, string>> pairs = Request.GetQueryNameValuePairs();
                ret.SuccessResponse = _catalogLogic.GetCategories(0, 2000, catalogType);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("GetCategories", ex);
            }
            return ret;
        }

        /// <summary>
        /// Retrieve Sub-Categories
        /// </summary>
        /// <param name="id">Parent Category</param>
        /// <param name="searchModel">Contains From and Size for specifing items to be retreived</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("catalog/category/{id}/categories")]
        public OperationReturnModel<CategoriesReturn> GetSubCategoriesByParentId(string id, [FromUri] SearchInputModel searchModel)
        {
            OperationReturnModel<CategoriesReturn> ret = new OperationReturnModel<CategoriesReturn>();
            try
            {
                ret.SuccessResponse = _catalogLogic.GetCategories(searchModel.From, searchModel.Size, "BEK");
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("GetCategories", ex);
            }
            return ret;
        }

        /// <summary>
        /// Retrieve Products for a Category
        /// </summary>
        /// <param name="catalogType">Catalog Type</param>
        /// <param name="categoryId">Category Id</param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("catalog/search/category/{catalogType}/{categoryId}/products")]
        public OperationReturnModel<ProductsReturn> GetProductsByCategoryId(string catalogType, string categoryId, [FromUri] SearchInputModel searchModel)
        {
            OperationReturnModel<ProductsReturn> ret = new OperationReturnModel<ProductsReturn>();
            try
            {
                searchModel.CatalogType = catalogType;

                ProductsReturn prods = _catalogService.GetProductsByCategory(this.SelectedUserContext, categoryId, searchModel, this.AuthenticatedUser);

                prods.Products = FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(this.AuthenticatedUser, this.SelectedUserContext, prods.Products, _listService);

                ContractInformationHelper.GetContractCategoriesFromLists(SelectedUserContext, prods.Products, _listService);

                ret.SuccessResponse = prods;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("GetProductsByCategory", ex);
            }
            return ret;
        }

        /// <summary>
        /// Retrieve Products by house brand
        /// </summary>
        /// <param name="brandControlLabel">House brand</param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("catalog/search/brands/house/{brandControlLabel}")]
        public OperationReturnModel<ProductsReturn> GetProductsByHouseBrand(string brandControlLabel, [FromUri] SearchInputModel searchModel) {
            OperationReturnModel<ProductsReturn> ret = new OperationReturnModel<ProductsReturn>();
            try
            {
                ProductsReturn prods = _catalogService.GetHouseProductsByBranch(this.SelectedUserContext, brandControlLabel, searchModel, this.AuthenticatedUser);

                prods.Products = FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(this.AuthenticatedUser, this.SelectedUserContext, prods.Products, _listService);

                ContractInformationHelper.GetContractCategoriesFromLists(SelectedUserContext, prods.Products, _listService);

                ret.SuccessResponse = prods;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("GetHouseProductsByBranch", ex);
            }
            return ret;
        }

        /// <summary>
        /// GetProductsSearchBrand
        /// </summary>
        /// <param name="catalogType"></param>
        /// <param name="brandName"></param>
        /// <param name="searchModel"></param>
        /// <param name="searchTerms"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("catalog/{catalogType}/search/brand/{brandName}/products")]
        public OperationReturnModel<ProductsReturn> GetProductsSearchBrand(string catalogType, string brandName, [FromUri] SearchInputModel searchModel, [FromUri] string searchTerms = null)
        {
            OperationReturnModel<ProductsReturn> ret = new OperationReturnModel<ProductsReturn>();
            try
            {
                searchModel.CatalogType = catalogType;
                searchModel.Facets = string.Format("brands:{0}", brandName.ToUpper());
                ProductsReturn prods = _catalogService.GetProductsBySearch(this.SelectedUserContext, searchTerms, searchModel, this.AuthenticatedUser);

                prods.Products = FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(this.AuthenticatedUser, this.SelectedUserContext, prods.Products, _listService);

                ContractInformationHelper.GetContractCategoriesFromLists(SelectedUserContext, prods.Products, _listService);

                ret.SuccessResponse = prods;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("GetProductsSearch", ex);
            }
            return ret;
        }

        /// <summary>
        /// Get Categories
        /// </summary>
        /// <param name="id">This is not currently being used</param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("catalog/category/{id}")]
		public OperationReturnModel<CategoriesReturn> GetCategoriesById(string id, [FromUri] SearchInputModel searchModel)
        {
            OperationReturnModel<CategoriesReturn> ret = new OperationReturnModel<CategoriesReturn>();
            try
            {
                //TODO: This is not actually getting a category by ID (ID is never used).
                ret.SuccessResponse = _catalogLogic.GetCategories(searchModel.From, searchModel.Size, "BEK");
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("GetCategories", ex);
            }
            return ret;
        }

        /// <summary>
        /// Get Product by Id
        /// </summary>
        /// <param name="catalogType">Catalog Type</param>
        /// <param name="id">Product Id (itemnumber)</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("catalog/{catalogType}/product/{id}")]
        public OperationReturnModel<Product> GetProductById(string catalogType, string id)
        {
            OperationReturnModel<Product> ret = new OperationReturnModel<Product>();
            try
            {
                IEnumerable<KeyValuePair<string, string>> pairs = Request.GetQueryNameValuePairs();

                Product prod = _catalogLogic.GetProductById(this.SelectedUserContext, id, this.AuthenticatedUser, catalogType);

                prod = FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(this.AuthenticatedUser, this.SelectedUserContext, prod, _listService);

                ContractInformationHelper.GetContractCategoriesFromLists(SelectedUserContext, prod, _listService);

                if (prod == null)
                    prod = new Product();

                ret.SuccessResponse = prod;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("GetProductById", ex);
            }
            return ret;
        }

        [HttpGet]
        [AllowAnonymous]
        [ApiKeyedRoute("catalog/external/{catalogType}/{branchId}/product/{id}")]
        public OperationReturnModel<ShallowProduct> GetShallowProductById(string catalogType, string branchId, string id)
        {
            OperationReturnModel<ShallowProduct> ret = new OperationReturnModel<ShallowProduct>();
            try
            {
                if(Svc.Impl.Configuration.ServePublicApi)
                {
                    ShallowProduct prod = _catalogLogic.GetShallowProductById(branchId,
                                                                              id,
                                                                              catalogType);

                    if (prod == null)
                    {
                        ret.ErrorMessage = "No product found";
                        ret.IsSuccess = false;
                    }
                    else
                    {
                        ret.SuccessResponse = prod;
                        ret.IsSuccess = true;
                    }
                }
                else
                {
                    ret.ErrorMessage = "Public Serve turned OFF";
                    ret.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("GetProductById", ex);
            }
            return ret;
        }

        /// <summary>
        /// Get Product by Id or UPC
        /// </summary>
        /// <param name="idorupc">Can be Itemnumber or UPC</param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("catalog/product/scan/{idorupc}")]
        public OperationReturnModel<Product> GetProductByIdorUPC(string idorupc) {
            OperationReturnModel<Product> ret = new OperationReturnModel<Product>();
            try
            {
                Product prod = _catalogLogic.GetProductByIdOrUPC(this.SelectedUserContext, idorupc, this.AuthenticatedUser);

                prod = FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(this.AuthenticatedUser, this.SelectedUserContext, prod, _listService);

                ContractInformationHelper.GetContractCategoriesFromLists(SelectedUserContext, prod, _listService);

                ret.SuccessResponse = prod;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("GetProductByIdOrUPC", ex);
            }
            return ret;
        }

        /// <summary>
        /// Search products
        /// </summary>
        /// <param name="catalogType">Catalog Type</param>
        /// <param name="searchTerms">Product Search term</param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("catalog/{catalogType}/search/{searchTerms}/products")]
		public OperationReturnModel<ProductsReturn> GetProductsSearch(string catalogType, string searchTerms, [FromUri] SearchInputModel searchModel)
        {
            OperationReturnModel<ProductsReturn> ret = new OperationReturnModel<ProductsReturn>();
            try
            {
                searchModel.CatalogType = catalogType;
                ProductsReturn prods = _catalogService.GetProductsBySearch(this.SelectedUserContext, searchTerms, searchModel, this.AuthenticatedUser);

                prods.Products = FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(this.AuthenticatedUser, this.SelectedUserContext, prods.Products, _listService);

                ContractInformationHelper.GetContractCategoriesFromLists(SelectedUserContext, prods.Products, _listService);

                ret.SuccessResponse = prods;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("GetProductsSearch", ex);
            }
            return ret;
        }

        [HttpPost]
        [ApiKeyedRoute("catalog/recommended")]
        public ProductsReturn GetRecommendedItemsByCart(RecommendedItemsRequestModel request) {
            return _catalogService.GetRecommendedItemsForCart(this.SelectedUserContext, 
                                                              request.itemnumbers, 
                                                              this.AuthenticatedUser, 
                                                              request.pagesize, 
                                                              request.hasimages);
        }

        [HttpPost]
        [ApiKeyedRoute("catalog/growthandrecovery")]
        public OperationReturnModel<List<GrowthAndRecoveriesReturnModel>> GetGrowthAndRecoveryGroupsByCustomer(int pagesize = Constants.GROWTHANDRECOVERY_DEFAULT_PAGESIZE,
                                                                                                       bool hasimages = Constants.GROWTHANDRECOVERY_DEFAULT_HASIMAGES)
        {
            OperationReturnModel<List<GrowthAndRecoveriesReturnModel>> ret = new OperationReturnModel<List<GrowthAndRecoveriesReturnModel>>();
            try
            {
                ret.SuccessResponse = _catalogService.GetGrowthAndRecoveryItemsForCustomer(this.SelectedUserContext
                                                                                           , this.AuthenticatedUser
                                                                                           , pagesize
                                                                                           , hasimages);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("GetGrowthAndRecoveryGroupsByCustomer", ex);
            }
            return ret;
        }

        /// <summary>
        /// Return list of products attached to a promotional catalog campaign
        /// </summary>
        /// <param name="campaignUri"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("catalog/campaign/{campaignUri}")]
        public OperationReturnModel<ProductsReturn> GetCampaignProducts(string campaignUri, [FromUri] SearchInputModel searchModel)
        {
            OperationReturnModel<ProductsReturn> returnValue = new OperationReturnModel<ProductsReturn>();
            try
            {
                ProductsReturn prods = _campaignService.GetCatalogCampaignProducts(campaignUri, this.SelectedUserContext, searchModel, this.AuthenticatedUser);

                prods.Products = FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(this.AuthenticatedUser, this.SelectedUserContext, prods.Products, _listService);

                ContractInformationHelper.GetContractCategoriesFromLists(SelectedUserContext, prods.Products, _listService);

                returnValue.SuccessResponse = prods;
                returnValue.IsSuccess = true;
            } catch (Exception ex)
            {
                returnValue.IsSuccess = false;
                returnValue.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("GetCampaignProducts", ex);
            }

            return returnValue;
        }

        /// <summary>
        /// Get the campaign header details
        /// </summary>
        /// <param name="campaignUri"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("catalog/campaign/{campaignUri}/info")]
        public OperationReturnModel<CatalogCampaignHeader> GetCampaignHeader(string campaignUri)
        {
            OperationReturnModel<CatalogCampaignHeader> returnValue = new OperationReturnModel<CatalogCampaignHeader>();
            try
            {
                returnValue.SuccessResponse = _campaignLogic.GetCampaignByUri(campaignUri, false);
                returnValue.IsSuccess = true;
            } catch (Exception ex)
            {
                returnValue.IsSuccess = false;
                returnValue.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("GetCampaignHeader", ex);
            }

            return returnValue;
        }

        [HttpGet]
        [ApiKeyedRoute("catalog/campaign")]
        public OperationReturnModel<CatalogCampaignsReturnModel> GetCampaignHeaders()
        {
            OperationReturnModel<CatalogCampaignsReturnModel> returnValue = new OperationReturnModel<CatalogCampaignsReturnModel>();
            try {
                returnValue.SuccessResponse = _campaignLogic.GetAllAvailableCampaigns(SelectedUserContext);
                returnValue.IsSuccess = true;
            } catch (Exception ex) {
                returnValue.IsSuccess = false;
                returnValue.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("GetAllCampaigns", ex);
            }

            return returnValue;
        }

        [HttpPost]
        [ApiKeyedRoute("catalog/campaign/add")]
        public OperationReturnModel<bool> AddOrUpdateCampaignHeader(CatalogCampaignAddOrUpdateRequestModel model)
        {
            OperationReturnModel<bool> returnValue = new OperationReturnModel<bool>();
            try
            {
                returnValue.SuccessResponse = _campaignLogic.AddOrUpdateCampaign(model);
                returnValue.IsSuccess = true;
            }
            catch (Exception ex)
            {
                returnValue.IsSuccess = false;
                returnValue.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("AddOrUpdateCampaign", ex);
            }

            return returnValue;
        }

        /// <summary>
        /// Retrieve divisions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [ApiKeyedRoute("catalog/divisions")]
        public OperationReturnModel<List<Division>> GetDivisions() {
            OperationReturnModel<List<Division>> ret = new OperationReturnModel<List<Division>>();
            try
            {
                // for some reason the unfi catalogs are present in the divisions, to suppress them in this list 
                // we squelch the ones with no matching branchsupport
                ret.SuccessResponse = _catalogLogic.GetDivisions().Where(d => d.BranchSupport != null).ToList();
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("GetDivisions", ex);
            }
            return ret;
        }

        /// <summary>
        /// Get Catalog export options
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiKeyedRoute("catalog/export")]
        public OperationReturnModel<ExportOptionsModel> ExportProducts() {
            OperationReturnModel<ExportOptionsModel> ret = new OperationReturnModel<ExportOptionsModel>();
            try
            {
                ret.SuccessResponse = _exportSettingRepository.ReadCustomExportOptions(this.AuthenticatedUser.UserId, ExportType.Products, 0);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("ExportProducts", ex);
            }
            return ret;
        }

        /// <summary>
		/// Export Catalog using provided search term
		/// </summary>
        /// <param name="catalogType">Catalog Type</param>
		/// <param name="searchTerms">Search term</param>
		/// <param name="searchModel"></param>
		/// <param name="exportRequest">Export options</param>
		/// <returns></returns>
		[HttpPost]
		[ApiKeyedRoute("catalog/{catalogType}/export/{searchTerms}/products")]
		public HttpResponseMessage ProductSearchExport(string catalogType, string searchTerms, [FromUri] SearchInputModel searchModel, ExportRequestModel exportRequest) {
            HttpResponseMessage ret;
            try
            {
                searchModel.Size = 500;
                searchModel.CatalogType = catalogType;

                ProductsReturn prods = _catalogService.GetProductsBySearch(this.SelectedUserContext, searchTerms, searchModel, this.AuthenticatedUser);

                prods.Products = FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(this.AuthenticatedUser, this.SelectedUserContext, prods.Products, _listService);

                ContractInformationHelper.GetContractCategoriesFromLists(SelectedUserContext, prods.Products, _listService);

                ItemOrderHistoryHelper.GetItemOrderHistories(_catalogLogic, SelectedUserContext, prods.Products);

                if (exportRequest.Fields != null)
                    _exportSettingRepository.SaveUserExportSettings(this.AuthenticatedUser.UserId, ExportType.Products, ListType.Custom, exportRequest.Fields, exportRequest.SelectedType);

                ret = ExportModel<Product>(prods.Products, exportRequest, SelectedUserContext);
            }
            catch (Exception ex)
            {
                _elRepo.WriteErrorLog("ProductSearchExport", ex);
                ret = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return ret;
        }

        /// <summary>
        /// Export products for a Category
        /// </summary>
        /// <param name="catalogType">Catalog Type</param>
        /// <param name="categoryId">Category Id</param>
        /// <param name="searchModel"></param>
        /// <param name="exportRequest">Export options</param>
        /// <returns></returns>
        [HttpPost]
        [ApiKeyedRoute("catalog/export/category/{catalogType}/{categoryId}/products")]
        public HttpResponseMessage GetProductsByCategoryIdExport(string catalogType, string categoryId, [FromUri] SearchInputModel searchModel, ExportRequestModel exportRequest) {
            HttpResponseMessage ret;
            try
            {
                searchModel.Size = 500;
                searchModel.CatalogType = catalogType;

                ProductsReturn prods = _catalogService.GetProductsByCategory(this.SelectedUserContext, categoryId, searchModel, this.AuthenticatedUser);

                prods.Products = FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(this.AuthenticatedUser, this.SelectedUserContext, prods.Products, _listService);

                ContractInformationHelper.GetContractCategoriesFromLists(SelectedUserContext, prods.Products, _listService);

                if (exportRequest.Fields != null)
                    _exportSettingRepository.SaveUserExportSettings(this.AuthenticatedUser.UserId, ExportType.Products, ListType.Custom, exportRequest.Fields, exportRequest.SelectedType);
                ret = ExportModel<Product>(prods.Products, exportRequest, SelectedUserContext);
            }
            catch (Exception ex)
            {
                _elRepo.WriteErrorLog("GetProductsByCategoryIdExport", ex);
                ret = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return ret;
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
        public HttpResponseMessage GetProductsByHouseBrandExport(string brandControlLabel, [FromUri] SearchInputModel searchModel, ExportRequestModel exportRequest) {
            HttpResponseMessage ret;
            try
            {
                searchModel.Size = 500;

                ProductsReturn prods = _catalogService.GetHouseProductsByBranch(this.SelectedUserContext, brandControlLabel, searchModel, this.AuthenticatedUser);

                prods.Products = FavoritesAndNotesHelper.GetFavoritesAndNotesFromLists(this.AuthenticatedUser, this.SelectedUserContext, prods.Products, _listService);

                ContractInformationHelper.GetContractCategoriesFromLists(SelectedUserContext, prods.Products, _listService);

                if (exportRequest.Fields != null)
                {
                    _exportSettingRepository.SaveUserExportSettings(this.AuthenticatedUser.UserId, ExportType.Products, ListType.Custom,
                                                                                            exportRequest.Fields, exportRequest.SelectedType);
                }

                ret = ExportModel<Product>(prods.Products, exportRequest, SelectedUserContext);
            }
            catch (Exception ex)
            {
                _elRepo.WriteErrorLog("GetProductsByHouseBrandExport", ex);
                ret = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return ret;
        }
        #endregion
    }
}
