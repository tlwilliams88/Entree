using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.PowerMenu;

namespace KeithLink.Svc.Core.Interface.PowerMenu {
    public interface IPowerMenuRepository {
        bool SendPowerMenuAccountRequests( PowerMenuSystemRequestModel request );
    }
}
