using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using KeithLink.Svc.Core;

namespace KeithLink.Svc.WebApi.Controllers
{
    public class CatalogController : ApiController
    {
        KeithLink.Svc.Core.ICatalogRepository _catalogRepository;

        CatalogController(ICatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _catalogRepository.GetProductsForCategory("");
        }

        public IEnumerable<Product> GetProductsForCategory(string id)
        {
            return _catalogRepository.GetProductsForCategory(id);
        }
    }
}
