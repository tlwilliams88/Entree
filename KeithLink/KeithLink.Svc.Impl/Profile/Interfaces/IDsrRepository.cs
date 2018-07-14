using Entree.Core.Models.EF;
using Entree.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.Profile {
    public interface IDsrRepository : IBaseEFREpository<Dsr> {
        Dsr GetDsrByBranchAndDsrNumber( string branchId, string dsrNumber );
        void SendImageToMultiDocs( string emailAddress, Byte[] fileBytes );
    }
}
