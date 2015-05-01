using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Models.Messaging.EF;
using KeithLink.Svc.Core.Models.Messaging.Queue;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Extensions.Messaging;
using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Common.Core.Helpers;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Interface.Profile;

namespace KeithLink.Svc.Impl.Logic.InternalSvc
{
	public class InternalMessagingLogic : IInternalMessagingLogic
	{
		private readonly IUnitOfWork unitOfWork;
        private readonly IUserMessageRepository userMessageRepository;
        private readonly IUserMessagingPreferenceRepository userMessagingPreferenceRepository;
        private readonly IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository;
        private readonly IPushNotificationMessageProvider pushNotificationMessageProvider;
        private readonly Common.Core.Logging.IEventLogRepository eventLogRepository;
		private readonly IUserProfileLogic userProfileLogic;

        public InternalMessagingLogic(IUnitOfWork unitOfWork, IUserMessageRepository userMessageRepository, IUserMessagingPreferenceRepository userMessagingPreferenceRepository,
            Common.Core.Logging.IEventLogRepository eventLogRepository, IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository,
            IPushNotificationMessageProvider pushNotificationMessageProvider, IUserProfileLogic userProfileLogic)
        {
            this.unitOfWork = unitOfWork;
            this.userMessageRepository = userMessageRepository;
            this.userMessagingPreferenceRepository = userMessagingPreferenceRepository;
            this.userPushNotificationDeviceRepository = userPushNotificationDeviceRepository;
            this.pushNotificationMessageProvider = pushNotificationMessageProvider;
            this.eventLogRepository = eventLogRepository;
			this.userProfileLogic = userProfileLogic;
        }

        public long CreateUserMessage(Guid userId, UserSelectedContext catalogInfo, UserMessageModel userMessage)
        {
            var newUserMessage = userMessage.ToEFUserMessage();
            newUserMessage.CustomerNumber = catalogInfo.CustomerId;
            newUserMessage.UserId = userId;

            userMessageRepository.CreateOrUpdate(newUserMessage);
            unitOfWork.SaveChanges();
            return newUserMessage.Id;
        }

        public List<UserMessageModel> ReadUserMessages(UserProfile user)
        {
            var userMessages = userMessageRepository.ReadUserMessages(user).ToList();

            if (userMessages == null)
                return null;

            return userMessages.Select(b => b.ToUserMessageModel()).ToList();

        }

        public void MarkAsReadUserMessages(List<UserMessageModel> userMessages)
        {
            foreach (var userMessage in userMessages)
            {
                var currentUserMessage = userMessageRepository.Read(a => a.Id.Equals(userMessage.Id)).FirstOrDefault();
                //update message read date
                currentUserMessage.MessageReadUtc = userMessage.MessageReadUtc;
            }
            unitOfWork.SaveChanges();
        }

        public UserMessagingPreferenceModel ReadUserMessagingPreference(long userMessagingPreferenceId)
        {
            var currentUserMessagingPreference = userMessagingPreferenceRepository.Read(u => u.Id.Equals(userMessagingPreferenceId)).FirstOrDefault();

            if (currentUserMessagingPreference == null)
                return null;

            return new UserMessagingPreferenceModel()
            {
                Id = currentUserMessagingPreference.Id,
                CustomerNumber = currentUserMessagingPreference.CustomerNumber,
                Channel = currentUserMessagingPreference.Channel,
                NotificationType = currentUserMessagingPreference.NotificationType,
                UserId = currentUserMessagingPreference.UserId
            };
        }

        public List<UserMessagingPreferenceModel> ReadMessagingPreferences(Guid userId)
        {
            var currentUserMessagingPreferences = userMessagingPreferenceRepository.Read(u => u.UserId.Equals(userId));

            if (currentUserMessagingPreferences == null)
                return null;

            var messagingPreferencesList = new List<UserMessagingPreferenceModel>();

            foreach (var currentMsgPref in currentUserMessagingPreferences)
            {
                messagingPreferencesList.Add(new UserMessagingPreferenceModel()
                {
                    Id = currentMsgPref.Id,
                    CustomerNumber = currentMsgPref.CustomerNumber,
                    Channel = currentMsgPref.Channel,
                    NotificationType = currentMsgPref.NotificationType,
                    UserId = currentMsgPref.UserId
                });
            }

            return messagingPreferencesList;
        }

        public int GetUnreadMessagesCount(Guid userId)
        {
            int count = userMessageRepository.GetUnreadMessagesCount(userId);

            return count;
        }

        public void UpdateMessagingPreferences(ProfileMessagingPreferenceModel updatedMessagingPreferenceModel, UserProfile user)
        {
            //first delete existing messaging preferences
            DeleteMessagingPreferencesByCustomer(user.UserId, updatedMessagingPreferenceModel.CustomerNumber, updatedMessagingPreferenceModel.BranchId);

            //then create messaging preferences
            CreateMessagingPreferencesByCustomer(user.UserId, updatedMessagingPreferenceModel);
        }

        //this also works for user default since customer = null for user default
        public void DeleteMessagingPreferencesByCustomer(Guid userId, string customerNumber, string branchId)
        {
            var messagingPreferences = userMessagingPreferenceRepository.Read(i => (i.UserId.Equals(userId) && i.CustomerNumber.Equals(customerNumber) && i.BranchId.Equals(branchId, StringComparison.InvariantCultureIgnoreCase)));

            foreach (var pref in messagingPreferences)
            {
                userMessagingPreferenceRepository.Delete(pref);
            }
            unitOfWork.SaveChanges();
        }

        //this also works for user default since customer = null for user default
        public void CreateMessagingPreferencesByCustomer(Guid userId, ProfileMessagingPreferenceModel messagingPrefModel)
        {
            foreach (var currentPreference in messagingPrefModel.Preferences)
            {
                foreach (var channel in currentPreference.SelectedChannels)
                {
                    var newPreference = new UserMessagingPreference()
                    {
                        Channel = channel.Channel,
                        CustomerNumber = messagingPrefModel.CustomerNumber,
                        NotificationType = currentPreference.NotificationType,
                        UserId = userId,
                        BranchId = messagingPrefModel.BranchId == null ? null : messagingPrefModel.BranchId.ToLower()
                    };
                    userMessagingPreferenceRepository.Create(newPreference);
                }
            }
            unitOfWork.SaveChanges();
        }
		
		public PagedResults<UserMessageModel> ReadPagedUserMessages(UserProfile user, PagingModel paging)
		{
			var userMessages = userMessageRepository.ReadUserMessages(user).ToList();

			return userMessages.Select(m => m.ToUserMessageModel()).AsQueryable<UserMessageModel>().GetPage<UserMessageModel>(paging, "MessageCreatedUtc");

			//var returnValue = new PagedResults<UserMessageModel>();

			//returnValue.TotalResults = userMessages.Count();

			//returnValue.Results = userMessages.Select(u => u.ToUserMessageModel()).Skip(paging.From.HasValue ? paging.From.Value : 0).Take(paging.Size.HasValue ? paging.Size.Value : int.MaxValue).ToList();

			//return returnValue;
		}


        public bool RegisterPushDevice(UserProfile user, PushDeviceRegistrationModel deviceRegistrationModel)
        {
            UserPushNotificationDevice userPushNotificationDevice =
                userPushNotificationDeviceRepository.ReadUserDevice(user.UserId, deviceRegistrationModel.DeviceId, deviceRegistrationModel.DeviceOS);

            if (userPushNotificationDevice == null)
            {
                userPushNotificationDevice = new UserPushNotificationDevice()
                {
                    DeviceOS = deviceRegistrationModel.DeviceOS,
                    DeviceId = deviceRegistrationModel.DeviceId,
                    UserId = user.UserId
                };
            }

            userPushNotificationDevice.ProviderToken = deviceRegistrationModel.ProviderToken;
            userPushNotificationDevice.ProviderEndpointId =
                pushNotificationMessageProvider.RegisterRecipient(userPushNotificationDevice);

            userPushNotificationDeviceRepository.CreateOrUpdate(userPushNotificationDevice);
            unitOfWork.SaveChanges();

            // now, to create/confirm/update the application endpoint in AWS
            return true;
        }


		public void CreateMailMessage(MailMessageModel mailMessage)
		{
			Dictionary<Guid, UserMessage> messages = new Dictionary<Guid, UserMessage>();

			//If CustomerIds are provided, send message to every user for that customer
			if (mailMessage.CustomerIds != null && mailMessage.CustomerIds.Any())
			{
				foreach (var customer in mailMessage.CustomerIds)
				{
					var users = userProfileLogic.GetUsers(new UserFilterModel() { CustomerId = customer });

					foreach (var user in users.UserProfiles)
					{
						if (!messages.ContainsKey(user.UserId))
							messages.Add(user.UserId, new UserMessage()
							{
								Body = mailMessage.Message.Body,
								Subject = mailMessage.Message.Subject,
								Label = mailMessage.Message.Label,
								Mandatory = mailMessage.Message.Mandatory,
								UserId = user.UserId,
								NotificationType = NotificationType.Mail
							});
					}

				}
			}

			//Add any messages to specific users
			if (mailMessage.UserIds != null && mailMessage.UserIds.Any())
			{
				foreach(var user in mailMessage.UserIds)
					if (!messages.ContainsKey(user))
						messages.Add(user, new UserMessage()
						{
							Body = mailMessage.Message.Body,
							Subject = mailMessage.Message.Subject,
							Label = mailMessage.Message.Label,
							Mandatory = mailMessage.Message.Mandatory,
							UserId = user,
							NotificationType = NotificationType.Mail
						});
			}

			//Create all of the message records
			foreach (var message in messages)
				userMessageRepository.Create(message.Value);

			unitOfWork.SaveChanges();

		}
	}
}
