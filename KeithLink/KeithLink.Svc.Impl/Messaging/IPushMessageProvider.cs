using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Core.Models.Messaging.Provider;
using Entree.Core.Models.Messaging.EF;

namespace Entree.Core.Interface.Messaging
{
    public interface IPushNotificationMessageProvider : IMessageProvider
    {
        string RegisterRecipient(UserPushNotificationDevice userPushNotificationDevice);
    }
}
