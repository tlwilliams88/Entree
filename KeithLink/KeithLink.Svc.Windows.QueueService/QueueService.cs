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

namespace KeithLink.Svc.Windows.QueueService {
    partial class QueueService : ServiceBase {
        #region attributes
        private IContainer container;
        private IConfirmationLogic _confirmationLogic;
        private IOrderHistoryLogic _orderHistoryLogic;
        private ISpecialOrderLogic _specialOrderLogic;
        private INotificationQueueConsumer _notificationQueueConsumer;
        private IPushMessageConsumer _pushmessageConsumer;
        private IEventLogRepository _log;
        private IEmailClient _emailClient;

        private ILifetimeScope confirmationScope;
        private ILifetimeScope lostOrdersScope;
        private ILifetimeScope orderHistoryScope;
        private ILifetimeScope specialOrderScope;
        private ILifetimeScope notificationScope;
        private ILifetimeScope pushMessagesScope;

        private Task lostOrdersTask;
        private static bool _checkLostOrdersProcessing;
        private Timer _checkLostOrdersTimer;

        const int TIMER_DURATION_TICKMINUTE = 60000;
        const int TIMER_DURATION_START = 1000;
        const int TIMER_DURATION_STOP = -1;
        const int TIMER_DURATION_IMMEDIATE = 1;
        #endregion

        #region ctor
        public QueueService()
        {
            container = DependencyMapFactory.GetQueueSvcContainer().Build();
            InitializeComponent();
        }
        #endregion

        #region methods
        private void InitializeCheckLostOrdersTimer() {

            AutoResetEvent auto = new AutoResetEvent( false );
            TimerCallback cb = new TimerCallback( ProcessCheckLostOrdersMinuteTick );

            string checksetting = Configuration.CheckLostOrders;
            List<string> excluded = new List<string>();
            if (checksetting.IndexOf('|') > -1)
            {
                string exclusions = checksetting.Substring(checksetting.IndexOf('|') + 1);
                checksetting = checksetting.Substring(0, checksetting.IndexOf('|'));
                excluded = Configuration.GetCommaSeparatedValues(exclusions.ToUpper());
            }

            if (checksetting.Equals( "true", StringComparison.CurrentCultureIgnoreCase ) && 
                excluded.Contains(System.Environment.MachineName) == false) {
                _log.WriteInformationLog("CheckLostOrders started");
                _checkLostOrdersTimer = new Timer( cb, auto, TIMER_DURATION_START, TIMER_DURATION_TICKMINUTE );
                lostOrdersScope = container.BeginLifetimeScope();
                _emailClient = lostOrdersScope.Resolve<IEmailClient>();
            }
            else
            {
                _log.WriteInformationLog("CheckLostOrders not started");
            }
        }

        protected override void OnStart( string[] args ) {
            _log = container.Resolve<IEventLogRepository>();
            _log.WriteInformationLog( "Service starting" );
            
            InitializeNotificationsThread();
            InitializePushMessageConsumerThread();
            InitializeConfirmationMoverThread();
            InitializeOrderUpdateThread();
            InitializeCheckLostOrdersTimer();
            InitializeSpecialOrderUpdateThread();
        }

        protected override void OnStop() {
            _log.WriteInformationLog("Service stopping");

            TerminateConfirmationThread();
            TerminateOrderHistoryThread();
            TerminateNotificationsThread();
            TerminatePushMessageConsumerThread();
            TerminateCheckLostOrdersTimer();
            TerminateSpecialOrderUpdateThread();

            _log.WriteInformationLog( "Service stopped" );
        }

        private void InitializeConfirmationMoverThread() {
            confirmationScope = container.BeginLifetimeScope();

            _confirmationLogic = confirmationScope.Resolve<IConfirmationLogic>();
            _confirmationLogic.ListenForQueueMessages();
        }

        private void InitializeNotificationsThread() {
            notificationScope = container.BeginLifetimeScope();
            _notificationQueueConsumer = notificationScope.Resolve<INotificationQueueConsumer>();
            _notificationQueueConsumer.ListenForNotificationMessagesOnQueue();
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
                _confirmationLogic.Stop();
            if (confirmationScope != null)
                confirmationScope.Dispose();
        }

        private void TerminateOrderHistoryThread() {
            if (_orderHistoryLogic != null)
                _orderHistoryLogic.StopListening();

            if (orderHistoryScope != null)
                orderHistoryScope.Dispose();
        }

        private void TerminateNotificationsThread() {
            if (_notificationQueueConsumer != null)
                _notificationQueueConsumer.Stop();

            if (notificationScope != null)
                notificationScope.Dispose();
        }

        private void TerminatePushMessageConsumerThread()
        {
            if (_pushmessageConsumer != null)
                _pushmessageConsumer.Stop();

            if (pushMessagesScope != null)
                pushMessagesScope.Dispose();
        }

        private void TerminateCheckLostOrdersTimer() {
            if (_checkLostOrdersTimer != null) {
                _checkLostOrdersTimer.Change(TIMER_DURATION_IMMEDIATE, TIMER_DURATION_STOP);
                if(lostOrdersTask != null) lostOrdersTask.Wait();
            }
            if (lostOrdersTask != null) _log.WriteWarningLog(string.Format("QueueService.lostOrdersTask.status = {0:G}", lostOrdersTask.Status));
        }

        private void ProcessCheckLostOrdersMinuteTick( object state ) {

            if ((_checkLostOrdersProcessing) && (DateTime.Now.Minute == 0))
                _log.WriteInformationLog("ProcessCheckLostOrdersMinuteTick, _checkLostOrdersProcessing=true");
            if (!_checkLostOrdersProcessing)
            {
                _checkLostOrdersProcessing = true;

                if (DateTime.Now.Hour >= 1 && DateTime.Now.Hour < 5)
                {
                    while (DateTime.Now.Hour < 5)
                    {
                        //_log.WriteInformationLog("ProcessCheckLostOrdersMinuteTick, asleep");
                        System.Threading.Thread.Sleep(60000);
                    }
                }

                // only process at the top of the hour
                if (DateTime.Now.Minute == 0)
                //if (true) // testing only
                {
                    lostOrdersTask = Task.Factory.StartNew(() => ProcessCheckLostOrders(),
                        CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
                }

                _checkLostOrdersProcessing = false;
            }
        }

        private void ProcessCheckLostOrders()
        {
            //_log.WriteInformationLog("ProcessCheckLostOrdersMinuteTick run");
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
        #endregion
    }
}