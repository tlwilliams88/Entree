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
	public class NoDivisionServiceRepositoryImpl: IDivisionServiceRepository
	{
		public NoDivisionServiceRepositoryImpl()
        {
		}

		public List<BranchSupportModel> ReadAllBranchSupports()
        {
            throw new NotImplementedException();
		}

	}
}
