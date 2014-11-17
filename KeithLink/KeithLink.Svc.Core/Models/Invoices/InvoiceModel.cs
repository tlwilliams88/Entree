using KeithLink.Svc.Core.Enumerations;
using KeithLink.Svc.Core.Helpers;
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

		[DataMember(Name = "invoicedate")]
		public DateTime? InvoiceDate { get; set; }

		[DataMember(Name = "orderdate")]
		public DateTime? OrderDate { get; set; }

		[DataMember(Name = "duedate")]
		public DateTime? DueDate { get; set; }

		[DataMember(Name = "status")]
		public InvoiceStatus Status { get; set; }

		[DataMember(Name = "statusdescription")]
		public string StatusDescription { get; set; }

		[DataMember(Name = "type")]
		public InvoiceType Type { get; set; }

		[DataMember(Name = "typedescription")]
		public string TypeDescription { get; set; }

		[DataMember(Name = "amount")]
		public decimal Amount { get; set; }

		[DataMember(Name = "items")]
		public List<InvoiceItemModel> Items { get; set; }
		
        [DataMember(Name="branchid")]
        public string BranchId { get; set; }

        [DataMember(Name = "itemsequence")]
        public int ItemSequence { get; set; }

        [DataMember(Name="ispayable")]
        public bool IsPayable { get; set; }
	}
}
