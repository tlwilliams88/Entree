using CommerceServer.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Helpers
{
	public class FoundationService
    {
        #region attributes
        static object lockObject = new object();
        static OperationServiceAgent serviceAgent = null;
        #endregion

        #region methods
        public static CommerceResponse ExecuteRequest(CommerceRequest request)
        {
            // Execute the operation and get the results back
            CommerceServer.Foundation.CommerceResponse response = GetServiceAgent().ProcessRequest(CreateRequestContext(), request);
            return response;
        }

		static CommerceRequestContext CreateRequestContext()
		{
            return new CommerceServer.Foundation.CommerceRequestContext()
            {
			    Channel = string.Empty,
			    RequestId = System.Guid.NewGuid().ToString("B"),
			    UserLocale = "en-US",
			    UserUILocale = "en-US"
		    };
        }

        static OperationServiceAgent GetServiceAgent()
        {
            if (serviceAgent == null)
            {
                lock (lockObject) // ensure object is only created once
                {
                    if (serviceAgent == null)
                        serviceAgent = new CommerceServer.Foundation.OperationServiceAgent();
                }
            }
            return serviceAgent;
        }
        #endregion
    }
}
