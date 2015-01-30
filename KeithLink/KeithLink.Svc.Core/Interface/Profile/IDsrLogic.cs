using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Profile;

namespace KeithLink.Svc.Core.Interface.Profile {
    public interface IDsrLogic {
        Dsr GetDsr( string branchId, string dsrNumber );
        void CreateOrUpdateDsr(Dsr dsr);
    }
}
