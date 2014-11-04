using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.InternalSvc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KeithLink.Svc.InternalSvc
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ListServcie" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select ListServcie.svc or ListServcie.svc.cs at the Solution Explorer and start debugging.
	public class DivisionService : IDivisionService
	{
		private readonly IInternalDivisionLogic divisionLogic;

		public DivisionService(IInternalDivisionLogic divisionLogic)
		{
			this.divisionLogic = divisionLogic;
		}

		//public List<BranchSupportModel> ReadBranchSupports()
		//{
		//	return divisionLogic.ReadBranchSupports();
		//}


		public List<Core.Models.SiteCatalog.BranchSupportModel> ReadBranchSupport()
		{
			return divisionLogic.ReadBranchSupport();
		}
	}
}
