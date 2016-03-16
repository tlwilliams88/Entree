using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Orders
{
    [DataContract(Name = "SaveOrderReturn")]
    public class SaveOrderReturn
	{
        [DataMember(Name = "numberOfOrders")]
        public int NumberOfOrders { get; set; }

		[DataMember(Name = "ordersReturned")]
		public List<NewOrderReturn> OrdersReturned { get; set; }
	}
}
