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
		Adjustment
	}

	public enum InvoiceStatus
	{
		[Description("Open")]
		Open,
		[Description("Paid")]
		Paid,
		[Description("Late")]
		Late
	}
}
