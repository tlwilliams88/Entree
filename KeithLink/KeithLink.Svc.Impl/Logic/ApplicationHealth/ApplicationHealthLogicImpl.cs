using KeithLink.Common.Core.Interfaces.Logging;
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
        private readonly IEventLogRepository _log;
        public ApplicationHealthLogicImpl(IGenericQueueRepository queueRepo, IEmailClient email, IEventLogRepository log)
        {
            _queueRepo = queueRepo;
            _email = email;
            _log = log;
        }
        public void CheckQueueProperties()
        {
            // New relic metrics only show on data explorer on metrics section (and should be added to dashboard)
            string checksetting = Configuration.QueuesToCheck;
            QueuesToCheckModel queues = JsonConvert.DeserializeObject<QueuesToCheckModel>(checksetting);
            foreach(QueueToCheckModel queue in queues.targets)
            {
                QueueDeclareOk rslt = _queueRepo.PassivelyDeclareQueue(queue.Server, 
                                                                       queue.UserName, 
                                                                       queue.Password, 
                                                                       queue.VirtualHost, 
                                                                       queue.Queue);

                NewRelic.Api.Agent.NewRelic.RecordMetric
                    ("Custom/MessageCounts/" + queue.LogicalName, rslt.MessageCount);
                NewRelic.Api.Agent.NewRelic.RecordMetric
                    ("Custom/ConsumerCounts/" + queue.LogicalName, rslt.ConsumerCount);
                //_log.WriteInformationLog(string.Format("{0} msgs/consumers {1}/{2}", 
                //                                       queue.LogicalName, rslt.MessageCount, rslt.ConsumerCount));

                if(DateTime.Now.Minute % 5 == 0) // only send email alerts every 5 minutes
                { // new relic alert service not ready for custom metrics
                    if(rslt != null)
                    {
                        // maximum > 0 to make check
                        // messagecount > maximum to send warning
                        if (int.Parse(queue.MaximumMessagesWarningThreshold) > 0 && 
                            rslt.MessageCount > int.Parse(queue.MaximumMessagesWarningThreshold))
                        {
                            string msgSubject = string.Format("{0} Too many messages", queue.LogicalName);
                            string msgBody = string.Format(
                                    "{0} messages on {1} queue with maximum warning above {2} from {3}",
                                                           rslt.MessageCount,
                                                           queue.Queue,
                                                           queue.MaximumMessagesWarningThreshold,
                                                           System.Environment.MachineName);
                            _email.SendEmail(Configuration.FailureEmailAdresses,
                                             null,
                                             null,
                                             msgSubject,
                                             msgBody);
                        }

                        // minimum > 0 to make check
                        // consumercount < minimum to send warning
                        if (int.Parse(queue.MinimumConsumersWarningThreshold) > 0 &&
                            rslt.ConsumerCount <= int.Parse(queue.MinimumConsumersWarningThreshold))
                        {
                            string msgSubject = string.Format("{0} Too few consumers", queue.LogicalName);
                            string msgBody = string.Format(
                                                "{0} consumers on {1} queue with minimum warning of {2} from {3}",
                                                rslt.ConsumerCount,
                                                queue.Queue,
                                                queue.MinimumConsumersWarningThreshold,
                                                System.Environment.MachineName);
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
