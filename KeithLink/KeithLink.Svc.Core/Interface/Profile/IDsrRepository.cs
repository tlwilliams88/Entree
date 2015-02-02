using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Profile {
    public interface IDsrRepository : IBaseEFREpository<Dsr> {
        Dsr GetDsrByBranchAndDsrNumber( string branchId, string dsrNumber );
        void SendImageToMultiDocs( string emailAddress, Byte[] fileBytes );
    }
}
