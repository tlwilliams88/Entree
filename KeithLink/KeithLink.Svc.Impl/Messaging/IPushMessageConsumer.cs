using Entree.Core.Models.Messaging.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.Messaging
{
    public interface IPushMessageConsumer
    {
        void ListenForQueueMessages();

        void Stop();

        void SubscribeToQueue();

        void Unsubscribe();

        bool ProcessIncomingPushMessage(PushMessage pushmessage);
    }
}
