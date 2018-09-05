using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Reflection;

using Autofac;

using FluentAssertions;

using KeithLink.Svc.Impl.Repository.Queue;
using KeithLink.Svc.Impl.Repository.SmartResolver;
using KeithLink.Svc.Impl.Tests.Integration.Repository;

using RabbitMQ.Client;
using Xunit;

namespace KeithLink.Svc.Impl.Tests.Integration.Repository.Queue
{
    public class QueueManagerTests
    {
        public class AddTtlToQueue 
        {
            [Fact]
            public void AddTtlToQueue_HasValidHeadersAndOrderDate()
            {
                // arrange
                QueueManager testunit = MakeUnitToBeTested();

                var queues = new List<(string name, int timeToLive)>
                    {
                        ("devonshire_confirmations", 36000000),   // time to live is 10 hours
                        ("devonshire_orders_historyrequest", 3600000),   // time to live is 1 hours
                    };

                // act
                List<QueueDeclareOk> responses = testunit.AddTtlToQueue(Configuration.RabbitMQConfirmationServer,
                                                               "qarqadmin",  // Configuration.RabbitMQUserNameConsumer, 
                                                               "1jqhqt34",  // Configuration.RabbitMQUserPasswordConsumer,
                                                               Configuration.RabbitMQVHostConfirmation,
                                                               queues);

                // assert
                responses.ForEach(response => response.QueueName.Should().Contain("devonshire"));
            }
        }

        private static QueueManager MakeUnitToBeTested()
        {
            //ContainerBuilder builder = DependencyMapFactory.GetOrderServiceContainer();
            //IContainer container = builder.Build();

            //QueueManager testunit = container.Resolve<QueueManager>();

            QueueManager testunit = new QueueManager();

            return testunit;
        }

    }
}