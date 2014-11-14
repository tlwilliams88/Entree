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
        [Description("Order Confirmation")]
		OrderConfirmation = 1,
        [Description("Order Update")]
		OrderUpdate = 2,
	}

    public enum Channel : int
    {
        [Description("Email")]
        Email = 1,
        [Description("Mobile Push")]
        MobilePush = 2,
        [Description("Web")]
        Web = 4,
    }
}
