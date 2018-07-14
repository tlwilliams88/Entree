using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Core.Models.Messaging.Queue;

namespace Entree.Core.Interface.Messaging
{
    public interface INotificationHandler
    {
        void ProcessNotification(BaseNotification notification);
        //void ProcessNotificationForExternalUsers(BaseNotification notification);
        //void ProcessNotificationForInternalUsers(BaseNotification notification);
    }
}
