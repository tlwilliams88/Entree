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
using AmazonSNS = Amazon.SimpleNotificationService;

namespace KeithLink.Svc.Impl.Logic.InternalSvc
{
	public class InternalMessagingLogic : IInternalMessagingLogic
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly ICustomerTopicRepository customerTopicRepository;

        public InternalMessagingLogic(IUnitOfWork unitOfWork, ICustomerTopicRepository customerTopicRepository)
        {
            this.unitOfWork = unitOfWork;
            this.customerTopicRepository = customerTopicRepository;
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
            if (topic.Subscriptions == null || !topic.Subscriptions.Any(s => s.NotificationType == channel && s.NotificationEndpoint == notificationEndpoint && s.UserId == userId))
            {
                // add the subscription
                CreateSubscriptionToTopic(channel, notificationEndpoint, topic, userId);
            }
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

            topic.Subscriptions.Add(new UserTopicSubscription() { NotificationEndpoint = notificationEndpoint, UserId = userId, NotificationType = channel, ProviderSubscriptionId = subscriptionArn });
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
    }
}
