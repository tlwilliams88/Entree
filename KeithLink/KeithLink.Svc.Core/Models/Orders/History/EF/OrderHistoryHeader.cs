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
        [Index("IdxOrderHeader", 0)]
		public string BranchId { get; set; }
		[MaxLength(6)]
		[Column(TypeName = "char")]
        [Index("IdxCustomerNumberByDate", 0)]
		public string CustomerNumber { get; set; }
		[MaxLength(10)]
		[Column(TypeName = "varchar")]		
        [Index("IdxOrderHeader", 1)]
		public string InvoiceNumber { get; set; }
        [Index("IdxCustomerNumberByDate", 1)]
		public DateTime? DeliveryDate { get; set; }
		[MaxLength(20)]
		public string PONumber { get; set; }
		[MaxLength(7)]
		[Column(TypeName = "char")]
		public string ControlNumber { get; set; }
        [MaxLength(7)]
        [Column(TypeName = "char")]
        public string OriginalControlNumber { get; set; }
        [MaxLength(1)]
		[Column(TypeName = "char")]
		public string OrderStatus { get; set; }
		public bool FutureItems { get; set; }
		public bool ErrorStatus { get; set; }
		[MaxLength(4)]
		[Column(TypeName = "char")]
		public string RouteNumber { get; set; }
		[MaxLength(3)]
		[Column(TypeName = "char")]
		public string StopNumber { get; set; }
        public DateTime? ScheduledDeliveryTime { get; set; }
        public DateTime? EstimatedDeliveryTime { get; set; }
        public DateTime? ActualDeliveryTime { get; set; }
        public bool? DeliveryOutOfSequence { get; set; }

		public virtual ICollection<OrderHistoryDetail> OrderDetails { get; set; }
	}
}
