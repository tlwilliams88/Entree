using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Messaging
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

        public void UpdateUserMessages(List<UserMessageModel> userMessages)
        {
            serviceClient.UpdateUserMessages(userMessages.ToArray());
        }

	}
}
