using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Models.Messaging.Queue;
using KeithLink.Svc.Core.Models.Messaging.EF;
using KeithLink.Svc.Core.Models.Messaging.Provider;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Models.Configuration;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Common.Core.Logging;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Impl.Repository.EF.Operational;

namespace KeithLink.Svc.Impl.Logic.Messaging
{
    public class EtaNotificationHandlerImpl : BaseNotificationHandlerImpl, INotificationHandler
    {
        #region attributes
        IEventLogRepository eventLogRepository;
        IUserProfileLogic userProfileLogic;
        IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository;
        ICustomerRepository customerRepository;
        IUserMessagingPreferenceRepository userMessagingPreferenceRepository;
        IOrderHistoryHeaderRepsitory orderHistoryRepository;
        Func<Channel, IMessageProvider> messageProviderFactory;
        IMessageTemplateLogic messageTemplateLogic;
        IUnitOfWork unitOfWork;
        #endregion

        #region ctor
        public EtaNotificationHandlerImpl(IEventLogRepository eventLogRepository, IUserProfileLogic userProfileLogic
            , IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository, ICustomerRepository customerRepository
            , IUserMessagingPreferenceRepository userMessagingPreferenceRepository, Func<Channel, IMessageProvider> messageProviderFactory
            , IOrderHistoryHeaderRepsitory orderHistoryRepository, IMessageTemplateLogic messageTemplateLogic, IUnitOfWork unitOfWork)
            : base(userProfileLogic, userPushNotificationDeviceRepository, customerRepository
                    , userMessagingPreferenceRepository, messageProviderFactory, eventLogRepository)
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

        public void ProcessNotification(Core.Models.Messaging.Queue.BaseNotification notification)
        {
            if (notification.NotificationType != NotificationType.Eta)
                throw new ApplicationException("notification/handler type mismatch");

            EtaNotification eta = (EtaNotification)notification;

            // load up recipients, customer and message
            eventLogRepository.WriteInformationLog("Received ETA Message with " + eta.Orders.Count + " orders");
            List<string> invoiceNumbers = eta.Orders.Select(x => x.OrderId).ToList();
            var orders = orderHistoryRepository.Read(x => invoiceNumbers.Contains(x.InvoiceNumber)); // get all orders for order ETAs
            
            foreach(OrderHistoryHeader order in orders)
            {
                try
                {
                    var etaInfo = eta.Orders.Where(o => o.OrderId.Equals(order.InvoiceNumber) && o.BranchId.Equals(order.BranchId))
                        .FirstOrDefault();

                    if (etaInfo != null)
                    {
                        order.ScheduledDeliveryTime = String.IsNullOrEmpty(etaInfo.ScheduledTime) ? new Nullable<DateTime>() : DateTime.Parse(etaInfo.ScheduledTime).ToUniversalTime();
                        order.EstimatedDeliveryTime = String.IsNullOrEmpty(etaInfo.EstimatedTime) ? new Nullable<DateTime>() : DateTime.Parse(etaInfo.EstimatedTime).ToUniversalTime();
                        order.ActualDeliveryTime = String.IsNullOrEmpty(etaInfo.ActualTime) ? new Nullable<DateTime>() : DateTime.Parse(etaInfo.ActualTime).ToUniversalTime();
                        order.RouteNumber = String.IsNullOrEmpty(etaInfo.RouteId) ? String.Empty : etaInfo.RouteId;
                        order.StopNumber = String.IsNullOrEmpty(etaInfo.StopNumber) ? String.Empty : etaInfo.StopNumber;
                        order.DeliveryOutOfSequence = etaInfo.OutOfSequence == null ? false : etaInfo.OutOfSequence;
                    }
                }
                catch(Exception ex)
                {
                    eventLogRepository.WriteErrorLog("Error processing ETA notification for : " + order.InvoiceNumber + ".  " + ex.Message + ".  " + ex.StackTrace);
                }
                
                
            }
                
            foreach (var order in orders)
            {
                try
                {
                    orderHistoryRepository.Update(order);
                }
                catch (Exception ex)
                {
                    eventLogRepository.WriteErrorLog("Error saving ETA notification for : " + order.InvoiceNumber + ".  " + ex.Message + ".  " + ex.StackTrace);
                } 
            }

            unitOfWork.SaveChanges();

            // send out notifications by customer - this may be enabled eventually, but for now, we just display the data in the UI
            //List<string> customerNumbers = orders
            //    .GroupBy(o => o.CustomerNumber)
            //    .Select(grp => grp.First().CustomerNumber)
            //    .ToList(); // get list of customer numbers

            // for now, just update the order history entry with the currently estimated/scheduled/arrived times
            //foreach (string customerNumber in customerNumbers)
            //{
            //    Svc.Core.Models.Profile.Customer customer = customerRepository.GetCustomerByCustomerNumber(notification.CustomerNumber);
            //    List<Recipient> recipients = base.LoadRecipients(NotificationType.OrderConfirmation, customer);
            //    Message message = GetEmailMessageForNotification(eta.Orders, orders.Where(o => o.CustomerNumber == customerNumber), customer);

                // send messages to providers...
                //base.SendMessage(recipients, message);
            //}
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
            message.MessageSubject = orderEtaMainTemplate.Subject.Inject(new { CustomerName = customer.CustomerName });
            message.MessageBody = orderEtaMainTemplate.Body.Inject(new { TimeZoneName = timeZoneName, EtaOrderLines = orderInfoDetails.ToString() });
            message.CustomerNumber = customer.CustomerNumber;
			message.NotificationType = NotificationType.OrderConfirmation;
            return message;
        }
    }
}
