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
	public class PipelineService : IPipelineService
	{
		private readonly IPriceLogic priceLogic;

		public PipelineService(IPriceLogic priceLogic)
		{
			this.priceLogic = priceLogic;
		}

		public Core.Models.SiteCatalog.PriceReturn GetPrices(string BranchId, string customerNumber, DateTime shipDate, List<Core.Models.SiteCatalog.Product> products)
		{
			return priceLogic.GetPrices(BranchId, customerNumber, shipDate, products);
		}
	}
}
