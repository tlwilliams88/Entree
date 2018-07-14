using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Core.Models.Messaging.EF;
using Entree.Core.Enumerations.Messaging;

namespace Entree.Core.Interface.Messaging
{
    public interface IUserMessagingPreferenceRepository : IBaseEFREpository<UserMessagingPreference>
    {
        IEnumerable<UserMessagingPreference> ReadByUserIdsAndNotificationType(IEnumerable<Guid> ids, NotificationType notificationType, bool defaultsOnly = false);
        IEnumerable<UserMessagingPreference> ReadByCustomerAndNotificationType(string customerNumber, string branchId, NotificationType notificationType);
        //List<UserTopicSubscription> GetUserTopicSubscriptions(Guid userId);
    }
}