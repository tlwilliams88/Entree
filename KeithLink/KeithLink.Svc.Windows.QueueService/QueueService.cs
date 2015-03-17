using Autofac;
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
        
		public QueueService(IContainer container)
		{
			this.container = container;
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			InitializeNotificationsThread();
			InitializeConfirmationMoverThread();
			InitializeOrderUpdateThread();
		}

		protected override void OnStop()
		{
			TerminateConfirmationThread();
			TerminateOrderHistoryThread();
			TerminateNotificationsThread();
		}

		private void InitializeConfirmationMoverThread()
		{
			_confirmationLogic = container.Resolve<IConfirmationLogic>();
			_confirmationLogic.ListenForQueueMessages();
		}

		private void InitializeNotificationsThread()
		{
			_notificationQueueConsumer = container.Resolve<Svc.Core.Interface.Messaging.INotificationQueueConsumer>();
			_notificationQueueConsumer.ListenForNotificationMessagesOnQueue();
		}

		private void InitializeOrderUpdateThread()
		{
			_orderHistoryLogic = container.Resolve<IInternalOrderHistoryLogic>();
			_orderHistoryLogic.ListenForQueueMessages();
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
