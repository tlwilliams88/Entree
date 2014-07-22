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

        public CatalogController(ICatalogRepository catalogRepository, IPriceRepository priceRepository)
        {
            _catalogRepository = catalogRepository;
            _priceRepository = priceRepository;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _catalogRepository.GetProductsForCategory("");
        }

        public IEnumerable<Product> GetProductsForCategory(string id)
        {
            return _catalogRepository.GetProductsForCategory(id);
        }

        [HttpGet]
        [Route("catalog/categories")]
        public CategoriesReturn GetCategories()
        {
            IEnumerable<KeyValuePair<string, string>> pairs = Request.GetQueryNameValuePairs();
            return _catalogRepository.GetCategories();
        }

        [HttpGet]
        [Route("catalog/category/{id}/categories")]
        public CategoriesReturn GetSubCategoriesByParentId(string id)
        {
            IEnumerable<KeyValuePair<string, string>> pairs = Request.GetQueryNameValuePairs();
            return _catalogRepository.GetCategories();
        }

        [HttpGet]
        [Route("catalog/category/{id}")]
        public CategoriesReturn GetCategoriesById(string id)
        {
            IEnumerable<KeyValuePair<string, string>> pairs = Request.GetQueryNameValuePairs();
            return _catalogRepository.GetCategories();
        }

        [HttpGet]
        [Route("catalog/product/{id}")]
        public Product GetProductById(string id)
        {

            //IEnumerable<KeyValuePair<string, string>> pairs = Request.GetQueryNameValuePairs();

            Product currentItem = new Product()
            {
                ItemNumber = "285141",
                Description = "Shrimp Raw Hdls 25/30",
                ExtendedDescription = "Premium Wild Texas White",
                Brand = "Cortona",
                Size = "5 LB",
                UPC = "00000000000000",
                ManufacturerNumber = "B-W-26/30",
                ManufacturerName = "Ellington Farms Seafood",
                Cases = "0",
                CategoryId = "FS490",
                Kosher = "true",
            };

            List<Product> products = new List<Product>();
            products.Add(currentItem);

            PriceReturn pricingInfo = _priceRepository.GetPrices("FDF", "010189", DateTime.Now.AddDays(1), products);
            currentItem.CasePrice = pricingInfo.Prices[0].CasePrice.ToString();
            currentItem.PackagePrice = pricingInfo.Prices[0].PackagePrice.ToString();

            return currentItem;
        }

        [HttpGet]
        [Route("catalog/search/products")]
        public ProductsReturn GetProductsSearch()
        {
            //IEnumerable<KeyValuePair<string, string>> pairs = Request.GetQueryNameValuePairs();
            ProductsReturn ret = new ProductsReturn();
            ret.Products = new List<Product>();

            ret.Products.Add(new Product()
            {
                ItemNumber = "285141",
                Description = "Shrimp Raw Hdls 25/30",
                ExtendedDescription = "Premium Wild Texas White",
                Brand = "Cortona",
                Size = "5 LB",
                UPC = "00000000000000",
                ManufacturerNumber = "B-W-26/30",
                ManufacturerName = "Ellington Farms Seafood",
                Cases = "0",
                CategoryId = "FS490",
                Kosher = "true"
            });
            ret.Products.Add(new Product()
            {
                ItemNumber = "285149",
                Description = "Shrimp Cooked Hdls 25/30",
                ExtendedDescription = "Premium Wild Texas White",
                Brand = "Cortona",
                Size = "6 LB",
                UPC = "00000000000001",
                ManufacturerNumber = "B-W-26/31",
                ManufacturerName = "Ellington Farms Seafood 2",
                Cases = "1",
                CategoryId = "FS490",
                Kosher = "true"
            });

            PriceReturn pricingInfo = _priceRepository.GetPrices("FDF", "010189", DateTime.Now.AddDays(1), ret.Products);

            foreach (Price currentPrice in pricingInfo.Prices)
            {
                for (int i = 0; i < ret.Products.Count; i++)
                {
                    if (ret.Products[i].ItemNumber.Equals(currentPrice.ItemNumber))
                    {
                        ret.Products[i].CasePrice = currentPrice.CasePrice.ToString();
                        ret.Products[i].PackagePrice = currentPrice.PackagePrice.ToString();
                    }
                }
            }

            return ret;
        }
    }
}
