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
    public class UserPushNotificationDeviceRepositoryImpl : EFBaseRepository<UserPushNotificationDevice>, IUserPushNotificationDeviceRepository
    {
        public UserPushNotificationDeviceRepositoryImpl(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<UserPushNotificationDevice> ReadUserDevices(Guid userGuid)
        {
            return this.Entities.Where(d => d.UserId == userGuid).ToList();
        }

        public UserPushNotificationDevice ReadUserDevice(Guid userGuid, string deviceId, DeviceOS deviceOs)
        {
            return this.Entities.Where(d => 
                d.UserId == userGuid 
                && d.DeviceId.Equals(deviceId, StringComparison.CurrentCultureIgnoreCase)
                && d.DeviceOS == deviceOs).FirstOrDefault();
        }
    }
}
