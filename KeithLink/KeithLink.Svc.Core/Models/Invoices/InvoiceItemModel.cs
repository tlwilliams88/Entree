using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Invoices
{
    [DataContract(Name = "InvoiceItem")]
    public class InvoiceItemModel
    {
		[DataMember(Name="id")]
		public long Id { get; set; }
		[DataMember(Name = "linenumber")]
		public string LineNumber { get; set; }
		[DataMember(Name="itemnumber")]
		public string ItemNumber { get; set; }
		[DataMember(Name = "quantityordered")]
		public int? QuantityOrdered { get; set; }
		[DataMember(Name = "quantityshipped")]
		public int? QuantityShipped { get; set; }
		[DataMember(Name = "catchweight")]
		public bool CatchWeightCode { get; set; }
		[DataMember(Name = "extcatchweight")]
		public decimal? ExtCatchWeight { get; set; }
		[DataMember(Name = "itemprice")]
		public decimal? ItemPrice { get; set; }
		[DataMember(Name = "salesnet")]
		public decimal? ExtSalesNet { get; set; }
		[DataMember(Name = "classcode")]
		public string ClassCode { get; set; }


		
		public string InvoiceNumber { get; set; }

	}
}
