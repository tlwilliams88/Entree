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
        private bool doListenForMessagesInTask = true;

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

        public bool SendMessage(Core.Models.Messaging.Queue.BaseNotification notification)
        {
            CustomerTopic topic = customerTopicRepository.ReadTopicForCustomerAndType(notification.CustomerNumber, NotificationType.OrderConfirmation);
            if (topic == null)
            {
                topic = CreateTopic(notification.NotificationType, notification.CustomerNumber);
                eventLogRepository.WriteInformationLog("no topic found for customer: " + notification.CustomerNumber + ", event type: " + notification.NotificationType.ToString());
                return true;
            }

            AmazonSNS.IAmazonSimpleNotificationService client =
                Amazon.AWSClientFactory.CreateAmazonSimpleNotificationServiceClient(
                    new Amazon.Runtime.BasicAWSCredentials("AKIAJ42DMK24ZMO56MYQ", "TwrfjLm5y1G4xcUvYMAgn+/+kXDuyNUZIRD7qvA0"), // TODO: Config
                    Amazon.RegionEndpoint.USEast1);
            
            string msg = GetMessageForNotification(notification);
            string subject = GetMessageSubjectForNotification(notification);
            client.Publish(
                new AmazonSNS.Model.PublishRequest(topic.ProviderTopicId, // topic arn
                    msg, // message
                    subject // subject
                    )
                );

            return true;
        }

        private string GetMessageSubjectForNotification(BaseNotification notification)
        {
            if (notification.NotificationType == NotificationType.OrderConfirmation)
            {
                OrderConfirmationNotification ocn = (OrderConfirmationNotification)notification;
                return "BEK: Order Confirmation for " + notification.CustomerNumber + " (" + ocn.OrderChange.OrderName + ")";
            }
            return "unknown message type";
        }

        private string GetMessageForNotification(BaseNotification notification)
        {
            if (notification.NotificationType == NotificationType.OrderConfirmation)
            {
                OrderConfirmationNotification ocn = (OrderConfirmationNotification)notification;
                string statusString = String.IsNullOrEmpty(ocn.OrderChange.OriginalStatus)
                    ? "Order confirmed with status: " + ocn.OrderChange.CurrentStatus
                    : "Order updated from status: " + ocn.OrderChange.OriginalStatus + " to " + ocn.OrderChange.CurrentStatus;

                string orderLineChanges = string.Empty;
                foreach (var line in ocn.OrderChange.ItemChanges)
                    orderLineChanges += orderLineChanges + "Item: " + line.ItemNumber +
                        (String.IsNullOrEmpty(line.SubstitutedItemNumber) ? string.Empty : ("replace by: " + line.SubstitutedItemNumber)) +
                        "  Status: " + line.NewStatus + (line.NewStatus == line.OriginalStatus || string.IsNullOrEmpty(line.OriginalStatus) 
                                                            ? string.Empty : (" change from: " + line.OriginalStatus)) + System.Environment.NewLine;

                string originalOrderInfo = "Original Order Information:" + System.Environment.NewLine;
                foreach (var line in ocn.OrderChange.Items)
                    originalOrderInfo += line.ItemNumber + ", " + line.ItemDescription + " (" + line.QuantityOrdered + ")" + System.Environment.NewLine;
                return statusString + System.Environment.NewLine + orderLineChanges + System.Environment.NewLine + originalOrderInfo;
            }
            return "unknown message type";
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

        public string ConsumeMessageFromQueue()
        {
            return this.genericQueueRepository.ConsumeFromQueue(Configuration.RabbitMQNotificationServer, Configuration.RabbitMQNotificationUserNameConsumer,
                Configuration.RabbitMQNotificationUserPasswordConsumer, Configuration.RabbitMQVHostNotification, Configuration.RabbitMQQueueNotification);
        }


        public void ListenForNotificationMessagesOnQueue()
        {
           listenForQueueMessagesTask = Task.Factory.StartNew(() => ListenToQueueInTask());
        }

        protected void ListenToQueueInTask()
        {
            while (doListenForMessagesInTask)
            {
                try
                {
                    System.Threading.Thread.Sleep(2000);
                    string msg = ConsumeMessageFromQueue();
                    if (msg != null)
                    {
                        BaseNotification notification = NotificationExtension.Deserialize(msg);
                        if (notification.NotificationType == NotificationType.OrderConfirmation)
                        {
                            OrderConfirmationNotification orderConfNotify = (OrderConfirmationNotification)notification;
                            SendMessage(orderConfNotify);
                        }
                    }
                }
                catch (Exception ex)
                {
                    eventLogRepository.WriteErrorLog("Exception while listening for notifications", ex);
                }
            }
        }

        public void Stop()
        {
            if (listenForQueueMessagesTask != null && doListenForMessagesInTask == true)
            {
                doListenForMessagesInTask = false;
                listenForQueueMessagesTask.Wait();
            }
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


        public int GetUnreadMessagesCount(UserProfile user)
        {
            int count = userMessageRepository.GetUnreadMessagesCount(user);

            return count;
        }

        public void UpdateMessagingPreferences(ProfileMessagingPreferenceModel updatedMessagingPreferenceModel, UserProfile user)
        {
            var currentUserMessagingPreferences = userMessagingPreferenceRepository.Read(u => (u.UserId.Equals(user.UserId) && u.CustomerNumber == updatedMessagingPreferenceModel.CustomerNumber));

            //first delete existing messaging preferences
            DeleteMessagingPreferencesByCustomer(user.UserId, updatedMessagingPreferenceModel.CustomerNumber);

            //then create messaging preferences
            CreateMessagingPreferencesByCustomer(user.UserId, updatedMessagingPreferenceModel);
        }

        //this also works for user default since customer = null for user default
        public void DeleteMessagingPreferencesByCustomer(Guid userId, string customerNumber)
        {
            var messagingPreferences = userMessagingPreferenceRepository.Read(i => (i.UserId.Equals(userId) && i.CustomerNumber == customerNumber));

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
                        Channel = channel,
                        CustomerNumber = messagingPrefModel.CustomerNumber,
                        NotificationType = currentPreference.NotificationType,
                        UserId = userId
                    };
                    userMessagingPreferenceRepository.Create(newPreference);
                }
            }
            unitOfWork.SaveChanges();
        }


    }
}
