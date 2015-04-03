using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Impl.Repository.PowerMenu;
using KeithLink.Svc.Core.Models.PowerMenu;
using KeithLink.Svc.Core.Interface.PowerMenu;

namespace KeithLink.Svc.Impl.Logic.PowerMenu {
    public class PowerMenuLogicImpl : IPowerMenuLogic {

        #region attributes
            PowerMenuRepositoryImpl _pmRepository;
        #endregion


        public PowerMenuLogicImpl( PowerMenuRepositoryImpl powermenuRepository ) {
            _pmRepository = powermenuRepository;
        }

        public List<string> SendAccountRequests( List<PowerMenuSystemRequestModel> accounts ) {
            // TODO : Need to serialize each request and send it to PowerMenu keeping record of which fail and which succeed and logging every serialized request for good record keeping.
            return new List<string> { };
        }
    }
}
