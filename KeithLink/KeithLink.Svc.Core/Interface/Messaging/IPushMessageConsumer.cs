﻿using KeithLink.Svc.Core.Models.Messaging.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Messaging
{
    public interface IPushMessageConsumer
    {
        void ListenForQueueMessages();

        bool ProcessIncomingPushMessage(PushMessage pushmessage);

        void Stop();
    }
}