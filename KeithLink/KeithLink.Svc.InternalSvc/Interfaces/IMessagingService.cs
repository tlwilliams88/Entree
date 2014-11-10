using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Models.Messaging.EF;
using System.ServiceModel;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Profile;

namespace KeithLink.Svc.InternalSvc.Interfaces
{
    [ServiceContract]
    interface IMessagingService
    {
        // notificationEndpoint can be an email address or a device id
        [OperationContract]
        bool AddUserSubscription(NotificationType notificationType, Channel channel, Guid userId, string customerNumber, string notificationEndpoint);
        [OperationContract]
        bool RemoveUserSubscription(NotificationType notificationType, Channel channel, Guid userId, string customerNumber, string notificationEndpoint);
        [OperationContract]
        List<UserMessage> GetUserMessages(Guid userId);
        [OperationContract]
        long CreateUserMessage(Guid userId, UserSelectedContext catalogInfo, UserMessageModel userMessage);
        [OperationContract]
        List<UserMessageModel> ReadUserMessages(UserProfile user);
        [OperationContract]
        void UpdateUserMessages(List<UserMessageModel> userMessages);
        [OperationContract]
        void TestNotification();
        [OperationContract]
        int GetUnreadMessagesCount(UserProfile user);
    }
}
