using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Messaging;

namespace KeithLink.Svc.Core.Interface.Messaging
{
    interface IUserTopicSubscription
    {
        List<UserTopicSubscription> GetUserTopicSubscriptions(Guid userId);
        bool AddUserTopicSubscription(Guid userId, CustomerTopic topic);
        bool RemoveUserTopicSubscription(Guid userId, CustomerTopic topic);
    }
}
