using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.PowerMenu;

namespace KeithLink.Svc.Core.Interface.PowerMenu {
    public interface IPowerMenuLogic {
        /// <summary>
        /// Returns a string array of the status for each account request
        /// </summary>
        /// <param name="accounts"></param>
        /// <returns></returns>
        List<string> SendAccountRequests( List<PowerMenuSystemRequestModel> accounts );
    }
}
