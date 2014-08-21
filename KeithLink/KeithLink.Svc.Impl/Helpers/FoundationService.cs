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
		public static CommerceRequestContext CreateFoundationServiceContext()
		{
			// create the request
			CommerceServer.Foundation.CommerceRequestContext requestContext = new CommerceServer.Foundation.CommerceRequestContext();
			requestContext.Channel = string.Empty;
			requestContext.RequestId = System.Guid.NewGuid().ToString("B");
			requestContext.UserLocale = "en-US";
			requestContext.UserUILocale = "en-US";
			return requestContext;
		}

		public static CommerceResponse ExecuteRequest(CommerceRequest request)
		{
			CommerceRequestContext requestContext = new CommerceServer.Foundation.CommerceRequestContext();
			requestContext.Channel = string.Empty;
			requestContext.RequestId = System.Guid.NewGuid().ToString("B");
			requestContext.UserLocale = "en-US";
			requestContext.UserUILocale = "en-US";

			// Execute the operation and get the results back
			CommerceServer.Foundation.OperationServiceAgent serviceAgent = new CommerceServer.Foundation.OperationServiceAgent();
			CommerceServer.Foundation.CommerceResponse response = serviceAgent.ProcessRequest(requestContext, request);
			return response;
		}
	}
}
