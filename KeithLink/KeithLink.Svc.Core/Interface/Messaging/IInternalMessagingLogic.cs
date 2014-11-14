using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Models.Messaging.EF;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Profile;

namespace KeithLink.Svc.Core.Interface.Messaging
{
    public interface IInternalMessagingLogic
    {
        bool AddUserSubscription(NotificationType notificationType, Channel channel, Guid userId, string customerNumber, string notificationEndpoint);
        bool RemoveUserSubscription(NotificationType notificationType, Channel channel, Guid userId, string customerNumber, string notificationEndpoint);
        List<UserMessage> GetUserMessages(Guid userId);
        long CreateUserMessage(Guid userId, UserSelectedContext catalogInfo, UserMessageModel userMessage);
        List<UserMessageModel> ReadUserMessages(UserProfile user);
        bool SendMessage(Core.Models.Messaging.Queue.BaseNotification notification);
        void MarkAsReadUserMessages(List<UserMessageModel> userMessages);
        void ListenForNotificationMessagesOnQueue();
        void Stop();



        int GetUnreadMessagesCount(UserProfile user);

        void UpdateMessagingPreferences(ProfileMessagingPreferenceModel messagingPreferenceModel, UserProfile user);

        List<UserMessagingPreferenceModel> ReadMessagingPreferences(Guid userId);
    }
}
