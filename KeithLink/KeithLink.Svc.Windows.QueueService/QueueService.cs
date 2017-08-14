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
using KeithLink.Svc.Core.Interface.Lists;

namespace KeithLink.Svc.Windows.QueueService {
    partial class QueueService : ServiceBase {
        #region attributes
        private IContainer container;
        private IConfirmationLogic _confirmationLogic;
        private IOrderHistoryLogic _orderHistoryLogic;
        private ISpecialOrderLogic _specialOrderLogic;
        private IContractListChangesLogic _contractListChangesLogic;
        private INotificationQueueConsumer _orderConfirmationQueueConsumer;
        private INotificationQueueConsumer _hasNewsQueueConsumer;
        private INotificationQueueConsumer _paymentConfirmationQueueConsumer;
        private INotificationQueueConsumer _ETAQueueConsumer;
        private IPushMessageConsumer _pushmessageConsumer;
        private IEventLogRepository _log;
        private IEmailClient _emailClient;

        private ILifetimeScope confirmationScope;
        private ILifetimeScope lostOrdersScope;
        private ILifetimeScope contractChangesScope;
        private ILifetimeScope orderHistoryScope;
        private ILifetimeScope specialOrderScope;
        private ILifetimeScope notificationOrderConfirmationScope;
        private ILifetimeScope notificationHasNewsScope;
        private ILifetimeScope notificationPaymentConfirmationScope;
        private ILifetimeScope notificationEtaScope;
        private ILifetimeScope pushMessagesScope;

        private static bool _checkLostOrdersProcessing;
        private static bool _contractChangesProcessing;
        private Timer _Timer;

        const int TIMER_DURATION_TICKMINUTE = 60000;
        const int TIMER_DURATION_START = 1000;
        const int TIMER_DURATION_STOP = -1;
        const int TIMER_DURATION_IMMEDIATE = 1;
        #endregion

        #region ctor
        public QueueService()
        {
            container = DependencyMapFactory.GetQueueSvcContainer(Core.Enumerations.Dependencies.DependencyInstanceType.InstancePerLifetimeScope).Build();
            InitializeComponent();
        }
        #endregion

        #region methods
        private void InitializeTimer() {

            AutoResetEvent auto = new AutoResetEvent( false );
            TimerCallback cb = new TimerCallback( ProcessMinuteTick );

            _Timer = new Timer( cb, auto, TIMER_DURATION_START, TIMER_DURATION_TICKMINUTE );
        }

        protected override void OnStart( string[] args ) {
            _log = container.Resolve<IEventLogRepository>();
            _log.WriteInformationLog( "Service starting" );

            InitializeNotificationsThreads();
            InitializePushMessageConsumerThread();
            InitializeConfirmationMoverThread();
            InitializeOrderUpdateThread();
            InitializeTimer();
            InitializeSpecialOrderUpdateThread();
        }

        protected override void OnStop() {
            _log.WriteInformationLog("Service stopping");

            TerminateConfirmationThread();
            TerminateOrderUpdateThread();
            TerminateNotificationsThreads();
            TerminatePushMessageConsumerThread();
            //TerminateCheckLostOrdersTimer();// moved to monitor service
            TerminateSpecialOrderUpdateThread();
            TerminateTimer();

            _log.WriteInformationLog( "Service stopped" );
        }

        private void InitializeConfirmationMoverThread() {
            confirmationScope = container.BeginLifetimeScope();

            _confirmationLogic = confirmationScope.Resolve<IConfirmationLogic>();
            //_confirmationLogic.SubscribeToQueue();
            _confirmationLogic.ListenForQueueMessages();
        }
        
        private void InitializeNotificationsThreads()
        {
            Task.Factory.StartNew(StartOrderNotifications);

            Task.Factory.StartNew(StartHasNewsNotifications);

            Task.Factory.StartNew(StartPaymentNotifications);

            Task.Factory.StartNew(StartEtaNotifications);
        }

        private void StartEtaNotifications()
        {
            notificationEtaScope = container.BeginLifetimeScope();

            _ETAQueueConsumer = notificationEtaScope.Resolve<INotificationQueueConsumer>();
            _ETAQueueConsumer.RabbitMQQueueName = Configuration.RabbitMQQueueETA;
            //_ETAQueueConsumer.SubscribeToQueue(Configuration.RabbitMQQueueETA);
            _ETAQueueConsumer.ListenForNotificationMessagesOnQueue();
        }

        private void StartHasNewsNotifications()
        {
            notificationHasNewsScope = container.BeginLifetimeScope();

            _hasNewsQueueConsumer = notificationHasNewsScope.Resolve<INotificationQueueConsumer>();
            _hasNewsQueueConsumer.RabbitMQQueueName = Configuration.RabbitMQQueueHasNews;
            //_hasNewsQueueConsumer.SubscribeToQueue(Configuration.RabbitMQQueueHasNews);
            _hasNewsQueueConsumer.ListenForNotificationMessagesOnQueue();
        }

        private void StartOrderNotifications()
        {
            notificationOrderConfirmationScope = container.BeginLifetimeScope();

            _orderConfirmationQueueConsumer = notificationOrderConfirmationScope.Resolve<INotificationQueueConsumer>();
            _orderConfirmationQueueConsumer.RabbitMQQueueName = Configuration.RabbitMQQueueOrderConfirmation;
            //_orderConfirmationQueueConsumer.SubscribeToQueue(Configuration.RabbitMQQueueOrderConfirmation);
            _orderConfirmationQueueConsumer.ListenForNotificationMessagesOnQueue();
        }

        private void StartPaymentNotifications()
        {
            notificationPaymentConfirmationScope = container.BeginLifetimeScope();

            _paymentConfirmationQueueConsumer = notificationPaymentConfirmationScope.Resolve<INotificationQueueConsumer>();
            _paymentConfirmationQueueConsumer.RabbitMQQueueName = Configuration.RabbitMQQueuePaymentConfirmation;
            //_paymentConfirmationQueueConsumer.SubscribeToQueue(Configuration.RabbitMQQueuePaymentConfirmation);
            _paymentConfirmationQueueConsumer.ListenForNotificationMessagesOnQueue();
        }

        private void InitializePushMessageConsumerThread()
        {
            pushMessagesScope = container.BeginLifetimeScope();
            _pushmessageConsumer = pushMessagesScope.Resolve<IPushMessageConsumer>();
            //_pushmessageConsumer.SubscribeToQueue();
            _pushmessageConsumer.ListenForQueueMessages();
        }

        private void InitializeOrderUpdateThread() {
            orderHistoryScope = container.BeginLifetimeScope();
            _orderHistoryLogic = orderHistoryScope.Resolve<IOrderHistoryLogic>();
            //_orderHistoryLogic.SubscribeToQueue();
            _orderHistoryLogic.ListenForQueueMessages();
        }

        private void InitializeSpecialOrderUpdateThread()
        {
            specialOrderScope = container.BeginLifetimeScope();
            _specialOrderLogic = specialOrderScope.Resolve<ISpecialOrderLogic>();
            //_specialOrderLogic.SubscribeToQueue();
            _specialOrderLogic.ListenForQueueMessages();
        }

        private void TerminateSpecialOrderUpdateThread()
        {
            if (_specialOrderLogic != null)
            {
                //_specialOrderLogic.Unsubscribe();
                _specialOrderLogic.StopListening();
            }

            if (specialOrderScope != null)
                specialOrderScope.Dispose();
        }

        private void TerminateConfirmationThread()
        {
            if (_confirmationLogic != null)
            {
                //_confirmationLogic.UnsubscribeFromQueue();
                _confirmationLogic.Stop();
            }
            if (confirmationScope != null)
                confirmationScope.Dispose();
        }

        private void TerminateOrderUpdateThread() {
            if (_orderHistoryLogic != null)
            {
                //_orderHistoryLogic.Unsubscribe();
                _orderHistoryLogic.StopListening();
            }

            if (orderHistoryScope != null)
                orderHistoryScope.Dispose();
        }

        private void TerminateNotificationsThreads() {
            if (_hasNewsQueueConsumer != null)
                //_hasNewsQueueConsumer.Unsubscribe();
                _hasNewsQueueConsumer.Stop();

            if (_paymentConfirmationQueueConsumer != null)
                //_paymentConfirmationQueueConsumer.Unsubscribe();
                _paymentConfirmationQueueConsumer.Stop();

            if (_orderConfirmationQueueConsumer != null)
                //_paymentConfirmationQueueConsumer.Unsubscribe();
                _orderConfirmationQueueConsumer.Stop();

            if (_ETAQueueConsumer != null)
                //_ETAQueueConsumer.Unsubscribe();
                _ETAQueueConsumer.Stop();

            if (notificationHasNewsScope != null)
                notificationHasNewsScope.Dispose();

            if (notificationPaymentConfirmationScope != null)
                notificationPaymentConfirmationScope.Dispose();

            if (notificationOrderConfirmationScope != null)
                notificationOrderConfirmationScope.Dispose();

            if (notificationEtaScope != null)
                notificationEtaScope.Dispose();
        }

        private void TerminatePushMessageConsumerThread()
        {
            if (_pushmessageConsumer != null)
            {
                //_pushmessageConsumer.Unsubscribe();
                _pushmessageConsumer.Stop();
            }

            if (pushMessagesScope != null)
                pushMessagesScope.Dispose();
        }

        private void TerminateTimer() {
            if (_Timer != null) {
                _Timer.Change(TIMER_DURATION_IMMEDIATE, TIMER_DURATION_STOP);
            }
        }

        private void ProcessMinuteTick(object state)
        {
            if (true)
            // only process at the top of the hour
            //if (DateTime.Now.Minute == 0)
            {
                //StartEntreeProcess(Configuration.CheckLostOrders, "CheckLostOrders", () => ProcessCheckLostOrdersTick());

                StartEntreeProcess(Configuration.ProcessContractChanges, "ProcessContractChanges", () => ProcessContractChangesTick());
            }
        }

        private void StartEntreeProcess(string checkstring, string processname, Action action)
        {
            string checksetting = checkstring;
            List<string> excluded = new List<string>();
            if (checksetting.IndexOf('|') > -1)
            {
                string exclusions = checksetting.Substring(checksetting.IndexOf('|') + 1);
                checksetting = checksetting.Substring(0, checksetting.IndexOf('|'));
                excluded = Configuration.GetCommaSeparatedValues(exclusions.ToUpper());
            }

            if (checksetting.Equals("true", StringComparison.CurrentCultureIgnoreCase) &&
                excluded.Contains(System.Environment.MachineName) == false)
            {
                _log.WriteInformationLog(string.Format("{0} started", processname));
                Task.Run(action);
            }
            else
            {
                _log.WriteInformationLog(string.Format("{0} not started", processname));
            }
        }

        private void ProcessCheckLostOrdersTick() {

            if (!_checkLostOrdersProcessing)
            {
                _checkLostOrdersProcessing = true;

                if (DateTime.Now.Hour >= 1 && DateTime.Now.Hour < 5)
                {
                    while (DateTime.Now.Hour < 5)
                    {
                        _checkLostOrdersProcessing = false;
                        return;
                    }
                }

                ProcessCheckLostOrders();

                _checkLostOrdersProcessing = false;
            }
        }

        private void ProcessCheckLostOrders()
        {
            try
            {
                string subject;
                string body;

                orderHistoryScope = container.BeginLifetimeScope();
                _orderHistoryLogic = orderHistoryScope.Resolve<IOrderHistoryLogic>();
                subject = _orderHistoryLogic.CheckForLostOrders(out body);

                StringBuilder sbMsgBody = new StringBuilder();
                sbMsgBody.Append(body);

                if ((subject != null) && (subject.Length > 0) && (body != null) && (body.Length > 0))
                {
                    _emailClient = lostOrdersScope.Resolve<IEmailClient>();
                    //_log.WriteErrorLog("BEK: " + subject + ";" + body );
                    _emailClient.SendEmail(Configuration.FailureEmailAdresses, null, null, "BEK: " + subject, body);
                }
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("Error in ProcessCheckLostOrdersMinuteTick", ex);
                KeithLink.Common.Impl.Email.ExceptionEmail.Send(ex);
            }
        }

        private void ProcessContractChangesTick()
        {

            if (!_contractChangesProcessing)
            {
                _contractChangesProcessing = true;

                if (DateTime.Now.Hour >= 1 && DateTime.Now.Hour < 5)
                {
                    while (DateTime.Now.Hour < 5)
                    {
                        _contractChangesProcessing = false;
                        return;
                    }
                }

                ProcessContractChanges();

                _contractChangesProcessing = false;
            }
        }

        private void ProcessContractChanges()
        {
            contractChangesScope = container.BeginLifetimeScope();
            _contractListChangesLogic = contractChangesScope.Resolve<IContractListChangesLogic>();
            _contractListChangesLogic.ProcessContractChanges();
        }
        #endregion
    }
}