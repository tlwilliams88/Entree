using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Payment {
	[DataContract]
    public class PaymentTransactionModel {

		[DataMember(Name = "customernumber")]
		public string CustomerNumber { get; set; }
		[DataMember(Name = "customername")]
		
		public string CustomerName { get; set; }
		[DataMember(Name = "invoicenumber")]
		public string InvoiceNumber { get; set; }
		[DataMember(Name = "account")]
        public string AccountNumber { get; set; }
		[DataMember(Name = "date")]
        public DateTime? PaymentDate { get; set; }
		[DataMember(Name = "amount")]
        public decimal PaymentAmount { get; set; }
		[DataMember(Name = "submittedby")]
		public string UserName { get; set; }
		[DataMember(Name = "confirmationid")]
		public int ConfirmationId { get; set; }
		[DataMember(Name = "submittedon")]
		public DateTime SubmittedDate { get; set; }

		[DataMember(Name = "editable")]
		public bool Editable { get; set; }

    }
}
