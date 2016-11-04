using KeithLink.Svc.Core.Exceptions.Queue;
using KeithLink.Svc.Core.Interface.ApplicationHealth;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Models.ApplicationHealth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.ApplicationHealth
{
    public class QueueHealthCheckImpl : IQueueHealthCheck
    {
        public QueueHealthCheckImpl(IGenericSubsriptionQueueRepository queueRepo)
        {
            _queueRepo = queueRepo;
        }
        private IGenericSubsriptionQueueRepository _queueRepo;
        public void CheckQueueProperties()
        {
            QueuesToCheck queuesToCheck = JsonConvert.DeserializeObject<QueuesToCheck>(Configuration.QueuesToCheckSizes);
            foreach(QueueToCheck queueToCheck in queuesToCheck.Targets)
            {
                string server = queueToCheck.Server;
                string userName = queueToCheck.UserName;
                string password = queueToCheck.Password;
                string virtualHost = queueToCheck.VirtualHost;
                string queueLogical = queueToCheck.LogicalName;
                string queueActual = queueToCheck.Queue;
                _queueRepo.CheckQueueHealth(server, userName, password, virtualHost, queueLogical, queueActual);
            }
        }

    }
}
