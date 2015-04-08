using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Extensions.Messaging;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Models.Messaging.Queue;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Repository.Queue;

using KeithLink.Utility.NotificationParser.Models;
using KeithLink.Utility.NotificationParser.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Utility.NotificationParser {
    class Program {
        static void Main(string[] args) {
            using (NotificationContext dbContext = new NotificationContext()) {
                dbContext.Configuration.AutoDetectChangesEnabled = false;

                GenericQueueRepositoryImpl rmq = new GenericQueueRepositoryImpl();

                string rmqMessage = rmq.ConsumeFromQueue(Configuration.RabbitMQNotificationServer,
                                                         Configuration.RabbitMQNotificationUserNameConsumer,
                                                         Configuration.RabbitMQNotificationUserPasswordConsumer,
                                                         Configuration.RabbitMQVHostNotification,
                                                         Configuration.RabbitMQQueueNotification);

                while (rmqMessage != null) {
                    BaseNotification rmqNotification = NotificationExtension.Deserialize(rmqMessage);

                    if (rmqNotification.NotificationType == NotificationType.Eta) {
                        KeithLink.Svc.Core.Models.Messaging.Queue.EtaNotification eta = (KeithLink.Svc.Core.Models.Messaging.Queue.EtaNotification)rmqNotification;
                        Console.WriteLine(string.Format("Processing {0} orders", eta.Orders.Count));

                        foreach (OrderEta order in eta.Orders) {
                            Models.EtaNotification notice = new Models.EtaNotification();

                            if (order.ActualTime != null && order.ActualTime.Length > 0) { notice.ActualTime = DateTime.Parse(order.ActualTime); }
                            notice.Branch = order.BranchId;
                            if (order.EstimatedTime != null && order.EstimatedTime.Length > 0) { notice.EstimatedTime = DateTime.Parse(order.EstimatedTime); }
                            notice.OrderId = order.OrderId;
                            if (order.OutOfSequence.HasValue) { notice.OutOfSequence = order.OutOfSequence.Value; }
                            notice.RouteId = order.RouteId;
                            if (order.ScheduledTime != null && order.ScheduledTime.Length > 0) { notice.ScheduledTime = DateTime.Parse(order.ScheduledTime); }
                            notice.StopNumber = order.StopNumber;

                            dbContext.Notifications.Add(notice);
                            dbContext.SaveChanges();
                        } // end foreach
                    } // end if

                    rmqMessage = rmq.ConsumeFromQueue(Configuration.RabbitMQNotificationServer,
                                                      Configuration.RabbitMQNotificationUserNameConsumer,
                                                      Configuration.RabbitMQNotificationUserPasswordConsumer,
                                                      Configuration.RabbitMQVHostNotification,
                                                      Configuration.RabbitMQQueueNotification);
                } // wend

                if (dbContext.Database.Connection.State == System.Data.ConnectionState.Open) { dbContext.Database.Connection.Close(); }
            } //end using
        }
    }
}
