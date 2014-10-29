using Autofac;
using Autofac.Integration.Wcf;
using KeithLink.Common.Core.Logging;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Interface.Confirmations;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Confirmations;
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
        const int TIMER_DURATION_TICK = 2000;
        const int TIMER_DURATION_START = 30000;
        const int TIMER_DURATION_STOP = -1;
        const int TIMER_DURATION_IMMEDIATE = 1;
        
        private bool _orderHistoryProcessing;
        private bool _keepQueueListening = true;

        IConfirmationLogic confirmationLogic;

        private Timer _orderHistoryTimer;
        private System.Threading.Tasks.Task _processConfirmationsTask;
        #endregion

        #region ctor
        public Global() {
            _orderHistoryProcessing = false;
        }
        #endregion

        #region events
        protected void Application_Start(object sender, EventArgs e)
        {
            IContainer container = AutofacContainerBuilder.BuildContainer();
            AutofacHostFactory.Container = container;

            //InitializeOrderUpdateTimer();
            InitializeConfirmationMoverThread();
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
            if (confirmationLogic != null)
                confirmationLogic.Stop();

            TerminateOrderHistoryTimer();

            if (_processConfirmationsTask != null)
                _processConfirmationsTask.Wait(); // graceful shutdown
        }
        #endregion

        #region methods
        private void InitializeOrderUpdateTimer() {
            AutoResetEvent auto = new AutoResetEvent(true);
            TimerCallback cb = new TimerCallback(OrderHistoryQueueListener);

            _orderHistoryTimer = new Timer(cb, auto, TIMER_DURATION_START, TIMER_DURATION_TICK);
        }

        private void OrderHistoryQueueListener(object state) 
        {
            while (_keepQueueListening)
            {
                if (_orderHistoryProcessing == false) {
                    _orderHistoryProcessing = true;

                    try {
                        IOrderHistoryQueueRepository historyQueue = ((IContainer)AutofacHostFactory.Container).Resolve<IOrderHistoryQueueRepository>();

                        string rawOrder = historyQueue.ConsumeFromQueue();

                        while (rawOrder != null) {
                            OrderHistoryFile historyFile = new OrderHistoryFile();

                            System.IO.StringReader xmlData = new System.IO.StringReader(rawOrder);
                            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(historyFile.GetType());

                            historyFile = (OrderHistoryFile)xs.Deserialize(xmlData);

                            rawOrder = historyQueue.ConsumeFromQueue();
                        }
                    } catch (Exception ex) {
                        IEventLogRepository eventLogRepository = ((IContainer)AutofacHostFactory.Container).Resolve<IEventLogRepository>();
                        eventLogRepository.WriteErrorLog("Error in Internal Service Queue Listener", ex);
                    }

                    _orderHistoryProcessing = false;
                }
            }
        }

        private void TerminateOrderHistoryTimer() {
            if (_orderHistoryTimer != null) {
                _orderHistoryTimer.Change(TIMER_DURATION_IMMEDIATE, TIMER_DURATION_STOP);
                _orderHistoryTimer.Dispose();
            }
        }

        private void InitializeConfirmationMoverThread()
        {
            confirmationLogic = ((IContainer)AutofacHostFactory.Container).Resolve<IConfirmationLogic>();
            confirmationLogic.ListenForQueueMessages();
        }
	}

    #endregion
}