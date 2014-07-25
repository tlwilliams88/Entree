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
        KeithLink.Svc.Core.IPriceRepository _priceRepository;
        string elasticSearchEndpoint = System.Configuration.ConfigurationManager.AppSettings[Constants.ElasticSearchEndpointConfigurationEntry];

        public CatalogController(ICatalogRepository catalogRepository, IPriceRepository priceRepository)
        {
            _catalogRepository = catalogRepository;
            _priceRepository = priceRepository;
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
            ProductsReturn prods = _catalogRepository.GetProductsByCategory(branchId, categoryId, elasticSearchEndpoint, 0, 500);
            GetPricingInfo(prods);
            return prods;
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
            Product prod = _catalogRepository.GetProductById(branchId, id, elasticSearchEndpoint);
            ProductsReturn prods = new ProductsReturn() { Products = new List<Product>() { prod } };
            GetPricingInfo(prods);
            return prod;
        }

        [HttpGet]
        [Route("catalog/search/{branchId}/{searchTerms}/products")]
        public ProductsReturn GetProductsSearch(string branchId, string searchTerms)
        {
            Dictionary<string, string> pairs = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
            int from = 0, size = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings[Constants.DefaultProductReturnSizeConfigurationEntry]);
            if (pairs.ContainsKey(Constants.ReturnSizeQueryStringParam))
            {
                size = Convert.ToInt32(pairs[Constants.ReturnSizeQueryStringParam]);
            }
            if (pairs.ContainsKey(Constants.ReturnFromQueryStringParam))
            {
                from = Convert.ToInt32(pairs[Constants.ReturnSizeQueryStringParam]);
            }

            ProductsReturn prods = _catalogRepository.GetProductsBySearch(branchId, searchTerms, elasticSearchEndpoint, from, size);
            GetPricingInfo(prods);

            return prods;
        }

        private void GetPricingInfo(ProductsReturn prods)
        {
            PriceReturn pricingInfo = _priceRepository.GetPrices("FAM", "011807", DateTime.Now.AddDays(1), prods.Products);

            foreach (Product p in prods.Products)
            {
                double casePrice = pricingInfo.Prices.Find(x => x.ItemNumber == p.ItemNumber).CasePrice;
                double packagePrice = pricingInfo.Prices.Find(x => x.ItemNumber == p.ItemNumber).PackagePrice;
                p.CasePrice = String.Format("{0:C}", Convert.ToInt32(casePrice)); ;
                p.PackagePrice = String.Format("{0:C}", Convert.ToInt32(packagePrice));
            }
        }
    }
}
