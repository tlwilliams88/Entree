using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Enumerations.Messaging;

namespace KeithLink.Svc.Core.Models.Messaging.Queue
{
    public class OrderConfirmationNotification : BaseNotification
    {
        public override NotificationType NotificationType
        {
            get { return NotificationType.OrderConfirmation; }
        }
    }
}
