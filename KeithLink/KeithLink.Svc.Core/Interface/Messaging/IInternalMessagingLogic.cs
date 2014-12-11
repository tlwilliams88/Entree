using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Models.Messaging.EF;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Paging;

namespace KeithLink.Svc.Core.Interface.Messaging
{
    public interface IInternalMessagingLogic
    {
        long CreateUserMessage(Guid userId, UserSelectedContext catalogInfo, UserMessageModel userMessage);
		void CreateMailMessage(MailMessageModel mailMessage);

        List<UserMessageModel> ReadUserMessages(UserProfile user);
        void MarkAsReadUserMessages(List<UserMessageModel> userMessages);
		PagedResults<UserMessageModel> ReadPagedUserMessages(UserProfile user, PagingModel paging);
        int GetUnreadMessagesCount(Guid userId);

        void UpdateMessagingPreferences(ProfileMessagingPreferenceModel messagingPreferenceModel, UserProfile user);
        List<UserMessagingPreferenceModel> ReadMessagingPreferences(Guid userId);

        bool RegisterPushDevice(UserProfile user, PushDeviceRegistrationModel deviceRegistrationModel);
    }
}
