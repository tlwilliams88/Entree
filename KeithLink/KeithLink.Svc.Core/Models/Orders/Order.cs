using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Orders
{
	[DataContract(Name = "Order")]
	public class Order
	{	
		[DataMember(Name = "ordernumber")]
		public string OrderNumber { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

		[DataMember(Name = "deliverydate")]
		public DateTime? DeliveryDate { get; set; }
						
		[DataMember(Name = "invoicenumber")]
		public string InvoiceNumber { get; set; }

		[DataMember(Name = "invoicestatus")]
		public string InvoiceStatus { get; set; }
				
		[DataMember(Name = "itemcount")]
		public int ItemCount { get { return this.LineItems.Sum(l => l.Quantity); } }

		[DataMember(Name = "ordertotal")]
		public double OrderTotal { get; set; }

		[DataMember(Name = "createddate")]
		public DateTime CreatedDate { get; set; }

        [DataMember(Name = "requestedshipdate")]
        public DateTime RequestedShipDate { get; set; }
        
        [DataMember(Name = "ischangeorderallowed")]
        public bool IsCangeOrderAllowed { get; set; }

		[DataMember(Name ="LineItems")]
		public List<OrderLine> LineItems { get; set; }

        [DataMember(Name = "CommerceId")]
        public Guid CommerceId { get; set; }
	}
}
