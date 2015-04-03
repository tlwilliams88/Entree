using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Core.Interface.PowerMenu;

namespace KeithLink.Svc.Impl.Repository.PowerMenu {
    public class PowerMenuRepositoryImpl : IPowerMenuRepository {

        #region attributes
        EventLogRepositoryImpl _log;
        #endregion

        #region constructor
        public PowerMenuRepositoryImpl(EventLogRepositoryImpl log) {
            _log = log;
        }
        #endregion

        /// <summary>
        /// Takes serialized version of the PowerMenuServiceRequestModel
        /// </summary>
        /// <param name="requests"></param>
        /// <returns></returns>
        public bool SendPowerMenuAccountRequests( string request ) {
            bool returnValue = false;

            

            return returnValue;
        }

    }
}
