using KeithLink.Common.Core.Logging;
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
using System.Threading.Tasks;

namespace KeithLink.Svc.Windows.QueueService {
    partial class QueueService : ServiceBase {
        #region attributes
        private IContainer container;
        private IConfirmationLogic _confirmationLogic;
        private IInternalOrderHistoryLogic _orderHistoryLogic;
        private Svc.Core.Interface.Messaging.INotificationQueueConsumer _notificationQueueConsumer;
        private IEventLogRepository _log;

        private ILifetimeScope confirmationScope;
        private ILifetimeScope orderHistoryScope;
        private ILifetimeScope notificationScope;
        #endregion

        #region ctor
        public QueueService(IContainer container) {
            this.container = container;
            InitializeComponent();
        }
        #endregion

        #region methods
        private void InitializeConfirmationMoverThread() {
            confirmationScope = container.BeginLifetimeScope();

            _confirmationLogic = confirmationScope.Resolve<IConfirmationLogic>();
            _confirmationLogic.ListenForQueueMessages();
        }

        private void InitializeNotificationsThread() {
            notificationScope = container.BeginLifetimeScope();

            _notificationQueueConsumer = notificationScope.Resolve<Svc.Core.Interface.Messaging.INotificationQueueConsumer>();
            _notificationQueueConsumer.ListenForNotificationMessagesOnQueue();
        }

        private void InitializeOrderUpdateThread() {
            orderHistoryScope = container.BeginLifetimeScope();
            _orderHistoryLogic = orderHistoryScope.Resolve<IInternalOrderHistoryLogic>();
            _orderHistoryLogic.ListenForQueueMessages();
        }

        protected override void OnStart(string[] args) {
            _log = container.Resolve<IEventLogRepository>();
            _log.WriteInformationLog("Service starting");


            InitializeNotificationsThread();
            InitializeConfirmationMoverThread();
            InitializeOrderUpdateThread();
        }

        protected override void OnStop() {
            TerminateConfirmationThread();
            TerminateOrderHistoryThread();
            TerminateNotificationsThread();

            _log.WriteInformationLog("Service stopped");
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
            if (_notificationQueueConsumer != null)
                _notificationQueueConsumer.Stop();

            if (notificationScope != null)
                notificationScope.Dispose();
        }
        #endregion
    }
}
