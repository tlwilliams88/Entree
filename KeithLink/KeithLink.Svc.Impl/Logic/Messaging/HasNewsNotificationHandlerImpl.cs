﻿using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Extensions.Messaging;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Messaging.Provider;
using KeithLink.Svc.Core.Models.Messaging.Queue;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Messaging {
    public class HasNewsNotificationHandlerImpl : BaseNotificationHandlerImpl, INotificationHandler {
        #region attributes
        private readonly IEventLogRepository eventLogRepository;
        private readonly IUserProfileLogic userProfileLogic;
        private readonly IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IUserMessagingPreferenceRepository userMessagingPreferenceRepository;
        private readonly Func<Channel, IMessageProvider> messageProviderFactory;
        #endregion

        #region ctor
        public HasNewsNotificationHandlerImpl(IEventLogRepository eventLogRepository, IUserProfileLogic userProfileLogic, IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository, 
                                              ICustomerRepository customerRepository, IUserMessagingPreferenceRepository userMessagingPreferenceRepository, Func<Channel, IMessageProvider> messageProviderFactory, 
                                              IDsrLogic dsrLogic) :
            base(userProfileLogic, userPushNotificationDeviceRepository, customerRepository, 
                 userMessagingPreferenceRepository, messageProviderFactory, 
                 eventLogRepository, dsrLogic) {
            this.eventLogRepository = eventLogRepository;
            this.userProfileLogic = userProfileLogic;
            this.userPushNotificationDeviceRepository = userPushNotificationDeviceRepository;
            this.customerRepository = customerRepository;
            this.userMessagingPreferenceRepository = userMessagingPreferenceRepository;
            this.messageProviderFactory = messageProviderFactory;
        }
        #endregion

        #region methods
        public void ProcessNotification(Core.Models.Messaging.Queue.BaseNotification notification) {
            try {
                if (notification.NotificationType != Core.Enumerations.Messaging.NotificationType.HasNews)
                    throw new ApplicationException("notification/handler type mismatch");

                var hasNewsNotification = (HasNewsNotification)notification;

                Svc.Core.Models.Profile.Customer customer = customerRepository.GetCustomerByCustomerNumber(notification.CustomerNumber, hasNewsNotification.BranchId);

                if (customer == null) {
                    System.Text.StringBuilder warningMessage = new StringBuilder();
                    warningMessage.AppendFormat("Could not find customer({0}-{1}) to send Has News notification.", notification.BranchId, hasNewsNotification.CustomerNumber);
                    warningMessage.AppendLine();
                    warningMessage.AppendLine();
                    warningMessage.AppendLine("Notification:");
                    warningMessage.AppendLine(notification.ToJson());

                    eventLogRepository.WriteWarningLog(warningMessage.ToString());
                }
                else {
                    List<Recipient> recipients = base.LoadRecipients(notification.NotificationType, customer, notification.DSRDSMOnly);

                    if (recipients != null && recipients.Count > 0) {
                        // send messages to providers...
                        Message msg = new Message() {
                            CustomerName = customer.CustomerName,
                            CustomerNumber = customer.CustomerNumber,
                            BranchId = customer.CustomerBranch,
                            MessageSubject = hasNewsNotification.Subject,
                            MessageBody = hasNewsNotification.Notification,
                            NotificationType = NotificationType.HasNews
                        };
                        msg.BodyIsHtml = true;

                        base.SendMessage(recipients, msg);
                    }
                }
            } catch (Exception ex) {
                throw new Core.Exceptions.Queue.QueueDataError<BaseNotification>(notification, "HasNews:ProcessNotification", "Sending HasNews notification", "There was an error sending a HasNews notification", ex);
            }
        }
        
        #endregion
    }
}
