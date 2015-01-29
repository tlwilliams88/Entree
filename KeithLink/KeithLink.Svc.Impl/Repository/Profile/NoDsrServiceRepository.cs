using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Profile;

namespace KeithLink.Svc.Impl.Repository.Profile {
    public class NoDsrServiceRepository : IDsrServiceRepository {

        public Dsr GetDsr( string branchId, string dsrNumber ) {
            return new Dsr();
        }
    }
}
