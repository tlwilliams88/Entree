using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Models.Messaging.EF;
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
using AmazonSNS = Amazon.SimpleNotificationService;
using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Logic.InternalSvc
{
	public class InternalMessagingLogic : IInternalMessagingLogic
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly ICustomerTopicRepository customerTopicRepository;
        private readonly IUserMessageRepository userMessageRepository;
        private readonly IUserMessagingPreferenceRepository userMessagingPreferenceRepository;
        private readonly IGenericQueueRepository genericQueueRepository;
        private readonly Common.Core.Logging.IEventLogRepository eventLogRepository;
        private Task listenForQueueMessagesTask;

        public InternalMessagingLogic(IUnitOfWork unitOfWork, ICustomerTopicRepository customerTopicRepository, IUserMessageRepository userMessageRepository, IUserMessagingPreferenceRepository userMessagingPreferenceRepository,
            IGenericQueueRepository genericQueueRepository, Common.Core.Logging.IEventLogRepository eventLogRepository)
        {
            this.unitOfWork = unitOfWork;
            this.customerTopicRepository = customerTopicRepository;
            this.userMessageRepository = userMessageRepository;
            this.userMessagingPreferenceRepository = userMessagingPreferenceRepository;
            this.genericQueueRepository = genericQueueRepository;
            this.eventLogRepository = eventLogRepository;
        }

        public bool AddUserSubscription(NotificationType notificationType, Channel channel, Guid userId, string customerNumber, string notificationEndpoint)
        {
            // find topics for customer; if not existing, then create
            CustomerTopic topic = customerTopicRepository.ReadTopicForCustomerAndType(customerNumber, notificationType);
            if (topic == null)
            {
                topic = CreateTopic(notificationType, customerNumber);
            }

            // look for existing subscription
            if (topic.Subscriptions == null || !topic.Subscriptions.Any(s => s.Channel == channel && s.NotificationEndpoint == notificationEndpoint && s.UserId == userId))
            {
                // add the subscription
                CreateSubscriptionToTopic(channel, notificationEndpoint, topic, userId);
            }
            return true;
        }

        public bool SendMessage(Core.Models.Messaging.Queue.OrderConfirmationNotification ocn)
        {
            CustomerTopic topic = customerTopicRepository.ReadTopicForCustomerAndType(ocn.CustomerNumber, NotificationType.OrderConfirmation);
            if (topic == null)
            {
                eventLogRepository.WriteInformationLog("no topic found for customer: " + ocn.CustomerNumber + ", event type: " + NotificationType.OrderConfirmation.ToString());
                return true;
            }

            AmazonSNS.IAmazonSimpleNotificationService client =
                Amazon.AWSClientFactory.CreateAmazonSimpleNotificationServiceClient(
                    new Amazon.Runtime.BasicAWSCredentials("AKIAJ42DMK24ZMO56MYQ", "TwrfjLm5y1G4xcUvYMAgn+/+kXDuyNUZIRD7qvA0"), // TODO: Config
                    Amazon.RegionEndpoint.USEast1);
            
            client.Publish(
                new AmazonSNS.Model.PublishRequest(topic.ProviderTopicId, // topic arn
                    "received order confirmation for " + ocn.CustomerNumber + ", order: " + ocn.OrderChange.OrderName, // topic message
                    "BEK: Order Confirmation for " + ocn.CustomerNumber // topic subject
                    )
                );

            return true;
        }

        private void CreateSubscriptionToTopic(Channel channel, string notificationEndpoint, CustomerTopic topic, Guid userId)
        {
            AmazonSNS.IAmazonSimpleNotificationService client =
                Amazon.AWSClientFactory.CreateAmazonSimpleNotificationServiceClient(
                    new Amazon.Runtime.BasicAWSCredentials("AKIAJ42DMK24ZMO56MYQ", "TwrfjLm5y1G4xcUvYMAgn+/+kXDuyNUZIRD7qvA0"), // TODO: Config
                    Amazon.RegionEndpoint.USEast1);

            if (channel != Channel.Email)
                throw new NotImplementedException();

            string subscriptionArn = client.Subscribe(new Amazon.SimpleNotificationService.Model.SubscribeRequest
            {
                TopicArn = topic.ProviderTopicId,
                Protocol = channel == Channel.Email ? "email" : "",
                Endpoint = notificationEndpoint
            }).SubscriptionArn;

            if (topic.Subscriptions == null) topic.Subscriptions = new List<UserTopicSubscription>();

            topic.Subscriptions.Add(new UserTopicSubscription() { NotificationEndpoint = notificationEndpoint, UserId = userId, Channel = channel, ProviderSubscriptionId = subscriptionArn });
            customerTopicRepository.Update(topic);
            unitOfWork.SaveChanges();
        }

        private CustomerTopic CreateTopic(NotificationType notificationType, string customerNumber)
        {
            AmazonSNS.IAmazonSimpleNotificationService client =
                Amazon.AWSClientFactory.CreateAmazonSimpleNotificationServiceClient(
                    new Amazon.Runtime.BasicAWSCredentials("AKIAJ42DMK24ZMO56MYQ", "TwrfjLm5y1G4xcUvYMAgn+/+kXDuyNUZIRD7qvA0"), // TODO: Config
                    Amazon.RegionEndpoint.USEast1);

            // Create topic
            string topicArn = client.CreateTopic(new AmazonSNS.Model.CreateTopicRequest
            {
                Name = customerNumber + "_" + notificationType.ToString()
            }).TopicArn;

            // Set display name to a friendly value
            client.SetTopicAttributes(new Amazon.SimpleNotificationService.Model.SetTopicAttributesRequest
            {
                TopicArn = topicArn,
                AttributeName = "DisplayName",
                AttributeValue = "Ben E Keith"
            });

            CustomerTopic topic = new CustomerTopic() { CustomerNumber = customerNumber, NotificationType = notificationType, ProviderTopicId = topicArn };
            customerTopicRepository.Create(topic);
            unitOfWork.SaveChanges();
            return topic;
        }

        public bool RemoveUserSubscription(NotificationType notificationType, Channel channel, Guid userId, string customerNumber, string notificationEndpoint)
        {
            throw new NotImplementedException();
        }

        public List<UserMessage> GetUserMessages(Guid userId)
        {
            throw new NotImplementedException();
        }

        public void PublishMessageToQueue()
        {
        }
        public void ConsumeMessageFromQueue()
        {
        }


        public void ListenForNotificationMessagesOnQueue()
        {
            //listenForQueueMessagesTask = Task.Factory.StartNew(() => );
        }

        public void Stop()
        {
            throw new NotImplementedException();
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

        public void UpdateUserMessages(List<UserMessageModel> userMessages)
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

        public void UpdateUserMessagingPreference(UserMessagingPreferenceModel userMessagingPreference)
        {
            var currentUserMessagingPreference = userMessagingPreferenceRepository.Read(u => u.Id.Equals(userMessagingPreference.Id)).FirstOrDefault();

            if (currentUserMessagingPreference == null)
                return;

            currentUserMessagingPreference.Channel = userMessagingPreference.Channel;
            currentUserMessagingPreference.CustomerNumber = userMessagingPreference.CustomerNumber;
            currentUserMessagingPreference.NotificationType = userMessagingPreference.NotificationType;
            currentUserMessagingPreference.UserId = userMessagingPreference.UserId;

            unitOfWork.SaveChanges();
        }


    }
}
