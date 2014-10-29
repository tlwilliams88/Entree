using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS = KeithLink.Svc.Core.Models.Generated;

namespace KeithLink.Svc.Core.Interface.Orders
{
	public interface IOrderQueueLogic
	{
		void ProcessOrders();
        void WriteFileToQueue(string orderingUserEmail, string orderNumber, CS.PurchaseOrder order, Enumerations.Order.OrderType orderType);
	}
}
