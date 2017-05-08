using KeithLink.Common.Core.Helpers;
using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core.Enumerations.Messaging;

using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Extensions.Messaging;

using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Profile;

using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Messaging.EF;
using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using KeithLink.Svc.Impl.Repository.EF.Operational;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Models.Configuration;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core;

namespace KeithLink.Svc.Impl.Logic.Messaging {
    public class MessagingLogicImpl : IMessagingLogic {
        #region attributes
        private readonly IUnitOfWork _uow;
        private readonly IEmailClient _emailClient;
        private readonly IUserMessageRepository _userMessageRepository;
        private readonly IUserMessagingPreferenceRepository _userMessagingPreferenceRepository;
        private readonly IUserPushNotificationDeviceRepository _userPushNotificationDeviceRepository;
        private readonly IPushNotificationMessageProvider _pushNotificationMessageProvider;
        private readonly IEventLogRepository _log;
        //private readonly IUserProfileLogic _userProfileLogic; //makes circular depend.
        private readonly ICustomerRepository _custRepo;
        private readonly IUserProfileRepository _userRepo;
        private readonly IMessageTemplateLogic _messageTemplateLogic;

        private const string MESSAGE_TEMPLATE_FORWARDUSERMESSAGE = "ForwardUserMessage";
        #endregion

        #region ctor
        public MessagingLogicImpl(IUnitOfWork unitOfWork, IUserMessageRepository userMessageRepository, IUserMessagingPreferenceRepository userMessagingPreferenceRepository,
                                  IEventLogRepository eventLogRepository, IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository, 
                                  IPushNotificationMessageProvider pushNotificationMessageProvider, ICustomerRepository custRepo,
                                  IUserProfileRepository userProfileRepository, IEmailClient emailClient,
                                  IMessageTemplateLogic messageTemplateLogic) {
            _log = eventLogRepository;
            _pushNotificationMessageProvider = pushNotificationMessageProvider;
            _uow = unitOfWork;
            _emailClient = emailClient;
            _userMessageRepository = userMessageRepository;
            _userMessagingPreferenceRepository = userMessagingPreferenceRepository;
            //_userProfileLogic = userProfileLogic;
            _custRepo = custRepo;
            _userRepo = userProfileRepository;
            _userPushNotificationDeviceRepository = userPushNotificationDeviceRepository;
            _messageTemplateLogic = messageTemplateLogic;
        }
        #endregion

        #region methods
        public void CreateMailMessage(MailMessageModel mailMessage) {
            Dictionary<Guid, UserMessage> messages = new Dictionary<Guid, UserMessage>();

            IfCustomersRelayMessageToEachCustomer(mailMessage, messages);

            IfUsersRelayMessageToEachUser(mailMessage, messages);

            //Add messages to all users in the case of system alert
            if (mailMessage.IsAlert)
            {
                ExtractLinkIfPresent(mailMessage);

                RelayAlert(mailMessage, messages);
            }

            //Create all of the message records
            foreach (var message in messages)
                _userMessageRepository.Create(message.Value);

            _uow.SaveChanges();
        }

        private void RelayAlert(MailMessageModel mailMessage, Dictionary<Guid, UserMessage> messages)
        {
            if ((mailMessage.BranchesToAlert != null) && (mailMessage.BranchesToAlert.Length > 0))
            {
                RelayAlertToUsersInBranches(mailMessage, messages);
            }
            else
            {
                RelayAlertToAllUsers(mailMessage, messages);
            }
        }

        private static void ExtractLinkIfPresent(MailMessageModel mailMessage)
        {
            if (mailMessage.Message.Link != null)
            {
                mailMessage.Message.Body += string.Format("{0}=\"{1}\"",
                                                          Constants.USERMESSAGES_LINKTOKEN,
                                                          mailMessage.Message.Link);
            }
        }

        private void RelayAlertToAllUsers(MailMessageModel mailMessage, Dictionary<Guid, UserMessage> messages)
        {
            List<UserProfile> users = _userRepo.GetExternalUsers();
            users.AddRange(_userRepo.GetInternalUsers());
            foreach (var user in users)
                if (!messages.ContainsKey(user.UserId))
                    messages.Add(user.UserId, new UserMessage()
                    {
                        Body = mailMessage.Message.Body,
                        Subject = mailMessage.Message.Subject,
                        Label = mailMessage.Message.Label,
                        Mandatory = mailMessage.Message.Mandatory,
                        UserId = user.UserId,
                        NotificationType = mailMessage.Message.NotificationType
                    });
        }

        private void RelayAlertToUsersInBranches(MailMessageModel mailMessage, Dictionary<Guid, UserMessage> messages)
        {
            List<UserProfile> users = new List<UserProfile>();
            GetBranchesToAlert(mailMessage, users);
            foreach (var user in users)
                if (!messages.ContainsKey(user.UserId))
                    messages.Add(user.UserId, new UserMessage()
                    {
                        Body = mailMessage.Message.Body,
                        Subject = mailMessage.Message.Subject,
                        Label = mailMessage.Message.Label,
                        Mandatory = mailMessage.Message.Mandatory,
                        UserId = user.UserId,
                        NotificationType = mailMessage.Message.NotificationType
                    });
        }

        private void GetBranchesToAlert(MailMessageModel mailMessage, List<UserProfile> users)
        {
            List<string> branches = Configuration.GetCommaSeparatedValues(mailMessage.BranchesToAlert);
            foreach (string branch in branches)
            {
                List<Customer> customers = _custRepo.GetCustomersForBranch(branch);
                Parallel.ForEach(customers,
                                 new ParallelOptions { MaxDegreeOfParallelism = 4 },
                                 customer =>
                                 {
                                     var lusers = _userRepo.GetUsersForCustomerOrAccount(customer.CustomerId);
                                     if (lusers != null && lusers.Count > 0)
                                         users.AddRange(lusers);
                                 });
            }
        }

        private void IfUsersRelayMessageToEachUser(MailMessageModel mailMessage, Dictionary<Guid, UserMessage> messages)
        {
            //Add any messages to specific users
            if (mailMessage.UserIds != null && mailMessage.UserIds.Any())
            {
                foreach (var user in mailMessage.UserIds)
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
        }

        private void IfCustomersRelayMessageToEachCustomer(MailMessageModel mailMessage, Dictionary<Guid, UserMessage> messages)
        {
            //If CustomerIds are provided, send message to every user for that customer
            if (mailMessage.CustomerIds != null && mailMessage.CustomerIds.Any())
            {
                foreach (var customer in mailMessage.CustomerIds)
                {
                    //var users = _userProfileLogic.GetUsers(new UserFilterModel() { CustomerId = customer });
                    var users = _userRepo.GetUsersForCustomerOrAccount(customer);

                    foreach (var user in users)
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
        }

        public void CreateMessagingPreferencesByCustomer(Guid userId, ProfileMessagingPreferenceModel messagingPrefModel) {
            //this also works for user default since customer = null for user default
            foreach(var currentPreference in messagingPrefModel.Preferences) {
                foreach(var channel in currentPreference.SelectedChannels) {
                    var newPreference = new UserMessagingPreference() {
                        Channel = channel.Channel,
                        CustomerNumber = messagingPrefModel.CustomerNumber,
                        NotificationType = currentPreference.NotificationType,
                        UserId = userId,
                        BranchId = messagingPrefModel.BranchId == null ? null : messagingPrefModel.BranchId.ToLower()
                    };
                    _userMessagingPreferenceRepository.Create(newPreference);
                }
            }
            _uow.SaveChanges();
        }

        public long CreateUserMessage(Guid userId, UserSelectedContext catalogInfo, UserMessageModel userMessage) {
            var newUserMessage = userMessage.ToEFUserMessage();
            newUserMessage.UserId = userId;

            _userMessageRepository.CreateOrUpdate(newUserMessage);
            _uow.SaveChanges();

            return newUserMessage.Id;
        }

        public void DeleteMessagingPreferencesByCustomer(Guid userId, string customerNumber, string branchId) {
            //this also works for user default since customer = null for user default
            var messagingPreferences = _userMessagingPreferenceRepository.Read(i => (i.UserId.Equals(userId) && i.CustomerNumber.Equals(customerNumber) && i.BranchId.Equals(branchId, StringComparison.InvariantCultureIgnoreCase)));

            foreach(var pref in messagingPreferences) {
                _userMessagingPreferenceRepository.Delete(pref);
            }

            _uow.SaveChanges();
        }

        public int GetUnreadMessagesCount(Guid userId) {
            int count = _userMessageRepository.GetUnreadMessagesCount(userId);

            return count;
        }

        public void MarkAllReadByUser(UserProfile user) {
            List<UserMessage> unreadMessages = _userMessageRepository.ReadUnreadMessagesByUser(user).ToList();
            
            foreach(UserMessage u in unreadMessages) {
                u.MessageReadUtc = DateTime.UtcNow;
            }

            _uow.SaveChanges();
        }

        public void MarkAsReadUserMessages(List<UserMessageModel> userMessages) {
            foreach(var userMessage in userMessages) {
                var currentUserMessage = _userMessageRepository.Read(a => a.Id.Equals(userMessage.Id)).FirstOrDefault();
                //update message read date
                currentUserMessage.MessageReadUtc = userMessage.MessageRead.HasValue ? userMessage.MessageRead.Value.ToUniversalTime() : userMessage.MessageRead;
            }
            _uow.SaveChanges();
        }

        public List<UserMessagingPreferenceModel> ReadMessagingPreferences(Guid userId) {
            var currentUserMessagingPreferences = _userMessagingPreferenceRepository.Read(u => u.UserId.Equals(userId));

            if(currentUserMessagingPreferences == null)
                return null;

            var messagingPreferencesList = new List<UserMessagingPreferenceModel>();

            foreach(var currentMsgPref in currentUserMessagingPreferences) {
                messagingPreferencesList.Add(new UserMessagingPreferenceModel() {
                    Id = currentMsgPref.Id,
                    CustomerNumber = currentMsgPref.CustomerNumber,
                    Channel = currentMsgPref.Channel,
                    NotificationType = currentMsgPref.NotificationType,
                    UserId = currentMsgPref.UserId
                });
            }

            return messagingPreferencesList;
        }

        public PagedResults<UserMessageModel> ReadPagedUserMessages(UserProfile user, PagingModel paging) {

            var userMessages = _userMessageRepository.ReadUserMessagesPaged(user, paging.Size, paging.From).ToList();

            paging.From = null; // we do a skip and take reading from the EF so we null the from so as not to do the skip again in the getpage

            var ret = userMessages.Select(m => m.ToUserMessageModel()).AsQueryable<UserMessageModel>()
                                                                      .GetPage<UserMessageModel>(paging, "MessageCreated");

            // this was added to get the full # of messages available for the frontend infinite scroll plugin to do its thing
            ret.TotalResults = _userMessageRepository.GetMessagesCount(user.UserId);

            return ret;
        }

        public bool ForwardUserMessage(UserProfile requester, ForwardUserMessageModel forwardrequest)
        {
            UserMessage userMessage = _userMessageRepository.ReadById(forwardrequest.Id);
            MessageTemplateModel forwardTemplate =
                _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_FORWARDUSERMESSAGE);

            try
            {
                string body = forwardTemplate.Body.Inject
                    (new {
                        UserEmail = requester.EmailAddress,
                        ForwardBody = userMessage.Body
                    });

                _emailClient.SendEmail
                    (new List<string>() { forwardrequest.EmailAddress },
                     null,
                     null,
                     userMessage.Subject,
                     body,
                     true);
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("ForwardUserMessage: Error sending email", ex);
            }

            return true;
        }

        public List<UserMessageModel> ReadUserMessages(UserProfile user) {
            var userMessages = _userMessageRepository.ReadUserMessages(user).ToList();

            if(userMessages == null)
                return null;

            return userMessages.Select(b => b.ToUserMessageModel()).ToList();

        }

        public UserMessagingPreferenceModel ReadUserMessagingPreference(long userMessagingPreferenceId) {
            var currentUserMessagingPreference = _userMessagingPreferenceRepository.Read(u => u.Id.Equals(userMessagingPreferenceId)).FirstOrDefault();

            if(currentUserMessagingPreference == null)
                return null;

            return new UserMessagingPreferenceModel() {
                Id = currentUserMessagingPreference.Id,
                CustomerNumber = currentUserMessagingPreference.CustomerNumber,
                Channel = currentUserMessagingPreference.Channel,
                NotificationType = currentUserMessagingPreference.NotificationType,
                UserId = currentUserMessagingPreference.UserId
            };
        }

        public bool RegisterPushDevice(UserProfile user, PushDeviceRegistrationModel deviceRegistrationModel) {
            UserPushNotificationDevice userPushNotificationDevice = _userPushNotificationDeviceRepository.ReadUserDevice(user.UserId, deviceRegistrationModel.DeviceId, deviceRegistrationModel.DeviceOS);

            if(userPushNotificationDevice == null) {
                userPushNotificationDevice = new UserPushNotificationDevice() {
                    DeviceOS = deviceRegistrationModel.DeviceOS,
                    DeviceId = deviceRegistrationModel.DeviceId,
                    UserId = user.UserId
                };
            }

            userPushNotificationDevice.ProviderToken = deviceRegistrationModel.ProviderToken;
            userPushNotificationDevice.ProviderEndpointId = _pushNotificationMessageProvider.RegisterRecipient(userPushNotificationDevice);
            userPushNotificationDevice.Enabled = true;

            _userPushNotificationDeviceRepository.CreateOrUpdate(userPushNotificationDevice);
            _uow.SaveChanges();

            // now, to create/confirm/update the application endpoint in AWS
            return true;
        }

        public void UpdateMessagingPreferences(ProfileMessagingPreferenceModel updatedMessagingPreferenceModel, UserProfile user) {
            //first delete existing messaging preferences
            DeleteMessagingPreferencesByCustomer(user.UserId, updatedMessagingPreferenceModel.CustomerNumber, updatedMessagingPreferenceModel.BranchId);

            //then create messaging preferences
            CreateMessagingPreferencesByCustomer(user.UserId, updatedMessagingPreferenceModel);
        }
        #endregion
    }
}
