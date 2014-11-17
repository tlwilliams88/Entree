using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Enumerations.Messaging
{
    // flags (powers of 2)
	public enum NotificationType
	{
        [Description("My Order is Confirmed")]
		OrderConfirmation = 1,
        [Description("My Order is Shipped")]
		OrderUpdate = 2,
        [Description("My Invoices Need Attention")]
        InvoiceAttention = 4,
        [Description("Ben E. Keith Has News For Me")]
        HasNews = 8
	}

    public enum Channel : int
    {
        [Description("Email")]
        Email = 1,
        [Description("Mobile Push")]
        MobilePush = 2,
        [Description("Web")]
        Web = 4
    }
}
