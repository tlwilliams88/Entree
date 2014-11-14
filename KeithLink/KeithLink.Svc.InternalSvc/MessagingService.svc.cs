using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Models.Messaging.EF;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.InternalSvc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Profile;

namespace KeithLink.Svc.InternalSvc
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "InvoiceService" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select InvoiceService.svc or InvoiceService.svc.cs at the Solution Explorer and start debugging.
	public class MessagingService : IMessagingService
	{
		private readonly IInternalMessagingLogic messagingLogic;

        public MessagingService(IInternalMessagingLogic messagingLogic)
		{
			this.messagingLogic = messagingLogic;
		}


        public void TestNotification()
        {
            // JUST SOME TESTS - TODO, move to appropriate location
            string tmp = (new Impl.Repository.Queue.GenericQueueRepositoryImpl()).ConsumeFromQueue(Svc.Impl.Configuration.RabbitMQNotificationServer, Svc.Impl.Configuration.RabbitMQNotificationUserNameConsumer,
                Svc.Impl.Configuration.RabbitMQNotificationUserPasswordConsumer, Svc.Impl.Configuration.RabbitMQVHostNotification, Svc.Impl.Configuration.RabbitMQQueueNotification);
            Core.Models.Messaging.Queue.BaseNotification ocn2 = Svc.Core.Extensions.Messaging.NotificationExtension.Deserialize(tmp);
            return;
        }

        public bool AddUserSubscription(NotificationType notificationType, Channel channel, Guid userId, string customerNumber, string notificationEndpoint)
        {
            return messagingLogic.AddUserSubscription(notificationType, channel, userId, customerNumber, notificationEndpoint);
        }

        public bool RemoveUserSubscription(NotificationType notificationType, Channel channel, Guid userId, string customerNumber, string notificationEndpoint)
        {
            throw new NotImplementedException();
        }

        public List<UserMessage> GetUserMessages(Guid userId)
        {
            throw new NotImplementedException();
        }

        public long CreateUserMessage(Guid userId, UserSelectedContext catalogInfo, UserMessageModel userMessage)
        {
            return messagingLogic.CreateUserMessage(userId, catalogInfo, userMessage);
        }

        public List<UserMessageModel> ReadUserMessages(UserProfile user)
        {
            return messagingLogic.ReadUserMessages(user);
        }

        public void MarkAsReadUserMessages(List<UserMessageModel> userMessages)
        {
            messagingLogic.MarkAsReadUserMessages(userMessages);
        }

        public int GetUnreadMessagesCount(UserProfile user)
        {
            return messagingLogic.GetUnreadMessagesCount(user);
        }

        public void UpdateMessagingPreferences(ProfileMessagingPreferenceModel messagingPreferenceModel, UserProfile user)
        {
            messagingLogic.UpdateMessagingPreferences(messagingPreferenceModel, user);
        }

        public List<UserMessagingPreferenceModel> ReadMessagingPreferences(Guid userId)
        {
            return messagingLogic.ReadMessagingPreferences(userId);
        }

    }
}
