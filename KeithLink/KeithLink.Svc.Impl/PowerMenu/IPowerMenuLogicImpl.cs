using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Core.Models.PowerMenu;

namespace Entree.Core.Interface.PowerMenu {
    public interface IPowerMenuLogic {
        /// <summary>
        /// Returns a string array of the status for each account request
        /// </summary>
        /// <param name="accounts"></param>
        /// <returns></returns>
        bool SendAccountRequestToPowerMenu( PowerMenuSystemRequestModel request );
    }
}
