using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS = Entree.Core.Models.Generated;

namespace Entree.Core.Interface.Orders
{
	public interface IOrderQueueLogic
	{
		void ProcessOrders();
        void WriteFileToQueue(string orderingUserEmail, string orderNumber, CS.PurchaseOrder order, Enumerations.Order.OrderType orderType, string catalogType,
            string dsrNumber = "", string addressStreet = "", string addressCity = "", string addressState = "", string addressZip = "");
	}
}
