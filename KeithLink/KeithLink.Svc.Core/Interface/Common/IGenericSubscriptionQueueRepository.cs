﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace KeithLink.Svc.Core.Interface.Common {
    public interface IGenericSubscriptionQueueRepository {
        ulong GetLastProcessedUndelivered();
        void Ack(EventingBasicConsumer consumer, ulong deliveryTag);
        void Nack(EventingBasicConsumer consumer, ulong deliveryTag);
        void Subscribe(ConnectionFactory connection, string queue);
        void Unsubscribe();


        event RabbitMQ.Client.Events.BasicDeliverEventHandler MessageReceived;
    }
}
