using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Impl.Repository.PowerMenu;
using Entree.Core.Models.PowerMenu;
using Entree.Core.Interface.PowerMenu;
using Entree.Core.Extensions.PowerMenu;

using KeithLink.Svc.Impl.Logic.Profile;
using Entree.Core.Models.Profile;
using Entree.Core.Interface.Profile;

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