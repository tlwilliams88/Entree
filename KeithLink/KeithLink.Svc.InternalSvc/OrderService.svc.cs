using KeithLink.Svc.InternalSvc.Interfaces;
using KeithLink.Svc.Core.Models.Orders.Confirmations;
using KeithLink.Common.Core.Logging;
using KeithLink.Common.Core.Extensions;
using CommerceServer.Core.Runtime.Orders;
using CommerceServer.Core.Orders;
using CommerceServer.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;


namespace KeithLink.Svc.InternalSvc
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PipelineService" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select PipelineService.svc or PipelineService.svc.cs at the Solution Explorer and start debugging.
	public class OrderService : IOrderService
	{
        private IEventLogRepository _eventLog;
		public OrderService(IEventLogRepository eventLog)
		{
            _eventLog = eventLog;
		}
	}
}
