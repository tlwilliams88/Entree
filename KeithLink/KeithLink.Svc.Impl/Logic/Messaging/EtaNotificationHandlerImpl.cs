using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Extensions.Messaging;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Messaging.Queue;
using KeithLink.Svc.Core.Models.Messaging.EF;
using KeithLink.Svc.Core.Models.Messaging.Provider;
using KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Models.Configuration;
using KeithLink.Svc.Impl.Repository.EF.Operational;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Messaging
{
    public class EtaNotificationHandlerImpl : BaseNotificationHandlerImpl, INotificationHandler
    {
        #region attributes
        private readonly IEventLogRepository eventLogRepository;
        private readonly IUserProfileLogic userProfileLogic;
        private readonly IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IUserMessagingPreferenceRepository userMessagingPreferenceRepository;
        private readonly IOrderHistoryHeaderRepsitory orderHistoryRepository;
        private readonly Func<Channel, IMessageProvider> messageProviderFactory;
        private readonly IMessageTemplateLogic messageTemplateLogic;
        private readonly IUnitOfWork unitOfWork;
        #endregion

        #region ctor
        public EtaNotificationHandlerImpl(IEventLogRepository eventLogRepository, IUserProfileLogic userProfileLogic, IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository, 
                                          ICustomerRepository customerRepository, IUserMessagingPreferenceRepository userMessagingPreferenceRepository, Func<Channel, IMessageProvider> messageProviderFactory, 
                                          IOrderHistoryHeaderRepsitory orderHistoryRepository, IMessageTemplateLogic messageTemplateLogic, IUnitOfWork unitOfWork, 
                                          IDsrLogic dsrLogic)
            : base(userProfileLogic, userPushNotificationDeviceRepository, customerRepository, 
                   userMessagingPreferenceRepository, messageProviderFactory, eventLogRepository, 
                   dsrLogic)
        {
            this.eventLogRepository = eventLogRepository;
            this.userProfileLogic = userProfileLogic;
            this.userPushNotificationDeviceRepository = userPushNotificationDeviceRepository;
            this.customerRepository = customerRepository;
            this.userMessagingPreferenceRepository = userMessagingPreferenceRepository;
            this.messageProviderFactory = messageProviderFactory;
            this.orderHistoryRepository = orderHistoryRepository;
            this.messageTemplateLogic = messageTemplateLogic;
            this.unitOfWork = unitOfWork;
        }
        #endregion

        #region methods
        private string IgnoreOffsetTimeString(string dateTimeString) {
            if (string.IsNullOrEmpty(dateTimeString)) {
                return null;
            } else {
                return DateTime.Parse(dateTimeString.Substring(0, dateTimeString.LastIndexOf('-'))).ToLongDateFormatWithTime();
            }
        }

        protected Message GetEmailMessageForNotification(IEnumerable<OrderEta> orderEtas, IEnumerable<OrderHistoryHeader> orders, Svc.Core.Models.Profile.Customer customer)
        {
            // TODO: Add logic for delivered orders (actualtime) - not needed now, but maybe in the future.
            StringBuilder orderInfoDetails = new StringBuilder();
            MessageTemplateModel orderEtaLineTemplate = messageTemplateLogic.ReadForKey("OrderEtaLine");
            string timeZoneName = string.Empty;
            foreach (var o in orders)
            { 
                OrderEta eta = orderEtas.Where(ordereta => ordereta.OrderId == o.InvoiceNumber).FirstOrDefault();
                DateTime? estimatedDelivery = DateTime.Parse(eta.EstimatedTime).ToCentralTime(); // will parse into local time, then convert to central
                DateTime? scheduledDelivery = DateTime.Parse(eta.ScheduledTime).ToCentralTime();
                DateTime? actualDelivery = DateTime.Parse(eta.ActualTime).ToCentralTime(); 
                object orderLineDetails =
                    new {
                            InvoiceNumber = o.InvoiceNumber,
                            ProductCount = o.OrderDetails.Count.ToString(),
                            ShippedQuantity = o.OrderDetails.Sum(od => od.ShippedQuantity).ToString(),
                            ScheduledDeliveryDate = scheduledDelivery.Value.ToShortDateString(),
                            ScheduledDeliveryTime = scheduledDelivery.Value.ToShortTimeString(),
                            EstimatedDeliveryDate = estimatedDelivery.Value.ToShortDateString(),
                            EstimatedDeliveryTime = estimatedDelivery.Value.ToShortTimeString()
                        };
                orderInfoDetails.Append(orderEtaLineTemplate.Body.Inject(orderLineDetails));

                if (timeZoneName == string.Empty)
                {
                    TimeZoneInfo centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                    timeZoneName = (centralTimeZone.IsDaylightSavingTime(scheduledDelivery.Value) ? centralTimeZone.DaylightName : centralTimeZone.StandardName);
                }
            }

            MessageTemplateModel orderEtaMainTemplate = messageTemplateLogic.ReadForKey("OrderEtaMain");

            Message message = new Message();
            //message.MessageSubject = orderEtaMainTemplate.Subject.Inject(new { CustomerName = string.Format("{0-{1}", customer.CustomerNumber, customer.CustomerName) });
            message.MessageSubject = orderEtaMainTemplate.Subject.Inject(customer);
            message.MessageBody = orderEtaMainTemplate.Body.Inject(new { TimeZoneName = timeZoneName, EtaOrderLines = orderInfoDetails.ToString() });
            message.CustomerNumber = customer.CustomerNumber;
			message.CustomerName = customer.CustomerName;
			message.BranchId = customer.CustomerBranch;
			message.NotificationType = NotificationType.OrderConfirmation;
            return message;
        }

        public void ProcessNotification(Core.Models.Messaging.Queue.BaseNotification notification) {
            try {
                if (notification.NotificationType != NotificationType.Eta) { throw new ApplicationException("notification/handler type mismatch"); }

                EtaNotification eta = (EtaNotification)notification;

                // load up recipients, customer and message
                eventLogRepository.WriteInformationLog("Received ETA Message with " + eta.Orders.Count + " orders");
                List<string> invoiceNumbers = eta.Orders.Select(x => x.OrderId).ToList();
                string etaBranch = eta.Orders[0].BranchId;
                var orders = orderHistoryRepository.Read(x => x.BranchId == etaBranch &&
                    invoiceNumbers.Contains(x.InvoiceNumber)).ToList(); // get all orders for order ETAs

                foreach (OrderHistoryHeader order in orders) {
                    try {
                        var etaInfo = eta.Orders.Where(o => o.OrderId.Equals(order.InvoiceNumber) && o.BranchId.Equals(order.BranchId)).FirstOrDefault();

                        if (etaInfo != null) {
                            //The information has a timezone offset that has already been accounted for, so we just need to ignore it
                            order.ScheduledDeliveryTime = String.IsNullOrEmpty(etaInfo.ScheduledTime) ? null : IgnoreOffsetTimeString(etaInfo.ScheduledTime);
                            order.EstimatedDeliveryTime = String.IsNullOrEmpty(etaInfo.EstimatedTime) ? null : IgnoreOffsetTimeString(etaInfo.EstimatedTime);
                            order.ActualDeliveryTime = String.IsNullOrEmpty(etaInfo.ActualTime) ? null : IgnoreOffsetTimeString(etaInfo.ActualTime);
                            order.RouteNumber = String.IsNullOrEmpty(etaInfo.RouteId) ? String.Empty : etaInfo.RouteId;
                            order.StopNumber = String.IsNullOrEmpty(etaInfo.StopNumber) ? String.Empty : etaInfo.StopNumber;
                            order.DeliveryOutOfSequence = etaInfo.OutOfSequence == null ? false : etaInfo.OutOfSequence;
                        }
                    }
                    catch (Exception ex) {
                        eventLogRepository.WriteErrorLog("Error processing ETA notification for : " + order.InvoiceNumber + ".  " + ex.Message + ".  " + ex.StackTrace);
                    }
                }

                foreach (var order in orders) {
                    try {
                        orderHistoryRepository.Update(order);
                        System.Threading.Thread.Sleep(200);
                    }
                    catch (Exception ex) {
                        eventLogRepository.WriteErrorLog("Error saving ETA notification for : " + order.InvoiceNumber + ".  " + ex.Message + ".  " + ex.StackTrace);
                    }
                }

                unitOfWork.SaveChanges();
            }
            catch (Exception ex) {
                throw new Core.Exceptions.Queue.QueueDataError<BaseNotification>(notification, "EtaNotification:ProcessNotification", "Send ETA Notification", "There was an error processing an ETA notification", ex);
            }
        }

        #endregion
    }
}
