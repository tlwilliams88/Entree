using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Messaging
{
    public interface INotificationQueueConsumer
    {
        string RabbitMQQueueName { get; set; }

        void ListenForNotificationMessagesOnQueue();

        void Stop();

        void SubscribeToQueue(string queue);

        void Unsubscribe();
    }
}
