using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Messaging.Provider;
using KeithLink.Svc.Core.Models.Messaging.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Messaging
{
	public class HasNewsNotificationHandlerImpl : BaseNotificationHandlerImpl, INotificationHandler
	{
		private readonly IEventLogRepository eventLogRepository;
		private readonly IUserProfileLogic userProfileLogic;
		private readonly IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository;
		private readonly ICustomerRepository customerRepository;
		private readonly IUserMessagingPreferenceRepository userMessagingPreferenceRepository;
		private readonly Func<Channel, IMessageProvider> messageProviderFactory;

		public HasNewsNotificationHandlerImpl(IEventLogRepository eventLogRepository, IUserProfileLogic userProfileLogic
			, IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository, ICustomerRepository customerRepository
			, IUserMessagingPreferenceRepository userMessagingPreferenceRepository, Func<Channel, IMessageProvider> messageProviderFactory) :
			base(userProfileLogic, userPushNotificationDeviceRepository, customerRepository
					, userMessagingPreferenceRepository, messageProviderFactory, eventLogRepository)
		{
			this.eventLogRepository = eventLogRepository;
			this.userProfileLogic = userProfileLogic;
			this.userPushNotificationDeviceRepository = userPushNotificationDeviceRepository;
			this.customerRepository = customerRepository;
			this.userMessagingPreferenceRepository = userMessagingPreferenceRepository;
			this.messageProviderFactory = messageProviderFactory;
		}

		public void ProcessNotification(Core.Models.Messaging.Queue.BaseNotification notification)
		{
			if (notification.NotificationType != Core.Enumerations.Messaging.NotificationType.HasNews)
				throw new ApplicationException("notification/handler type mismatch");

			var hasNewsNotification = (HasNewsNotification)notification;

			Svc.Core.Models.Profile.Customer customer = customerRepository.GetCustomerByCustomerNumber(notification.CustomerNumber, hasNewsNotification.BranchId);
            
			List<Recipient> recipients = base.LoadRecipients(notification.NotificationType, customer);

			// send messages to providers...
			base.SendMessage(recipients, new Message()
			{
				CustomerName = customer.CustomerName,
				CustomerNumber = customer.CustomerNumber,
				MessageSubject = hasNewsNotification.Subject,
				MessageBody = hasNewsNotification.Notification,
				NotificationType = NotificationType.HasNews
			});
		}
	}
}
