using KeithLink.Svc.Core.Interface.ModelExport;
using KeithLink.Svc.Core.Models.ModelExport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Payment {
	[DataContract]
	public class PaymentTransactionModel : IExportableModel
	{

		[DataMember(Name = "customernumber")]
		[Description("Customer Number")]
		public string CustomerNumber { get; set; }
		[DataMember(Name = "customername")]
		[Description("Customer Name")]
		public string CustomerName { get; set; }
		[DataMember(Name = "branchid")]
		[Description("Branch")]
		public string BranchId { get; set; }
		[DataMember(Name = "invoicenumber")]
		[Description("Invoice #")]
		public string InvoiceNumber { get; set; }
		[DataMember(Name = "account")]
		[Description("Account")]
        public string AccountNumber { get; set; }
		[DataMember(Name = "date")]
		[Description("Scheduled Date")]
        public DateTime? PaymentDate { get; set; }
		[DataMember(Name = "amount")]
		[Description("Amount")]
        public decimal PaymentAmount { get; set; }
		[DataMember(Name = "submittedby")]
		[Description("Submitted By")]
		public string UserName { get; set; }
		[DataMember(Name = "confirmationid")]
		[Description("Confirmation Id")]
		public int ConfirmationId { get; set; }
		[DataMember(Name = "submittedon")]
		[Description("Submitted Date")]
		public DateTime SubmittedDate { get; set; }

		[DataMember(Name = "editable")]
		public bool Editable { get; set; }


		public List<ExportModelConfiguration> DefaultExportConfiguration()
		{
			var defaultConfig = new List<ExportModelConfiguration>();

			defaultConfig.Add(new ExportModelConfiguration() { Field = "CustomerName", Order = 1, Label = "Customer Name" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "CustomerNumber", Order = 10, Label = "Customer Number" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "ConfirmationId", Order = 20, Label = "Confirmation Id" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "InvoiceNumber", Order = 30, Label = "Invoice #" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "PaymentAmount", Order = 40, Label = "Amount" });
			defaultConfig.Add(new ExportModelConfiguration() { Field = "AccountNumber", Order = 50, Label = "Account" });

			return defaultConfig;
		}
	}
}
