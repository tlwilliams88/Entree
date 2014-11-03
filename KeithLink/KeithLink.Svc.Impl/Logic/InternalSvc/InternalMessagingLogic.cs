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

        InternalMessagingLogic(IUnitOfWork unitOfWOrk, ICustomerTopicRepository customerTopicRepository)
        {
            this.unitOfWork = unitOfWork;
            this.customerTopicRepository = customerTopicRepository;
        }

        public bool AddUserSubscription(Core.Enumerations.Messaging.MessageType messageType, Core.Enumerations.Messaging.NotificationType notificationType, Guid userId, string customerNumber, string notificationEndpoint)
        {
            // find topics for customer; if not existing, then create
            CustomerTopic topic = customerTopicRepository.ReadTopicForCustomerAndType(customerNumber, messageType);
            if (topic == null)
            {
                topic = CreateTopic(messageType, customerNumber, topic);
            }

            // look for existing subscription
            if (!topic.Subscriptions.Any(s => s.NotificationType == notificationType && s.NotificationEndpoint == notificationEndpoint && s.UserId == userId))
            {
                // add the subscription
                CreateSubscriptionToTopic(notificationType, notificationEndpoint, topic, userId);
            }
            return true;
        }

        private void CreateSubscriptionToTopic(Core.Enumerations.Messaging.NotificationType notificationType, string notificationEndpoint, CustomerTopic topic, Guid userId)
        {
            AmazonSNS.IAmazonSimpleNotificationService client =
                Amazon.AWSClientFactory.CreateAmazonSimpleNotificationServiceClient(
                    new Amazon.Runtime.BasicAWSCredentials("AKIAJ42DMK24ZMO56MYQ", "TwrfjLm5y1G4xcUvYMAgn+/+kXDuyNUZIRD7qvA0"), // TODO: Config
                    Amazon.RegionEndpoint.USEast1);

            if (notificationType != NotificationType.Email)
                throw new NotImplementedException();

            client.Subscribe(new Amazon.SimpleNotificationService.Model.SubscribeRequest
            {
                TopicArn = topic.ProviderTopicId,
                Protocol = notificationType == NotificationType.Email ? "email" : "",
                Endpoint = notificationEndpoint
            });

            topic.Subscriptions.Add(new UserTopicSubscription() { NotificationEndpoint = notificationEndpoint, UserId = userId, NotificationType = notificationType });
            customerTopicRepository.Update(topic);
        }

        private CustomerTopic CreateTopic(Core.Enumerations.Messaging.MessageType messageType, string customerNumber, CustomerTopic topic)
        {
            AmazonSNS.IAmazonSimpleNotificationService client =
                Amazon.AWSClientFactory.CreateAmazonSimpleNotificationServiceClient(
                    new Amazon.Runtime.BasicAWSCredentials("AKIAJ42DMK24ZMO56MYQ", "TwrfjLm5y1G4xcUvYMAgn+/+kXDuyNUZIRD7qvA0"), // TODO: Config
                    Amazon.RegionEndpoint.USEast1);

            // Create topic
            string topicArn = client.CreateTopic(new AmazonSNS.Model.CreateTopicRequest
            {
                Name = customerNumber + "_" + messageType.ToString()
            }).TopicArn;

            // Set display name to a friendly value
            client.SetTopicAttributes(new Amazon.SimpleNotificationService.Model.SetTopicAttributesRequest
            {
                TopicArn = topicArn,
                AttributeName = "DisplayName",
                AttributeValue = "Ben E Keith"
            });

            topic = new CustomerTopic() { CustomerNumber = customerNumber, MessageType = messageType, ProviderTopicId = topicArn };
            customerTopicRepository.Create(topic);
            return topic;
        }

        public bool RemoveUserSubscription(Core.Enumerations.Messaging.MessageType messageType, Core.Enumerations.Messaging.NotificationType notificationType, Guid userId, string customerNumber, string notificationEndpoint)
        {
            throw new NotImplementedException();
        }

        public List<UserMessage> GetUserMessages(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
