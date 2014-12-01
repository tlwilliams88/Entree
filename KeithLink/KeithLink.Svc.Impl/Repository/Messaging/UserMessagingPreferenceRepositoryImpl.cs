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
    public class UserMessagingPreferenceRepositoryImpl : EFBaseRepository<UserMessagingPreference>, IUserMessagingPreferenceRepository
    {
        public UserMessagingPreferenceRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public IEnumerable<UserMessagingPreference> ReadByUserIdsAndNotificationType(IEnumerable<Guid> ids, NotificationType notificationType)
        {
            var ret = this.Entities.Where(u => ids.Contains(u.UserId) && u.NotificationType == notificationType);
            return ret;
        }
    }
}