using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.EF
{
	public class OrderHistoryDetail: BaseEFModel
	{
		[MaxLength(6)]
		[Column(TypeName = "char")]		
		public string ItemNumber { get; set; }
		public int LineNumber { get; set; }
		public int OrderQuantity { get; set; }
		public int ShippedQuantity { get; set; }
		[MaxLength(1)]
		[Column(TypeName = "char")]
		public string UnitOfMeasure { get; set; }
		public bool CatchWeight { get; set; }
		public bool ItemDeleted { get; set; }
		[MaxLength(6)]
		[Column(TypeName = "char")]
		public string SubbedOriginalItemNumber { get; set; }
		[MaxLength(6)]
		[Column(TypeName = "char")]
		public string ReplacedOriginalItemNumber { get; set; }
		[MaxLength(1)]
		[Column(TypeName = "char")]
		public string ItemStatus { get; set; }
		public decimal TotalShippedWeight { get; set; }
	}
}
