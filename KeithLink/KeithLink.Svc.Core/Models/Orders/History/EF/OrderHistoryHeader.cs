using KeithLink.Svc.Core.Models.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Orders.History.EF
{
	public class OrderHistoryHeader: BaseEFModel
	{
		[MaxLength(1)]
		[Column(TypeName = "char")]
		public string OrderSystem { get; set; }
		[MaxLength(3)]
		[Column(TypeName = "char")]
		public string BranchId { get; set; }
		[MaxLength(6)]
		[Column(TypeName = "char")]
		public string CustomerNumber { get; set; }
		[MaxLength(8)]
		[Column(TypeName = "char")]		
		public string InvoiceNumber { get; set; }
		public DateTime? DeliveryDate { get; set; }
		[MaxLength(20)]
		public string PONumber { get; set; }
		[MaxLength(7)]
		[Column(TypeName = "char")]
		public string ControlNumber { get; set; }
		[MaxLength(1)]
		[Column(TypeName = "char")]
		public string OrderStatus { get; set; }
		public bool FutureItems { get; set; }
		public bool ErrorStatus { get; set; }
		[MaxLength(3)]
		[Column(TypeName = "char")]
		public string RouteNumber { get; set; }
		[MaxLength(3)]
		[Column(TypeName = "char")]
		public string StropNumber { get; set; }

		public virtual ICollection<OrderHistoryDetail> OrderDetails { get; set; }
	}
}
