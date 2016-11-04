using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.Confirmations;
using KeithLink.Svc.Core.Interface.Orders.History;

using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Repository.SmartResolver;

using Autofac;

using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.ApplicationHealth;

namespace KeithLink.Svc.Windows.QueueService {
    partial class QueueService : ServiceBase {
        #region attributes
        private IContainer container;
        private IConfirmationLogic _confirmationLogic;
        private IOrderHistoryLogic _orderHistoryLogic;
        private ISpecialOrderLogic _specialOrderLogic;
        private INotificationQueueConsumer _orderConfirmationQueueConsumer;
        private INotificationQueueConsumer _hasNewsQueueConsumer;
        private INotificationQueueConsumer _paymentConfirmationQueueConsumer;
        private INotificationQueueConsumer _ETAQueueConsumer;
        private IPushMessageConsumer _pushmessageConsumer;
        private IEventLogRepository _log;

        private ILifetimeScope confirmationScope;
        private ILifetimeScope orderHistoryScope;
        private ILifetimeScope specialOrderScope;
        private ILifetimeScope notificationOrderConfirmationScope;
        private ILifetimeScope notificationHasNewsScope;
        private ILifetimeScope notificationPaymentConfirmationScope;
        private ILifetimeScope notificationEtaScope;
        private ILifetimeScope pushMessagesScope;
        #endregion

        #region ctor
        public QueueService()
        {
            container = DependencyMapFactory.GetQueueSvcContainer
                            (Core.Enumerations.Dependencies.DependencyInstanceType.InstancePerLifetimeScope).Build();
            InitializeComponent();
        }
        #endregion

        #region methods
        protected override void OnStart( string[] args ) {
            _log = container.Resolve<IEventLogRepository>();
            _log.WriteInformationLog( "Service starting" );

            InitializeNotificationsThreads();
            InitializePushMessageConsumerThread();
            InitializeConfirmationMoverThread();
            InitializeOrderUpdateThread();
            InitializeSpecialOrderUpdateThread();
        }

        protected override void OnStop() {
            _log.WriteInformationLog("Service stopping");

            TerminateConfirmationThread();
            TerminateOrderUpdateThread();
            TerminateNotificationsThreads();
            TerminatePushMessageConsumerThread();
            TerminateSpecialOrderUpdateThread();

            _log.WriteInformationLog( "Service stopped" );
        }

        private void InitializeConfirmationMoverThread() {
            confirmationScope = container.BeginLifetimeScope();

            _confirmationLogic = confirmationScope.Resolve<IConfirmationLogic>();
            _confirmationLogic.SubscribeToQueue();
            //_confirmationLogic.ListenForQueueMessages();
        }
        
        private void InitializeNotificationsThreads() {
            notificationOrderConfirmationScope = container.BeginLifetimeScope();
            notificationHasNewsScope = container.BeginLifetimeScope();
            notificationPaymentConfirmationScope = container.BeginLifetimeScope();
            notificationEtaScope = container.BeginLifetimeScope();

            _orderConfirmationQueueConsumer = notificationOrderConfirmationScope.Resolve<INotificationQueueConsumer>();
            _orderConfirmationQueueConsumer.RabbitMQQueueName = Configuration.RabbitMQQueueOrderConfirmation;
            _orderConfirmationQueueConsumer.SubscribeToQueue(Configuration.RabbitMQQueueOrderConfirmation);

            _hasNewsQueueConsumer = notificationHasNewsScope.Resolve<INotificationQueueConsumer>();
            _hasNewsQueueConsumer.RabbitMQQueueName = Configuration.RabbitMQQueueHasNews;
            _hasNewsQueueConsumer.SubscribeToQueue(Configuration.RabbitMQQueueHasNews);

            _paymentConfirmationQueueConsumer = notificationPaymentConfirmationScope.Resolve<INotificationQueueConsumer>();
            _paymentConfirmationQueueConsumer.RabbitMQQueueName = Configuration.RabbitMQQueuePaymentConfirmation;
            _paymentConfirmationQueueConsumer.SubscribeToQueue(Configuration.RabbitMQQueuePaymentConfirmation);

            _ETAQueueConsumer = notificationEtaScope.Resolve<INotificationQueueConsumer>();
            _ETAQueueConsumer.RabbitMQQueueName = Configuration.RabbitMQQueueETA;
            _ETAQueueConsumer.SubscribeToQueue(Configuration.RabbitMQQueueETA);
        }

        private void InitializePushMessageConsumerThread()
        {
            pushMessagesScope = container.BeginLifetimeScope();
            _pushmessageConsumer = pushMessagesScope.Resolve<IPushMessageConsumer>();
            _pushmessageConsumer.ListenForQueueMessages();
        }

        private void InitializeOrderUpdateThread() {
            orderHistoryScope = container.BeginLifetimeScope();
            _orderHistoryLogic = orderHistoryScope.Resolve<IOrderHistoryLogic>();
            _orderHistoryLogic.ListenForQueueMessages();
        }

        private void InitializeSpecialOrderUpdateThread()
        {
            specialOrderScope = container.BeginLifetimeScope();
            _specialOrderLogic = specialOrderScope.Resolve<ISpecialOrderLogic>();
            _specialOrderLogic.ListenForQueueMessages();
        }

        private void TerminateSpecialOrderUpdateThread()
        {
            if (_specialOrderLogic != null)
                _specialOrderLogic.StopListening();

            if (specialOrderScope != null)
                specialOrderScope.Dispose();
        }

        private void TerminateConfirmationThread()
        {
            if (_confirmationLogic != null)
                _confirmationLogic.UnsubscribeFromQueue();
            if (confirmationScope != null)
                confirmationScope.Dispose();
        }

        private void TerminateOrderUpdateThread() {
            if (_orderHistoryLogic != null)
                _orderHistoryLogic.StopListening();

            if (orderHistoryScope != null)
                orderHistoryScope.Dispose();
        }

        private void TerminateNotificationsThreads() {
            if (_orderConfirmationQueueConsumer != null)
                _orderConfirmationQueueConsumer.Unsubscribe();

            if (_hasNewsQueueConsumer != null)
                _hasNewsQueueConsumer.Unsubscribe();

            if (_paymentConfirmationQueueConsumer != null)
                _paymentConfirmationQueueConsumer.Unsubscribe();

            if (_ETAQueueConsumer != null)
                _ETAQueueConsumer.Unsubscribe();

            if (notificationOrderConfirmationScope != null)
                notificationOrderConfirmationScope.Dispose();

            if (notificationHasNewsScope != null)
                notificationHasNewsScope.Dispose();

            if (notificationPaymentConfirmationScope != null)
                notificationPaymentConfirmationScope.Dispose();

            if (notificationEtaScope != null)
                notificationEtaScope.Dispose();
        }

        private void TerminatePushMessageConsumerThread()
        {
            if (_pushmessageConsumer != null)
                _pushmessageConsumer.Stop();

            if (pushMessagesScope != null)
                pushMessagesScope.Dispose();
        }

        #endregion
    }
}