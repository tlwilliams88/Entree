using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Orders
{
	[DataContract(Name = "OrderLine")]
	public class OrderLine: BaseProductInfo
	{
		[DataMember(Name = "linetotal")]
		public double LineTotal { get { return this.Quantity * this.Price; } }
		
		[DataMember(Name = "quantity")]
		public int Quantity { get; set; }
				
		[DataMember(Name = "packsize")]
		public string PackSize { get; set; }
		
		[DataMember(Name = "name")]
		public string Name { get; set; }
		
		[DataMember(Name = "notes")]
		public string Notes { get; set; }
		
		[DataMember(Name = "each")]
		public bool Each { get; set; }
		
		[DataMember(Name = "storagetemp")]
		public string StorageTemp { get; set; }

		[DataMember(Name = "price")]
		public double Price { get; set; }

        [DataMember(Name = "quantityordered")]
        public int QuantityOrdered { get; set; }

        [DataMember(Name = "quantityshipped")]
        public int QantityShipped { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }
	}
}
