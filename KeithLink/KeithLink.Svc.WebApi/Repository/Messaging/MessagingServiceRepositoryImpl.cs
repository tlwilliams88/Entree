using KeithLink.Svc.Core.Interface.Messaging;
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

namespace KeithLink.Svc.WebApi.Repository.Messaging
{
	public class MessagingServiceRepositoryImpl: IMessagingServiceRepository
	{
        private com.benekeith.MessagingService.IMessagingService serviceClient;

        public MessagingServiceRepositoryImpl(com.benekeith.MessagingService.IMessagingService serviceClient)
        {
            this.serviceClient = serviceClient;
        }

        public List<UserMessageModel> ReadUserMessages(UserProfile user)
        {
            return serviceClient.ReadUserMessages(user).ToList();
        }

        public void MarkAsReadUserMessages(List<UserMessageModel> userMessages)
        {
            serviceClient.MarkAsReadUserMessages(userMessages.ToArray());
        }

        public int GetUnreadMessagesCount(UserProfile user)
        {
            return serviceClient.GetUnreadMessagesCount(user);
        }

        public void UpdateMessagingPreferences(ProfileMessagingPreferenceModel messagingPreferenceModel, UserProfile user)
        {
            serviceClient.UpdateMessagingPreferences(messagingPreferenceModel, user);
        }

        public List<UserMessagingPreferenceModel> ReadMessagingPreferences(Guid userId)
        {
            return serviceClient.ReadMessagingPreferences(userId).ToList();
        }

		public PagedResults<UserMessageModel> ReadPagedUserMessages(UserProfile user, PagingModel paging)
		{
			return serviceClient.ReadPagedUserMessages(user, paging);
		}


        public bool RegisterPushDevice(UserProfile user, PushDeviceRegistrationModel deviceRegistrationModel)
        {
            return serviceClient.RegisterPushDevice(user, deviceRegistrationModel);
        }
    }
}
