using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using KeithLink.Common.Core.Logging;
using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Core.Interface.PowerMenu;

namespace KeithLink.Svc.Impl.Repository.PowerMenu {
    public class PowerMenuRepositoryImpl : IPowerMenuRepository {

        #region attributes
        IEventLogRepository _log;
        #endregion

        #region constructor
        public PowerMenuRepositoryImpl(IEventLogRepository log) {
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

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    System.Net.Http.HttpResponseMessage response = client.PostAsXmlAsync(Configuration.PowerMenuWebServiceUrl, request).Result;

                    if (response.StatusCode.Equals( System.Net.HttpStatusCode.OK ) || response.StatusCode.Equals( System.Net.HttpStatusCode.NoContent )) {
                        returnValue = true;
                    } else {
                        _log.WriteErrorLog( String.Format( "Error communicating with powermenu service request: {0} - HttpResposne: {1}", request, response.StatusCode ) );
                        throw new Exception( "There was an error communicating with the powermenu service" );
                    }
                }
                catch (Exception ex)
                {
                    _log.WriteErrorLog( "Error communicating with the powermenu service", ex );
                    throw new Exception( "There was an error communicating with the powermenu service" );
                }
            }

            return returnValue;
        }

    }
}
