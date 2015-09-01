using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Messaging
{
    public interface INotificationQueueConsumer
    {
        void ListenForNotificationMessagesOnQueue();
        void Stop();
        void StopInternal();
        void StopExternal();
    }
}
