using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Models.Messaging.EF;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Impl.Repository.EF.Operational;

namespace KeithLink.Svc.Impl.Repository.Messaging
{
    public class UserTopicSubscriptionRepositoryImpl : EFBaseRepository<UserTopicSubscription>, IUserTopicSubscriptionRepository
    {
        public UserTopicSubscriptionRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<UserTopicSubscription> GetUserTopicSubscriptions(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
