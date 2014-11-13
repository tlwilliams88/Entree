using Autofac;
using Autofac.Integration.Wcf;
using KeithLink.Common.Core.Logging;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Interface.Orders.Confirmations;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Models.Orders.Confirmations;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.InternalSvc.Interfaces;
using CommerceServer.Core.Runtime.Orders;
using CommerceServer.Core.Orders;
using CommerceServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace KeithLink.Svc.InternalSvc
{
    public class Global : System.Web.HttpApplication {
        #region attributes
        private IConfirmationLogic _confirmationLogic;
        private IOrderHistoryLogic _orderHistoryLogic;
        private Svc.Core.Interface.Messaging.IInternalMessagingLogic _messagingLogic;
        #endregion

        #region events
        protected void Application_Start(object sender, EventArgs e)
        {
            IContainer container = AutofacContainerBuilder.BuildContainer();
            AutofacHostFactory.Container = container;

            if (Svc.Impl.Configuration.RunInternalServiceQueues)
            {
				//InitializeConfirmationMoverThread();
				//InitializeNotificationsThread();
				//InitializeOrderUpdateThread();
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            TerminateConfirmationThread();
            TerminateOrderHistoryThread();
            TerminateNotificationsThread();
        }
        #endregion

        #region methods
        private void InitializeConfirmationMoverThread() {
            _confirmationLogic = ((IContainer)AutofacHostFactory.Container).Resolve<IConfirmationLogic>();
            _confirmationLogic.ListenForQueueMessages();
        }

        private void InitializeNotificationsThread()
        {
            _messagingLogic = ((IContainer)AutofacHostFactory.Container).Resolve<Svc.Core.Interface.Messaging.IInternalMessagingLogic>();
            _messagingLogic.ListenForNotificationMessagesOnQueue();
        }

        private void InitializeOrderUpdateThread() {
            //System.Diagnostics.Debugger.Launch();

            _orderHistoryLogic = ((IContainer)AutofacHostFactory.Container).Resolve<IOrderHistoryLogic>();
            _orderHistoryLogic.ListenForQueueMessages();
        }

        private void TerminateConfirmationThread() {
            if (_confirmationLogic != null)
                _confirmationLogic.Stop();
        }

        private void TerminateOrderHistoryThread() {
            if (_orderHistoryLogic != null)
                _orderHistoryLogic.StopListening();
        }

        private void TerminateNotificationsThread()
        {
            if (_messagingLogic != null)
                _messagingLogic.Stop();
        }
	}

    #endregion
}