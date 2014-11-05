using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Enumerations.Messaging
{
    // flags (powers of 2)
	public enum NotificationType
	{
		OrderConfirmation = 1,
		OrderUpdate = 2,
	}

    public enum Channel : int
    {
        Email = 1,
        MobilePush = 2,
        Web = 4,
    }
}
