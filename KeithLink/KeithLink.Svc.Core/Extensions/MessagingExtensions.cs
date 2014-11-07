using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Messaging.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions
{
	public static class MessagingExtensions
	{
        public static UserMessage ToEFUserMessage(this UserMessageModel userMessage)
        {
            return new KeithLink.Svc.Core.Models.Messaging.EF.UserMessage()
            {
                 Body = userMessage.Body,
                 MessageReadUtc = userMessage.MessageReadUtc,
                 NotificationType = userMessage.NotificationType,
                 Subject = userMessage.Subject,
                 Mandatory = userMessage.Mandatory,
                 OrderNumber = userMessage.OrderNumber
            };
        }

        public static UserMessageModel ToUserMessageModel(this UserMessage userMessage)
        {
            return new UserMessageModel()
            {
                Body = userMessage.Body,
                MessageReadUtc = userMessage.MessageReadUtc,
                NotificationType = userMessage.NotificationType,
                CustomerNumber = userMessage.CustomerNumber,
                Subject = userMessage.Subject,
                Mandatory = userMessage.Mandatory,
                MessageCreatedUtc = userMessage.CreatedUtc,
                UserId = userMessage.UserId,
                Id = userMessage.Id,
                OrderNumber = userMessage.OrderNumber
            };
        }

	}
}
