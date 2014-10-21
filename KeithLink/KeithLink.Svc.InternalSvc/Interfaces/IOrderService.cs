using KeithLink.Svc.Core.Models.Confirmations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace KeithLink.Svc.InternalSvc.Interfaces
{
	[ServiceContract]
	public interface IOrderService
	{
        [OperationContract]
        bool OrderConfirmation(ConfirmationFile order);
	}
}