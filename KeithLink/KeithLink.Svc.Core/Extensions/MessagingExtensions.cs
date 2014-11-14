﻿using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Messaging.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
                 Label = userMessage.Label
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
                Label = userMessage.Label
            };
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

	}
}
