using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Core.Models.Profile;

namespace Entree.Core.Interface.Profile {
    public interface IDsrLogic {
        Dsr GetDsr( string branchId, string dsrNumber );
        void CreateOrUpdateDsr(Dsr dsr);
        void SendImageToMultiDocs( string emailAddress, Byte[] fileBytes );
		List<Dsr> GetAllDsrInfo();
    }
}
