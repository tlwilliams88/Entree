using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using KeithLink.Svc.Core.Models.Profile;

namespace KeithLink.Svc.InternalSvc.Interfaces {
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDsrService" in both code and config file together.
    [ServiceContract]
    public interface IDsrService {
        [OperationContract]
        Dsr GetDsr( string branchId, string dsrNumber );

		[OperationContract]
		List<Dsr> GetAllDsrInfo();
    }
}
