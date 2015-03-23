using Autofac;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.Orders.Confirmations;
using KeithLink.Svc.Core.Interface.Orders.History;
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
		private Svc.Core.Interface.Messaging.INotificationQueueConsumer _notificationQueueConsumer;
        private IEventLogRepository _log;
        
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
            using (var threadLifetime = container.BeginLifetimeScope()) {
			    _confirmationLogic = container.Resolve<IConfirmationLogic>();
			    _confirmationLogic.ListenForQueueMessages();

            }
		}

		private void InitializeNotificationsThread()
		{
            using (var threadLifetime = container.BeginLifetimeScope()) {
                _notificationQueueConsumer = container.Resolve<Svc.Core.Interface.Messaging.INotificationQueueConsumer>();
                _notificationQueueConsumer.ListenForNotificationMessagesOnQueue();
            }
		}

		private void InitializeOrderUpdateThread()
		{
            using (var threadLifetime = container.BeginLifetimeScope()) {
                _orderHistoryLogic = container.Resolve<IInternalOrderHistoryLogic>();
                _orderHistoryLogic.ListenForQueueMessages();
            }
		}

		private void TerminateConfirmationThread()
		{
			if (_confirmationLogic != null)
				_confirmationLogic.Stop();
		}

		private void TerminateOrderHistoryThread()
		{
			if (_orderHistoryLogic != null)
				_orderHistoryLogic.StopListening();
		}

		private void TerminateNotificationsThread()
		{
			if (_notificationQueueConsumer != null)
				_notificationQueueConsumer.Stop();
		}
	}
}
