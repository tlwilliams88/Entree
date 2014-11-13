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
	public class NoMessagingServiceRepositoryImpl: IMessagingServiceRepository
	{
        public NoMessagingServiceRepositoryImpl()
        {
        }

        public List<UserMessageModel> ReadUserMessages(UserProfile user)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserMessages(List<UserMessageModel> userMessages)
        {
            throw new NotImplementedException();
        }

        public int GetUnreadMessagesCount(UserProfile user)
        {
            throw new NotImplementedException();
        }

	}
}
