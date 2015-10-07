using KeithLink.Svc.Core.Models.Configuration;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Messaging
{
	public interface IMessagingServiceRepository
	{
        void CreateMailMessage(MailMessageModel mailMessage);

        int GetUnreadMessagesCount(Guid userId);

        void MarkAsReadUserMessages(List<UserMessageModel> updatedUserMessages);

        void MarkAllReadByUser( UserProfile userId );
        
        MessageTemplateModel ReadMessageTemplateForKey(string key);

        List<UserMessagingPreferenceModel> ReadMessagingPreferences(Guid userId);

        PagedResults<UserMessageModel> ReadPagedUserMessages(UserProfile user, PagingModel paging);

        List<UserMessageModel> ReadUserMessages(UserProfile userProfile);

        bool RegisterPushDevice(UserProfile user, PushDeviceRegistrationModel deviceRegistrationModel);

        void UpdateMessagingPreferences(ProfileMessagingPreferenceModel messagingPreferenceModel, UserProfile userProfile);
    }
}
