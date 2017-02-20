using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Messaging {
    public interface IMessagingLogic {
        void CreateMailMessage(MailMessageModel mailMessage);

        long CreateUserMessage(Guid userId, UserSelectedContext catalogInfo, UserMessageModel userMessage);

        int GetUnreadMessagesCount(Guid userId);

        void MarkAllReadByUser(UserProfile userId);

        void MarkAsReadUserMessages(List<UserMessageModel> userMessages);

        List<UserMessagingPreferenceModel> ReadMessagingPreferences(Guid userId);

        PagedResults<UserMessageModel> ReadPagedUserMessages(UserProfile user, PagingModel paging);

        bool ForwardUserMessage(UserProfile requester, ForwardUserMessageModel forwardrequest);

        List<UserMessageModel> ReadUserMessages(UserProfile user);

        bool RegisterPushDevice(UserProfile user, PushDeviceRegistrationModel deviceRegistrationModel);

        void UpdateMessagingPreferences(ProfileMessagingPreferenceModel messagingPreferenceModel, UserProfile user);
    }
}
