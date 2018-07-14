﻿using System;
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
    public class UserMessagingPreferenceRepositoryImpl : EFBaseRepository<UserMessagingPreference>, IUserMessagingPreferenceRepository
    {
        public UserMessagingPreferenceRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public IEnumerable<UserMessagingPreference> ReadByUserIdsAndNotificationType(IEnumerable<Guid> ids, NotificationType notificationType, bool defaultsOnly = false)
        {
            if(defaultsOnly) {
                return this.Entities.Where(u => ids.Contains(u.UserId) && 
                                                u.NotificationType == notificationType && 
                                                u.CustomerNumber == null);
            } else {
                return this.Entities.Where(u => ids.Contains(u.UserId) && 
                                                u.NotificationType == notificationType);
            }
        }

        public IEnumerable<UserMessagingPreference> ReadByCustomerAndNotificationType(string customerNumber, string branchId, NotificationType notificationType)
        {
            var ret = this.Entities.Where(u => 
                   u.CustomerNumber.Equals(customerNumber, StringComparison.InvariantCultureIgnoreCase)
                && u.BranchId.Equals(branchId, StringComparison.InvariantCultureIgnoreCase)
                && u.NotificationType == notificationType);
            return ret;
        }
    }
}