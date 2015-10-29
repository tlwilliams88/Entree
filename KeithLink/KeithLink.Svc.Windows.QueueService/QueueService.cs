using Autofac;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.Orders.Confirmations;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Windows.QueueService
{
    partial class QueueService : ServiceBase
    {
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

        public QueueService(IContainer container)
        {
            this.container = container;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _log = container.Resolve<IEventLogRepository>();
            _log.WriteInformationLog("Service starting");


            InitializeNotificationsThread();
            InitializeConfirmationMoverThread();
            InitializeOrderUpdateThread();
        }

        protected override void OnStop()
        {
            TerminateConfirmationThread();
            TerminateOrderHistoryThread();
            TerminateNotificationsThread();

            _log.WriteInformationLog("Service stopped");
        }

        private void InitializeConfirmationMoverThread()
        {
            confirmationScope = container.BeginLifetimeScope();

            _confirmationLogic = confirmationScope.Resolve<IConfirmationLogic>();
            _confirmationLogic.ListenForQueueMessages();
        }

        private void InitializeNotificationsThread()
        {
            externalNotificationScope = container.BeginLifetimeScope();

            _externalNotificationQueueConsumer = externalNotificationScope.Resolve<Svc.Core.Interface.Messaging.INotificationQueueConsumer>();
            _externalNotificationQueueConsumer.ListenForExternalNotificationMessagesOnQueue();

            internalNotificationScope = container.BeginLifetimeScope();
            _internalNotificationQueueConsumer = internalNotificationScope.Resolve<Svc.Core.Interface.Messaging.INotificationQueueConsumer>();
            _internalNotificationQueueConsumer.ListenForInternalNotificationMessagesOnQueue();
        }

        private void InitializeOrderUpdateThread()
        {
            orderHistoryScope = container.BeginLifetimeScope();
            _orderHistoryLogic = orderHistoryScope.Resolve<IInternalOrderHistoryLogic>();
            _orderHistoryLogic.ListenForQueueMessages();
        }

        private void TerminateConfirmationThread()
        {
            if (_confirmationLogic != null)
                _confirmationLogic.Stop();
            if (confirmationScope != null)
                confirmationScope.Dispose();
        }

        private void TerminateOrderHistoryThread()
        {
            if (_orderHistoryLogic != null)
                _orderHistoryLogic.StopListening();

            if (orderHistoryScope != null)
                orderHistoryScope.Dispose();
        }

        private void TerminateNotificationsThread()
        {
            if (_externalNotificationQueueConsumer != null)
                _externalNotificationQueueConsumer.Stop();

            if (externalNotificationScope != null)
                externalNotificationScope.Dispose();

            if (_internalNotificationQueueConsumer != null)
                _internalNotificationQueueConsumer.Stop();

            if (internalNotificationScope != null)
                internalNotificationScope.Dispose();
        }
    }
}
