using Autofac;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Core.Interface.Orders.Confirmations;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Impl.Repository.EF.Operational;

using Autofac;

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;


namespace KeithLink.Svc.Windows.QueueService {
    partial class QueueService : ServiceBase {
        #region attributes
        private IContainer container;
        private IConfirmationLogic _confirmationLogic;
        private IInternalOrderHistoryLogic _orderHistoryLogic;
        private Svc.Core.Interface.Messaging.INotificationQueueConsumer _externalNotificationQueueConsumer;
        private Svc.Core.Interface.Messaging.INotificationQueueConsumer _internalNotificationQueueConsumer;
        private IEventLogRepository _log;

        private ILifetimeScope confirmationScope;
        private ILifetimeScope orderHistoryScope;
        private ILifetimeScope externalNotificationScope;
        private ILifetimeScope internalNotificationScope;

        private static bool _checkLostOrdersProcessing;
        private System.Threading.Timer _checkLostOrdersTimer;

        const int TIMER_DURATION_TICKMINUTE = 60000;
        const int TIMER_DURATION_START = 1000;
        const int TIMER_DURATION_STOP = -1;
        const int TIMER_DURATION_IMMEDIATE = 1;
        #endregion

        #region ctor
        public QueueService( IContainer container ) {
            this.container = container;
            InitializeComponent();
        }
        #endregion

        #region methods
        private void InitializeCheckLostOrdersTimer() {

            AutoResetEvent auto = new AutoResetEvent( false );
            TimerCallback cb = new TimerCallback( ProcessCheckLostOrdersMinuteTick );

            if (Configuration.CheckLostOrders.Equals( "true", StringComparison.CurrentCultureIgnoreCase )) {
                _checkLostOrdersTimer = new System.Threading.Timer( cb, auto, TIMER_DURATION_START, TIMER_DURATION_TICKMINUTE );
            }
        }

        protected override void OnStart( string[] args ) {
            _log = container.Resolve<IEventLogRepository>();
            _log.WriteInformationLog( "Service starting" );


            InitializeNotificationsThread();
            InitializeConfirmationMoverThread();
            InitializeOrderUpdateThread();
            InitializeCheckLostOrdersTimer();
        }

        protected override void OnStop() {
            TerminateConfirmationThread();
            TerminateOrderHistoryThread();
            TerminateNotificationsThread();
            TerminateCheckLostOrdersTimer();

            _log.WriteInformationLog( "Service stopped" );
        }

        private void InitializeConfirmationMoverThread() {
            confirmationScope = container.BeginLifetimeScope();

            _confirmationLogic = confirmationScope.Resolve<IConfirmationLogic>();
            _confirmationLogic.ListenForQueueMessages();
        }

        private void InitializeNotificationsThread() {
            externalNotificationScope = container.BeginLifetimeScope();

            _externalNotificationQueueConsumer = externalNotificationScope.Resolve<Svc.Core.Interface.Messaging.INotificationQueueConsumer>();
            _externalNotificationQueueConsumer.ListenForExternalNotificationMessagesOnQueue();

            internalNotificationScope = container.BeginLifetimeScope();
            _internalNotificationQueueConsumer = internalNotificationScope.Resolve<Svc.Core.Interface.Messaging.INotificationQueueConsumer>();
            _internalNotificationQueueConsumer.ListenForInternalNotificationMessagesOnQueue();
        }

        private void InitializeOrderUpdateThread() {
            orderHistoryScope = container.BeginLifetimeScope();
            _orderHistoryLogic = orderHistoryScope.Resolve<IInternalOrderHistoryLogic>();
            _orderHistoryLogic.ListenForQueueMessages();
        }

        private void TerminateConfirmationThread() {
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
            if (_externalNotificationQueueConsumer != null)
                _externalNotificationQueueConsumer.Stop();

            if (externalNotificationScope != null)
                externalNotificationScope.Dispose();

            if (_internalNotificationQueueConsumer != null)
                _internalNotificationQueueConsumer.Stop();

            if (internalNotificationScope != null)
                internalNotificationScope.Dispose();
        }

        private void TerminateCheckLostOrdersTimer() {
            if (_checkLostOrdersTimer != null) {
                _checkLostOrdersTimer.Change( TIMER_DURATION_IMMEDIATE, TIMER_DURATION_STOP );
            }
        }

        private void ProcessCheckLostOrdersMinuteTick( object state ) {

            if (!_checkLostOrdersProcessing) {
                _checkLostOrdersProcessing = true;

                // do not process between 1 and 5
                if (DateTime.Now.Hour >= 1 && DateTime.Now.Hour < 5) {
                    _log.WriteInformationLog( "Script stopped for processing window" );

                    while (DateTime.Now.Hour < 5) {
                        System.Threading.Thread.Sleep( 60000 );
                    }

                    _log.WriteInformationLog( "Script started after processing window" );
                }

                // only process at the top of the hour
                if (DateTime.Now.Minute == 0) 
                {
                    _log.WriteInformationLog("ProcessCheckLostOrdersMinuteTick run at " + DateTime.Now.ToString("h:mm:ss tt"));
                    try {
                        string subject;
                        string body;

                        orderHistoryScope = container.BeginLifetimeScope();
                        _orderHistoryLogic = orderHistoryScope.Resolve<IInternalOrderHistoryLogic>();
                        subject = _orderHistoryLogic.CheckForLostOrders( out body );

                        StringBuilder sbMsgBody = new StringBuilder();
                        sbMsgBody.Append( body );

                        if ((subject != null) && (subject.Length > 0) && (body != null) && (body.Length > 0)) {
                            _log.WriteErrorLog( subject + " " + body );
                            KeithLink.Common.Core.Email.ExceptionEmail.Send( new Exception( body ), "BEK: " + subject );
                        }
                    } catch (Exception ex) {
                        _log.WriteErrorLog( "Error in ProcessCheckLostOrdersMinuteTick", ex );
                        KeithLink.Common.Core.Email.ExceptionEmail.Send( ex );
                    }
                }

                _checkLostOrdersProcessing = false;
            }
        }
        #endregion
    }
}