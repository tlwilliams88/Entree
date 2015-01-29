using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Svc.Core.Interface.Profile;

namespace KeithLink.Svc.InternalSvc {
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DsrService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select DsrService.svc or DsrService.svc.cs at the Solution Explorer and start debugging.
    [GlobalErrorBehaviorAttribute(typeof(ErrorHandler))]
    public class DsrService : IDsrService {
        private readonly IDsrLogic _dsrLogic;

        public DsrService( IDsrLogic dsrLogic ) {
            _dsrLogic = dsrLogic;
        }

        public Dsr GetDsr( string branchId, string dsrNumber ) {
            return _dsrLogic.GetDsr( branchId, dsrNumber );
        }
    }
}
