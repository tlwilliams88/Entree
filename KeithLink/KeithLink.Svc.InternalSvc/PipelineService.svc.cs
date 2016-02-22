using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.SiteCatalog;

using KeithLink.Svc.InternalSvc.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KeithLink.Svc.InternalSvc
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PipelineService" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select PipelineService.svc or PipelineService.svc.cs at the Solution Explorer and start debugging.
	[GlobalErrorBehaviorAttribute(typeof(ErrorHandler))]
	public class PipelineService : IPipelineService
	{
		private readonly IPriceLogic _priceLogic;
        private readonly ICatalogLogic _catalogLogic;

		public PipelineService(IPriceLogic priceLogic, ICatalogLogic catalogLogic)
		{
			_priceLogic = priceLogic;
            _catalogLogic = catalogLogic;
		}

		public Core.Models.SiteCatalog.PriceReturn GetPrices(string BranchId, string customerNumber, DateTime shipDate, List<Product> products)
		{
            var catalogList = products.Select(i => i.CatalogId).Distinct().ToList();

            ProductsReturn completedProducts = new ProductsReturn();
            
            foreach (var catalogId in catalogList) {
                completedProducts.AddRange(_catalogLogic.GetProductsByIds(catalogId, 
                                                                          products.Where(i => i.CatalogId.Equals(catalogId, StringComparison.InvariantCultureIgnoreCase))
                                                                                  .Select(i => i.ItemNumber)
                                                                                  .Distinct()
                                                                                  .ToList()
                                                                          )
                                          );
            }

			return _priceLogic.GetPrices(BranchId, customerNumber, shipDate, completedProducts.Products);
		}
	}
}
