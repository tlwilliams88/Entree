using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace KeithLink.Svc.InternalSvc.Interfaces
{
	[ServiceContract]
	public interface IPipelineService
	{
		[OperationContract]
		PriceReturn GetPrices(string BranchId, string customerNumber, DateTime shipDate, List<Product> products);
	}
}