using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Models.Configuration;
using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Messaging.EF;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace KeithLink.Svc.InternalSvc.Interfaces {
    [ServiceContract]
    interface IMessagingService {
        [OperationContract]
        void CreateMailMessage(MailMessageModel mailMessage);

        [OperationContract]
        long CreateUserMessage(Guid userId, UserSelectedContext catalogInfo, UserMessageModel userMessage);

        [OperationContract]
        int GetUnreadMessagesCount(Guid userId);

        [OperationContract]
        void MarkAsReadUserMessages(List<UserMessageModel> userMessages);

        [OperationContract]
        List<UserMessagingPreferenceModel> ReadMessagingPreferences(Guid userId);

        [OperationContract]
        MessageTemplateModel ReadMessageTemplateForKey(string key);
        
        [OperationContract]
		PagedResults<UserMessageModel> ReadPagedUserMessages(UserProfile user, PagingModel paging);

        [OperationContract]
        List<UserMessageModel> ReadUserMessages(UserProfile user);

        [OperationContract]
        bool RegisterPushDevice(UserProfile user, PushDeviceRegistrationModel deviceRegistrationModel);

        [OperationContract]
        void UpdateMessagingPreferences(ProfileMessagingPreferenceModel messagingPreferenceModel, UserProfile user);
    }
}
