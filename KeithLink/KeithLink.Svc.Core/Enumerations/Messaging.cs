using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Enumerations.Messaging
{
	public enum MessageType
	{
		OrderConfirmation,
		OrderUpdate,
	}

    public enum NotificationType
    {
        Email,
        MobilePush,
        // SMS //FUTURE STATE
    }
}
