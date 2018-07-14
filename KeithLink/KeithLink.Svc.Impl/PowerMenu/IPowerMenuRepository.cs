using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Core.Models.PowerMenu;

namespace Entree.Core.Interface.PowerMenu {
    public interface IPowerMenuRepository {
        bool SendPowerMenuAccountRequests( PowerMenuSystemRequestModel request );
    }
}
