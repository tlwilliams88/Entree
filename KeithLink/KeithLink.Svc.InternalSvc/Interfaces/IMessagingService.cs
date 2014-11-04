using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Models.Messaging.EF;
using System.ServiceModel;

namespace KeithLink.Svc.InternalSvc.Interfaces
{
    [ServiceContract]
    interface IMessagingService
    {
        // notificationEndpoint can be an email address or a device id
        [OperationContract]
        bool AddUserSubscription(MessageType messageType, NotificationType notificationType, Guid userId, string customerNumber, string notificationEndpoint);
        [OperationContract]
        bool RemoveUserSubscription(MessageType messageType, NotificationType notificationType, Guid userId, string customerNumber, string notificationEndpoint);
        [OperationContract]
        List<UserMessage> GetUserMessages(Guid userId);
    }
}
