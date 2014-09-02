using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web;

namespace KeithLink.Svc.InternalSvc
{
	public class ErrorServiceBehavior: IServiceBehavior
	{

		public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
		{
			//Do Nothing
		}

		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
		{
			ErrorHandler handler = new ErrorHandler();
			foreach (ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers)
				dispatcher.ErrorHandlers.Add(handler);
		}

		public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
		{
			//Do Nothing
		}
	}
}