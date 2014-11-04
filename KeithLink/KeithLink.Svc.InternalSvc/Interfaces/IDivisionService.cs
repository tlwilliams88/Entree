using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KeithLink.Svc.InternalSvc.Interfaces
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IListServcie" in both code and config file together.
	[ServiceContract]
	public interface IDivisionService
	{
		[OperationContract]
		List<BranchSupportModel> ReadBranchSupport();
	
	}
}
