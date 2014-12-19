using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.ShoppingCart
{
	[DataContract(Name="ShoppingCart")]
	public class ShoppingCart
	{
		[DataMember(Name="id")]
		public Guid CartId { get; set; }
		[DataMember(Name="name")]
		public string Name { get; set; }
		public string BranchId { get; set; }
		[DataMember(Name="requestedshipdate")]
		public DateTime? RequestedShipDate { get; set; }
		[DataMember(Name="ponumber")]
		public string PONumber { get; set; }
		[DataMember(Name="active")]
		public bool Active { get; set; }
		[DataMember(Name="items")]
		public List<ShoppingCartItem> Items { get; set; }
	}
}
