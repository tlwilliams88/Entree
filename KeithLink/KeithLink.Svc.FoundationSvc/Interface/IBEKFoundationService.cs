using CommerceServer.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace KeithLink.Svc.FoundationSvc.Interface
{
	[ServiceContract]
	public interface IBEKFoundationService : IOperationService
	{
		[OperationContract]
		string SaveCartAsOrder(Guid userId, Guid cartId);
	}
}