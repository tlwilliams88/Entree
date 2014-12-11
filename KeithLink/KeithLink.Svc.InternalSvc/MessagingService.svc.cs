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
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Models.Configuration;

namespace KeithLink.Svc.InternalSvc
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "InvoiceService" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select InvoiceService.svc or InvoiceService.svc.cs at the Solution Explorer and start debugging.
	public class MessagingService : IMessagingService
	{
		private readonly IInternalMessagingLogic messagingLogic;
		private readonly IMessageTemplateLogic messageTemplateLogic;

        public MessagingService(IInternalMessagingLogic messagingLogic, IMessageTemplateLogic messageTemplateLogic)
		{
			this.messagingLogic = messagingLogic;
			this.messageTemplateLogic = messageTemplateLogic;
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

        public int GetUnreadMessagesCount(Guid userId)
        {
			return messagingLogic.GetUnreadMessagesCount(userId);
        }

        public void UpdateMessagingPreferences(ProfileMessagingPreferenceModel messagingPreferenceModel, UserProfile user)
        {
            messagingLogic.UpdateMessagingPreferences(messagingPreferenceModel, user);
        }

        public List<UserMessagingPreferenceModel> ReadMessagingPreferences(Guid userId)
        {
            return messagingLogic.ReadMessagingPreferences(userId);
        }
		
		public PagedResults<UserMessageModel> ReadPagedUserMessages(UserProfile user, PagingModel paging)
		{
			return messagingLogic.ReadPagedUserMessages(user, paging);
		}


        public bool RegisterPushDevice(UserProfile user, PushDeviceRegistrationModel deviceRegistrationModel)
        {
            return messagingLogic.RegisterPushDevice(user, deviceRegistrationModel);
        }


		public MessageTemplateModel ReadMessageTemplateForKey(string key)
		{
			return messageTemplateLogic.ReadForKey(key);
		}


		public void CreateMailMessage(MailMessageModel mailMessage)
		{
			messagingLogic.CreateMailMessage(mailMessage);
		}
	}
}
