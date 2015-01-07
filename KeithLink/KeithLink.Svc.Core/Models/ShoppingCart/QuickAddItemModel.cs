using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.ShoppingCart
{
	[DataContract]
	public class QuickAddItemModel
	{
		[DataMember(Name = "itemnumber")]
		public string ItemNumber { get; set; }

		[DataMember(Name = "quantity")]
		public decimal Quantity { get; set; }

		[DataMember(Name = "each")]
		public bool Each { get; set; }
	}
}
