using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Impl.Repository.PowerMenu;
using KeithLink.Svc.Core.Models.PowerMenu;
using KeithLink.Svc.Core.Interface.PowerMenu;
using KeithLink.Svc.Core.Extensions.PowerMenu;

using KeithLink.Svc.Impl.Logic.Profile;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Interface.Profile;

namespace KeithLink.Svc.Impl.Logic.PowerMenu {
    public class PowerMenuLogicImpl : IPowerMenuLogic {

        #region attributes
        IPowerMenuRepository _pmRepository;
        #endregion


        public PowerMenuLogicImpl( IPowerMenuRepository powermenuRepository ) {
            _pmRepository = powermenuRepository;
        }

        /// <summary>
        /// Serialize and send xml object to PowerMenu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool SendAccountRequestToPowerMenu( PowerMenuSystemRequestModel request ) {
            return _pmRepository.SendPowerMenuAccountRequests( request );
        }

    }
}