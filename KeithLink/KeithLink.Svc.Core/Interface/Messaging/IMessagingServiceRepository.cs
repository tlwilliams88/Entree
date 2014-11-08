using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Messaging;
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

        void UpdateUserMessages(List<UserMessageModel> updatedUserMessages);
    }
}
