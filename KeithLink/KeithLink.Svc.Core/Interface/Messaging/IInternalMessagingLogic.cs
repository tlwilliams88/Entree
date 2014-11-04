﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Models.Messaging.EF;

namespace KeithLink.Svc.Core.Interface.Messaging
{
    public interface IInternalMessagingLogic
    {
        bool AddUserSubscription(MessageType messageType, NotificationType notificationType, Guid userId, string customerNumber, string notificationEndpoint);
        bool RemoveUserSubscription(MessageType messageType, NotificationType notificationType, Guid userId, string customerNumber, string notificationEndpoint);
        List<UserMessage> GetUserMessages(Guid userId);
    }
}
