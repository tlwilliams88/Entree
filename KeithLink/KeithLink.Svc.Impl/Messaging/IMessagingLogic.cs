using Entree.Core.Models.Messaging;
using Entree.Core.Models.Paging;
using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;

namespace Entree.Core.Interface.Messaging {
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
