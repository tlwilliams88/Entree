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
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Configuration;

namespace KeithLink.Svc.InternalSvc.Interfaces
{
    [ServiceContract]
    interface IMessagingService
    {
        [OperationContract]
        long CreateUserMessage(Guid userId, UserSelectedContext catalogInfo, UserMessageModel userMessage);
        [OperationContract]
        List<UserMessageModel> ReadUserMessages(UserProfile user);
        [OperationContract]
        void MarkAsReadUserMessages(List<UserMessageModel> userMessages);
        [OperationContract]
        int GetUnreadMessagesCount(Guid userId);
        [OperationContract]
        void UpdateMessagingPreferences(ProfileMessagingPreferenceModel messagingPreferenceModel, UserProfile user);
        [OperationContract]
        List<UserMessagingPreferenceModel> ReadMessagingPreferences(Guid userId);
		[OperationContract]
		PagedResults<UserMessageModel> ReadPagedUserMessages(UserProfile user, PagingModel paging);
        [OperationContract]
        bool RegisterPushDevice(UserProfile user, PushDeviceRegistrationModel deviceRegistrationModel);
		[OperationContract]
		MessageTemplateModel ReadMessageTemplateForKey(string key);
		[OperationContract]
		void CreateMailMessage(MailMessageModel mailMessage);
    }
}
