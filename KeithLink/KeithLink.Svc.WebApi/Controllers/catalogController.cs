using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using KeithLink.Svc.Core;
using System.Web.Http.Cors;

namespace KeithLink.Svc.WebApi.Controllers
{
    public class CatalogController : ApiController
    {
        KeithLink.Svc.Core.ICatalogRepository _catalogRepository;
        string elasticSearchEndpoint = System.Configuration.ConfigurationManager.AppSettings[Constants.ElasticSearchEndpointConfigurationEntry];

        public CatalogController(ICatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
        }

        public ProductsReturn GetAllProducts()
        {
            return _catalogRepository.GetProductsByCategory("", "", "");
        }

        public ProductsReturn GetProductsForCategory(string id)
        {
            return _catalogRepository.GetProductsByCategory("", id, "");
        }

        [HttpGet]
        [Route("catalog/categories")]
        public CategoriesReturn GetCategories()
        {
            IEnumerable<KeyValuePair<string, string>> pairs = Request.GetQueryNameValuePairs();
            return _catalogRepository.GetCategories(elasticSearchEndpoint);
        }

        [HttpGet]
        [Route("catalog/category/{id}/categories")]
        public CategoriesReturn GetSubCategoriesByParentId(string id)
        {
            IEnumerable<KeyValuePair<string, string>> pairs = Request.GetQueryNameValuePairs();
            return _catalogRepository.GetCategories();
        }

        [HttpGet]
        [Route("catalog/search/category/{branchId}/{categoryId}/products")]
        public ProductsReturn GetProductsByCategoryId(string branchId, string categoryId)
        {
            IEnumerable<KeyValuePair<string, string>> pairs = Request.GetQueryNameValuePairs();
            return _catalogRepository.GetProductsByCategory(branchId, categoryId, elasticSearchEndpoint);
        }

        [HttpGet]
        [Route("catalog/category/{id}")]
        public CategoriesReturn GetCategoriesById(string id)
        {
            IEnumerable<KeyValuePair<string, string>> pairs = Request.GetQueryNameValuePairs();
            return _catalogRepository.GetCategories();
        }

        [HttpGet]
        [Route("catalog/product/{branchId}/{id}")]
        public Product GetProductById(string branchId, string id)
        {
            IEnumerable<KeyValuePair<string, string>> pairs = Request.GetQueryNameValuePairs();
            return _catalogRepository.GetProductById(branchId, id, elasticSearchEndpoint);
        }

        [HttpGet]
        [Route("catalog/search/{branchId}/{searchTerms}/products")]
        public ProductsReturn GetProductsSearch(string branchId, string searchTerms)
        {
            IEnumerable<KeyValuePair<string, string>> pairs = Request.GetQueryNameValuePairs();
            return _catalogRepository.GetProductsBySearch(branchId, searchTerms, elasticSearchEndpoint);
        }
    }
}
