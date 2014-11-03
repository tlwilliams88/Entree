using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.EF
{
	public class InvoiceItem: BaseEFModel
	{
		[MaxLength(10)]
		public string ItemNumber { get; set; }
		public int? QuantityOrdered { get; set; }
		public int? QuantityShipped { get; set; }
		public bool CatchWeightCode { get; set; }
		public decimal? ExtCatchWeight { get; set; }
		public decimal? ItemPrice { get; set; }
		public decimal? ExtSalesNet { get; set; }
		[MaxLength(6)]
		public string LineNumber { get; set; }
		[MaxLength(2)]
		[Column(TypeName = "char")]
		public string ClassCode { get; set; }

		public long InvoiceId { get; set; }

		[NotMapped]
		public string InvoiceNumber { get; set; }

	}
}
