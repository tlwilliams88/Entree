﻿using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Common.Core.Helpers;
using KeithLink.Svc.Core.Models.Configuration;
using KeithLink.Svc.Core.Models.Configuration.EF;
using KeithLink.Svc.Core.Models.Messaging;
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
                 MessageReadUtc = userMessage.MessageRead.HasValue ? userMessage.MessageRead.Value.ToUniversalTime() : userMessage.MessageRead,
                 NotificationType = userMessage.NotificationType,
                 Subject = userMessage.Subject,
                 Mandatory = userMessage.Mandatory,
                 Label = userMessage.Label,
				 CustomerName = userMessage.CustomerName,
				 CustomerNumber = userMessage.CustomerNumber,
				 BranchId = userMessage.BranchId
            };
        }

        public static UserMessageModel ToUserMessageModel(this UserMessage userMessage)
        {
            // replace switch |LOGO| with blank space, if it's in this message in the body
            return new UserMessageModel()
            {
                Body = userMessage.Body.Replace("|LOGO|", "&nbsp;"),
                MessageRead = userMessage.MessageReadUtc.HasValue ? DateTime.SpecifyKind(userMessage.MessageReadUtc.Value.ToLocalTime(), DateTimeKind.Unspecified) : userMessage.MessageReadUtc,
                NotificationType = userMessage.NotificationType,
                CustomerNumber = userMessage.CustomerNumber,
                Subject = userMessage.Subject,
                Mandatory = userMessage.Mandatory,
                MessageCreated = DateTime.SpecifyKind(userMessage.CreatedUtc.ToLocalTime(), DateTimeKind.Unspecified),
                UserId = userMessage.UserId,
                Id = userMessage.Id,
                Label = userMessage.Label,
				NotificationTypeDescription = EnumUtils<NotificationType>.GetDescription(userMessage.NotificationType),
                CustomerName = userMessage.CustomerName,
                BranchId = userMessage.BranchId
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

		public static MessageTemplateModel ToMessageTemplateModel(this MessageTemplate template)
		{
			return new MessageTemplateModel() { 
				Id = template.Id, 
				Body = template.Body, 
				IsBodyHtml = template.IsBodyHtml, 
				ModifiedOn = template.ModifiedUtc, 
				Subject = template.Subject, 
				TemplateKey = template.TemplateKey 
			};
		}

	}
}
