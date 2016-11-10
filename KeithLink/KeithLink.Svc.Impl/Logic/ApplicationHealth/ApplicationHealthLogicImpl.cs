using KeithLink.Svc.Core.Interface.ApplicationHealth;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Models.ApplicationHealth;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.ApplicationHealth
{
    public class ApplicationHealthLogicImpl: IApplicationHealthLogic
    {
        private IGenericQueueRepository _queueRepo;
        private IEmailClient _email; 
        public ApplicationHealthLogicImpl(IGenericQueueRepository queueRepo, IEmailClient email)
        {
            _queueRepo = queueRepo;
            _email = email;
        }
        public void CheckQueueProperties()
        {
            string checksetting = Configuration.QueuesToCheck;
            QueuesToCheckModel queues = JsonConvert.DeserializeObject<QueuesToCheckModel>(checksetting);
            foreach(QueueToCheckModel queue in queues.targets)
            {
                QueueDeclareOk rslt = _queueRepo.PassivelyDeclareQueue(queue.Server, 
                                                                       queue.UserName, 
                                                                       queue.Password, 
                                                                       queue.VirtualHost, 
                                                                       queue.Queue);

                NewRelic.Api.Agent.NewRelic.RecordMetric(queue.LogicalName + ".messages", rslt.MessageCount);
                NewRelic.Api.Agent.NewRelic.RecordMetric(queue.LogicalName + ".consumers", rslt.ConsumerCount);

                if(DateTime.Now.Minute % 5 == 0) // only send email alerts every 5 minutes
                { // new relic alert service not ready for custom metrics
                    if(rslt != null)
                    {
                        // maximum > 0 to make check
                        // messagecount > maximum to send warning
                        if (queue.MaximumMessagesWarningThreshold > 0 && 
                            rslt.MessageCount > queue.MaximumMessagesWarningThreshold)
                        {
                            string msgSubject = string.Format("BEK: {0} Too many messages", System.Environment.MachineName);
                            string msgBody = string.Format("queue {0} currently has {1} messages and sends warning above {2}",
                                                           queue.Queue,
                                                           rslt.MessageCount,
                                                           queue.MaximumMessagesWarningThreshold);
                            _email.SendEmail(Configuration.FailureEmailAdresses,
                                             null,
                                             null,
                                             msgSubject,
                                             msgBody);
                        }

                        // minimum > 0 to make check
                        // consumercount < minimum to send warning
                        if (queue.MinimumConsumersWarningThreshold > 0 &&
                            rslt.ConsumerCount <= queue.MinimumConsumersWarningThreshold)
                        {
                            string msgSubject = string.Format("BEK: {0} Too few consumers", System.Environment.MachineName);
                            string msgBody = string.Format("{0} consumers on {1} queue with minimum warning is {2}",
                                                           rslt.ConsumerCount,
                                                           queue.Queue,
                                                           queue.MinimumConsumersWarningThreshold);
                            _email.SendEmail(Configuration.FailureEmailAdresses,
                                             null,
                                             null,
                                             msgSubject,
                                             msgBody);
                        }
                    }
                }
            }
        }
    }
}
