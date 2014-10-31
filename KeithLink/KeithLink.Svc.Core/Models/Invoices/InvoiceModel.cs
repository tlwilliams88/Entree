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
		[DataMember (Name = "id")]
		public long Id { get; set; }
		[DataMember(Name = "customernumber")]
		public string CustomerNumber { get; set; }
		[DataMember(Name = "invoicenumber")]
		public string InvoiceNumber { get; set; }
		[DataMember(Name = "shipdate")]
		public DateTime? ShipDate { get; set; }
		[DataMember(Name = "orderdate")]
		public DateTime? OrderDate { get; set; }

		[DataMember(Name = "items")]
		public List<InvoiceItemModel> Items { get; set; }


	}
}
