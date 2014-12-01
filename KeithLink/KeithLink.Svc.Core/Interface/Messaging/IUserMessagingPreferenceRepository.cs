using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Messaging.EF;
using KeithLink.Svc.Core.Enumerations.Messaging;

namespace KeithLink.Svc.Core.Interface.Messaging
{
    public interface IUserMessagingPreferenceRepository : IBaseEFREpository<UserMessagingPreference>
    {
        IEnumerable<UserMessagingPreference> ReadByUserIdsAndNotificationType(IEnumerable<Guid> ids, NotificationType notificationType);
        //List<UserTopicSubscription> GetUserTopicSubscriptions(Guid userId);
    }
}