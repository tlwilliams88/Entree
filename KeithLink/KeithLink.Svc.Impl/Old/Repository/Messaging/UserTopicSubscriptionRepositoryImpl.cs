using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Core.Interface.Messaging;
using Entree.Core.Models.Messaging.EF;
using Entree.Core.Enumerations.Messaging;
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
