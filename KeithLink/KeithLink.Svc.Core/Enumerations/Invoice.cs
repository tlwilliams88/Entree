using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Enumerations
{
	public enum InvoiceType
	{
		[Description("Invoice")]
		Invoice,
		[Description("Credit Memo")]
		CreditMemo,
		[Description("Adjustment")]
		Adjustment,
		[Description("Write Off")]
		WriteOff,
		[Description("Billing Only Invoice")]
		BillingOnlyInvoice,
		[Description("On Account")]
		OnAccount,
		[Description("Write Off Credit")]
		WriteOffCredit,
		[Description("Debit Memo")]
		DebitMemo,
		[Description("Payment")]
		Payment,
		[Description("Maintenance")]
		Maintenance
	}

	public enum InvoiceStatus
	{
		[Description("Open")]
		Open,
		[Description("Paid")]
		Paid,
		[Description("Past Due")]
		PastDue
	}
}
