using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Core.Models.Messaging.EF;
using Entree.Core.Enumerations.Messaging;

namespace Entree.Core.Interface.Messaging
{
    public interface IUserPushNotificationDeviceRepository : IBaseEFREpository<UserPushNotificationDevice>
    {
        List<UserPushNotificationDevice> ReadUserDevices(Guid userGuid);
        UserPushNotificationDevice ReadUserDevice(Guid userGuid, string deviceId, DeviceOS deviceOs);
    }
}
