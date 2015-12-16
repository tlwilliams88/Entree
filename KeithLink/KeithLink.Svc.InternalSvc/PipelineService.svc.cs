using KeithLink.Svc.Core.Interface.SiteCatalog;
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
		private readonly IPriceLogic priceLogic;
        private readonly ICatalogLogic catalogLogic;

		public PipelineService(IPriceLogic priceLogic, ICatalogLogic catalogLogic)
		{
			this.priceLogic = priceLogic;
            this.catalogLogic = catalogLogic;
		}

		public Core.Models.SiteCatalog.PriceReturn GetPrices(string BranchId, string customerNumber, DateTime shipDate, List<Core.Models.SiteCatalog.Product> products)
		{
            var catalogList = products.Select(i => i.CatalogId).Distinct().ToList();
            var pricing = new Core.Models.SiteCatalog.PriceReturn() { Prices = new List<Core.Models.SiteCatalog.Price>() };
            foreach (var catalogId in catalogList)
            {
                var tempProducts = catalogLogic.GetProductsByIds(catalogId, products.Where(i => i.CatalogId.Equals(catalogId)).Select(i => i.ItemNumber).Distinct().ToList());
                if (!catalogLogic.IsSpecialtyCatalog(null, catalogId))
                {
                    pricing.AddRange(priceLogic.GetPrices(catalogId, customerNumber, shipDate, tempProducts.Products)); //BEK
                }
                else {
                    var source = catalogLogic.GetCatalogTypeFromCatalogId(catalogId);
                    pricing.AddRange(priceLogic.GetNonBekItemPrices(BranchId, customerNumber, source, shipDate, tempProducts.Products));
                }                    
            }
            // need to split for bek/non-bek
			return priceLogic.GetPrices(BranchId, customerNumber, shipDate, products);
		}
	}
}
