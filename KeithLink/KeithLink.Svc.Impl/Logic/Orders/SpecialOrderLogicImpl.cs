using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Common.Impl.Email;

using KeithLink.Svc.Core;

using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;

using EF = KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Models.SpecialOrders;

using KeithLink.Svc.Impl.Helpers;
using KeithLink.Svc.Impl.Repository.EF.Operational;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using KeithLink.Svc.Impl.Tasks;
using KeithLink.Svc.Core.Exceptions.Queue;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Models.Messaging.Provider;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Impl.Logic.Messaging;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Messaging.EF;
using KeithLink.Svc.Core.Models.Configuration;
using KeithLink.Common.Core.Extensions;

namespace KeithLink.Svc.Impl.Logic.Orders {
    public class SpecialOrderLogicImpl : ISpecialOrderLogic
    {
        #region attributes
        private const string MESSAGE_TEMPLATE_SPECIALORDERCONFIRMATION = "SpecialOrderConfirmation";
        private const int THREAD_SLEEP_DURATION = 2000;

        private readonly IOrderHistoryHeaderRepsitory _headerRepo;
        private readonly IOrderHistoryDetailRepository _detailRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventLogRepository _log;
        private readonly IGenericQueueRepository _queue;
        private readonly IGenericSubscriptionQueueRepository _genericSubscriptionQueue;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMessageTemplateLogic _messageTemplateLogic;
        private readonly IDsrLogic _dsrLogic;
        private readonly IUserProfileLogic _userProfileLogic;
        private readonly IUserMessagingPreferenceRepository _userMessagingPreferenceRepository;
        private readonly IUserPushNotificationDeviceRepository _userPushNotificationDeviceRepository;
        private readonly Func<Channel, IMessageProvider> _messageProviderFactory;

        private bool _keepListening;
        private Task _queueTask;
        #endregion

        #region ctor
        public SpecialOrderLogicImpl(IUnitOfWork unitOfWork, IEventLogRepository log, IGenericQueueRepository queue,
                                     IGenericSubscriptionQueueRepository genericSubscriptionQueue,
                                     IOrderHistoryDetailRepository detailRepo, IOrderHistoryHeaderRepsitory headerRepo,
                                     ICustomerRepository customerRepository, IMessageTemplateLogic messageTemplateLogic,
                                     IDsrLogic dsrLogic, IUserProfileLogic userProfileLogic,
                                     IUserMessagingPreferenceRepository userMessagingPreferenceRepository,
                                     IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository,
                                     Func<Channel, IMessageProvider> messageProviderFactory)
        {
            _unitOfWork = unitOfWork;
            _log = log;
            _queue = queue;
            _genericSubscriptionQueue = genericSubscriptionQueue;
            _keepListening = true;
            _headerRepo = headerRepo;
            _detailRepo = detailRepo;
            _customerRepository = customerRepository;
            _messageTemplateLogic = messageTemplateLogic;
            _dsrLogic = dsrLogic;
            _userProfileLogic = userProfileLogic;
            _userMessagingPreferenceRepository = userMessagingPreferenceRepository;
            _userPushNotificationDeviceRepository = userPushNotificationDeviceRepository;
            _messageProviderFactory = messageProviderFactory;

            // subscribe to event to receive message through subscription
            _genericSubscriptionQueue.MessageReceived += GenericSubscriptionQueue_MessageReceived;
        }
        #endregion

        #region methods
        // queue listener methods
        #region subscription
        public void SubscribeToQueue()
        {
            RabbitMQ.Client.ConnectionFactory config = new RabbitMQ.Client.ConnectionFactory();
            config.HostName = Configuration.RabbitMQConfirmationServer;
            config.UserName = Configuration.RabbitMQUserNameConsumer;
            config.Password = Configuration.RabbitMQUserPasswordConsumer;
            config.VirtualHost = Configuration.RabbitMQVHostConfirmation;

            _log.WriteInformationLog
                (string.Format("Subscribing to special order updates queue: {0}", 
                    Configuration.RabbitMQQueueSpecialOrderUpdateRequests));

            this._queueTask = Task.Factory.StartNew
                (() => _genericSubscriptionQueue.Subscribe(config, Configuration.RabbitMQQueueSpecialOrderUpdateRequests));
        }

        public void Unsubscribe()
        {
            _genericSubscriptionQueue.Unsubscribe();
        }

        private void GenericSubscriptionQueue_MessageReceived
            (RabbitMQ.Client.IBasicConsumer sender, RabbitMQ.Client.Events.BasicDeliverEventArgs args)
        {
            RabbitMQ.Client.Events.EventingBasicConsumer consumer = (RabbitMQ.Client.Events.EventingBasicConsumer)sender;

            try
            {
                // don't reprocess items that have been processed
                if (_genericSubscriptionQueue.GetLastProcessedUndelivered() != args.DeliveryTag)
                {
                    string rawOrder = Encoding.ASCII.GetString(args.Body);

                    ProcessSpecialOrderItemUpdate(rawOrder);
                }

                _genericSubscriptionQueue.Ack(consumer, args.DeliveryTag);
            }
            catch (QueueDataError<string> serializationEx)
            {
                _log.WriteErrorLog("Serializing problem with special order update.", serializationEx);
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("Unhandled error processing special order update.", ex);
            }
        }
        #endregion

        private string CreateTestSample() {
            SpecialOrderResponseModel specialorder = new SpecialOrderResponseModel();
            specialorder.Header = new ResponseHeader();
            specialorder.Item = new ResponseItem();
            return JsonConvert.SerializeObject(specialorder);
        }

        private void ProcessSpecialOrderItemUpdate(string rawOrder) {
            try
            {
                SpecialOrderResponseModel specialorder = JsonConvert.DeserializeObject<SpecialOrderResponseModel>(rawOrder);

                _log.WriteInformationLog(string.Format("Consuming specialorder update from queue for message ({0}) with status {1}", specialorder.MessageId, specialorder.Item.ItemStatusId));

                // retry trying to find the record up to 3 times. some times an exception is thrown for a bad command definition, but then processes fine the next time
                EF.OrderHistoryDetail detail =
                    Retry.Do<EF.OrderHistoryDetail>(() =>
                    FindOrderHistoryDetailForUpdate(specialorder), TimeSpan.FromSeconds(5), 3);

                if (detail != null)
                { // only process if we match the order specified on this system
                    ProcessOrderHistoryDetailByUpdateStatus(specialorder, detail);
                }
                else
                {
                    _log.WriteInformationLog(string.Format(" ({0}) Specialorder update from queue for message not an order on this system", specialorder.MessageId));
                }

                // Always clear context at the end of a transaction
                _unitOfWork.ClearContext();
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("ProcessSpecialOrderItemUpdate", ex);
            }
        }

        private EF.OrderHistoryDetail FindOrderHistoryDetailForUpdate(SpecialOrderResponseModel specialorder) {
            EF.OrderHistoryDetail detail = null;

            // try to find detail by specialorderheaderid and linenumber
            if (!String.IsNullOrEmpty(specialorder.Header.RequestHeaderId) && !String.IsNullOrEmpty(specialorder.Item.LineNumber)) {
                detail = _detailRepo.Read(d => d.BranchId.Equals(specialorder.Header.BranchId, StringComparison.InvariantCultureIgnoreCase) &&
                                               d.SpecialOrderHeaderId == specialorder.Header.RequestHeaderId &&
                                               d.SpecialOrderLineNumber == specialorder.Item.LineNumber).FirstOrDefault();
            }

            return detail;
        }

        private void ProcessOrderHistoryDetailByUpdateStatus(SpecialOrderResponseModel specialorder, EF.OrderHistoryDetail detail) {
            switch(specialorder.Item.ItemStatusId) {
                case Constants.SPECIALORDERITEM_DEL_STATUS_CODE:
                case Constants.SPECIALORDERITEM_PUR_STATUS_CODE: {
                        ProcessOrderHistoryDetailOnDeletedOrPurgedStatusUpdate(detail);
                        break;
                    }
                case Constants.SPECIALORDERITEM_SHP_STATUS_CODE: {
                        ProcessOrderHistoryDetailOnItemShipped(specialorder, detail);
                        break;
                    }
                    //default: // not exposing any status from KSOS right now
                    //    {
                    //        ProcessOrderHistoryDetailOnStatusUpdate(specialorder, detail);
                    //        break;
                    //    }
            }
        }

        private void ProcessOrderHistoryDetailOnDeletedOrPurgedStatusUpdate(EF.OrderHistoryDetail detail) {
            EF.OrderHistoryHeader header = detail.OrderHistoryHeader;
            if(header != null) {
                if(header.OrderDetails.Count > 1) {
                    _detailRepo.Delete(detail);
                } else {
                    _headerRepo.Delete(header);
                }
                _unitOfWork.SaveChanges();
            } else {
                _detailRepo.Delete(detail);
                _unitOfWork.SaveChanges();
            }
        }

        //private void ProcessOrderHistoryDetailOnStatusUpdate(SpecialOrderResponseModel specialorder, EF.OrderHistoryDetail detail) {
        //    //_log.WriteInformationLog(string.Format(" ({0})  InternalSpecialOrderLogic.ProcessOrderHistoryDetailOnStatusUpdate", specialorder.MessageId));
        //    switch(specialorder.Item.ItemStatusId) {
        //        case Constants.SPECIALORDERITEM_NEW_STATUS_CODE:
        //            detail.ItemStatus = Constants.SPECIALORDERITEM_NEW_STATUS_TRANSLATED_CODE;
        //            break;
        //        case Constants.SPECIALORDERITEM_ERR_STATUS_CODE:
        //            detail.ItemStatus = Constants.SPECIALORDERITEM_ERR_STATUS_TRANSLATED_CODE;
        //            break;
        //        case Constants.SPECIALORDERITEM_2MF_STATUS_CODE:
        //            detail.ItemStatus = Constants.SPECIALORDERITEM_2MF_STATUS_TRANSLATED_CODE;
        //            break;
        //        case Constants.SPECIALORDERITEM_REQ_STATUS_CODE:
        //            detail.ItemStatus = Constants.SPECIALORDERITEM_REQ_STATUS_TRANSLATED_CODE;
        //            break;
        //        case Constants.SPECIALORDERITEM_ACC_STATUS_CODE:
        //            detail.ItemStatus = Constants.SPECIALORDERITEM_ACC_STATUS_TRANSLATED_CODE;
        //            break;
        //        case Constants.SPECIALORDERITEM_APP_STATUS_CODE:
        //            detail.ItemStatus = Constants.SPECIALORDERITEM_APP_STATUS_TRANSLATED_CODE;
        //            break;
        //        case Constants.SPECIALORDERITEM_DEL_STATUS_CODE:
        //            detail.ItemStatus = Constants.SPECIALORDERITEM_DEL_STATUS_TRANSLATED_CODE;
        //            break;
        //        case Constants.SPECIALORDERITEM_HLD_STATUS_CODE:
        //            detail.ItemStatus = Constants.SPECIALORDERITEM_HLD_STATUS_TRANSLATED_CODE;
        //            break;
        //        case Constants.SPECIALORDERITEM_RCV_STATUS_CODE:
        //            detail.ItemStatus = Constants.SPECIALORDERITEM_RCV_STATUS_TRANSLATED_CODE;
        //            break;
        //        case Constants.SPECIALORDERITEM_R_H_STATUS_CODE:
        //            detail.ItemStatus = Constants.SPECIALORDERITEM_R_H_STATUS_TRANSLATED_CODE;
        //            break;
        //        case Constants.SPECIALORDERITEM_ATT_STATUS_CODE:
        //            detail.ItemStatus = Constants.SPECIALORDERITEM_ATT_STATUS_TRANSLATED_CODE;
        //            break;
        //        case Constants.SPECIALORDERITEM_PTL_STATUS_CODE:
        //            detail.ItemStatus = Constants.SPECIALORDERITEM_PTL_STATUS_TRANSLATED_CODE;
        //            break;
        //        case Constants.SPECIALORDERITEM_SHP_STATUS_CODE:
        //            detail.ItemStatus = Constants.SPECIALORDERITEM_SHP_STATUS_TRANSLATED_CODE;
        //            break;
        //        case Constants.SPECIALORDERITEM_PUR_STATUS_CODE:
        //            detail.ItemStatus = Constants.SPECIALORDERITEM_PUR_STATUS_TRANSLATED_CODE;
        //            break;
        //    }
        //    _detailRepo.Update(detail);
        //    _unitOfWork.SaveChanges();
        //}

        private void ProcessOrderHistoryDetailOnItemShipped
            (SpecialOrderResponseModel specialorder, EF.OrderHistoryDetail detail)
        {
            _log.WriteInformationLog
                (string.Format
                    (" ({0})  InternalSpecialOrderLogic.ProcessOrderHistoryDetailOnStatusUpdate", specialorder.MessageId));

            UpdateOrder(specialorder, detail);

            SendNotification(specialorder);
        }

        private void SendNotification(SpecialOrderResponseModel specialorder)
        {
            Customer customer =
                _customerRepository.GetCustomerByCustomerNumber(specialorder.Header.CustomerNumber,
                                                                specialorder.Header.BranchId);

            if (customer == null)
            {
                StringBuilder warningMessage = new StringBuilder();
                warningMessage.AppendFormat("Could not find customer({0}-{1}) to send SpecialOrder Confirmation notification.",
                                            specialorder.Header.BranchId,
                                            specialorder.Header.CustomerNumber);
                warningMessage.AppendLine();
                warningMessage.AppendLine();
                warningMessage.AppendLine("SpecialOrder:");
                warningMessage.AppendLine(JsonConvert.SerializeObject(specialorder));

                _log.WriteWarningLog(warningMessage.ToString());
            }
            else
            {
                List<Recipient> recipients = LoadRecipients(NotificationType.OrderConfirmation, customer);
                Message message = GetEmailMessageForNotification(specialorder, customer);

                // send messages to providers...
                if (recipients != null && recipients.Count > 0)
                {
                    SendMessage(recipients, message);
                }
            }
        }

        #region polling
        public void ListenForQueueMessages()
        {
            _queueTask = Task.Factory.StartNew(() => ListenForQueueMessagesInTask(),
                CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                new LimitedConcurrencyLevelTaskScheduler(Constants.LIMITEDCONCURRENCYTASK_SPECIALORDERUPDATES));
        }

        private void ListenForQueueMessagesInTask()
        {
            while (_keepListening)
            {
                System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);

                try
                {
                    string rawOrder = ReadOrderFromQueue();

                    // for testing; create a sample specialorderresponse
                    //rawOrder = CreateTestSample();

                    while (_keepListening && !string.IsNullOrEmpty(rawOrder))
                    {
                        ProcessSpecialOrderItemUpdate(rawOrder);
                        // to make sure we do not pull an order off the queue without processing it
                        // check to make sure we can still process before pulling off the queue
                        if (_keepListening)
                        {
                            rawOrder = ReadOrderFromQueue();
                        }
                        else
                        {
                            rawOrder = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionEmail.Send(ex, subject: "Exception processing SpecialOrderLogic.ListenForQueueMessagesInTask");

                    _log.WriteErrorLog("Error in SpecialOrderLogic.ListenForQueueMessagesInTask ", ex);
                }
            }
        }

        private string ReadOrderFromQueue()
        {
            return _queue.ConsumeFromQueue(Configuration.RabbitMQConfirmationServer, Configuration.RabbitMQUserNameConsumer, Configuration.RabbitMQUserPasswordConsumer,
                                           Configuration.RabbitMQVHostConfirmation, Configuration.RabbitMQQueueSpecialOrderUpdateRequests);
        }

        public void StopListening()
        {
            _keepListening = false;

            if (_queueTask != null && _queueTask.Status == TaskStatus.Running)
            {
                _queueTask.Wait();
            }
            if (_queueTask != null)
            {
                //_log.WriteWarningLog(string.Format("SpecialOrderLogicImpl._queueTask.status = {0:G}", _queueTask.Status));
            }
        }
        #endregion

        private Message GetEmailMessageForNotification(SpecialOrderResponseModel specialorder, Customer customer)
        {
            Message message = new Message();
            MessageTemplateModel template = _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_SPECIALORDERCONFIRMATION);
            message.MessageSubject = template.Subject.Inject(new
            {
                OrderStatus = "SpecialOrder Confirmation",
                CustomerNumber = customer.CustomerNumber,
                CustomerName = customer.CustomerName
            });
            StringBuilder header = _messageTemplateLogic.BuildHeader
                ("Your special order will arrive at the Ben E. Keith warehouse on the date below. " +
                 "Your special order items will ship on your next order following the arrival date in our warehouse.", customer);
            message.MessageBody += template.Body.Inject(new
            {
                NotifHeader = header.ToString(),
                InvoiceNumber = specialorder.Item.InvoiceNumber,
                ShipDate = specialorder.Item.EstimatedArrival,
                Total = double.Parse(specialorder.Item.Price).ToString("f2"),
                ItemNumber = specialorder.Item.ItemNumber.ToString(),
                GTIN = specialorder.Item.GtinUpc,
                Source = specialorder.Header.ManufacturerName,
                Description = specialorder.Item.Description,
                Quantity = specialorder.Item.QuantityRequested
            });
            message.BodyIsHtml = template.IsBodyHtml;
            message.CustomerNumber = customer.CustomerNumber;
            message.CustomerName = customer.CustomerName;
            message.BranchId = customer.CustomerBranch;
            message.NotificationType = NotificationType.OrderConfirmation;

            return message;
        }

        private void UpdateOrder(SpecialOrderResponseModel specialorder, EF.OrderHistoryDetail detail)
        {
            EF.OrderHistoryHeader header = _headerRepo.ReadById(detail.OrderHistoryHeader.Id);
            header.DeliveryDate = specialorder.Item.EstimatedArrival;
            _headerRepo.Update(header);

            detail.ItemStatus = Constants.CONFIRMATION_DETAIL_FILLED_CODE;
            _detailRepo.Update(detail);

            _unitOfWork.SaveChanges();
        }

        private UserProfileReturn GetUsers(Customer customer, bool dsrDSMOnly)
        {
            UserProfileReturn users = new UserProfileReturn();

            users = GetSpecifiedUsers(customer, dsrDSMOnly, users);

            // make sure that we return good data in the users list
            if (users == null || users.UserProfiles == null)
            {
                users = new UserProfileReturn();
            }
            else
            {
                users.UserProfiles.RemoveAll(u => u == null || u.UserId == null);
            }

            return users;
        }

        private UserProfileReturn GetSpecifiedUsers(Customer customer, bool dsrDSMOnly, UserProfileReturn users)
        {
            if (dsrDSMOnly)
            {
                users = GetOnlyDSRsDSMs(customer, users);
            }
            else
            {
                users = GetAllUsers(customer);
            }

            return users;
        }

        private UserProfileReturn GetAllUsers(Customer customer)
        {
            UserProfileReturn users = _userProfileLogic.GetUsers(new UserFilterModel() { CustomerId = customer.CustomerId });
            users.UserProfiles.AddRange(_userProfileLogic.GetInternalUsersWithAccessToCustomer(customer.CustomerNumber, customer.CustomerBranch)); //Retreive any internal users that have access to this customer
            return users;
        }

        private UserProfileReturn GetOnlyDSRsDSMs(Customer customer, UserProfileReturn users)
        {
            //Only load DSRs and DSMs for the customer

            //Load DSRs
            users = GetDSRs(customer, users);

            //Load DSM
            List<UserProfile> customerUsers = GetDSMs(customer);

            GatherDSRsDSMs(customer, users, customerUsers);

            return users;
        }

        private void GatherDSRsDSMs(Customer customer, UserProfileReturn users, List<UserProfile> customerUsers)
        {
            if (customerUsers == null || customerUsers.Count == 0)
            {
                // no internal users found with access to the customer
                _log.WriteWarningLog(string.Format("Could not find any internal users with access to {0}-{1}", customer.CustomerBranch, customer.CustomerNumber));
            }
            else
            {
                UserProfile dsm = customerUsers.Where(x => x.IsDSM &&
                                                            !string.IsNullOrEmpty(x.DSMNumber) &&
                                                            x.DSMNumber == customer.DsmNumber)
                                               .FirstOrDefault();

                if (dsm != null)
                {
                    users.UserProfiles.Add(dsm);
                }
            }
        }

        private List<UserProfile> GetDSMs(Customer customer)
        {
            return _userProfileLogic.GetInternalUsersWithAccessToCustomer(customer.CustomerNumber, customer.CustomerBranch);
        }

        private UserProfileReturn GetDSRs(Customer customer, UserProfileReturn users)
        {
            var dsr = _dsrLogic.GetDsr(customer.CustomerBranch, customer.DsrNumber);
            if (dsr != null && dsr.DsrNumber != "000" && !string.IsNullOrEmpty(dsr.EmailAddress))
            {
                users = (_userProfileLogic.GetUserProfile(dsr.EmailAddress, createBekProfile: false));
            }

            return users;
        }

        protected List<Recipient> LoadRecipients(NotificationType notificationType, Customer customer, bool dsrDSMOnly = false)
        {
            if (customer == null) { return new List<Recipient>(); }

            UserProfileReturn users = GetUsers(customer, dsrDSMOnly);

            if (users.UserProfiles.Count == 0) { return new List<Recipient>(); }

            List<UserMessagingPreference> userDefaultMessagingPreferences, customerMessagingPreferences, ump;
            GatherUserMessagingPreferences(notificationType, 
                                           customer, 
                                           users, 
                                           out userDefaultMessagingPreferences, 
                                           out customerMessagingPreferences, 
                                           out ump);

            string prefs = string.Empty;

            foreach (var u in ump)
                prefs += u.Channel + u.UserId.ToString("B") + u.NotificationType + "; ";

            _log.WriteInformationLog
                (String.Format(
                    "notification prefs: {0}, profiles count: {1}, profiles: {2}, userDefaultMessagingPreferences: {3}, customerMessagingPreferences: {4}",
                    prefs,
                    users.UserProfiles.Count,
                    JsonConvert.SerializeObject(users.UserProfiles.Select(p => new
                    {
                        UserId = p.UserId,
                        EmailAddress = p.EmailAddress
                    }).ToList()),
                    userDefaultMessagingPreferences,
                    customerMessagingPreferences));

            List<Recipient> recipients = new List<Recipient>();

            GetRecipientsFromUsers(customer, users, userDefaultMessagingPreferences, customerMessagingPreferences, recipients);

            Dictionary<string, Recipient> dict = new Dictionary<string, Recipient>();
            foreach (Recipient rec in recipients)
            {
                string dupkey = rec.UserId + "_" + rec.CustomerNumber + "_" + rec.Channel + "_" + rec.ProviderEndpoint;
                if (dict.Keys.Contains(dupkey, StringComparer.CurrentCultureIgnoreCase) == false)
                {
                    dict.Add(dupkey, rec);
                }
            }

            return dict.Values.ToList();
        }

        private void GatherUserMessagingPreferences(NotificationType notificationType, Customer customer, UserProfileReturn users, out List<UserMessagingPreference> userDefaultMessagingPreferences, out List<UserMessagingPreference> customerMessagingPreferences, out List<UserMessagingPreference> ump)
        {
            GetUserMessagingPreferences
    (notificationType,
     customer,
     users,
     out userDefaultMessagingPreferences,
     out customerMessagingPreferences,
     out ump);

            ump.AddRange(userDefaultMessagingPreferences);
            ump.AddRange(customerMessagingPreferences);
        }

        private void GetRecipientsFromUsers(Customer customer, UserProfileReturn users, List<UserMessagingPreference> userDefaultMessagingPreferences, List<UserMessagingPreference> customerMessagingPreferences, List<Recipient> recipients)
        {
            foreach (UserProfile userProfile in users.UserProfiles)
            {
                if (userDefaultMessagingPreferences != null)
                {
                    // first, check for customer specific prefs
                    List<UserMessagingPreference> prefsToUse = customerMessagingPreferences.Where(
                        p => p.UserId == userProfile.UserId).ToList(); // check for customer specific prefs first
                    if (prefsToUse == null || prefsToUse.Count() == 0) // then check for defaults
                        prefsToUse = userDefaultMessagingPreferences.Where(p => p.UserId == userProfile.UserId).ToList();

                    AddSpecificRecipients(customer, recipients, userProfile, prefsToUse);
                }
            }
        }

        private void AddSpecificRecipients(Customer customer, List<Recipient> recipients, UserProfile userProfile, List<UserMessagingPreference> prefsToUse)
        {
            foreach (var pref in prefsToUse)
            {
                if (pref.Channel == Channel.Email)
                {
                    AddSpecificEmailRecipient(customer, recipients, userProfile);
                }
                else if (pref.Channel == Channel.MobilePush)
                {
                    AddSpecificMobileRecipient(customer, recipients, userProfile);
                }
                else if (pref.Channel == Channel.Web)
                {
                    AddSpecificWebRecipient(customer, recipients, userProfile);
                }
            }
        }

        private void AddSpecificWebRecipient(Customer customer, List<Recipient> recipients, UserProfile userProfile)
        {
            recipients.Add(new Recipient()
            {
                UserId = userProfile.UserId,
                CustomerNumber = customer.CustomerNumber,
                Channel = Channel.Web,
                UserEmail = userProfile.EmailAddress
            });
        }

        private void AddSpecificMobileRecipient(Customer customer, List<Recipient> recipients, UserProfile userProfile)
        {
            // lookup any and all mobile devices
            foreach (var device in _userPushNotificationDeviceRepository.ReadUserDevices(userProfile.UserId))
                if (device.Enabled != false)
                {
                    recipients.Add(new Recipient()
                    {
                        ProviderEndpoint = device.ProviderEndpointId,
                        DeviceOS = device.DeviceOS,
                        Channel = Channel.MobilePush,
                        UserId = userProfile.UserId,
                        UserEmail = userProfile.EmailAddress,
                        DeviceId = device.DeviceId,
                        CustomerNumber = customer.CustomerNumber
                    });
                }
        }

        private void AddSpecificEmailRecipient(Customer customer, List<Recipient> recipients, UserProfile userProfile)
        {
            recipients.Add(new Recipient()
            {
                ProviderEndpoint = userProfile.EmailAddress,
                Channel = Channel.Email,
                UserId = userProfile.UserId,
                UserEmail = userProfile.EmailAddress,
                CustomerNumber = customer.CustomerNumber
            });
        }

        private void GetUserMessagingPreferences(NotificationType notificationType, Customer customer, UserProfileReturn users, out List<UserMessagingPreference> userDefaultMessagingPreferences, out List<UserMessagingPreference> customerMessagingPreferences, out List<UserMessagingPreference> ump)
        {
            userDefaultMessagingPreferences = _userMessagingPreferenceRepository.ReadByUserIdsAndNotificationType(users.UserProfiles.Select(u => u.UserId), notificationType, true).ToList();
            customerMessagingPreferences = _userMessagingPreferenceRepository.ReadByCustomerAndNotificationType(customer.CustomerNumber, customer.CustomerBranch, notificationType).ToList();
            ump = new List<UserMessagingPreference>();
        }

        protected void SendMessage(List<Recipient> recipients, Message message)
        {
            // TODO: Turn this into one line of code that doesn't depend on specific channels...
            _messageProviderFactory(Channel.Email).SendMessage(recipients.Where(r => r.Channel == Channel.Email).ToList(), message);
            _messageProviderFactory(Channel.MobilePush).SendMessage(recipients.Where(r => r.Channel == Channel.MobilePush).ToList(), message);
            _messageProviderFactory(Channel.Web).SendMessage(recipients.Where(r => r.Channel == Channel.Web).ToList(), message);
        }
        #endregion
    }
}
