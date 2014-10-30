using KeithLink.Svc.Core.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.Invoices
{
	[DataContract(Name = "Invoice")]
	public class InvoiceModel
	{
		[DataMember(Name = "invoiceid")]
		public long InvoiceId { get; set; }

		[DataMember(Name = "invoicenumber")]
		public string InvoiceNumber { get; set; }
		[DataMember(Name = "duedate")]
		public DateTime? DueDate { get; set; }
		[DataMember(Name = "shipdate")]
		public DateTime? ShipDate { get; set; }
		[DataMember(Name = "orderdate")]
		public DateTime? OrderDate { get; set; }
		[DataMember(Name = "routenumber")]
		public int? RouteNumber { get; set; }
		[DataMember(Name = "stopnumber")]
		public int? StopNumber { get; set; }
		[DataMember(Name = "datetimeoflastorder")]
		public DateTime? DateTimeOfLastOrder { get; set; }

		[DataMember(Name = "customernumber")]
		public int CustomerNumber { get; set; }
		[DataMember(Name = "division")]
		public string Division { get; set; }
		[DataMember(Name = "company")]
		public string Company { get; set; }
		[DataMember(Name = "department")]
		public string Department { get; set; }
		[DataMember(Name = "whnumber")]
		public string WHNumber { get; set; }
		[DataMember(Name = "ordernumber")]
		public int? OrderNumber { get; set; }
		[DataMember(Name = "memobillcode")]
		public string MemoBillCode { get; set; }
		[DataMember(Name = "creditholdflag")]
		public string CreditHoldFlag { get; set; }
		[DataMember(Name = "tradeswflag")]
		public string TradeSWFlag { get; set; }
		[DataMember(Name = "customergroup")]
		public string CustomerGroup { get; set; }
		[DataMember(Name = "salesrep")]
		public string SalesRep { get; set; }
		[DataMember(Name = "chainstorecode")]
		public string ChainStoreCode { get; set; }

		[DataMember(Name = "invoicetype")]
		public InvoiceType InvoiceType { get; set; }

		[DataMember(Name = "items")]
		public List<InvoiceItemModel> Items { get; set; }


	}
}
