using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.ShoppingCart
{
	[DataContract(Name="ShoppingCartItem")]
	public class ShoppingCartItem
	{
		[DataMember(Name = "cartitemid")]
		public Guid CartItemId { get; set; }
		[DataMember(Name = "itemnumber")]
		public string ItemNumber { get; set; }
		[DataMember(Name = "quantity")]
		public decimal Quantity { get; set; }
		[DataMember(Name = "packsize")]
		public string PackSize { get; set; }
		[DataMember(Name = "name")]
		public string Name { get; set; }
		[DataMember(Name ="notes")]
		public string Notes { get; set; }
		[DataMember(Name="packageprice")]
		public double PackagePrice { get; set; }
		[DataMember(Name = "caseprice")]
		public double CasePrice { get; set; }
	}
}
