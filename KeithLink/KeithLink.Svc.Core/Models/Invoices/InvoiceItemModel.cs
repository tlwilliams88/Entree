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
		[DataMember(Name = "quantityordered")]
		public int? QuantityOrdered { get; set; }
		[DataMember(Name = "quantityshipped")]
		public int? QuantityShipped { get; set; }
		[DataMember(Name = "brokencasecode")]
		public string BrokenCaseCode { get; set; }
		[DataMember(Name = "catchweightcode")]
		public string CatchWeightCode { get; set; }
		[DataMember(Name = "extcatchweight")]
		public decimal? ExtCatchWeight { get; set; }
		[DataMember(Name = "itemprice")]
		public decimal? ItemPrice { get; set; }
		[DataMember(Name = "pricebooknumber")]
		public string PriceBookNumber { get; set; }
		[DataMember(Name = "itempricesalesrep")]
		public decimal? ItemPriceSalesRep { get; set; }
		[DataMember(Name = "extsalesrepamount")]
		public decimal? ExtSalesRepAmount { get; set; }
		[DataMember(Name = "extsalesgross")]
		public decimal? ExtSalesGross { get; set; }
		[DataMember(Name = "extsalesnet")]
		public decimal? ExtSalesNet { get; set; }
		[DataMember(Name = "vendornumber")]
		public int? VendorNumber { get; set; }
		[DataMember(Name = "customerpo")]
		public string CustomerPO { get; set; }
		[DataMember(Name = "combinedstatmentcustomer")]
		public string CombinedStatmentCustomer { get; set; }
		[DataMember(Name = "pricebook")]
		public string PriceBook { get; set; }

	}
}
