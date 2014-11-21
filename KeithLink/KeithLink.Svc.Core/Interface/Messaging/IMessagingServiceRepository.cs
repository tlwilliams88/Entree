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
        List<UserMessageModel> ReadUserMessages(UserProfile userProfile);

        void MarkAsReadUserMessages(List<UserMessageModel> updatedUserMessages);

        int GetUnreadMessagesCount(UserProfile userProfile);

        void UpdateMessagingPreferences(ProfileMessagingPreferenceModel messagingPreferenceModel, UserProfile userProfile);

        List<UserMessagingPreferenceModel> ReadMessagingPreferences(Guid userId);

		PagedResults<UserMessageModel> ReadPagedUserMessages(UserProfile user, PagingModel paging);

        bool RegisterPushDevice(UserProfile user, PushDeviceRegistrationModel deviceRegistrationModel);
    }
}
