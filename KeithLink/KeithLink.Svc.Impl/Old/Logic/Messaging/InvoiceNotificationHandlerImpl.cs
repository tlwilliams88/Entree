using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Core.Interface.Messaging;
using Entree.Core.Models.Messaging.Queue;
using Entree.Core.Models.Messaging.EF;
using Entree.Core.Models.Messaging.Provider;
using Entree.Core.Enumerations.Messaging;
using KeithLink.Common.Core.Interfaces.Logging;
using Entree.Core.Interface.Profile;

namespace KeithLink.Svc.Impl.Logic.Messaging
{
    public class InvoiceNotificationHandlerImpl : INotificationHandler
    {
        #region attributes
        IEventLogRepository eventLogRepository;
        IUserProfileLogic userProfileLogic;
        IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository;
        ICustomerRepository customerRepository;
        IPushNotificationMessageProvider pushNotificationMessageProvider;
        #endregion
        public InvoiceNotificationHandlerImpl(IEventLogRepository eventLogRepository, IUserProfileLogic userProfileLogic
            , IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository, ICustomerRepository customerRepository
            , IPushNotificationMessageProvider pushNotificationMessageProvider)
        {
            this.eventLogRepository = eventLogRepository;
            this.userProfileLogic = userProfileLogic;
            this.userPushNotificationDeviceRepository = userPushNotificationDeviceRepository;
            this.customerRepository = customerRepository;
            this.pushNotificationMessageProvider = pushNotificationMessageProvider;
        }

        public void ProcessNotification(Core.Models.Messaging.Queue.BaseNotification notification)
        {
            if (notification.NotificationType != Core.Enumerations.Messaging.NotificationType.InvoiceAttention)
                throw new ApplicationException("notification/handler type mismatch");
        }

        //public void ProcessNotificationForExternalUsers(Core.Models.Messaging.Queue.BaseNotification notification)
        //{
        //    if (notification.NotificationType != Core.Enumerations.Messaging.NotificationType.InvoiceAttention)
        //        throw new ApplicationException("notification/handler type mismatch");
        //}

        //public void ProcessNotificationForInternalUsers(Core.Models.Messaging.Queue.BaseNotification notification)
        //{
        //    if (notification.NotificationType != Core.Enumerations.Messaging.NotificationType.InvoiceAttention)
        //        throw new ApplicationException("notification/handler type mismatch");
        //}
    }
}
