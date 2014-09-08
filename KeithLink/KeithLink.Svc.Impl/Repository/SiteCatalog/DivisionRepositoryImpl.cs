using CommerceServer.Foundation;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Impl.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.SiteCatalog
{
	public class DivisionRepositoryImpl: IDivisionRepository
	{
		List<Catalog> IDivisionRepository.GetDivisions()
		{
			var queryCatalog = new CommerceQuery<CommerceEntity, CommerceModelSearch<CommerceEntity>>("Catalog");
			var response = FoundationService.ExecuteRequest(queryCatalog.ToRequest());

			
			CommerceQueryOperationResponse basketResponse = response.OperationResponses[0] as CommerceQueryOperationResponse;

			return basketResponse.CommerceEntities.Cast<CommerceEntity>().Select(i => (Catalog)i).ToList();
		}
	}
}
