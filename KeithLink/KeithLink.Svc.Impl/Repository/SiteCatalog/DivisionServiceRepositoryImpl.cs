using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.SiteCatalog
{
	public class DivisionServiceRepositoryImpl: IDivisionServiceRepository
	{
		private com.benekeith.DivisionService.IDivisionService serviceClient;

		public DivisionServiceRepositoryImpl(com.benekeith.DivisionService.IDivisionService serviceClient)
		{
			this.serviceClient = serviceClient;
		}

		public List<BranchSupportModel> ReadAllBranchSupports()
		{
			return serviceClient.ReadBranchSupport().ToList();
		}

	}
}
