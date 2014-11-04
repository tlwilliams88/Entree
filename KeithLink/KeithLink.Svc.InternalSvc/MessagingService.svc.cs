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
    }
}
