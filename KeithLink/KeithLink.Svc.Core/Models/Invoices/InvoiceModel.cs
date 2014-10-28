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
		public DateTime DueDate { get; set; }
		[DataMember(Name = "shipdate")]
		public DateTime ShipDate { get; set; }
		[DataMember(Name = "orderdate")]
		public DateTime OrderDate { get; set; }
		[DataMember(Name = "routenumber")]
		public int RouteNumber { get; set; }
		[DataMember(Name = "stopnumber")]
		public int StopNumber { get; set; }
		[DataMember(Name = "datetimeoflastorder")]
		public DateTime DateTimeOfLastOrder { get; set; }

		[DataMember(Name = "customernumber")]
		public int CustomerNumber { get; set; }
		[DataMember(Name = "division")]
		public string Division { get; set; }
		public string Company { get; set; }
		public string Department { get; set; }
		public string WHNumber { get; set; }
		public int OrderNumber { get; set; }
		public string MemoBillCode { get; set; }
		public string CreditHoldFlag { get; set; }
		public string TradeSWFlag { get; set; }
		public string CustomerGroup { get; set; }
		public string SalesRep { get; set; }
		public int ChainStoreCode { get; set; }

		[DataMember(Name = "iskeithnet")]
		public bool IsKeithNet { get; set; }

		[DataMember(Name = "iskeithpay")]
		public bool IsKeithPay { get; set; }

		[DataMember(Name = "items")]
		public List<InvoiceItemModel> Items { get; set; }


	}
}
