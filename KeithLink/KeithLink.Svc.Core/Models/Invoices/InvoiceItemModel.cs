using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Invoices
{
    [DataContract(Name = "InvoiceItem")]
    public class InvoiceItemModel
    {
        [DataMember(Name = "invoiceitemid")]
        public long InvoiceItemId { get; set; }
        [DataMember(Name = "lineitem")]
		public int LineItem { get; set; }
		[DataMember(Name = "invoicetype")]
		public string InvoiceType { get; set; }
		[DataMember(Name = "invoicedate")]
		public DateTime? InvoiceDate { get; set; }
		[DataMember(Name = "amountdue")]
		public decimal? AmountDue { get; set; }
		[DataMember(Name = "deleteflag")]
		public int? DeleteFlag { get; set; }
		public int? QuantityOrdered { get; set; }
		public int? QuantityShipped { get; set; }
		public string BrokenCaseCode { get; set; }
		public string CatchWeightCode { get; set; }
		public decimal? ExtCatchWeight { get; set; }
		public decimal? ItemPrice { get; set; }
		public string PriceBookNumber { get; set; }
		public decimal? ItemPriceSalesRep { get; set; }
		public decimal? ExtSalesRepAmount { get; set; }
		public decimal? ExtSalesGross { get; set; }
		public decimal? ExtSalesNet { get; set; }
		public int? VendorNumber { get; set; }
		public string CustomerPO { get; set; }
		public string CombinedStatmentCustomer { get; set; }
		public string PriceBook { get; set; }

	}
}
